// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Jungo.wdapi_dotnet;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using UINT32 = System.UInt32;
using DWORD = System.UInt32;
using WORD = System.UInt16;

namespace Jungo.plx_lib
{
    /* Run-time registers of PLX target devices (9030, 9050, 9052) */
    public enum PLX_T_REGS 
    {
        /* Local configuration registers */
        PLX_T_LAS0RR = 0x00,      /* Local Addr Space 0 Range */
        PLX_T_LAS1RR = 0x04,      /* Local Addr Space 1 Range */
        PLX_T_LAS2RR = 0x08,      /* Local Addr Space 2 Range */
        PLX_T_LAS3RR = 0x0C,      /* Local Addr Space 3 Range */
        PLX_T_EROMRR = 0x10,      /* Expansion ROM Range */
        PLX_T_LAS0BA = 0x14,      /* Local Addr Space 0 Local BAR (Remap) */
        PLX_T_LAS1BA = 0x18,      /* Local Addr Space 1 Local BAR (Remap) */
        PLX_T_LAS2BA = 0x1C,      /* Local Addr Space 2 Local BAR (Remap) */
        PLX_T_LAS3BA = 0x20,      /* Local Addr Space 3 Local BAR (Remap) */
        PLX_T_EROMBA = 0x24,      /* Expansion ROM Local BAR (Remap) */
        PLX_T_LAS0BRD = 0x28,     /* Local Addr Space 0 Bus Region Descriptors */
        PLX_T_LAS1BRD = 0x2C,     /* Local Addr Space 1 Bus Region Descriptors */
        PLX_T_LAS2BRD = 0x30,     /* Local Addr Space 2 Bus Region Descriptors */
        PLX_T_LAS3BRD = 0x34,     /* Local Addr Space 3 Bus Region Descriptors */
        PLX_T_EROMBRD = 0x38,     /* Expansion ROM Bus Region Descriptors */

        /* Chip select registers */
        PLX_T_CS0BASE = 0x3C,     /* Chip Select 0 Base Address */
        PLX_T_CS1BASE = 0x40,     /* Chip Select 1 Base Address */
        PLX_T_CS2BASE = 0x44,     /* Chip Select 2 Base Address */
        PLX_T_CS3BASE = 0x48,     /* Chip Select 3 Base Address */

        /* Control registers */
        PLX_T_INTCSR = 0x4C,      /* Interrupt Control/Status (16 bit)*/
        PLX_T_PROT_AREA = 0x4E,   /* Serial EEPROM Write-Protected Addr Boundary (16 bit)*/
        PLX_T_CNTRL = 0x50,       /* PCI Target Response; Serial EEPROM; Init Ctr */
        PLX_T_GPIOC = 0x54,       /* General Purpose I/O Control */
        PLX_T_PMDATASEL = 0x70,   /* Hidden 1 Power Management Data Select */
        PLX_T_PMDATASCALE = 0x74  /* Hidden 2 Power Management Data Scale */
    };        

    /* PLX diagnostics interrupt handler function type */
    public delegate void USER_INTERRUPT_TARGET_CALLBACK(PLX_TargetDevice device);

    public class PLX_TargetDevice: PLX_Device
    {
        private USER_INTERRUPT_TARGET_CALLBACK m_userIntHandler;

	private const WORD PLX_TARGET_INT_MASK = (WORD)
		( BITS.BIT2  /* LINTi1 Status */ 
		| BITS.BIT5  /* LINTi2 Status */ 
		);

        /*constructors*/
        internal protected PLX_TargetDevice(WD_PCI_SLOT slot): this(0, 0, slot, ""){}

        internal protected PLX_TargetDevice(DWORD dwVendorId, DWORD dwDeviceId, 
            WD_PCI_SLOT slot): this(dwVendorId, dwDeviceId, slot, ""){}

        internal protected PLX_TargetDevice(DWORD dwVendorId, DWORD dwDeviceId, 
            WD_PCI_SLOT slot, string sKP_PLX_DRIVER_NAME):
            base(dwVendorId, dwDeviceId, slot, sKP_PLX_DRIVER_NAME)
        {
            SetTarget();
        }

        public PLX_TargetDevice(PLX_Device dev):
            base(dev)
        {
            SetTarget();
        }

        /* This method should only be used by CTORs.
         * Note that the copy CTOR of this class can't initialize
         * the following parameters via the parameterized CTOR
         * (doing do will cause unexpected results, as the base copy CTOR
         * will not be called).So they must be initialized in
         * a seperate method, which is used by both copy CTOR
         * and the parameterized CTOR */
        internal void SetTarget()
        {
            m_bIsMaster = false;
            m_dwLASBA = new DWORD[4];
        }

        protected override void DeviceInit()
        {
            /* NOTE: You can modify the implementation of this function in
             * order to perform any additional device initialization you
             * require */

            /* Set specific registers infomation */
            m_dwINTCSR = (DWORD)PLX_T_REGS.PLX_T_INTCSR;
            m_dwCNTRL = (DWORD)PLX_T_REGS.PLX_T_CNTRL;
            m_dwPROT_AREA = (DWORD)PLX_T_REGS.PLX_T_PROT_AREA;
            m_dwLASBA[0] = (DWORD)PLX_T_REGS.PLX_T_LAS0BA;
            m_dwLASBA[1] = (DWORD)PLX_T_REGS.PLX_T_LAS1BA;
            m_dwLASBA[2] = (DWORD)PLX_T_REGS.PLX_T_LAS2BA;
            m_dwLASBA[3] = (DWORD)PLX_T_REGS.PLX_T_LAS3BA;
        }

