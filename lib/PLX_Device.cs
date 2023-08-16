// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Data;

using Jungo.wdapi_dotnet;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using UINT64 = System.UInt64;
using UINT32 = System.UInt32;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using BYTE = System.Byte;
using BOOL = System.Boolean;
using WDC_DEVICE_HANDLE = System.IntPtr;
using HANDLE = System.IntPtr;
using PHYS_ADDR = System.UInt64;
#if KERNEL_64BIT
using KPTR = System.UInt64;
#else
using KPTR = System.UInt32;
#endif

namespace Jungo.plx_lib
{
    /* Device address description struct */
    public struct PLX_DEV_ADDR_DESC
    {
        public DWORD dwNumAddrSpaces; /* Total number of device address
                                       * spaces */
        public IntPtr AddrDesc; /* Array of device address spaces information */
    };

#region "PLX header definitions"
    /* PLX configuration registers */
    public enum PLX_CFG_REGS
    {
        PLX_PMCAPID = 0x40,   /* Power Management Capability ID */
        PLX_PMNEXT = 0x41,    /* Power Management Next Capability Pointer */
        PLX_PMCAP = 0x42,     /* Power Management Capabilities Register */
        PLX_PMCSR = 0x44,     /* Power Management Control/Status Register */
        PLX_HS_CAPID = 0x48,  /* Hot Swap Capability ID */
        PLX_HS_NEXT = 0x49,   /* Hot Swap Next Capability Pointer */
        PLX_HS_CSR = 0x4A,    /* Hot Swap Control/Status Register */
        PLX_VPD_CAPID = 0x4C, /* PCI Vital Product Data Capability ID */
        PLX_VPD_NEXT = 0x4D,  /* PCI Vital Product Data Next Capability Pointer
                               */
        PLX_VPD_ADDR = 0x4E,  /* PCI Vital Product Data Address */
        PLX_VPD_DATA = 0x50   /* PCI VPD Data */
    };

    public enum PLX_ADDR
    {
        /*if REG_IO_ACCESS is defines, then the access to the device's
         * registers will be done through the device's I/O space BAR1 */
#if REG_IO_ACCESS
        PLX_ADDR_REG = PCI_BARS.AD_PCI_BAR1;
#else
        PLX_ADDR_REG = PCI_BARS.AD_PCI_BAR0,
#endif
        PLX_ADDR_REG_IO = PCI_BARS.AD_PCI_BAR1,
        PLX_ADDR_SPACE0 = PCI_BARS.AD_PCI_BAR2,
        PLX_ADDR_SPACE1 = PCI_BARS.AD_PCI_BAR3,
        PLX_ADDR_SPACE2 = PCI_BARS.AD_PCI_BAR4,
        PLX_ADDR_SPACE3 = PCI_BARS.AD_PCI_BAR5,
    };
#endregion

    /* PLX diagnostics plug-and-play and power management events handler
     * function type */
    public delegate void USER_EVENT_CALLBACK(ref WD_EVENT pEvent,
            PLX_Device dev);

    public abstract class PLX_Device
    {
        protected bool m_bIsMaster;
        protected WDC_DEVICE m_wdcDevice = new WDC_DEVICE();
        protected MarshalWdcDevice m_wdcDeviceMarshaler;
        private EEPROM m_EEPROM;
        private USER_EVENT_CALLBACK m_userEventHandler;
        private INT_HANDLER m_intHandler;
        protected EVENT_HANDLER_DOTNET m_eventHandler;
        protected string m_sDeviceLongDesc;
        protected string m_sDeviceShortDesc;
        protected const DWORD PLX_ADDR_REG = (DWORD)PLX_ADDR.PLX_ADDR_REG;
        /* Kernel PlugIn driver name (should be no more than 8 characters) */
        protected string m_sKP_PLX_DRIVER_NAME;

