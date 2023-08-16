// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using System.Runtime.InteropServices;
using System.Threading;

using Jungo.wdapi_dotnet;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using WDC_DEVICE_HANDLE = System.IntPtr;
using DWORD = System.UInt32;
using UINT32 = System.UInt32;
using WORD = System.UInt16;
using BOOL = System.Boolean;

namespace Jungo.plx_lib
{
    /* Run-time registers of PLX master devices (9054, 9056, 9080, 9656) */
    public enum PLX_M_REGS
    {
        /* Local configuration registers */
        PLX_M_LAS0RR = 0x00,    /* Local Addr Space 0 Range for PCI-to-Local Bus */
        PLX_M_LAS0BA = 0x04,    /* Local BAR (Remap) for PCI-to-Local Addr 
                                   Space 0 */
        PLX_M_MARBR = 0x08,     /* Mode/DMA Arbitration */
        PLX_M_BIGEND = 0x0C,    /* Big/Little Endian Descriptor */
        PLX_M_LMISC = 0x0D,     /* Local Miscellananeous Control */
        PLX_M_PROT_AREA = 0x0E, /* Serial EEPROM Write-Protected Addr Boundary */
        PLX_M_EROMRR = 0x10,    /* Expansion ROM Range */
        PLX_M_EROMBA = 0x14,    /* EROM Local BAR (Remap) & BREQ0 Control */
        PLX_M_LBRD0 = 0x18,     /* Local Addr Space 0 Bus Region Descriptor */
        PLX_M_DMRR = 0x1C,      /* Local Range for PCI initiatior-to-PCI */
        PLX_M_DMLBAM = 0x20,    /* Local Bus Addr for PCI Initiatior-to-PCI Mem */
        PLX_M_DMLBAI = 0x24,    /* Local BAR for PCI Initiatior-to-PCI I/O */
        PLX_M_DMPBAM = 0x28,    /* PCI BAR (Remap) for Initiatior-to-PCI Mem */
        PLX_M_DMCFGA = 0x2C,    /* PCI Config Addr for PCI Initiatior-to-PCI I/O */

        PLX_M_LAS1RR = 0xF0,    /* Local Addr Space 1 Range for PCI-to-Local Bus */
        PLX_M_LAS1BA = 0xF4,    /* Local Addr Space 1 Local BAR (Remap) */
        PLX_M_LBRD1 = 0xF8,     /* Local Addr Space 1 Bus Region Descriptor */
        PLX_M_DMDAC = 0xFC,     /* PCI Initiatior PCI Dual Address Cycle */
        PLX_M_PCIARB = 0x100,   /* PCI Arbiter Control*/
        PLX_M_PABTADR = 0x104,  /* PCI Abort Address */

        /* mailbox, doorbell, interrupt status, control, id registers */
        PLX_M_MBOX0 = 0x40,     /* Mailbox 0 */
        PLX_M_MBOX1 = 0x44,     /* Mailbox 1 */
        PLX_M_MBOX2 = 0x48,     /* Mailbox 2 */
        PLX_M_MBOX3 = 0x4C,     /* Mailbox 3 */
        PLX_M_MBOX4 = 0x50,     /* Mailbox 4 */
        PLX_M_MBOX5 = 0x54,     /* Mailbox 5 */
        PLX_M_MBOX6 = 0x58,     /* Mailbox 6 */
        PLX_M_MBOX7 = 0x5C,     /* Mailbox 7 */
        PLX_M_P2LDBELL = 0x60,  /* PCI-to-Local Doorbell */
        PLX_M_L2PDBELL = 0x64,  /* Local-to-PCI Doorbell */
        PLX_M_INTCSR = 0x68,    /* INTCSR - Interrupt Control/Status */
        PLX_M_CNTRL = 0x6C,     /* Serial EEPROM/User I/O/Init Ctr & PCI Cmd 
                                   Codes */
        PLX_M_PCIHIDR = 0x70,   /* PCI Hardcoded Configuration ID */
        PLX_M_PCIHREV = 0x74,   /* PCI Hardcoded Revision ID */
        PLX_M_MBOX0_I2O = 0x78, /* Mailbox 0 - I2O enabled */
        PLX_M_MBOX1_I2O = 0x7C, /* Mailbox 1 - I2O enabled */

