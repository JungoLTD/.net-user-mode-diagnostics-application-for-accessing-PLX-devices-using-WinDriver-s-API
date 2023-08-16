// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using System.Runtime.InteropServices;

using Jungo.wdapi_dotnet;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using WDC_DEVICE_HANDLE = System.IntPtr;
using DWORD = System.UInt32;
using UINT32 = System.UInt32;
using BOOL = System.Boolean;

namespace Jungo.plx_lib
{
    public struct DmaDesc
    {
        public uint u32PADR;
        public uint u32LADR;
        public uint u32SIZ;
        public uint u32DPR;
    };

    public abstract class DmaBuffer
    {
        internal WDC_DEVICE_HANDLE m_hDev;
        internal IntPtr m_pDma;
        internal DWORD m_dwBuffSize;
        internal UINT32 m_u32LocalAddr;
        internal WDC_ADDR_MODE m_mode;
        internal bool m_bIsRead;
        internal bool m_bAutoInc;
        private bool m_bIsPolling;
        protected UINT32 m_u32ChannelNumber;

        internal DmaBuffer(PLX_MasterDevice dev)
        {
            m_hDev = dev.Handle;
        }

        public WDC_DEVICE_HANDLE DeviceHandle
        {
            get
            {
                return m_hDev;
            }
        }

        public IntPtr pWdDma
        {
            get
            {
                return m_pDma;
            }
        }

        public BOOL IsRead
        {
            get
            {
                return m_bIsRead;
            }
        }

        public BOOL IsAutoInc
        {
            get
            {
                return m_bAutoInc;
            }
        }

        public BOOL IsPolling
        {
            get
            {
                return m_bIsPolling;
            }
            set
            {
               m_bIsPolling = value;
            }
        }

        public DWORD BuffSize
        {
            get
            {
                return m_dwBuffSize;
            }
        }

        internal UINT32 LocalAddress
        {
            get
            {
                return m_u32LocalAddr;
            }
        }
        internal UINT32 ChannelNumber
        {
            get
            {
                return m_u32ChannelNumber;
            }
        }

        public WDC_ADDR_MODE Mode
        {
            get
            {
                return m_mode;
            }
        }

        public BOOL IsDMAOpen()
        {
            return (m_pDma != IntPtr.Zero);
        }

        internal WD_DMA MarshalDMA(IntPtr pDma)
        {
            MarshalWdDma m_wdDmaMarshaler = new MarshalWdDma();
            return (WD_DMA)m_wdDmaMarshaler.MarshalNativeToManaged(pDma);
        }

        public abstract DWORD Open(uint u32LocalAddr, bool bIsRead,
            DWORD dwBytes, ref IntPtr pBuffer, bool bAutoIncrement,
            WDC_ADDR_MODE mode, uint u32ChannelNumber);

        public abstract void Close();

        protected virtual void CommonClose()
        {
            DWORD dwStatus;

            if (m_pDma != IntPtr.Zero)
            {
                dwStatus = wdc_lib_decl.WDC_DMABufUnlock(m_pDma);
                if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
                {
                    Log.ErrLog("DmaBuffer.Close: Failed unlocking DMA buffer."
                        + "Error 0x" + dwStatus.ToString("X") + ": " +
                        utils.Stat2Str(dwStatus));
                }
                m_pDma = IntPtr.Zero;
            }
            else
                Log.ErrLog("DmaBuffer.Close: DMA is not currently open ... ");

        }

        public virtual void Dispose()
        {
                Close();
        }
    }

    public class DmaBufferSG: DmaBuffer
    {
        protected IntPtr m_pDmaList;
        public DmaBufferSG(PLX_MasterDevice dev)
            : base(dev){}

        public IntPtr pWdDmaList
        {
            get
            {
                return m_pDmaList;
            }
        }