        /* offsets of some useful registers */
        internal DWORD m_dwINTCSR;
        internal DWORD m_dwCNTRL;
        internal DWORD m_dwPROT_AREA;
        internal DWORD[] m_dwLASBA;

#region " constructors "
        /* c'tors */
        internal PLX_Device(WD_PCI_SLOT slot):
            this(0, 0, slot, ""){}

        internal PLX_Device(DWORD dwVendorId, DWORD dwDeviceId,
                WD_PCI_SLOT slot, string sKP_PLX_DRIVER_NAME)
        {
            m_wdcDevice = new WDC_DEVICE();
            m_wdcDevice.id.dwVendorId = dwVendorId;
            m_wdcDevice.id.dwDeviceId = dwDeviceId;
            m_wdcDevice.slot = slot;
            m_wdcDeviceMarshaler = new MarshalWdcDevice();
            m_eventHandler = new EVENT_HANDLER_DOTNET(EventHandler);
            m_sKP_PLX_DRIVER_NAME = sKP_PLX_DRIVER_NAME;
            SetDescription();
        }

        public PLX_Device(PLX_Device dev)
        {
            m_wdcDevice = dev.wdcDevice;
            m_wdcDevice.id.dwVendorId = dev.id.dwVendorId;
            m_wdcDevice.id.dwDeviceId = dev.id.dwDeviceId;
            m_wdcDevice.slot = dev.slot;

            /* the following properties cannot be copied
               from device and are created for each object */
            m_wdcDeviceMarshaler = new MarshalWdcDevice();
            m_eventHandler = new EVENT_HANDLER_DOTNET(EventHandler);
            m_sKP_PLX_DRIVER_NAME = dev.KP_DriverName;
            SetDescription();
        }

#endregion

#region " utilities "
        /*****************
         *  utilities     *
         * ***************/

        /* public methods */

        public static bool IsMasterDevice(ref WD_PCI_SLOT slot)
        {
            WORD wIsMaster = 0;
            DWORD dwStatus = wdc_lib_decl.WDC_PciReadCfgBySlot16(ref slot,
                (DWORD)PCI_CFG_REG.PCI_CR, ref wIsMaster);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_Device.IsMasterDevice:Failed reading " +
                    "the vendor ID from the configuration registers. " +
                    "(Physical Location: " + slot.dwBus.ToString("X") + ":" +
                    slot.dwSlot.ToString("X") + ":"
                    + slot.dwFunction.ToString("X") +")");
                return false;
            }