        /* DMA registers */
        PLX_M_DMAMODE0 = 0x80,  /* DMA Channel 0 Mode */
        PLX_M_DMAPADR0 = 0x84,  /* DMA Channel 0 PCI Address */
        PLX_M_DMALADR0 = 0x88,  /* DMA Channel 0 Local Address */
        PLX_M_DMASIZ0 = 0x8C,   /* DMA Channel 0 Transfer Size (bytes) */
        PLX_M_DMADPR0 = 0x90,   /* DMA Channel 0 Descriptor Pointer */
        PLX_M_DMAMODE1 = 0x94,  /* DMA Channel 1 Mode */
        PLX_M_DMAPADR1 = 0x98,  /* DMA Channel 1 PCI Address */
        PLX_M_DMALADR1 = 0x9C,  /* DMA Channel 1 Local Address */
        PLX_M_DMASIZ1 = 0xA0,   /* DMA Channel 1 Transfer Size (bytes) */
        PLX_M_DMADPR1 = 0xA4,   /* DMA Channel 1 Descriptor Pointer */
        PLX_M_DMACSR0 = 0xA8,   /* DMA Channel 0 Command/Status */
        PLX_M_DMACSR1 = 0xA9,   /* DMA Channel 1 Command/Status */
        PLX_M_DMAARB = 0xAC,    /* DMA Arbitration */
        PLX_M_DMATHR = 0xB0,    /* DMA Threshold (Channel 0 only) */
        PLX_M_DMADAC0 = 0xB4,   /* DMA 0 PCI Dual Address Cycle Address */
        PLX_M_DMADAC1 = 0xB8,   /* DMA 1 PCI Dual Address Cycle Address */

        /* Messaging queue (I20) registers */
        PLX_M_OPQIS = 0x30,     /* Outbound Post Queue Interrupt Status */
        PLX_M_OPQIM = 0x34,     /* Outbound Post Queue Interrupt Mask */
        PLX_M_IQP = 0x40,       /* Inbound Queue Post */
        PLX_M_OQP = 0x44,       /* Outbound Queue Post */
        PLX_M_MQCR = 0xC0,      /* Messaging Queue Configuration */
        PLX_M_QBAR = 0xC4,      /* Queue Base Address */
        PLX_M_IFHPR = 0xC8,     /* Inbound Free Head Pointer */
        PLX_M_IFTPR = 0xCC,     /* Inbound Free Tail Pointer */
        PLX_M_IPHPR = 0xD0,     /* Inbound Post Head Pointer */
        PLX_M_IPTPR = 0xD4,     /* Inbound Post Tail Pointer */
        PLX_M_OFHPR = 0xD8,     /* Outbound Free Head Pointer */
        PLX_M_OFTPR = 0xDC,     /* Outbound Free Tail Pointer */
        PLX_M_OPHPR = 0xE0,     /* Outbound Post Head Pointer */
        PLX_M_OPTPR = 0xE4,     /* Outbound Post Tail Pointer */
        PLX_M_QSR = 0xE8,        /* Queue Status/Control */
    };

    /* DMA channels */
    public enum PLX_DMA_CHANNEL
    {
        PLX_DMA_CHANNEL_0 = 0,
        PLX_DMA_CHANNEL_1 = 1
    };

    public struct PlxChannel
    {
        public PLX_DMA_CHANNEL channel;
        public BOOL bIsEnabled;
        /* offsets of DMA registers */
        public DWORD dwDMACSR;
        public DWORD dwDMAMODE;
        public DWORD dwDMAPADR;
        public DWORD dwDMALADR;
        public DWORD dwDMADPR;
        public DWORD dwDMASIZ;
        public UINT32 u32AbortMask;

