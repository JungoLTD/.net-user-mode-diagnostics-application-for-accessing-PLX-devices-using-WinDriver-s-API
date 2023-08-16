// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;

using Jungo.wdapi_dotnet;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using wdc_lib_decl = Jungo.wdapi_dotnet.wdc_lib_decl;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using WDC_DEVICE_HANDLE = System.IntPtr;   
using HANDLE = System.IntPtr;

namespace Jungo.plx_lib
{
    abstract public class EEPROM
    {
        internal WDC_DEVICE_HANDLE m_devHandle;
        internal DWORD m_dwCNTRL;

        internal EEPROM(WDC_DEVICE_HANDLE hDev, DWORD dwCntrlReg)
        {
            m_devHandle = hDev;
            m_dwCNTRL = dwCntrlReg;                        
        }

        public bool IsVPD()
        {
            return this.GetType() == typeof(EEPROM_VPD);
        }

        static public bool EEPROMIsPresent(WDC_DEVICE_HANDLE hDev, 
            DWORD dwCntrlReg)
        {
            uint u32CNTRL = 0;

            wdc_lib_decl.WDC_ReadAddr32(hDev, (DWORD)PLX_ADDR.PLX_ADDR_REG,
                dwCntrlReg, ref u32CNTRL);
            return (u32CNTRL & (DWORD)BITS.BIT28) == (DWORD)BITS.BIT28;
        }

        /* VPD EEPROM access */
        static public bool VPD_Validate(WDC_DEVICE_HANDLE devHandle, 
            DWORD dwDevCntrlReg)
        {
            byte bData = 0;

            if (devHandle == IntPtr.Zero)
            {
                Log.ErrLog("EEPROM.VPD_Validate: device's handle is null");
                return false;
            }

            /* Verify that a blank or programmed serial EEPROM is present */
            if (!EEPROMIsPresent(devHandle, dwDevCntrlReg))
            {
                Log.ErrLog("EEPROM.VPD_Validate: Error - serial EEPROM " +
                    "was not found on board");
                return false;
            }

            /* Check the next capability pointers */
            wdc_lib_decl.WDC_PciReadCfg8(devHandle, (DWORD)PCI_CFG_REG.PCI_CAP,
                ref bData);
            if (bData != (byte)PLX_CFG_REGS.PLX_PMCAPID)
            {
                Log.TraceLog("EEPROM.VPD_Validate: PMCAP register validation failed");
                return false;
            }

            wdc_lib_decl.WDC_PciReadCfg8(devHandle, 
                (DWORD)PLX_CFG_REGS.PLX_PMNEXT, ref bData);
            if (bData != (byte)PLX_CFG_REGS.PLX_HS_CAPID)
            {
                Log.TraceLog("EEPROM.VPD_Validate: PMNEXT register validation failed");
                return false;
            }

            wdc_lib_decl.WDC_PciReadCfg8(devHandle, 
                (DWORD)PLX_CFG_REGS.PLX_HS_NEXT, ref bData);
            if (bData != (byte)PLX_CFG_REGS.PLX_VPD_CAPID)
            {
                Log.TraceLog("EEPROM.VPD_Validate: HS_NEXT register validation failed");
                return false;
            }

            wdc_lib_decl.WDC_PciReadCfg8(devHandle, 
                (DWORD)PLX_CFG_REGS.PLX_VPD_NEXT, ref bData);
            if (bData != 0)
            {
                Log.TraceLog("EEPROM.VPD_Validate: VPD_NEXT register validation failed");
                return false;
            }
            return true;
        }

        abstract public DWORD EEPROM_Write(DWORD dwOffset, uint u32Data);
        abstract public DWORD EEPROM_Read(DWORD dwOffset,ref uint u32Data);
    };


    public class EEPROM_VPD: EEPROM
    {
        private DWORD m_devProtAreaReg;

        internal protected EEPROM_VPD(WDC_DEVICE_HANDLE hDev, DWORD dwCntrlReg,
            DWORD dwDevProtAreaReg) :base(hDev, dwCntrlReg)
        {
            m_devProtAreaReg = dwDevProtAreaReg;
        }

        private DWORD EEPROM_Delay()
        {
            return wdc_lib_decl.WDC_Sleep(30000, 
                WDC_SLEEP_OPTIONS.WDC_SLEEP_BUSY); 
        }