            if((wIsMaster & (WORD)BITS.BIT2) != 0)
                return true;
            else
                return false;
        }

        public bool IsMaster
        {
            get
            {
                return m_bIsMaster;
            }
        }

        public string KP_DriverName
        {
            get
            {
                return m_sKP_PLX_DRIVER_NAME;
            }
            set
            {
                m_sKP_PLX_DRIVER_NAME = value;
            }
        }

        public IntPtr Handle
        {
            get
            {
                if (m_wdcDevice == null)
                    return IntPtr.Zero;
                return m_wdcDevice.hDev;
            }
            set
            {
                m_wdcDevice.hDev = value;
            }
        }

        public WD_INTERRUPT wdcInterrupt
        {
            get
            {
                return wdcDevice.Int;
            }
            set
            {
                wdcDevice.Int = value;
            }
        }

        public WDC_DEVICE wdcDevice
        {
            get
            {
                return m_wdcDevice;
            }
            set
            {
                m_wdcDevice = value;
            }
        }

        public WD_PCI_ID id
        {
            get
            {
                return m_wdcDevice.id;
            }
            set
            {
                m_wdcDevice.id = value;
            }
        }

        public WD_PCI_SLOT slot
        {
            get
            {
                return m_wdcDevice.slot;
            }
            set
            {
                m_wdcDevice.slot = value;
            }
        }

        public WDC_ADDR_DESC[] AddrDesc
        {
            get
            {
                return m_wdcDevice.pAddrDesc;
            }
            set
            {
                m_wdcDevice.pAddrDesc = value;
            }
        }

        public string[] AddrDescToString(bool bMemOnly)
        {
            int iFirstSpace = (int)(bMemOnly? PLX_ADDR.PLX_ADDR_SPACE0 :
                PLX_ADDR.PLX_ADDR_REG);
            string[] sAddr = new string[AddrDesc.Length - iFirstSpace];
            for (int i = iFirstSpace; i<sAddr.Length + iFirstSpace; ++i)
            {
                sAddr[i - iFirstSpace] = "BAR " +
                    AddrDesc[i].dwAddrSpace.ToString() +
                    ((AddrDesc[i].fIsMemory)? " Memory " : " I/O ");

                if (wdc_lib_decl.WDC_AddrSpaceIsActive(Handle,
                    AddrDesc[i].dwAddrSpace))
                {
                    WD_ITEMS item =
                        m_wdcDevice.cardReg.Card.Item[AddrDesc[i].dwItemIndex];
                    PHYS_ADDR pAddr = (AddrDesc[i].fIsMemory?
                        item.I.Mem.pPhysicalAddr : item.I.IO.pAddr);

                    sAddr[i - iFirstSpace] += pAddr.ToString("X") + " - " +
                        (pAddr + AddrDesc[i].qwBytes - 1).ToString("X") +
                        " (" + AddrDesc[i].qwBytes.ToString("X") + " bytes)";
                }
                else
                {
                    sAddr[i - iFirstSpace] += "Inactive address space";
                }
            }
            return sAddr;
        }

        public  string ToString(BOOL bLong)
        {
            return (bLong) ? m_sDeviceLongDesc: m_sDeviceShortDesc;
        }

        public EEPROM EEprom
        {
            get
            {
                return m_EEPROM;
            }
            set
            {
                m_EEPROM = value;
            }
        }

        public bool IsMySlot(ref WD_PCI_SLOT slot)
        {
            if(m_wdcDevice.slot.dwBus == slot.dwBus &&
                m_wdcDevice.slot.dwSlot == slot.dwSlot &&
                m_wdcDevice.slot.dwFunction == slot.dwFunction)
                return true;

            return false;
        }

        /* private methods */

        protected void SetDescription()
        {
            m_sDeviceLongDesc = string.Format("PLX Device: Vendor ID 0x{0:X}, "
                + "Device ID 0x{1:X}, Physical Location {2:X}:{3:X}:{4:X}",
                id.dwVendorId, id.dwDeviceId, slot.dwBus, slot.dwSlot,
                slot.dwFunction);

            m_sDeviceShortDesc = string.Format("Device " +
                "{0:X},{1:X} {2:X}:{3:X}:{4:X}", id.dwVendorId,
                id.dwDeviceId, slot.dwBus, slot.dwSlot, slot.dwFunction);
        }

        private bool DeviceValidate()
        {
            DWORD i, dwNumAddrSpaces = m_wdcDevice.dwNumAddrSpaces;

            /* NOTE: You can modify the implementation of this function in
             * order to verify that the device has the resources you expect to
             * find */

            /* Verify that the device has at least one active address space */
            for (i = 0; i < dwNumAddrSpaces; i++)
            {
                if (wdc_lib_decl.WDC_AddrSpaceIsActive(Handle, i))
                    return true;
            }

            Log.ErrLog("PLX_Device.DeviceValidate: Device does not have "
                + "any active memory or I/O address spaces " + "(" +
                this.ToString(false) + ")" );
            return false;
        }

#endregion