        public override DWORD Open(uint u32LocalAddr, bool bIsRead,
            DWORD dwBytes, ref IntPtr pBuffer, bool bAutoIncrement,
            WDC_ADDR_MODE mode, uint u32ChannelNumber)
        {
            DWORD dwStatus;
            WD_DMA wdDma;
            m_u32LocalAddr = u32LocalAddr;
            m_bAutoInc = bAutoIncrement;
            m_mode = mode;
            m_dwBuffSize = dwBytes;
            m_bIsRead = bIsRead;
            m_u32ChannelNumber = u32ChannelNumber;
            DWORD dwOptions = (DWORD)((m_bIsRead)?
                WD_DMA_OPTIONS.DMA_FROM_DEVICE:
                WD_DMA_OPTIONS.DMA_TO_DEVICE);

            if (pBuffer == IntPtr.Zero)
            {
                Log.ErrLog("DmaBuffer.Open: Scatter/Gather DMA data buffer " +
                    "is null");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            dwStatus = wdc_lib_decl.WDC_DMASGBufLock(m_hDev, pBuffer, dwOptions,
                m_dwBuffSize, ref m_pDma);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("DmaBuffer.Open: Failed locking a DMA buffer. " +
                    "Error 0x" + dwStatus.ToString("X") +": " +
                    utils.Stat2Str(dwStatus));
                goto Error;
            }

            /* marshaling WD_DMA */
            wdDma = MarshalDMA(m_pDma);

            /* DMA of one page ==> block DMA */
            if (wdDma.dwPages == 1)
                return (DWORD)wdc_err.WD_STATUS_SUCCESS;

            /* DMA of more then one page ==> chain DMA */
            IntPtr pList = IntPtr.Zero;

            /* Allocate a kernel buffer to hold the chain of DMA descriptors
               includes extra 0x10 bytes for quadword alignment */
            dwStatus = wdc_lib_decl.WDC_DMAContigBufLock(m_hDev, ref pList,
                (DWORD)WD_DMA_OPTIONS.DMA_TO_DEVICE,
                wdDma.dwPages * (DWORD)Marshal.SizeOf(typeof(DmaDesc)) + 0x10,
                ref m_pDmaList);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("DmaBuffer.Open: Failed locking DMA list buffer" +
                    " Error 0x" + dwStatus.ToString("X") + ": " +
                    utils.Stat2Str(dwStatus));
                goto Error;
            }

            /* flush the list of DMA descriptors from CPU cache to system
             * memory */
            wdc_lib_decl.WDC_DMASyncCpu(m_pDmaList);

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;

Error:
            Close();
            return dwStatus;
        }