        public PlxChannel(PLX_DMA_CHANNEL chn)
        {
            channel = chn;
            bIsEnabled = false;

            if (chn == PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0)
            {
                dwDMACSR = (DWORD)PLX_M_REGS.PLX_M_DMACSR0;
                dwDMAMODE = (DWORD)PLX_M_REGS.PLX_M_DMAMODE0;
                dwDMAPADR = (DWORD)PLX_M_REGS.PLX_M_DMAPADR0;
                dwDMALADR = (DWORD)PLX_M_REGS.PLX_M_DMALADR0;
                dwDMADPR = (DWORD)PLX_M_REGS.PLX_M_DMADPR0;
                dwDMASIZ = (DWORD)PLX_M_REGS.PLX_M_DMASIZ0;
                u32AbortMask = (UINT32)BITS.BIT25;
            }
            else
            {
                dwDMACSR = (DWORD)PLX_M_REGS.PLX_M_DMACSR1;
                dwDMAMODE = (DWORD)PLX_M_REGS.PLX_M_DMAMODE1;
                dwDMAPADR = (DWORD)PLX_M_REGS.PLX_M_DMAPADR1;
                dwDMALADR = (DWORD)PLX_M_REGS.PLX_M_DMALADR1;
                dwDMADPR = (DWORD)PLX_M_REGS.PLX_M_DMADPR1;
                dwDMASIZ = (DWORD)PLX_M_REGS.PLX_M_DMASIZ1;
                u32AbortMask = (UINT32)BITS.BIT26;
            }
        }
    }

    /* PLX diagnostics interrupt handler function type */
    public delegate void USER_INTERRUPT_MASTER_CALLBACK(PLX_MasterDevice
        device, IntPtr pWdDma);

    public class PLX_MasterDevice : PLX_Device
    {
        private USER_INTERRUPT_MASTER_CALLBACK m_userIntHandler;
        private PlxChannel[] m_plxChannels;
        private PlxChannel m_enabledChn;
        private IntPtr m_pDmaCtx;
        private Mutex m_hMutex = null;
        private DmaBuffer m_interruptsDmaBuffer;
        private const UINT32 PLX_MASTER_INT_MASK = (UINT32)
            (BITS.BIT5  /* power management */
            | BITS.BIT13 /* PCI Doorbell */
            | BITS.BIT14 /* PCI Abort */
            | BITS.BIT15 /* Local Input */
            | BITS.BIT20 /* Local Doorbell */
            | BITS.BIT21 /* DMA Channel 0 */
            | BITS.BIT22 /* DMA Channel 1 */
            | BITS.BIT23 /* BIST */
            );

        public DmaBuffer InterruptsDmaBuffer
        {
            get
            {
                return m_interruptsDmaBuffer;
            }
        }

        /* constructors */
        internal protected PLX_MasterDevice(WD_PCI_SLOT slot) :
            this(0, 0, slot, "")
        { }

        internal protected PLX_MasterDevice(DWORD dwVendorId, DWORD dwDeviceId,
            WD_PCI_SLOT slot) : this(dwVendorId, dwDeviceId, slot, "") { }

        internal protected PLX_MasterDevice(DWORD dwVendorId, DWORD dwDeviceId,
            WD_PCI_SLOT slot, string sKP_PLX_DRIVER_NAME) :
            base(dwVendorId, dwDeviceId, slot, sKP_PLX_DRIVER_NAME)
        {
            SetMaster();
        }

        public PLX_MasterDevice(PLX_Device dev) :
            base(dev)
        {
            SetMaster();
        }

        /* This method should only be used by CTORs.
         * Note that the copy CTOR of this class can't initialize
         * the following parameters via the parameterized CTOR
         * (doing do will cause unexpected results, as the base copy CTOR
         * will not be called).So they must be initialized in
         * a seperate method, which is used by both copy CTOR
         * and the parameterized CTOR */
        internal void SetMaster()
        {
            m_bIsMaster = true;
            m_plxChannels = new PlxChannel[]{
                new PlxChannel(PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0),
                new PlxChannel(PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_1)
            };
            m_dwLASBA = new DWORD[2];
            m_hMutex = new Mutex();
        }