        override public DWORD EEPROM_Read(DWORD dwOffset, ref uint u32Data)
        {
            DWORD i;
            uint u32EnableAccess = 0;
            WORD wAddr, wData = 0;

            if (m_devHandle == IntPtr.Zero)
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;

            if (dwOffset % 4 != 0)
            {
                Log.ErrLog("EEPROM_Read: Error - offset (0x" + 
                    dwOffset.ToString("X") + ") is not a multiple of 4 ");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            /* Clear EEDO Input Enable */
            EEPROM_EnableAccess(ref u32EnableAccess);

            /* Write a destination serial EEPROM address and flag of operation,
             * value of 0 */
            wAddr = (WORD)(dwOffset & ~(DWORD)BITS.BIT15);
            wdc_lib_decl.WDC_PciWriteCfg16(m_devHandle, 
                (DWORD)PLX_CFG_REGS.PLX_VPD_ADDR, wAddr);

            /* Probe a flag of operation until it changes to a 1 to ensure the
             * Read data is available */
            for (i = 0; i < 10; i++)
            {
                EEPROM_Delay();
                wdc_lib_decl.WDC_PciReadCfg16(m_devHandle, 
                    (DWORD)PLX_CFG_REGS.PLX_VPD_ADDR, ref wData);

                if ((wData & (DWORD)BITS.BIT15) != 0)
                    break;
            }

            if (i == 10)
            {
                Log.ErrLog("PLX_EEPROM_VPD_Read32: Error - Acknowledge " +
                    "to EEPROM read was not received");
                return (DWORD)wdc_err.WD_OPERATION_FAILED;
            }

            /* Read back the requested data from PVPDATA register */
            wdc_lib_decl.WDC_PciReadCfg32(m_devHandle, 
                (DWORD)PLX_CFG_REGS.PLX_VPD_DATA, ref u32Data);

            /* Restore EEDO Input Enable */
            EEPROM_RestoreAccess(u32EnableAccess);

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        override public DWORD EEPROM_Write(DWORD dwOffset, uint u32Data)
        {
            DWORD i;
            uint u32ReadBack = 0, u32EnableAccess = 0;
            WORD wAddr, wData = 0;
            byte bEnableOffset = 0;

            if (m_devHandle == IntPtr.Zero)
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;

            if (dwOffset % 4 != 0)
            {
                Log.ErrLog("EEPROM_Write: Error - offset (0x" +
                    dwOffset.ToString("X") + " is not a multiple of 4");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            /* Clear EEDO Input Enable */
            EEPROM_EnableAccess(ref u32EnableAccess);

            wAddr = (WORD)dwOffset;
            EEPROM_RemoveWriteProtection(wAddr, ref bEnableOffset);

            EEPROM_Delay();

            /* Write desired data to PVPDATA register */
            wdc_lib_decl.WDC_PciWriteCfg32(m_devHandle,
                (DWORD)PLX_CFG_REGS.PLX_VPD_DATA, u32Data);

            /* Write a destination serial EEPROM address and flag of operation,
             * value of 1 */
            wAddr = (WORD)(wAddr | (WORD)BITS.BIT15);
            wdc_lib_decl.WDC_PciWriteCfg16(m_devHandle, 
                (DWORD)PLX_CFG_REGS.PLX_VPD_ADDR, wAddr);

            /* Probe a flag of operation until it changes to a 0 to ensure the
             * write completes */
            for (i = 0; i < 10; i++)
            {
                EEPROM_Delay();
                wdc_lib_decl.WDC_PciReadCfg16(m_devHandle,
                    (DWORD)PLX_CFG_REGS.PLX_VPD_ADDR, ref wData);
                if ((wData & (WORD)BITS.BIT15) != 0)
                    break;
            }

            EEPROM_RestoreWriteProtection(bEnableOffset);

            /* Restore EEDO Input Enable */
            EEPROM_RestoreAccess(u32EnableAccess);

            EEPROM_Read(dwOffset, ref u32ReadBack);

            if (u32ReadBack != u32Data)
            {
                Log.ErrLog(string.Format("EEPROM_Write: Error - Wrote " +
                    "0x{0,8}, read back 0x{1,8}", u32Data.ToString("X"),
                    u32ReadBack.ToString("X"))); 
                return (DWORD)wdc_err.WD_OPERATION_FAILED;
            }
            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        /* Enable EEPROM access via VPD mechanism - disable EEDO Input 
         * (CNTRL[31]=0, default). This bit is specific to PLX 9656 and PLX 
         * 9056 chips (it is reserved on other boards) */
        private DWORD EEPROM_EnableAccess(ref uint u32DataOld)
        {
            wdc_lib_decl.WDC_ReadAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, ref u32DataOld);
            return wdc_lib_decl.WDC_WriteAddr32(m_devHandle,
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, 
                u32DataOld & ~(DWORD)BITS.BIT31);
        }

        /* Restore EEDO Input Enable */
        private DWORD EEPROM_RestoreAccess(uint u32Data)
        {
            return wdc_lib_decl.WDC_WriteAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, u32Data);
        }

        private DWORD EEPROM_RemoveWriteProtection(WORD wAddr, 
            ref byte bDataOld)
        {
            wdc_lib_decl.WDC_ReadAddr8(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_devProtAreaReg, ref bDataOld);

            wAddr /= 4;
            wAddr &= 0x7F;

            wdc_lib_decl.WDC_WriteAddr8(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_devProtAreaReg, (byte)wAddr);

            bDataOld *= 4; /* Expand from DWORD to BYTE count */

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        private DWORD EEPROM_RestoreWriteProtection(WORD wAddr)
        {    
            return wdc_lib_decl.WDC_WriteAddr8(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_devProtAreaReg, (byte)wAddr);
        }
    };

    public class EEPROM_RT: EEPROM
    {
        private DWORD m_EEPROMmsb;

        internal protected EEPROM_RT(WDC_DEVICE_HANDLE hDev, DWORD dwCntrlReg,
            DWORD EEPROMmsb) :base(hDev, dwCntrlReg)
        {
            m_EEPROMmsb = EEPROMmsb;
        }

        private DWORD EEPROM_Delay()
        {
            return wdc_lib_decl.WDC_Sleep(700, 
                WDC_SLEEP_OPTIONS.WDC_SLEEP_BUSY);
        }

        /* To use the Read16/Write16 methods, you can cast a "base" EEPROM
         * object to EEPROM_RT, and that will give the access to those methods */

        public DWORD EEPROM_Read16(DWORD dwOffset, ref WORD wData)
        {
            WORD i;
            DWORD dwAddr = dwOffset >> 1;
            bool bit = false;

            if (m_devHandle == IntPtr.Zero)
                return (DWORD)wdc_err.WD_INVALID_HANDLE;

            wData = 0;

            EEPROM_ChipSelect(true);
            EEPROM_WriteBit(true);
            EEPROM_WriteBit(true);
            EEPROM_WriteBit(false);

            /* CS06, CS46 EEPROM - send 6bit address
             * CS56, CS66 EEPROM - send 8bit address */
            for (i = (WORD)m_EEPROMmsb; i!=0 ; i >>= 1)
                EEPROM_WriteBit((dwAddr & i) == i);

            for (i = (WORD)BITS.BIT15; i != 0; i >>= 1)
            {
                EEPROM_ReadBit(ref bit);
                wData |= (bit) ? i : (WORD)0;
            }

            EEPROM_ChipSelect(false);

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        public DWORD EEPROM_Write16(DWORD dwOffset, WORD wData)
        {
            WORD i;
            DWORD dwAddr = dwOffset >> 1;
            WORD wReadBack = 0;

            if (m_devHandle == IntPtr.Zero)
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;

            EEPROM_WriteEnableDisable(true);

            EEPROM_ChipSelect(true);

            /* Send a PRWRITE instruction */
            EEPROM_WriteBit(true);
            EEPROM_WriteBit(false);
            EEPROM_WriteBit(true);

            /* CS06, CS46 EEPROM - send 6bit address
             * CS56, CS66 EEPROM - send 8bit address */
            for (i = (WORD)m_EEPROMmsb; i != 0; i >>= 1)
                EEPROM_WriteBit((dwAddr & i) == i);

            for (i = (WORD)BITS.BIT15; i != 0; i >>= 1)
                EEPROM_WriteBit((wData & i) == i);

            EEPROM_ChipSelect(false);

            EEPROM_WriteEnableDisable(true);

            EEPROM_Read16(dwOffset, ref wReadBack);

            if (wData != wReadBack)
            {
                Log.ErrLog(string.Format("EEPROM_Write16: Error - Wrote " +
                    "0x{0,4}, read back 0x{1,4}", wData.ToString("X"),
                    wReadBack.ToString("X")));
                return (DWORD)wdc_err.WD_OPERATION_FAILED;
            }

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        override public DWORD EEPROM_Read(DWORD dwOffset,ref uint u32Data)
        {
            WORD wData1 =0 , wData2 = 0;

            if (m_devHandle == IntPtr.Zero)
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;

            if ((dwOffset % 4) != 0)
            {
                Log.ErrLog("EEPROM_Read: Error - offset 0x" +
                    dwOffset.ToString("X") + "is not a multiple of 4");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            EEPROM_Read16(dwOffset, ref wData1);

            EEPROM_Read16(dwOffset + 2, ref wData2);

            u32Data = (uint)((wData1 << 16) + wData2);

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        override public DWORD EEPROM_Write(DWORD dwOffset, uint u32Data)
        {
            WORD wData1, wData2;

            if (m_devHandle == IntPtr.Zero)
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;

            if ((dwOffset % 4) != 0)
            {
                Log.ErrLog("EEPROM_Write: Error - offset 0x" + 
                    dwOffset.ToString("X") + "is not a multiple of 4");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            wData1 = (WORD)(u32Data >> 16);
            wData2 = (WORD)(u32Data & 0xFFFF);

            EEPROM_Write16(dwOffset, wData1);

            EEPROM_Write16(dwOffset + 2, wData2);

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        private void EEPROM_ChipSelect(bool fSelect)
        {
            uint u32CNTRL = 0;

            wdc_lib_decl.WDC_ReadAddr32(m_devHandle,
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, ref u32CNTRL);

            wdc_lib_decl.WDC_WriteAddr32(m_devHandle,
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, fSelect ?
                (u32CNTRL | (uint)BITS.BIT25) : (u32CNTRL & ~(uint)BITS.BIT25));

            EEPROM_Delay();
        }

        private void EEPROM_ReadBit(ref bool bit)
        {
            uint u32CNTRL = 0;

            wdc_lib_decl.WDC_ReadAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, ref u32CNTRL);

            /* clock */
            wdc_lib_decl.WDC_WriteAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL,
                u32CNTRL & ~(uint)BITS.BIT24);

            EEPROM_Delay();

            wdc_lib_decl.WDC_WriteAddr32(m_devHandle,
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, 
                u32CNTRL | (uint)BITS.BIT24);

            EEPROM_Delay();

            wdc_lib_decl.WDC_WriteAddr32(m_devHandle,
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, 
                u32CNTRL & ~(uint)BITS.BIT24);

            EEPROM_Delay();

            /* data */
            wdc_lib_decl.WDC_ReadAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, ref u32CNTRL);
            bit = (u32CNTRL & (uint)BITS.BIT27) == (uint)BITS.BIT27;
        }

        private void EEPROM_WriteBit(bool bit)
        {
            uint u32CNTRL = 0;

            wdc_lib_decl.WDC_ReadAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, ref u32CNTRL);

            if (bit) /* data */
                u32CNTRL |= (uint)BITS.BIT26;
            else
                u32CNTRL &= (uint)~BITS.BIT26;

            /* clock */
            wdc_lib_decl.WDC_WriteAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, 
                u32CNTRL & ~(uint)BITS.BIT24);

            EEPROM_Delay();

            wdc_lib_decl.WDC_WriteAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL,
                u32CNTRL | (uint)BITS.BIT24);

            EEPROM_Delay();

            wdc_lib_decl.WDC_WriteAddr32(m_devHandle, 
                (DWORD)PLX_ADDR.PLX_ADDR_REG, m_dwCNTRL, 
                u32CNTRL & ~(uint)BITS.BIT24);

            EEPROM_Delay();
        }

        private void EEPROM_WriteEnableDisable(bool fEnable)
        {
            EEPROM_ChipSelect(true);

            /* Send a WEN instruction */
            EEPROM_WriteBit(true);
            EEPROM_WriteBit(false);
            EEPROM_WriteBit(false);
            EEPROM_WriteBit(fEnable ? true : false);
            EEPROM_WriteBit(fEnable ? true : false);

            EEPROM_WriteBit(false);
            EEPROM_WriteBit(false);
            EEPROM_WriteBit(false);
            EEPROM_WriteBit(false);

            EEPROM_ChipSelect(false);
        }
    };
}