        public override void Close()
        {
            CommonClose();
            if (m_pDmaList != IntPtr.Zero)
            {
                DWORD dwStatus = wdc_lib_decl.WDC_DMABufUnlock(m_pDmaList);
                if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
                {
                    Log.TraceLog("DmaBuffer.Close: Failed unlocking DMA list " +
                        "buffer. Error 0x" + dwStatus.ToString("X") + ": " +
                        utils.Stat2Str(dwStatus));
                }
                m_pDmaList = IntPtr.Zero;
            }
        }
    }

    public class DmaBufferContig: DmaBuffer
    {
        public DmaBufferContig(PLX_MasterDevice dev):base(dev){}

        public override DWORD Open(uint u32LocalAddr, bool bIsRead,
            DWORD dwBytes, ref IntPtr pBuffer, bool bAutoIncrement,
            WDC_ADDR_MODE mode, uint u32ChannelNumber)
        {
            DWORD dwStatus;
            WD_DMA wdDma;
            m_u32LocalAddr = u32LocalAddr;
            m_bAutoInc = bAutoIncrement;
            m_mode = mode;
            m_bIsRead = bIsRead;
            m_dwBuffSize = dwBytes;
            m_u32ChannelNumber = u32ChannelNumber;

            DWORD dwOptions = (DWORD)((m_bIsRead)?
                WD_DMA_OPTIONS.DMA_FROM_DEVICE:
                WD_DMA_OPTIONS.DMA_TO_DEVICE) |
                (DWORD)WD_DMA_OPTIONS.DMA_KERNEL_BUFFER_ALLOC;

            /* Allocate and lock a DMA buffer */
            dwStatus = wdc_lib_decl.WDC_DMAContigBufLock(m_hDev, ref pBuffer,
                dwOptions, dwBytes, ref m_pDma);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("DmaBuffer.Open: Failed locking a DMA buffer. " +
                    "Error 0x" + dwStatus.ToString("X") +": " +
                    utils.Stat2Str(dwStatus));
                goto Error;
            }

            /* marshaling WD_DMA */
            wdDma = MarshalDMA(m_pDma);

            /* DMA of one page ==> block DMA */
            if (wdDma.dwPages == 1)
                return (DWORD)wdc_err.WD_STATUS_SUCCESS;

Error:
            Close();
            return dwStatus;
        }

        public override void Close()
        {
            CommonClose();
        }
    }

    public class DmaBufferTransactionSG : DmaBufferSG
    {
        // this value can be changed
        private const DWORD MaxTransferSize = 8192;

        public DmaBufferTransactionSG(PLX_MasterDevice dev)
            : base(dev) { }

        public override DWORD Open(uint u32LocalAddr, bool bIsRead,
            DWORD dwBytes, ref IntPtr pBuffer, bool bAutoIncrement,
            WDC_ADDR_MODE mode, uint u32ChannelNumber)
        {
            DWORD dwStatus;
            m_u32LocalAddr = u32LocalAddr;
            m_bAutoInc = bAutoIncrement;
            m_mode = mode;
            m_dwBuffSize = dwBytes;
            m_bIsRead = bIsRead;
            m_u32ChannelNumber = u32ChannelNumber;
            DWORD dwOptions = (DWORD)((m_bIsRead) ?
                WD_DMA_OPTIONS.DMA_FROM_DEVICE :
                WD_DMA_OPTIONS.DMA_TO_DEVICE);

            if (pBuffer == IntPtr.Zero)
            {
                Log.ErrLog("DmaBuffer.Open: Scatter/Gather DMA data buffer " +
                    "is null");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            dwStatus = wdc_lib_decl.WDC_DMATransactionSGInit(m_hDev, pBuffer,
                dwOptions, m_dwBuffSize, ref m_pDma, IntPtr.Zero,
                MaxTransferSize, (DWORD)Marshal.SizeOf(typeof(DmaDesc)));

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("DmaBuffer.Open: Failed initializing a DMA " +
                    "transaction. Error 0x" + dwStatus.ToString("X") + ": " +
                    utils.Stat2Str(dwStatus));
                goto Error;
            }

            IntPtr pList = IntPtr.Zero;

            DWORD dwPages = BYTES_TO_PAGES((DWORD)ROUND_TO_PAGES(
                MaxTransferSize) + (DWORD)Environment.SystemPageSize);

            dwStatus = wdc_lib_decl.WDC_DMAContigBufLock(m_hDev, ref pList,
                (DWORD)WD_DMA_OPTIONS.DMA_TO_DEVICE,
                dwPages * (DWORD)Marshal.SizeOf(typeof(DmaDesc)),
                ref m_pDmaList);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("DmaBuffer.Open: Failed locking DMA list buffer" +
                    " Error 0x" + dwStatus.ToString("X") + ": " +
                    utils.Stat2Str(dwStatus));
                goto Error;
            }

            return (DWORD)wdc_err.WD_STATUS_SUCCESS;

        Error:
            Close();
            return dwStatus;
        }

        static UINT32 ROUND_TO_PAGES(DWORD dwSize)
        {
            int pageSize = Environment.SystemPageSize;
            return (UINT32)((dwSize + pageSize - 1) & (~(pageSize - 1)));
        }

        static UINT32 BYTES_TO_PAGES(DWORD dwSize)
        {
            int pageSize = Environment.SystemPageSize;
            int pageShift = (int)Math.Log(pageSize, 2);
            bool needExtraPage = (dwSize & (pageSize - 1)) != 0;

            return (dwSize >> pageShift) + (UINT32)(needExtraPage ? 1 : 0);
        }

        public override void Close()
        {
            CommonClose();
            if (m_pDmaList != IntPtr.Zero)
            {
                DWORD dwStatus = wdc_lib_decl.WDC_DMABufUnlock(m_pDmaList);
                if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
                {
                    Log.TraceLog("DmaBuffer.Close: Failed unlocking DMA list " +
                        "buffer. Error 0x" + dwStatus.ToString("X") + ": " +
                        utils.Stat2Str(dwStatus));
                }
                m_pDmaList = IntPtr.Zero;
            }
        }
    }

    public class DmaBufferTransactionContig : DmaBuffer
    {
        private const DWORD ContigAlignment = 0x0000000f;
        public DmaBufferTransactionContig(PLX_MasterDevice dev) : base(dev) { }

        public override DWORD Open(uint u32LocalAddr, bool bIsRead,
            DWORD dwBytes, ref IntPtr pBuffer, bool bAutoIncrement,
            WDC_ADDR_MODE mode, uint u32ChannelNumber)
        {
            DWORD dwStatus;
            m_u32LocalAddr = u32LocalAddr;
            m_bAutoInc = bAutoIncrement;
            m_mode = mode;
            m_bIsRead = bIsRead;
            m_dwBuffSize = dwBytes;
            m_u32ChannelNumber = u32ChannelNumber;
            DWORD dwOptions = (DWORD)((m_bIsRead) ?
                WD_DMA_OPTIONS.DMA_FROM_DEVICE :
                WD_DMA_OPTIONS.DMA_TO_DEVICE) |
                (DWORD)WD_DMA_OPTIONS.DMA_KERNEL_BUFFER_ALLOC;

            /* Allocate and lock a DMA buffer */
            dwStatus = wdc_lib_decl.WDC_DMATransactionContigInit(m_hDev,
                ref pBuffer, dwOptions, dwBytes, ref m_pDma, IntPtr.Zero,
                ContigAlignment);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("DmaBuffer.Open: Failed initializing a DMA " +
                    "transaction. Error 0x" + dwStatus.ToString("X") + ": " +
                    utils.Stat2Str(dwStatus));
                goto Error;
            }

            goto End;

        Error:
            Close();
        End:
            return dwStatus;
        }

        public override void Close()
        {
            CommonClose();
        }
    }
}