        protected override void DeviceInit()
        {
            /* NOTE: You can modify the implementation of this function in
             * order to perform any additional device initialization you 
             * require */

            /* Set specific registers infomation */
            m_dwINTCSR = (DWORD)PLX_M_REGS.PLX_M_INTCSR;
            m_dwCNTRL = (DWORD)PLX_M_REGS.PLX_M_CNTRL;
            m_dwPROT_AREA = (DWORD)PLX_M_REGS.PLX_M_PROT_AREA;
            m_dwLASBA[0] = (DWORD)PLX_M_REGS.PLX_M_LAS0BA;
            m_dwLASBA[1] = (DWORD)PLX_M_REGS.PLX_M_LAS1BA;

            /* Enable target abort for master devices */
            uint u32IntStatus = 0;

            ReadReg32((DWORD)PLX_M_REGS.PLX_M_INTCSR, ref u32IntStatus);
            WriteReg32((DWORD)PLX_M_REGS.PLX_M_INTCSR, u32IntStatus |
                (uint)BITS.BIT12);
        }

        public override DWORD CreateIntTransCmds(out WD_TRANSFER[] pIntTransCmds,
            out DWORD dwNumCmds)
        {
            WDC_ADDR_DESC addrDesc = AddrDesc[PLX_ADDR_REG];
            int NUM_TRANS_CMDS_MASTER = 4;
            pIntTransCmds = new WD_TRANSFER[NUM_TRANS_CMDS_MASTER];

            /* Prepare the interrupt transfer commands */
            /* The transfer commands will be executed by WinDriver in the
             * kernel for each interrupt that is received */

            byte i = 0;
            byte bDmaCsrIndex;

            /* Read interrupt status from the INTCSR register */
            pIntTransCmds[i].pPort = addrDesc.pAddr + m_dwINTCSR;
            pIntTransCmds[i].cmdTrans =
                wdc_defs_macros.WDC_ADDR_IS_MEM(addrDesc) ?
                (DWORD)WD_TRANSFER_CMD.RM_DWORD :
                (DWORD)WD_TRANSFER_CMD.RP_DWORD;
            i++;

            /* Mask interrupt status from the INTCSR register */
            pIntTransCmds[i].cmdTrans = (DWORD)WD_TRANSFER_CMD.CMD_MASK;
            pIntTransCmds[i].Data.Dword = PLX_MASTER_INT_MASK;
            i++;

            /* Read DMA status from the DMACSR register */
            pIntTransCmds[i].pPort = addrDesc.pAddr +
                m_enabledChn.dwDMACSR;
            pIntTransCmds[i].cmdTrans =
                wdc_defs_macros.WDC_ADDR_IS_MEM(addrDesc) ?
                (DWORD)WD_TRANSFER_CMD.RM_BYTE :
                (DWORD)WD_TRANSFER_CMD.RP_BYTE;
            bDmaCsrIndex = i;
            i++;

            /* Write to the DMACSR register to clear the DMA DONE 
             * interrupt */
            pIntTransCmds[i].pPort = pIntTransCmds[bDmaCsrIndex].pPort;
            pIntTransCmds[i].cmdTrans =
                wdc_defs_macros.WDC_ADDR_IS_MEM(addrDesc) ?
                (DWORD)WD_TRANSFER_CMD.WM_BYTE :
                (DWORD)WD_TRANSFER_CMD.WP_BYTE;
            pIntTransCmds[i].Data.Byte = (byte)(BITS.BIT3);
            i++;

            dwNumCmds = i;
            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        public DWORD EnableInterrupts(USER_INTERRUPT_MASTER_CALLBACK userIntCb,
            PLX_DMA_CHANNEL dmaChn)
        {
            uint u32INTCSR = 0;
            uint u32DMAMODE = 0;
            m_pDmaCtx = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(System.IntPtr)));
            m_hMutex.WaitOne();

            if (!IsEnabledInt())
                m_enabledChn = m_plxChannels[(DWORD)dmaChn];

            if (userIntCb == null)
            {
                Log.ErrLog("PLX_Device.EnableInterrupts: "
                    + "user callback is null");
                m_hMutex.ReleaseMutex();
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }
            m_userIntHandler = userIntCb;

            DWORD dwStatus = base.EnableInterrupts(new INT_HANDLER(DMAIntHandler),
                (DWORD)WD_INTERRUPT_OPTIONS.INTERRUPT_CMD_COPY, m_pDmaCtx);

            m_hMutex.ReleaseMutex();
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                m_userIntHandler = null;
                return dwStatus;
            }

            m_plxChannels[(DWORD)dmaChn].bIsEnabled = true;