	public override DWORD CreateIntTransCmds(out WD_TRANSFER[] pIntTransCmds,
		out DWORD dwNumCmds)
        {
            WDC_ADDR_DESC addrDesc = AddrDesc[PLX_ADDR_REG];
            WORD wINTCSR=0;
            int NUM_TRANS_CMDS_TARGET = 3;
            pIntTransCmds = new WD_TRANSFER[NUM_TRANS_CMDS_TARGET];

            DWORD dwStatus = ReadReg16(m_dwINTCSR, ref wINTCSR);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_TargetDevice.CreateIntTransCmds: Failed reading "
                    + "from the INTCSR register (" + this.ToString(false) + 
                    ")");
		dwNumCmds = 0;
                return dwStatus;
            }

            /* Prepare the interrupt transfer commands */
            /* The transfer commands will be executed by WinDriver in the kernel
               for each interrupt that is received */

	    byte i = 0;
            byte bIntCsrIndex;

            /* Read status from the INTCSR register */
            pIntTransCmds[i].pPort = addrDesc.pAddr + m_dwINTCSR;
            pIntTransCmds[i].cmdTrans = (DWORD)(addrDesc.fIsMemory ? 
                WD_TRANSFER_CMD.RM_DWORD : WD_TRANSFER_CMD.RP_DWORD);
	    bIntCsrIndex = i;
	    i++;

	    /* Mask interrupt status from the INTCSR register */
	    pIntTransCmds[i].cmdTrans = (DWORD)WD_TRANSFER_CMD.CMD_MASK;
	    pIntTransCmds[i].Data.Word = PLX_TARGET_INT_MASK;
	    i++;

            /* Write to the INTCSR register to clear the interrupt */
            pIntTransCmds[i].pPort = pIntTransCmds[bIntCsrIndex].pPort;
            pIntTransCmds[i].cmdTrans = (DWORD)(addrDesc.fIsMemory ?
                WD_TRANSFER_CMD.WM_WORD : WD_TRANSFER_CMD.WP_WORD);
            pIntTransCmds[i].Data.Word =(WORD)((DWORD)(wINTCSR) & 
                (DWORD)(~(BITS.BIT8 | BITS.BIT10)));
	    i++;

	    dwNumCmds = i;

            return dwStatus;
        }

        public DWORD EnableInterrupts(USER_INTERRUPT_TARGET_CALLBACK userIntCb)
        {
            WORD wINTCSR=0;
            
            if(userIntCb == null)
            {
                Log.TraceLog("PLX_TargetDevice.EnableTargetInterrupts: "
                    + "user callback is invalid");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }
            
            m_userIntHandler = userIntCb;

            DWORD dwStatus = base.EnableInterrupts(new INT_HANDLER(IntHandler), 
                (DWORD)WD_INTERRUPT_OPTIONS.INTERRUPT_CMD_COPY, Handle);
       
            if(dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
                goto Error;

            /* Physically enable the interrupts on the board */
            dwStatus = ReadReg16(m_dwINTCSR, ref wINTCSR);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_TargetDevice.EnableInterrupts: Failed reading "
                    + "from the INTCSR register (" + this.ToString(false) + 
                    ")");
                goto Error;
            }

            dwStatus = WriteReg16(m_dwINTCSR, (WORD)(wINTCSR |
                (WORD)(BITS.BIT8 | BITS.BIT10)));
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_TargetDevice.EnableInterrupts: Faild " +
                    "writing to the INTCSR register to physically enable " +
                    " the interrupts on the board. Error 0x" + 
                    dwStatus.ToString("X") + ": " + utils.Stat2Str(dwStatus)
                    + " (" + this.ToString(false) + ")");
                goto Error;
            }

            Log.TraceLog("PLX_TargetDevice.EnableInterrupts: enabled interrupts ("
                + this.ToString(false) + ")");
            return (DWORD)wdc_err.WD_STATUS_SUCCESS;

Error:
            m_userIntHandler = null;
            return dwStatus;            
        }

        public override void DisableCardInts()
        {
            WORD wINTCSR = 0;
            ReadReg16((DWORD)PLX_T_REGS.PLX_T_INTCSR, ref wINTCSR);
            WriteReg16((DWORD)PLX_T_REGS.PLX_T_INTCSR,
                (WORD)(wINTCSR & (DWORD)(~(BITS.BIT8 | BITS.BIT10))));
        }

        public void GenerateTargetInterrupt(DWORD addr_space)
        {
            wdc_lib_decl.WDC_WriteAddr16(Handle, addr_space, m_dwINTCSR, 0xc3);
        }

        private void IntHandler(IntPtr pDev)
        {
            wdcDevice.Int =
                (WD_INTERRUPT)m_wdcDeviceMarshaler.MarshalDevWdInterrupt(pDev);

	    /* to obtain the data that was read at interrupt use:
	     * WD_TRANSFER[] transCommands;
	     * transCommands = (WD_TRANSFER[])m_wdcDeviceMarshaler.MarshalDevpWdTrans(
	     *     wdcDevice.Int.Cmd, wdcDevice.Int.dwCmds);
	     */

            if(m_userIntHandler != null)
                m_userIntHandler(this);        
        }
    }
}