#region " Device Open/Close "
        /****************************
         *  Device Open & Close     *
         * **************************/

        /* public methods */
        public virtual DWORD DeviceOpen()
        {
            DWORD dwStatus;
            WD_PCI_CARD_INFO deviceInfo = new WD_PCI_CARD_INFO();
            WDC_DEVICE tmpWdcDevice = m_wdcDevice;

            /* Retrieve the device's resources information */
            deviceInfo.pciSlot = slot;
            dwStatus = wdc_lib_decl.WDC_PciGetDeviceInfo(deviceInfo);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_Device.Open: Failed retrieving the "
                    + "device's resources information. Error 0x" +
                    dwStatus.ToString("X") + ": " + utils.Stat2Str(dwStatus) +
                    "(" + this.ToString(false) +")" );
                return dwStatus;
            }

            /* NOTE: You can modify the device's resources information here,
             * if necessary (mainly the deviceInfo.Card.Items array or the
             * items number - deviceInfo.Card.dwItems) in order to register
             * only some of the resources or register only a portion of a
             * specific address space, for example. */

            dwStatus = wdc_lib_decl.WDC_PciDeviceOpen(ref tmpWdcDevice,
                deviceInfo, IntPtr.Zero);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_Device.DeviceOpen: Failed opening a " +
                    "WDC device handle. Error 0x" + dwStatus.ToString("X") +
                    ": " + utils.Stat2Str(dwStatus) + "(" +
                    this.ToString(false) + ")");
                goto Error;
            }

            Log.TraceLog("PLX_Device.DeviceOpen: Opened a PLX device " +
                this.ToString(false));

            /* Open a Kernel PlugIn handle to the device */
            if(m_sKP_PLX_DRIVER_NAME != "")
            {
                PLX_DEV_ADDR_DESC devAddrDesc;
                GCHandle gc_handle;
                IntPtr pDevAddrDesc;

                /* We're calling a c-language method, passing a pointer.
                 * To create this pointer in c#, we use the 'pinned' method and
                 * Marshal allocation, which creates unmanaged memory (which we
                 * must free - no garbage collection) */
                devAddrDesc.dwNumAddrSpaces = m_wdcDevice.dwNumAddrSpaces;
                gc_handle = GCHandle.Alloc(AddrDesc);
                devAddrDesc.AddrDesc =
                        Marshal.UnsafeAddrOfPinnedArrayElement(AddrDesc, 0);
                pDevAddrDesc = Marshal.AllocHGlobal(
                    Marshal.SizeOf(typeof(PLX_DEV_ADDR_DESC)));
                Marshal.StructureToPtr(devAddrDesc, pDevAddrDesc, false);

                dwStatus = wdc_lib_decl.WDC_KernelPlugInOpen(m_wdcDevice,
                        m_sKP_PLX_DRIVER_NAME, pDevAddrDesc);
                if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
                {
                    Log.ErrLog("PLX_Device.DeviceOpen: Failed opening a " +
                        "Kernel PlugIn handle to the KP driver. Error 0x" +
                        dwStatus.ToString("X") + ": " + utils.Stat2Str(dwStatus)
                        + "(" + this.ToString(false) + ")");
                }
                else
                {
                    Log.TraceLog("PLX_Device.DeviceOpen: Opened a handle to " +
                        "Kernel PlugIn driver " + m_sKP_PLX_DRIVER_NAME +
                        this.ToString(false));
                }

                Marshal.FreeHGlobal(pDevAddrDesc);
                gc_handle.Free();
            }

            /* Validate device information */
            if (DeviceValidate() != true)
            {
                dwStatus = (DWORD)wdc_err.WD_NO_RESOURCES_ON_DEVICE;
                goto Error;
            }

            DeviceInit();

            if(EEPROM.EEPROMIsPresent(Handle, m_dwCNTRL))
            {
                if(EEPROM.VPD_Validate(Handle, m_dwCNTRL))
                {
                    m_EEPROM = new EEPROM_VPD(Handle, m_dwCNTRL, m_dwPROT_AREA);
                }
                else
                {
                    switch (m_wdcDevice.id.dwDeviceId)
                    {
                    case 0x9080:
                        m_EEPROM = new EEPROM_RT(Handle, m_dwCNTRL,
                            (DWORD)BITS.BIT5);
                        break;

                    case 0x9030:
                    case 0x9050:
                    case 0x9052:
                    case 0x9054:
                    case 0x9056:
                    case 0x9656:
                    default:
                        m_EEPROM = new EEPROM_RT(Handle, m_dwCNTRL,
                            (DWORD)BITS.BIT7);
                        break;
                    }
                }
            }
            return dwStatus;

