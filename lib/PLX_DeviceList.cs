// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using System.Collections;

using Jungo.wdapi_dotnet;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using DWORD = System.UInt32;
using BOOL = System.Boolean;
using WDC_DRV_OPEN_OPTIONS = System.UInt32;

namespace Jungo.plx_lib
{
    public class PLX_DeviceList: ArrayList
    {
        /* WinDriver license registration string */
        /* TODO: When using a registered WinDriver version, replace the license
                 string below with the development license in order to use on
                 the development machine.
                 Once you require to distribute the driver's package to other
                 machines, please replace the string with a distribution
                 license */
        private string PCI_DEFAULT_LICENSE_STRING = "12345abcde1234.license";

        private static PLX_DeviceList instance;

        public static PLX_DeviceList TheDeviceList()
        {
            if (instance == null)
            {
                instance = new PLX_DeviceList();
            }
            return instance;
        }

        private PLX_DeviceList(){}

        public DWORD Init()
        {
            DWORD dwStatus = wdc_lib_decl.WDC_SetDebugOptions(wdc_lib_consts.WDC_DBG_DEFAULT,
                null);
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                Log.ErrLog("PLX_DeviceList: Failed to initialize debug options for " +
                    " WDC library. Error 0x" + dwStatus.ToString("X") + 
                    utils.Stat2Str(dwStatus));        
                return dwStatus;
            }  
            
            dwStatus = wdc_lib_decl.WDC_DriverOpen(
                (WDC_DRV_OPEN_OPTIONS)wdc_lib_consts.WDC_DRV_OPEN_DEFAULT,
                PCI_DEFAULT_LICENSE_STRING);
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                Log.ErrLog("PLX_DeviceList: Failed to initialize the WDC library. "
                    + "Error 0x" + dwStatus.ToString("X") + utils.Stat2Str(dwStatus));
                return dwStatus;
            }            
            return (DWORD)wdc_err.WD_STATUS_SUCCESS;
        }

        public PLX_Device Get(int index)
        {
            if(index >= this.Count || index < 0)
                return null;
            return (PLX_Device)this[index];
        }

        public PLX_Device Get(WD_PCI_SLOT slot)
        {
            foreach(PLX_Device device in this)
            {
                if(device.IsMySlot(ref slot))
                    return device;
            }
            return null;
        }

        public int Populate(DWORD dwVendorId, DWORD dwDeviceId)
        {
            return Populate(dwVendorId, dwDeviceId, "");
        }

        public int Populate(DWORD dwVendorId, DWORD dwDeviceId, string sKP_DRIVER_NAME)
        {
            int retIndex=0;
            DWORD dwStatus;
            WDC_PCI_SCAN_RESULT scanResult = new WDC_PCI_SCAN_RESULT();

            dwStatus = wdc_lib_decl.WDC_PciScanDevices(dwVendorId, 
                dwDeviceId, scanResult);

            if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
            {
                Log.ErrLog("PLX_Sample.FindPLXDevices: Failed scanning "
                    + "the PCI bus");
                return -1;
            }

            if (scanResult.dwNumDevices == 0)
            {
                Log.ErrLog("PLX_Sample.FindPLXDevices: No matching PLX " +
                    "device was found for search criteria " + dwVendorId.ToString("X") 
                    + ", " + dwDeviceId.ToString("X"));
                return -1;
            }

            for (int i = 0; i < scanResult.dwNumDevices; ++i)
            {
                PLX_Device device;
                WD_PCI_SLOT slot = scanResult.deviceSlot[i];

                if(PLX_Device.IsMasterDevice(ref slot))
                {
                    device = new
                        PLX_MasterDevice(scanResult.deviceId[i].dwVendorId,
                            scanResult.deviceId[i].dwDeviceId, slot, sKP_DRIVER_NAME);
                }
                else
                {
                    device = new
                        PLX_TargetDevice(scanResult.deviceId[i].dwVendorId,
                            scanResult.deviceId[i].dwDeviceId, slot, sKP_DRIVER_NAME);
                }

                retIndex = this.Add(device);                                
            }                        
            return retIndex;
        }

        public void Dispose()
        {
            foreach (PLX_Device device in this)
                device.Dispose();
            this.Clear();

            DWORD dwStatus = wdc_lib_decl.WDC_DriverClose();
            if(dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                Exception excp = new Exception("PLX_Sample. PLX_Sample_Closing: " +
                    "Failed to uninit the WDC library. Error 0x" +
                    dwStatus.ToString("X") + utils.Stat2Str(dwStatus));
                throw excp;
            }
        }
    };
}
 