            /* Physically enable the interrupts on the board */
            /* DMA DONE interrupt enable, route DMA channel interrupt to
             * PCI interrupt */
            ReadReg32(m_enabledChn.dwDMAMODE, ref u32DMAMODE);
            WriteReg32(m_enabledChn.dwDMAMODE,
                u32DMAMODE | (DWORD)(BITS.BIT10 | BITS.BIT17));

            /* PCI interrupt enable, DMA channel local interrupt enable */
            ReadReg32(m_dwINTCSR, ref u32INTCSR);
            WriteReg32(m_dwINTCSR, u32INTCSR | (DWORD)(BITS.BIT8 |
                ((PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0 == dmaChn) ?
                BITS.BIT18 : BITS.BIT19)));

            Log.TraceLog("PLX_MasterDevice: enabled interrupts ("
                + this.ToString(false) + ")");

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        public override void DisableCardInts()
        {
            uint u32INTCSR = 0, u32DMAMODE = 0;
            ReadReg32((DWORD)PLX_M_REGS.PLX_M_INTCSR, ref u32INTCSR);
            WriteReg32((DWORD)PLX_M_REGS.PLX_M_INTCSR,
                u32INTCSR & ~((DWORD)(BITS.BIT8 | BITS.BIT18 | BITS.BIT19)));

            ReadReg32((DWORD)PLX_M_REGS.PLX_M_DMAMODE0, ref u32DMAMODE);
            WriteReg32((DWORD)PLX_M_REGS.PLX_M_DMAMODE0,
                u32DMAMODE & ~(DWORD)BITS.BIT10);

            ReadReg32((DWORD)PLX_M_REGS.PLX_M_DMAMODE1, ref u32DMAMODE);
            WriteReg32((DWORD)PLX_M_REGS.PLX_M_DMAMODE1,
                u32DMAMODE & ~(DWORD)BITS.BIT10);

            Marshal.FreeHGlobal(m_pDmaCtx);
            m_plxChannels[0].bIsEnabled = false;
            m_plxChannels[1].bIsEnabled = false;
        }


        public BOOL IsChannelIntEnabled(PLX_DMA_CHANNEL chn)
        {
            return (IsEnabledInt() && m_plxChannels[(DWORD)chn].bIsEnabled);
        }