Error:
            if (Handle != IntPtr.Zero)
                DeviceClose();

            return dwStatus;
        }

        public virtual bool DeviceClose()
        {
            DWORD dwStatus;

            if (Handle == IntPtr.Zero)
            {
                Log.ErrLog("PLX_Device.DeviceClose: Error - NULL "
                    + "device handle");
                return false;
            }

            /* unregister events*/
            dwStatus = EventUnregister();

            /* Disable interrupts */
            dwStatus = DisableInterrupts();

            /* Close the device */
            dwStatus = wdc_lib_decl.WDC_PciDeviceClose(Handle);
            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_Device.DeviceClose: Failed closing a "
                    + "WDC device handle (0x" + Handle.ToInt64().ToString("X")
                    + ". Error 0x" + dwStatus.ToString("X") + ": " +
                    utils.Stat2Str(dwStatus) + this.ToString(false));
            }
            else
            {
                Log.TraceLog("PLX_Device.DeviceClose: " +
                    this.ToString(false) + " was closed successfully");
            }

            return ((DWORD)wdc_err.WD_STATUS_SUCCESS == dwStatus);
        }

        /* protected methods */
        protected abstract void DeviceInit();

        public void Dispose()
        {
            DeviceClose();
        }

#endregion

#region " Read/Write "

        public DWORD ReadReg8(DWORD dwOffset, ref BYTE bData)
        {
            return wdc_lib_decl.WDC_ReadAddr8(Handle, PLX_ADDR_REG, dwOffset,
                ref bData);
        }

        public DWORD ReadReg16(DWORD dwOffset, ref WORD wData)
        {
            return wdc_lib_decl.WDC_ReadAddr16(Handle, PLX_ADDR_REG, dwOffset,
                ref wData);
        }

        public DWORD ReadReg32(DWORD dwOffset, ref UINT32 u32Data)
        {
            return wdc_lib_decl.WDC_ReadAddr32(Handle, PLX_ADDR_REG, dwOffset,
                ref u32Data);
        }

        public DWORD ReadReg64(DWORD dwOffset, ref UINT64 u64Data)
        {
            return wdc_lib_decl.WDC_ReadAddr64(Handle, PLX_ADDR_REG, dwOffset,
                ref u64Data);
        }

        public DWORD WriteReg8(DWORD dwOffset, BYTE bData)
        {
            return wdc_lib_decl.WDC_WriteAddr8(Handle, PLX_ADDR_REG, dwOffset,
                bData);
        }

        public DWORD WriteReg16(DWORD dwOffset, WORD wData)
        {
            return wdc_lib_decl.WDC_WriteAddr16(Handle, PLX_ADDR_REG, dwOffset,
                wData);
        }

        public DWORD WriteReg32(DWORD dwOffset, UINT32 u32Data)
        {
            return wdc_lib_decl.WDC_WriteAddr32(Handle, PLX_ADDR_REG, dwOffset,
                u32Data);
        }

        public DWORD WriteReg64(DWORD dwOffset, UINT64 u64Data)
        {
            return wdc_lib_decl.WDC_WriteAddr64(Handle, PLX_ADDR_REG, dwOffset,
                u64Data);
        }

#endregion

#region " Interrupts "
        /****************************
         *       Interrupts         *
         * **************************/

        /* public methods */
        public bool IsEnabledInt()
        {
            return wdc_lib_decl.WDC_IntIsEnabled(this.Handle);
        }

    public abstract DWORD CreateIntTransCmds(out WD_TRANSFER[] pIntTransCmds,
	    out DWORD dwNumCmds);
    public abstract void DisableCardInts();

    public bool IsMsiInt()
    {
        return wdc_defs_macros.WDC_INT_IS_MSI(
            wdc_defs_macros.WDC_GET_ENABLED_INT_TYPE(wdcDevice));
    }

    public DWORD GetEnableIntLastMsg()
    {
        return wdc_defs_macros.WDC_GET_ENABLED_INT_LAST_MSG(wdcDevice);
    }

    public string WDC_DIAG_IntTypeDescriptionGet()
    {
        DWORD dwIntType = wdc_defs_macros.WDC_GET_ENABLED_INT_TYPE(wdcDevice);

        if ((dwIntType & (DWORD)(WD_INTERRUPT_TYPE.INTERRUPT_MESSAGE_X)) != 0)
        {
            return "Extended Message-Signaled Interrupt (MSI-X)";
        }
        else if ((dwIntType & (DWORD)(WD_INTERRUPT_TYPE.INTERRUPT_MESSAGE))
            != 0)
        {
            return "Message-Signaled Interrupt (MSI)";
        }
        else if ((dwIntType &
            (DWORD)(WD_INTERRUPT_TYPE.INTERRUPT_LEVEL_SENSITIVE)) != 0)
        {
            return "Level-Sensitive Interrupt";
        }

        return "Edge-Triggered Interrupt";
    }

    protected DWORD EnableInterrupts(INT_HANDLER intHandler, DWORD dwOptions,
        IntPtr pData)
    {
        WD_TRANSFER[] pIntTransCmds = null;
        DWORD dwNumCmds;

        if(intHandler == null)
        {
            Log.ErrLog("PLX_Device.EnableInterrupts: interrupt handler is " +
                "null (" + this.ToString(false) + ")" );
            return (DWORD)wdc_err.WD_INVALID_PARAMETER;
        }

        if(wdc_lib_decl.WDC_IntIsEnabled(Handle))
        {
            Log.ErrLog("PLX_Device.EnableInterrupts: "
                + "interrupts are already enabled (" +
                this.ToString(false) + ")" );
            return (DWORD)wdc_err.WD_OPERATION_ALREADY_DONE;
        }

        DWORD dwStatus = CreateIntTransCmds(out pIntTransCmds, out dwNumCmds);
        if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            return dwStatus;

        m_intHandler = intHandler;

        dwStatus = wdc_lib_decl.WDC_IntEnable(wdcDevice, pIntTransCmds,
            dwNumCmds, dwOptions, m_intHandler, pData,
            wdc_defs_macros.WDC_IS_KP(m_wdcDevice));

        if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
        {
            Log.ErrLog("PLX_Device.EnableInterrupts: Failed "
                + "enabling interrupts. Error " + dwStatus.ToString("X") + ": "
                + utils.Stat2Str(dwStatus) + "(" + this.ToString(false) + ")");
            m_intHandler = null;
            return dwStatus;
        }

        return dwStatus;
    }

    public virtual DWORD DisableInterrupts()
    {
        DWORD dwStatus;

        if (!wdc_lib_decl.WDC_IntIsEnabled(this.Handle))
        {
            Log.ErrLog("PLX_Device.DisableInterrupts: interrupts are already" +
                    "disabled... (" + this.ToString(false) + ")" );
            return (DWORD)wdc_err.WD_OPERATION_ALREADY_DONE;
        }

        DisableCardInts();

        dwStatus = wdc_lib_decl.WDC_IntDisable(m_wdcDevice);
        if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
        {
            Log.ErrLog("PLX_Device.DisableInterrupts: Failed to" +
                "disable interrupts. Error " + dwStatus.ToString("X")
                + ": " + utils.Stat2Str(dwStatus) + " (" +
                this.ToString(false) + ")" );
        }
        else
        {
            Log.TraceLog("PLX_Device.DisableInterrupts: Interrupts are " +
                    "disabled (" + this.ToString(false) + ")");
        }

        return dwStatus;
    }