        public void DMAStart(DmaBuffer dmaBuffer)
        {
            DWORD u32DMAMODE;
            WD_DMA wdDma = dmaBuffer.MarshalDMA(dmaBuffer.pWdDma);
            DWORD u32LocalAddr = dmaBuffer.m_u32LocalAddr +
                wdDma.dwBytesTransferred;
            PlxChannel plxChn = m_plxChannels[(DWORD)dmaBuffer.ChannelNumber];
            /* Common settings for chain and direct DMA */
            u32DMAMODE = (dmaBuffer.IsAutoInc ? 0 : (DWORD)BITS.BIT11)
                | (DWORD)BITS.BIT6 /* Enable Ready input */
                | (DWORD)BITS.BIT8 /* Local burst */
                | (DWORD)(WDC_ADDR_MODE.WDC_MODE_8 == dmaBuffer.Mode ? 0 :
                    WDC_ADDR_MODE.WDC_MODE_16 == dmaBuffer.Mode ?
                    (DWORD)BITS.BIT0 :
                    (DWORD)BITS.BIT1 | (DWORD)BITS.BIT0);

            if (!dmaBuffer.IsPolling)
                u32DMAMODE |= (DWORD)(BITS.BIT10 | BITS.BIT17);

            if (dmaBuffer is DmaBufferContig ||
                dmaBuffer is DmaBufferTransactionContig)
            {
                /* DMA of one page ==> direct DMA */
                WriteReg32(plxChn.dwDMAMODE, u32DMAMODE);
                WriteReg32(plxChn.dwDMAPADR, (uint)wdDma.Page[0].pPhysicalAddr);
                WriteReg32(plxChn.dwDMALADR, u32LocalAddr);
                WriteReg32(plxChn.dwDMASIZ, (uint)wdDma.Page[0].dwBytes);
                WriteReg32(plxChn.dwDMADPR, dmaBuffer.IsRead ? (DWORD)BITS.BIT3 : 0);
            }
            else
            {
                DmaBufferSG dmaSG = (DmaBufferSG)dmaBuffer;
                WD_DMA wdDmaList = dmaSG.MarshalDMA(dmaSG.pWdDmaList);
                DWORD u32StartOfChain = (uint)wdDmaList.Page[0].pPhysicalAddr;
                IntPtr pList = wdDmaList.pUserAddr;
                DWORD dwPageNumber;
                DmaDesc[] list = new DmaDesc[wdDma.dwPages];

                /* Setting chain of DMA pages in the memory */
                for (dwPageNumber = 0; dwPageNumber < wdDma.dwPages;
                    dwPageNumber++)
                {
                    list[dwPageNumber].u32PADR =
                        (uint)wdDma.Page[dwPageNumber].pPhysicalAddr;
                    list[dwPageNumber].u32LADR =
                        (dmaSG.IsAutoInc ? u32LocalAddr : 0);
                    list[dwPageNumber].u32SIZ =
                        (uint)wdDma.Page[dwPageNumber].dwBytes;
                    list[dwPageNumber].u32DPR =
                        (uint)(u32StartOfChain +
                            Marshal.SizeOf(typeof(DmaDesc)) * (dwPageNumber
                            + 1)) | (uint)(BITS.BIT0 | (dmaSG.IsRead ? BITS.BIT3
                            : 0));

                    /* Mark end of chain */
                    if (dwPageNumber == wdDma.dwPages - 1)
                        list[dwPageNumber].u32DPR |= (uint)BITS.BIT1;

                    Marshal.StructureToPtr(list[dwPageNumber], pList, false);
                    pList = new IntPtr(pList.ToInt64() +
                        Marshal.SizeOf(typeof(DmaDesc)));
                    u32LocalAddr += wdDma.Page[dwPageNumber].dwBytes;
                }

                u32StartOfChain |= (DWORD)BITS.BIT0 | (DWORD)BITS.BIT2 |
                    (dmaBuffer.IsRead ? (DWORD)BITS.BIT3 : 0);

                /* Chain transfer */
                u32DMAMODE |= (DWORD)BITS.BIT9;
                WriteReg32(plxChn.dwDMAMODE, u32DMAMODE);
                WriteReg32(plxChn.dwDMADPR, u32StartOfChain);
            }

            wdc_lib_decl.WDC_DMASyncCpu(dmaBuffer.pWdDma);
            WriteReg8(plxChn.dwDMACSR, (byte)(BITS.BIT0 | BITS.BIT1));
        }

        private bool DMAIsAborted(PLX_DMA_CHANNEL plxDmaChn)
        {
            uint intcsr = 0, mask = 0;

            mask = m_plxChannels[(DWORD)plxDmaChn].u32AbortMask;
            ReadReg32(m_dwINTCSR, ref intcsr);

            return ((intcsr & mask) != mask);
        }

        private bool DMAIsDone(PLX_DMA_CHANNEL plxDmaChn)
        {
            byte dmacsr = 0;
            PlxChannel plxChn = m_plxChannels[(DWORD)plxDmaChn];

            ReadReg8(plxChn.dwDMACSR, ref dmacsr);

            return (dmacsr & (byte)BITS.BIT4) == (byte)BITS.BIT4;
        }

        private bool DMAPollCompletion(PLX_DMA_CHANNEL plxChn, DmaBuffer dmaBuffer)
        {
            while (!DMAIsDone(plxChn) && !DMAIsAborted(plxChn)) ;

            wdc_lib_decl.WDC_DMASyncIo(dmaBuffer.pWdDma);
            return !DMAIsAborted(plxChn);
        }