#endregion

#region " Local Bus Read/Write "
        /****************************
         *    Local Bus Read/Write  *
         * **************************/

        /* public methods */
        public DWORD ReadAddrLocalBlock(PLX_ADDR addrSpace,
            DWORD dwLocalAddr, DWORD dwBytes, IntPtr pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_ReadAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD ReadAddrLocalBlock(PLX_ADDR addrSpace,
            DWORD dwLocalAddr, DWORD dwBytes, UInt64[] pData,
            WDC_ADDR_MODE mode, WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_ReadAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD ReadAddrLocalBlock(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            DWORD dwBytes, DWORD[] pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_ReadAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD ReadAddrLocalBlock(PLX_ADDR addrSpace,
            DWORD dwLocalAddr, DWORD dwBytes, WORD[] pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_ReadAddrBlock(Handle, (DWORD)addrSpace,
                qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD ReadAddrLocalBlock(PLX_ADDR addrSpace,
            DWORD dwLocalAddr, DWORD dwBytes, byte[] pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_ReadAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD WriteAddrLocalBlock(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            DWORD dwBytes, IntPtr pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_WriteAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD WriteAddrLocalBlock(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            DWORD dwBytes, UInt64[] pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_WriteAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD WriteAddrLocalBlock(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            DWORD dwBytes, DWORD[] pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_WriteAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD WriteAddrLocalBlock(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            DWORD dwBytes, WORD[] pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_WriteAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD WriteAddrLocalBlock(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            DWORD dwBytes, byte[] pData, WDC_ADDR_MODE mode,
            WDC_ADDR_RW_OPTIONS options)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);

            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_WriteAddrBlock(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, dwBytes, pData, mode, options);
        }

        public DWORD ReadAddrLocal8(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            ref byte bData)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);
            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_ReadAddr8(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, ref bData);
        }

        public DWORD WriteAddrLocal8(PLX_ADDR addrSpace,
            DWORD dwLocalAddr, byte bData)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);
            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_WriteAddr8(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, bData);
        }

        public DWORD ReadAddrLocal16(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            ref WORD wData)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);
            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_ReadAddr16(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, ref wData);
        }

        public DWORD WriteAddrLocal16(PLX_ADDR addrSpace,
            DWORD dwLocalAddr, WORD wData)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);
            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_WriteAddr16(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, wData);
        }

        public DWORD ReadAddrLocal32(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            ref uint u32Data)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);
            LocalAddrSetMode(addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_ReadAddr32(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, ref u32Data);
        }

        public DWORD WriteAddrLocal32(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            uint u32Data)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);
            LocalAddrSetMode( addrSpace, dwLocalAddr);
            return wdc_lib_decl.WDC_WriteAddr32(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, u32Data);
        }

        public DWORD ReadAddrLocal64(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            ref ulong u64Data)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);
            LocalAddrSetMode(addrSpace, dwLocalAddr);

            return wdc_lib_decl.WDC_ReadAddr64(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, ref u64Data);
        }

        public DWORD WriteAddrLocal64(PLX_ADDR addrSpace, DWORD dwLocalAddr,
            ulong u64Data)
        {
            KPTR qwOffset = (KPTR)(MASK_LOCAL(addrSpace) & dwLocalAddr);
            LocalAddrSetMode(addrSpace, dwLocalAddr);

            return wdc_lib_decl.WDC_WriteAddr64(Handle, (DWORD)addrSpace,
                (DWORD)qwOffset, u64Data);
        }

        /* private methods */
        private UINT64 MASK_LOCAL(PLX_ADDR addrSpace)
        {
            return AddrDesc[(int)addrSpace].qwBytes - 1;
        }

        private DWORD LocalAddrSetMode(PLX_ADDR addrSpace, DWORD dwLocalAddr)
        {
            DWORD dwLocalBase = (DWORD)(dwLocalAddr & ~MASK_LOCAL(addrSpace)) |
                (DWORD)BITS.BIT0;
            DWORD dwOffset = m_dwLASBA[(DWORD)addrSpace - 2];

            return wdc_lib_decl.WDC_WriteAddr32(Handle, PLX_ADDR_REG,
                dwOffset, (uint)dwLocalBase);
        }

#endregion

#region " Events "
        /****************************
         *          Events          *
         ****************************/

        /* public methods */
        public bool IsEventRegistered()
        {
            if (Handle == IntPtr.Zero)
                return false;

            return wdc_lib_decl.WDC_EventIsRegistered(Handle);
        }

        public DWORD EventRegister(USER_EVENT_CALLBACK userEventHandler)
        {
            DWORD dwStatus;
            DWORD dwActions = (DWORD)windrvr_consts.WD_ACTIONS_ALL;
            /* TODO: Modify the above to set up the plug-and-play/power
             * management events for which you wish to receive notifications.
             * dwActions can be set to any combination of the WD_EVENT_ACTION
             * flags defined in windrvr.h */

            if(userEventHandler == null)
            {
                Log.ErrLog("PLX_Device.EventRegister: user callback is "
                    + "null");
                return (DWORD)wdc_err.WD_INVALID_PARAMETER;
            }

            /* Check if event is already registered */
            if(wdc_lib_decl.WDC_EventIsRegistered(Handle))
            {
                Log.ErrLog("PLX_Device.EventRegister: Events are already "
                    + "registered ...");
                return (DWORD)wdc_err.WD_OPERATION_ALREADY_DONE;
            }

            m_userEventHandler = userEventHandler;

            /* Register event */
            dwStatus = wdc_lib_decl.WDC_EventRegister(m_wdcDevice, dwActions,
                m_eventHandler, Handle, wdc_defs_macros.WDC_IS_KP(m_wdcDevice));

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_Device.EventRegister: Failed to register "
                    + "events. Error 0x"
                    + dwStatus.ToString("X") + utils.Stat2Str(dwStatus));
                m_userEventHandler = null;
            }
            else
            {
                Log.TraceLog("PLX_Device.EventRegister: events are " +
                    " registered (" + this.ToString(false) +")" );
            }

            return dwStatus;
        }

        public DWORD EventUnregister()
        {
            DWORD dwStatus;

            if (!wdc_lib_decl.WDC_EventIsRegistered(Handle))
            {
                Log.ErrLog("PLX_Device.EventUnregister: No events " +
                    "currently registered ...(" + this.ToString(false) + ")" );
                return (DWORD)wdc_err.WD_OPERATION_ALREADY_DONE;
            }

            dwStatus = wdc_lib_decl.WDC_EventUnregister(m_wdcDevice);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_Device.EventUnregister: Failed to " +
                    " unregister events. Error 0x" + dwStatus.ToString("X") +
                    ": " + utils.Stat2Str(dwStatus) + "(" +
                    this.ToString(false) + ")");
            }
            else
            {
                Log.TraceLog("PLX_Device.EventUnregister: Unregistered " +
                    " events (" + this.ToString(false) + ")" );
            }

            return dwStatus;
        }

        /* private methods */

        /* event callback method */
        private void EventHandler(IntPtr pWdEvent, IntPtr pDev)
        {
            MarshalWdEvent wdEventMarshaler = new MarshalWdEvent();
            WD_EVENT wdEvent =
                (WD_EVENT)wdEventMarshaler.MarshalNativeToManaged(pWdEvent);

            m_wdcDevice.Event =
                (WD_EVENT)m_wdcDeviceMarshaler.MarshalDevWdEvent(pDev);
            if(m_userEventHandler != null)
                m_userEventHandler(ref wdEvent, this);
        }
#endregion
    }
}