        public DWORD DMATransfer(PLX_DMA_CHANNEL plxChn, DmaBuffer dmaBuffer,
            BOOL bIsCompInt)
        {
            dmaBuffer.IsPolling = !bIsCompInt;

            if (bIsCompInt)
            {
                if (!IsChannelIntEnabled(plxChn))
                {
                    Log.TraceLog("PLX_MasterDevice.DMATransfer: interrupts "
                        + "on channel " + ((DWORD)plxChn).ToString() +
                        "are disabled. can not initiate transfer with interrupt "
                        + "completion mode (" + this.ToString(false) + ")");
                    return (DWORD)wdc_err.WD_INVALID_PARAMETER;
                }
                //copy the DMA data to the context IntPtr that was passed
                //to WDC_IntEnable.
                Marshal.WriteIntPtr(m_pDmaCtx, dmaBuffer.pWdDma);
            }
            else
            {
                if (IsChannelIntEnabled(plxChn))
                {
                    Log.TraceLog("PLX_MasterDevice.DMATransfer: interrupts "
                        + "on channel " + ((DWORD)plxChn).ToString() + " are enabled. " +
                        "can not initiate transfer with polling completion "
                        + "mode (" + this.ToString(false) + ")");
                    return (DWORD)wdc_err.WD_INVALID_PARAMETER;
                }
            }

            if (dmaBuffer is DmaBufferTransactionSG ||
                dmaBuffer is DmaBufferTransactionContig)
            {
                if (bIsCompInt)
                {
                    UINT32 u32INTCSR = 0;
                    m_interruptsDmaBuffer = dmaBuffer;

                    ReadReg32(m_dwINTCSR, ref u32INTCSR);
                    WriteReg32(m_dwINTCSR, u32INTCSR | (DWORD)(BITS.BIT8 |
                        ((PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0 == plxChn) ?
                        BITS.BIT18 : BITS.BIT19)));
                }

                wdc_lib_decl.WDC_DMATransactionExecute(dmaBuffer.pWdDma, null, IntPtr.Zero);

                if (!bIsCompInt)
                    DMATransactionPolling(plxChn, dmaBuffer);
                else
                    DMAStart(dmaBuffer);
            }
            else
            {
                DMAStart(dmaBuffer);

                if (!bIsCompInt)
                {
                    if (!DMAPollCompletion(plxChn, dmaBuffer))
                    {
                        Log.TraceLog("PLX_MasterDevice.DMATransfer: aborted");
                        return (DWORD)wdc_err.WD_OPERATION_FAILED;
                    }
                }
            }
            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }
        void DMATransactionPolling(PLX_DMA_CHANNEL plxDmaChn,
            DmaBuffer dmaBuffer)
        {
            DWORD dwStatus;

            do
            {
                DMAStart(dmaBuffer);

                if (!DMAPollCompletion(plxDmaChn, dmaBuffer))
                {
                    Log.TraceLog("DMATransactionPolling: Channel " +
                        plxDmaChn.ToString("D") + " DMA aborted");
                    return;
                }

                Log.TraceLog("DMATransactionPolling: DMA transfer has been " +
                    "finished\n");
                dwStatus = wdc_lib_decl.WDC_DMATransferCompletedAndCheck(
                    dmaBuffer.pWdDma, false);
            } while (dwStatus ==
                unchecked((DWORD)wdc_err.WD_MORE_PROCESSING_REQUIRED));

            if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                wdc_lib_decl.WDC_DMATransactionRelease(dmaBuffer.pWdDma);
                Log.TraceLog("DMATransactionPolling: Channel " +
                    plxDmaChn.ToString("D") + " DMA transaction completed " +
                    "and released\n");
            }
            else
            {
                Log.TraceLog("DMATransactionPolling: Channel " +
                    plxDmaChn.ToString("D") + " DMA transaction failed\n");
            }
        }
        private void DMAIntHandler(IntPtr pCtx)
        {
            wdcDevice.Int = (WD_INTERRUPT)m_wdcDeviceMarshaler.
                MarshalDevWdInterrupt(Handle);
            /* to obtain the data that was read at interrupt use:
             * WD_TRANSFER[] transCommands;
             * transCommands = (WD_TRANSFER[])m_wdcDeviceMarshaler.
             * MarshalDevpWdTrans(wdcDevice.Int.Cmd, wdcDevice.Int.dwCmds); */

            IntPtr pWdDma = Marshal.ReadIntPtr(pCtx);
            DWORD dwStatus = wdc_lib_decl.WDC_DMASyncIo(pWdDma);

            if (m_userIntHandler != null)
                m_userIntHandler(this, pWdDma);
        }
    }
}

