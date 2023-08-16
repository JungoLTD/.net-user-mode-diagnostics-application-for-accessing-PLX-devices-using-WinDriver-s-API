// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

using Jungo.wdapi_dotnet;
using Jungo.plx_lib;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using BYTE = System.Byte;
using BOOL = System.Boolean;
using UINT32 = System.UInt32;
using UINT64 = System.UInt64;
using WDC_DEVICE_HANDLE = System.IntPtr;

namespace Jungo.PLX_Sample
{
    public enum RW
    {
        READ = 0,
        WRITE = 1,
        READ_ALL = 2
    }

    public enum TRANSFER_TYPE
    {
        BLOCK = 0,
        NONBLOCK = 1
    }

    public enum COMPLETION
    {
        INTERRUPT = 0,
        POLLING = 1
    }

    public enum ALLOCATION_TYPE
    {
        SG = 0,
        CONTIG,
        TRANSACTION_SG,
        TRANSACTION_CONTIG,
    }

    public enum ACTION_TYPE
    {
        DMA = 0,
        CFG = 1,
        RT = 2,
        EEPROM = 3
    }

    public class PLX_Sample : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components = null;

        private DWORD PLX_DEFAULT_VENDOR_ID = 0x10b5;
        private DWORD PLX_DEFAULT_DEVICE_ID = 0x0;
        private PLX_DeviceList plxDevList;
        private DmaInfoList dmaList;
        private Log log;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuAddrSpaces;
        private System.Windows.Forms.MenuItem menuEnableInt;
        private System.Windows.Forms.MenuItem menuDMA;
        private System.Windows.Forms.MenuItem menuEvents;
        private System.Windows.Forms.MenuItem menuEEPROM;
        private System.Windows.Forms.MenuItem menuRegisterEvent;
        private System.Windows.Forms.MenuItem menuChannel0;
        private System.Windows.Forms.MenuItem menuChannel1;
        private System.Windows.Forms.MenuItem menuOpenChn0;
        private System.Windows.Forms.MenuItem menuTransferChn0;
        private System.Windows.Forms.MenuItem menuOpenChn1;
        private System.Windows.Forms.MenuItem menuCfgSpace;
        private System.Windows.Forms.MenuItem menuRTRegs;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.Label lblPLXDev;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btLog;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.ListBox lstBxDevices;
        private System.Windows.Forms.MenuItem menuTransferChn1;
        private System.Windows.Forms.MenuItem menuCfgOffset;
        private System.Windows.Forms.MenuItem menuCfgReg;
        private System.Windows.Forms.MenuItem menuAddrRW;
        private System.Windows.Forms.MenuItem menuLocalRW;
        private System.Windows.Forms.MenuItem menuRTRegsRW;
        private System.Windows.Forms.MenuItem menuEEpromRW;
        private System.Windows.Forms.MenuItem menuInt0;
        private System.Windows.Forms.MenuItem menuMasterInterrupts;
        private System.Windows.Forms.MenuItem menuTargetInterrupts;
        private System.Windows.Forms.MenuItem menuGenerateInt;
        private System.Windows.Forms.MenuItem menuTargetEnableInt;
        private System.Windows.Forms.Button btDevice;
        private System.Windows.Forms.MenuItem menuInt1;

        public PLX_Sample()
        {
            InitializeComponent();

            dmaList = new DmaInfoList();
            log = new Log(new Log.TRACE_LOG(TraceLog),
               new Log.ERR_LOG(ErrLog));
            plxDevList = PLX_DeviceList.TheDeviceList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

#region Windows Form Designer generated code
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        private void InitializeComponent()
        {
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lstBxDevices = new System.Windows.Forms.ListBox();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuAddrSpaces = new System.Windows.Forms.MenuItem();
            this.menuAddrRW = new System.Windows.Forms.MenuItem();
            this.menuLocalRW = new System.Windows.Forms.MenuItem();
            this.menuTargetInterrupts = new System.Windows.Forms.MenuItem();
            this.menuTargetEnableInt = new System.Windows.Forms.MenuItem();
            this.menuGenerateInt = new System.Windows.Forms.MenuItem();
            this.menuMasterInterrupts = new System.Windows.Forms.MenuItem();
            this.menuEnableInt = new System.Windows.Forms.MenuItem();
            this.menuInt0 = new System.Windows.Forms.MenuItem();
            this.menuInt1 = new System.Windows.Forms.MenuItem();
            this.menuDMA = new System.Windows.Forms.MenuItem();
            this.menuChannel0 = new System.Windows.Forms.MenuItem();
            this.menuOpenChn0 = new System.Windows.Forms.MenuItem();
            this.menuTransferChn0 = new System.Windows.Forms.MenuItem();
            this.menuChannel1 = new System.Windows.Forms.MenuItem();
            this.menuOpenChn1 = new System.Windows.Forms.MenuItem();
            this.menuTransferChn1 = new System.Windows.Forms.MenuItem();
            this.menuEvents = new System.Windows.Forms.MenuItem();
            this.menuRegisterEvent = new System.Windows.Forms.MenuItem();
            this.menuEEPROM = new System.Windows.Forms.MenuItem();
            this.menuEEpromRW = new System.Windows.Forms.MenuItem();
            this.menuCfgSpace = new System.Windows.Forms.MenuItem();
            this.menuCfgOffset = new System.Windows.Forms.MenuItem();
            this.menuCfgReg = new System.Windows.Forms.MenuItem();
            this.menuRTRegs = new System.Windows.Forms.MenuItem();
            this.menuRTRegsRW = new System.Windows.Forms.MenuItem();
            this.lblLog = new System.Windows.Forms.Label();
            this.lblPLXDev = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btLog = new System.Windows.Forms.Button();
            this.btExit = new System.Windows.Forms.Button();
            this.btDevice = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // txtLog
            this.txtLog.AutoSize = false;
            this.txtLog.Location = new System.Drawing.Point(24, 176);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(624, 208);
            this.txtLog.TabIndex = 24;
            this.txtLog.Text = "";
            this.txtLog.ReadOnly = true;
            // lstBxDevices
            this.lstBxDevices.Location = new System.Drawing.Point(24, 80);
            this.lstBxDevices.Name = "lstBxDevices";
            this.lstBxDevices.Size = new System.Drawing.Size(416, 56);
            this.lstBxDevices.TabIndex = 27;
            this.lstBxDevices.SelectedIndexChanged +=
                new System.EventHandler(this.lstBxDevices_SelectedIndexChanged);
            this.lstBxDevices.DoubleClick += new
                System.EventHandler(this.lstBxDevices_DoubleClicked);
            // mainMenu1
            this.mainMenu1.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuAddrSpaces,
                this.menuTargetInterrupts,
                this.menuMasterInterrupts,
                this.menuDMA,
                this.menuEvents,
                this.menuEEPROM,
                this.menuCfgSpace,
                this.menuRTRegs});
            // menuAddrSpaces
            this.menuAddrSpaces.Index = 0;
            this.menuAddrSpaces.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuAddrRW,
                this.menuLocalRW});
            this.menuAddrSpaces.Text = "Address Spaces";
            // menuAddrRW
            this.menuAddrRW.Index = 0;
            this.menuAddrRW.Text = "Read/Write Address Space";
            this.menuAddrRW.Click += new
                System.EventHandler(this.menuAddrRW_Click);
            // menuLocalRW
            this.menuLocalRW.Index = 1;
            this.menuLocalRW.Text = "Read/Write Local Bus";
            this.menuLocalRW.Click += new
                System.EventHandler(this.menuLocalRW_Click);
            // menuTargetInterrupts
            this.menuTargetInterrupts.Index = 1;
            this.menuTargetInterrupts.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuTargetEnableInt,
                this.menuGenerateInt});
            this.menuTargetInterrupts.Text = "Interrupts";
            this.menuTargetInterrupts.Select +=
                new System.EventHandler(this.menuTargetInterrupts_Select);
            // menuTargetEnableInt
            this.menuTargetEnableInt.Index = 0;
            this.menuTargetEnableInt.Text = "Enable Interrupts";
            this.menuTargetEnableInt.Click += new
                System.EventHandler(this.menuTargetEnableInt_Click);
            // menuGenerateInt
            this.menuGenerateInt.Index = 1;
            this.menuGenerateInt.Text = "Generate Interrupt (slave only)";
            this.menuGenerateInt.Click += new
                System.EventHandler(this.menuGenerateInt_Click);
            // menuMasterInterrupts
            this.menuMasterInterrupts.Index = 2;
            this.menuMasterInterrupts.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuEnableInt});
            this.menuMasterInterrupts.Text = "Interrupts";
            this.menuMasterInterrupts.Select += new
                EventHandler(menuMasterInterrupts_Select);
            // menuEnableInt
            this.menuEnableInt.Index = 0;
            this.menuEnableInt.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuInt0,
                this.menuInt1});
            this.menuEnableInt.Text = "Enable Interrupts";
            this.menuEnableInt.Select += new
                System.EventHandler(this.menuEnableInt_Select);
            // menuInt0
            this.menuInt0.Index = 0;
            this.menuInt0.Text = "On DMA Channel 0";
            this.menuInt0.Click += new System.EventHandler(this.menuInt0_Click);
            // menuInt1
            this.menuInt1.Index = 1;
            this.menuInt1.Text = "On DMA Channel 1";
            this.menuInt1.Click += new
                System.EventHandler(this.menuInt1_Click);
            // menuDMA
            this.menuDMA.Index = 3;
            this.menuDMA.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuChannel0,
                this.menuChannel1});
            this.menuDMA.Text = "DMA";
            // menuChannel0
            this.menuChannel0.Index = 0;
            this.menuChannel0.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuOpenChn0,
                this.menuTransferChn0});
            this.menuChannel0.Text = "Channel 0";
            this.menuChannel0.Select += new
                System.EventHandler(this.menuChannel0_Select);
            // menuOpenChn0
            this.menuOpenChn0.Index = 0;
            this.menuOpenChn0.Text = "Open";
            this.menuOpenChn0.Click += new
                System.EventHandler(this.menuOpenChn0_Click);
            // menuTransferChn0
            this.menuTransferChn0.Index = 1;
            this.menuTransferChn0.Text = "Read/Write";
            this.menuTransferChn0.Click += new
                System.EventHandler(this.menuTransferChn0_Click);
            // menuChannel1
            this.menuChannel1.Index = 1;
            this.menuChannel1.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuOpenChn1,
                this.menuTransferChn1});
            this.menuChannel1.Text = "Channel 1";
            this.menuChannel1.Select += new
                System.EventHandler(this.menuChannel1_Select);
            // menuOpenChn1
            this.menuOpenChn1.Index = 0;
            this.menuOpenChn1.Text = "Open";
            this.menuOpenChn1.Click += new
                System.EventHandler(this.menuOpenChn1_Click);
            // menuTransferChn1
            this.menuTransferChn1.Index = 1;
            this.menuTransferChn1.Text = "Read/Write";
            this.menuTransferChn1.Click += new
                System.EventHandler(this.menuTransferChn1_Click);
            // menuEvents
            this.menuEvents.Index = 4;
            this.menuEvents.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuRegisterEvent});
            this.menuEvents.Text = "Events";
            this.menuEvents.Select += new
                System.EventHandler(this.menuEvents_Select);
            // menuRegisterEvent
            this.menuRegisterEvent.Index = 0;
            this.menuRegisterEvent.Text = "Regsiter Events";
            this.menuRegisterEvent.Click += new
                System.EventHandler(this.menuRegisterEvent_Click);
            // menuEEPROM
            this.menuEEPROM.Index = 5;
            this.menuEEPROM.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuEEpromRW});
            this.menuEEPROM.Text = "EEPROM";
            // menuEEpromRW
            this.menuEEpromRW.Index = 0;
            this.menuEEpromRW.Text = "Read/Write EEPROM";
            this.menuEEpromRW.Click += new
                System.EventHandler(this.menuEEpromRW_Click);
            // menuCfgSpace
            this.menuCfgSpace.Index = 6;
            this.menuCfgSpace.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuCfgOffset,
                this.menuCfgReg});
            this.menuCfgSpace.Text = "Configuration Space";
            // menuCfgOffset
            this.menuCfgOffset.Index = 0;
            this.menuCfgOffset.Text = "By Offset ";
            this.menuCfgOffset.Click += new
                System.EventHandler(this.menuCfgOffset_Click);
            // menuCfgReg
            this.menuCfgReg.Index = 1;
            this.menuCfgReg.Text = "By Register";
            this.menuCfgReg.Click += new
                System.EventHandler(this.menuCfgReg_Click);
            // menuRTRegs
            this.menuRTRegs.Index = 7;
            this.menuRTRegs.MenuItems.AddRange(new
                System.Windows.Forms.MenuItem[] {
                this.menuRTRegsRW});
            this.menuRTRegs.Text = "RunTime Registers";
            // menuRTRegsRW
            this.menuRTRegsRW.Index = 0;
            this.menuRTRegsRW.Text = "Read/Write RT Registers";
            this.menuRTRegsRW.Click += new
                System.EventHandler(this.menuRTRegsRW_Click);
            // lblLog
            this.lblLog.Location = new System.Drawing.Point(24, 152);
            this.lblLog.Name = "lblLog";
            this.lblLog.TabIndex = 28;
            this.lblLog.Text = "Log";
            // lblPLXDev
            this.lblPLXDev.Location = new System.Drawing.Point(24, 56);
            this.lblPLXDev.Name = "lblPLXDev";
            this.lblPLXDev.Size = new System.Drawing.Size(120, 23);
            this.lblPLXDev.TabIndex = 29;
            this.lblPLXDev.Text = "PLX Devices Found:";
            // label1
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif",
                9.75F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 23);
            this.label1.TabIndex = 30;
            this.label1.Text = "Select a device to activate its menu";
            // btLog
            this.btLog.Location = new System.Drawing.Point(672, 208);
            this.btLog.Name = "btLog";
            this.btLog.Size = new System.Drawing.Size(80, 40);
            this.btLog.TabIndex = 31;
            this.btLog.Text = "Clear Log";
            this.btLog.Click += new System.EventHandler(this.btLog_Click);
            // btExit
            this.btExit.Location = new System.Drawing.Point(672, 304);
            this.btExit.Name = "btExit";
            this.btExit.Size = new System.Drawing.Size(80, 40);
            this.btExit.TabIndex = 32;
            this.btExit.Text = "Exit";
            this.btExit.Click += new System.EventHandler(this.btExit_Click);
            // btDevice
            this.btDevice.Location = new System.Drawing.Point(464, 96);
            this.btDevice.Name = "btDevice";
            this.btDevice.Size = new System.Drawing.Size(112, 23);
            this.btDevice.TabIndex = 33;
            this.btDevice.Text = "Open Device";
            this.btDevice.Click += new System.EventHandler(this.btDevice_Click);
            // PLX_Sample
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(768, 409);
            this.Controls.Add(this.btDevice);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.btLog);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPLXDev);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.lstBxDevices);
            this.Controls.Add(this.txtLog);
            this.Menu = this.mainMenu1;
            this.Name = "PLX_Sample";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PLX .NET Sample";
            this.Load += new System.EventHandler(this.PLX_Sample_Load);
            this.Closed += new System.EventHandler(this.PLX_Sample_Closing);
            this.ResumeLayout(false);
        }
#endregion

        /// The main entry point for the application.
        [STAThread]
            static void Main()
            {
                Application.Run(new PLX_Sample());
            }

        /* Open a handle to a device */
        private bool DeviceOpen(int iSelectedIndex)
        {
            DWORD dwStatus;
            PLX_Device device = plxDevList.Get(iSelectedIndex);

            dwStatus = device.DeviceOpen();
            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                Log.ErrLog("PLX_Sample.DeviceOpen: Failed opening a " +
                    "handle to the device (" + device.ToString(false) + ")" );
                return false;
            }
            Log.TraceLog("PLX_Sample.DeviceOpen: The device was successfully" +
                " opened. You can now activate the device through the enabled" +
                " menu above");
            return true;
        }

        /* Close handle to a PLX device */
        private BOOL DeviceClose(int iSelectedIndex)
        {
            PLX_Device device = plxDevList.Get(iSelectedIndex);
            BOOL bStatus = false;
            DmaInfoList dmaDevList = dmaList.GetItems(device.Handle);

            foreach (DmaInfo dmaInfo in dmaDevList)
            {
                dmaList.Remove(dmaInfo);
                dmaInfo.Close();
                Log.TraceLog("DMA Channel " +
                    (((DWORD)dmaInfo.DmaChannel == 0) ? "0 " : "1 ") +
                    "was closed. {" + device.ToString(false) + "}");
            }

            if (device.Handle != IntPtr.Zero &&
                !(bStatus = device.DeviceClose()))
            {
                Log.ErrLog("PLX_Sample.DeviceClose: Failed closing PLX "
                    + "device (" + device.ToString(false) + ")");
            }
            else
            {
                device.Handle = IntPtr.Zero;
            }

            return bStatus;
        }

        private void PLX_Sample_Load(object sender, System.EventArgs e)
        {
            DWORD dwStatus = plxDevList.Init();

            if (dwStatus != (DWORD)wdc_err.WD_STATUS_SUCCESS)
                goto Error;

            int index = plxDevList.Populate(PLX_DEFAULT_VENDOR_ID,
                PLX_DEFAULT_DEVICE_ID);
            if (index < 0)
                goto Error;

            foreach(PLX_Device dev in plxDevList)
                lstBxDevices.Items.Add(dev.ToString(true));
            lstBxDevices.SelectedIndex = 0;

            return;
Error:
            DisableMenu();
            btDevice.Enabled = false;
        }

        private void PLX_IntHandler(PLX_TargetDevice dev)
        {
            Log.TraceLog("interrupt for device {" + dev.ToString(false) +
                "} received!");

            Log.TraceLog("Interrupt Type: " +
                dev.WDC_DIAG_IntTypeDescriptionGet());

            if (dev.IsMsiInt())
                Log.TraceLog("Message Data: " + dev.GetEnableIntLastMsg());
        }

        private void PLX_DMAIntHandler(PLX_MasterDevice dev, IntPtr pWdDma)
        {
            MarshalWdDma m_wdDmaMarshaler = new MarshalWdDma();
            WD_DMA wdDma =
                (WD_DMA)m_wdDmaMarshaler.MarshalNativeToManaged(pWdDma);
            BOOL bIsRead =
                (wdDma.dwOptions & (DWORD)WD_DMA_OPTIONS.DMA_FROM_DEVICE) != 0;
            BOOL bIsDmaTransaction =
                (wdDma.dwOptions & (DWORD)WD_DMA_OPTIONS.DMA_TRANSACTION) != 0;

            byte[] data = new byte[wdDma.dwBytes];

            if (bIsDmaTransaction)
            {
                DWORD dwStatus = wdc_lib_decl.WDC_DMATransferCompletedAndCheck(
                    pWdDma, false);

                if (dwStatus ==
                    unchecked((DWORD)wdc_err.WD_MORE_PROCESSING_REQUIRED))
                {
                    Log.TraceLog("Got DMA interrupt (DMA transfer has been " +
                        "finished [" + wdDma.dwPages + " pages])");
                    dev.DMAStart(dev.InterruptsDmaBuffer);
                }
                else if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                {
                    wdc_lib_decl.WDC_DMATransactionRelease(pWdDma);
                    Log.TraceLog("DMA transaction has been completed");

                    Marshal.Copy(wdDma.pUserAddr, data, 0, (int)wdDma.dwBytes);
                    Log.TraceLog((bIsRead ? "read: " : "wrote: ") +
                        diag_lib.DisplayHexBuffer(data, wdDma.dwBytes));
                }
            }
            else
            {
                Marshal.Copy(wdDma.pUserAddr, data, 0, (int)wdDma.dwBytes);
                Log.TraceLog("interrupt for device {" + dev.ToString(false) +
                    "} received! " + Environment.NewLine +
                    (bIsRead ? "read: " : "wrote: ") +
                    diag_lib.DisplayHexBuffer(data, wdDma.dwBytes));

                Log.TraceLog("Interrupt Type: " +
                    dev.WDC_DIAG_IntTypeDescriptionGet());

                if (dev.IsMsiInt())
                    Log.TraceLog("Message Data: " + dev.GetEnableIntLastMsg());
            }
        }

        private void PLX_EventHandler(ref WD_EVENT wdEvent, PLX_Device dev)
        {
            string sAction;

            switch((WD_EVENT_ACTION)wdEvent.dwAction)
            {
                case WD_EVENT_ACTION.WD_INSERT:
                    sAction = "WD_INSERT";
                    break;
                case WD_EVENT_ACTION.WD_REMOVE:
                    sAction = "WD_REMOVE";
                    break;
                case WD_EVENT_ACTION.WD_POWER_CHANGED_D0:
                    sAction = "WD_POWER_CHANGED_D0";
                    break;
                case WD_EVENT_ACTION.WD_POWER_CHANGED_D1:
                    sAction = "WD_POWER_CHANGED_D1";
                    break;
                case WD_EVENT_ACTION.WD_POWER_CHANGED_D2:
                    sAction = "WD_POWER_CHANGED_D2";
                    break;
                case WD_EVENT_ACTION.WD_POWER_CHANGED_D3:
                    sAction = "WD_POWER_CHANGED_D3";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_WORKING:
                    sAction = "WD_POWER_SYSTEM_WORKING";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_SLEEPING1:
                    sAction = "WD_POWER_SYSTEM_SLEEPING1";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_SLEEPING2:
                    sAction = "WD_POWER_SYSTEM_SLEEPING2";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_SLEEPING3:
                    sAction = "WD_POWER_SYSTEM_SLEEPING3";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_HIBERNATE:
                    sAction = "WD_POWER_SYSTEM_HIBERNATE";
                    break;
                case WD_EVENT_ACTION.WD_POWER_SYSTEM_SHUTDOWN:
                    sAction = "WD_POWER_SYSTEM_SHUTDOWN";
                    break;
                default:
                    sAction = wdEvent.dwAction.ToString("X");
                    break;
            }
            Log.TraceLog("Received event notification of type " + sAction +
                " on " + dev.ToString(false));
        }

        private void PLX_Sample_Closing(object sender, System.EventArgs e)
        {
            plxDevList.Dispose();
            dmaList.Dispose();
        }

        /* list box lstBxDevices */
        private void lstBxDevices_SelectedIndexChanged(object sender,
            System.EventArgs e)
        {
            if (lstBxDevices.SelectedIndex < 0)
            {
                DisableMenu();
                btDevice.Enabled = false;
            }
            else
            {
                PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
                UpdateMenu(lstBxDevices.SelectedIndex);
                btDevice.Enabled = true;
                if (dev.Handle == IntPtr.Zero)
                    btDevice.Text = "Open Device";
                else
                    btDevice.Text = "Close Device";
            }
        }

        private void lstBxDevices_DoubleClicked(object sender,
            System.EventArgs e)
        {
            btDevice_Click(sender, e);
        }

        /* device button */
        private void btDevice_Click(object sender, System.EventArgs e)
        {
            if (btDevice.Text == "Open Device")
            {
                if (DeviceOpen(lstBxDevices.SelectedIndex) == true)
                {
                    btDevice.Text = "Close Device";
                    EnableMenu();
                }
            }
            else
            {
                PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
                DeviceClose(lstBxDevices.SelectedIndex);
                btDevice.Text = "Open Device";
                DisableMenu();
            }
        }

        /* Menu*/
        private void UpdateMenu(int index)
        {
            PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
            if (dev.Handle == IntPtr.Zero)
                DisableMenu();
            else
                EnableMenu();

            menuTargetInterrupts.Visible = (dev.IsMaster) ? false : true;
            menuMasterInterrupts.Visible = (dev.IsMaster) ? true : false;
            menuDMA.Visible = (dev.IsMaster) ? true : false;
        }

        private void EnableMenu()
        {
            ToggleMenu(true);
        }

        private void DisableMenu()
        {
            ToggleMenu(false);
        }

        private void ToggleMenu(bool flag)
        {
            for (int index = 0; index < mainMenu1.MenuItems.Count; ++index)
                mainMenu1.MenuItems[index].Enabled = flag;
        }

        /* Address Space Item */
        private void menuAddrRW_Click(object sender, System.EventArgs e)
        {
            PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
            string[] sBars = dev.AddrDescToString(false);
            AddrSpaceTransferForm addrSpcFrm = new
                AddrSpaceTransferForm(dev, sBars, false);
            addrSpcFrm.GetInput();
        }

        private void menuLocalRW_Click(object sender, System.EventArgs e)
        {
            PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
            string[] sBars = dev.AddrDescToString(true);
            AddrSpaceTransferForm addrSpcFrm = new AddrSpaceTransferForm(dev,
                sBars, true);
            addrSpcFrm.GetInput();
        }

        /* Interrupts items*/

        private void menuTargetInterrupts_Select(object sender,
            System.EventArgs e)
        {
            if (menuTargetInterrupts.Enabled == false)
                return;

            PLX_TargetDevice dev = (PLX_TargetDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);
            bool bIsEnb = dev.IsEnabledInt();

            menuTargetEnableInt.Text = bIsEnb ?
                "Disable Interrupts" : "Enable Interrupts";
            menuGenerateInt.Enabled = bIsEnb ? true : false;
        }

        private void menuTargetEnableInt_Click(object sender,
            System.EventArgs e)
        {
            PLX_TargetDevice dev = (PLX_TargetDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);
            if (menuTargetEnableInt.Text == "Enable Interrupts")
            {
                DWORD dwStatus = dev.EnableInterrupts(new
                    USER_INTERRUPT_TARGET_CALLBACK(PLX_IntHandler));

                if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                    menuTargetEnableInt.Text = "Disable Interrupts";
            }
            else
            {
                DWORD dwStatus = dev.DisableInterrupts();

                if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                    menuTargetEnableInt.Text = "Enable Interrupts";
            }
        }

        private void menuGenerateInt_Click(object sender, System.EventArgs e)
        {
            ((PLX_TargetDevice)(plxDevList.Get(
                lstBxDevices.SelectedIndex))).GenerateTargetInterrupt(0);
        }

        private void menuMasterInterrupts_Select(object sender,
            System.EventArgs e)
        {
            if (menuMasterInterrupts.Enabled == false)
                return;

            PLX_MasterDevice dev = (PLX_MasterDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);
            bool bIsEnb = dev.IsEnabledInt();

            menuEnableInt.Text = bIsEnb ?
                "Disable Interrupts" : "Enable Interrupts";
        }

        public void menuEnableInt_Select(object sender, System.EventArgs e)
        {
            if (menuEnableInt.Enabled == false)
                return;

            PLX_MasterDevice dev = (PLX_MasterDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);
            if (menuEnableInt.Text == "Enable Interrupts")
            {
                menuInt0.Visible = true;
                menuInt1.Visible = true;
            }
            else
            {
                bool bIsChn0 = dev.IsChannelIntEnabled(
                    PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0);

                menuInt0.Visible = bIsChn0 ? true : false;
                menuInt1.Visible = bIsChn0 ? false : true;
            }
        }

        private void menuInt0_Click(object sender, System.EventArgs e)
        {
            menuInt_Click(PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0);
        }

        private void menuInt1_Click(object sender, System.EventArgs e)
        {
            menuInt_Click(PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_1);
        }

        private void menuInt_Click(PLX_DMA_CHANNEL chn)
        {
            PLX_MasterDevice dev = (PLX_MasterDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);

            if (dev.IsEnabledInt())
            {
                dev.DisableInterrupts();
            }
            else
            {
                dev.EnableInterrupts(new
                    USER_INTERRUPT_MASTER_CALLBACK(PLX_DMAIntHandler), chn);
            }
        }

        /* DMA items */
        private void menuChannel0_Select(object sender, System.EventArgs e)
        {
            if (menuChannel0.Enabled == false)
                return;

            PLX_MasterDevice dev = (PLX_MasterDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);
            bool bIsOpen = dmaList.IsDMAOpen(dev.Handle,
                PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0);

            menuOpenChn0.Text = bIsOpen ? "Close" : "Open";
            menuTransferChn0.Enabled = bIsOpen ? true : false;
        }

        private void menuOpenChn_Click(PLX_DMA_CHANNEL plxChn,
            System.Windows.Forms.MenuItem chnItem)
        {
            PLX_MasterDevice dev = (PLX_MasterDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);

            if (chnItem.Text == "Open")
            {
                DmaBuffer dmaBuff = null;
                UINT32 u32localAddr = 0;
                RW direction = 0;
                ALLOCATION_TYPE wAllocType = 0;
                DWORD dwBytes = 0;
                BOOL bAutoInc = true;
                IntPtr pBuffer = IntPtr.Zero;

                OpenDMAForm openDmaFrm = new OpenDMAForm(plxChn);
                if (openDmaFrm.GetInput(ref u32localAddr, ref direction,
                    ref wAllocType, ref dwBytes, ref bAutoInc) == false)
                {
                    return;
                }

                if (wAllocType == ALLOCATION_TYPE.CONTIG)
                {
                    dmaBuff = new DmaBufferContig(dev);
                }
                else if (wAllocType == ALLOCATION_TYPE.TRANSACTION_CONTIG)
                {
                    dmaBuff = new DmaBufferTransactionContig(dev);
                }
                else
                {
                    pBuffer = Marshal.AllocHGlobal((int)dwBytes);
                    if (pBuffer == IntPtr.Zero)
                    {
                        Log.ErrLog("PLX_Sample: Failed to allocate buffer for " +
                            "Scatter-Gather DMA");
                        return;
                    }
                    if (wAllocType == ALLOCATION_TYPE.SG)
                        dmaBuff = new DmaBufferSG(dev);
                    else
                        dmaBuff = new DmaBufferTransactionSG(dev);
                }

                if (dmaBuff.Open(u32localAddr, (direction == RW.READ), dwBytes,
                    ref pBuffer, bAutoInc, WDC_ADDR_MODE.WDC_MODE_32,
                    (uint)plxChn) != (DWORD)wdc_err.WD_STATUS_SUCCESS)
                {
                    return;
                }

                string menuText = (direction == RW.READ) ? "Read" : "Write";
                dmaList.AddItem(dmaBuff, plxChn, pBuffer);
                chnItem.Text = "Close";

                if (plxChn == PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0)
                    this.menuTransferChn0.Text = menuText;
                else
                    this.menuTransferChn1.Text = menuText;

                Log.TraceLog("DMA buffer for {" +
                    dev.ToString(false) + "}, Channel " +
                    (((DWORD)plxChn == 0) ? "0 " : "1 ") +
                    "was opened successfully");

                return;
            }
            else
            {
                DmaInfo dmaInfo = dmaList.GetItem(dev.Handle, plxChn);

                dmaInfo.Close();
                dmaList.Remove(dmaInfo);
                chnItem.Text = "Open";
                if (plxChn == PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0)
                    this.menuTransferChn0.Text = "Read/Write";
                else
                    this.menuTransferChn1.Text = "Read/Write";

                Log.TraceLog("DMA Channel " +
                    (((DWORD)plxChn == 0) ? "0 " : "1 ") + "was closed. {" +
                    dev.ToString(false) + "}");
                return;
            }
        }

        private void menuOpenChn0_Click(object sender, System.EventArgs e)
        {
            menuOpenChn_Click(PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0, menuOpenChn0);
        }

        private void menuTransferChn0_Click(object sender, System.EventArgs e)
        {
            menuTransferChn_Click(PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_0);
        }

        private void menuChannel1_Select(object sender, System.EventArgs e)
        {
            if (menuChannel1.Enabled == false)
                return;

            PLX_MasterDevice dev = (PLX_MasterDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);
            bool bIsOpen = dmaList.IsDMAOpen(dev.Handle,
                PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_1);

            menuOpenChn1.Text = bIsOpen ? "Close" : "Open";
            menuTransferChn1.Enabled = bIsOpen ? true : false;
        }

        private void menuOpenChn1_Click(object sender, System.EventArgs e)
        {
            menuOpenChn_Click(PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_1, menuOpenChn1);
        }

        private void menuTransferChn1_Click(object sender, System.EventArgs e)
        {
            menuTransferChn_Click(PLX_DMA_CHANNEL.PLX_DMA_CHANNEL_1);
        }

        private void menuTransferChn_Click(PLX_DMA_CHANNEL chn)
        {
            PLX_MasterDevice dev = (PLX_MasterDevice)plxDevList.Get(
                lstBxDevices.SelectedIndex);
            DmaBuffer dmaBuffer = dmaList.GetItem(dev.Handle, chn).DmaBuffer;
            MarshalWdDma wdDmaMarshaler = new MarshalWdDma();
            IntPtr pBuffer = ((WD_DMA)wdDmaMarshaler.MarshalNativeToManaged(
                dmaBuffer.pWdDma)).pUserAddr;
            DMATransferForm transFrom = new DMATransferForm(dev, dmaBuffer,
                pBuffer, chn, dmaBuffer.IsRead);

            transFrom.GetInput();
        }

        /* Event Items */
        private void menuEvents_Select(object sender, System.EventArgs e)
        {
            if (menuEvents.Enabled == false)
                return;

            PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);

            menuRegisterEvent.Text = dev.IsEventRegistered() ?
                "Unregister Events" : "Register Events";
        }

        private void menuRegisterEvent_Click(object sender, System.EventArgs e)
        {
            if (menuRegisterEvent.Text == "Register Events")
            {
                plxDevList.Get(lstBxDevices.SelectedIndex).EventRegister(
                    new USER_EVENT_CALLBACK(PLX_EventHandler));
                menuRegisterEvent.Text = "Unregister Events";
            }
            else
            {
                plxDevList.Get(lstBxDevices.SelectedIndex).
                    EventUnregister();
                menuRegisterEvent.Text = "Register Events";
            }
        }

        /* EEPROM Items */
        private void menuEEpromRW_Click(object sender, System.EventArgs e)
        {
            PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
            RegistersForm EForm = new RegistersForm(dev, ACTION_TYPE.EEPROM);

            EForm.GetInput();
        }

        /* Configuration Space Items */
        private void menuCfgOffset_Click(object sender, System.EventArgs e)
        {
            PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
            CfgTransfersForm cfgOffsetFrom = new CfgTransfersForm(dev);

            cfgOffsetFrom.GetInput();
        }

        private void menuCfgReg_Click(object sender, System.EventArgs e)
        {
            PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
            RegistersForm regForm = new RegistersForm(dev, ACTION_TYPE.CFG);

            regForm.GetInput();
        }

        /* RunTime Registers Items */
        private void menuRTRegsRW_Click(object sender, System.EventArgs e)
        {
            PLX_Device dev = plxDevList.Get(lstBxDevices.SelectedIndex);
            RegistersForm regForm = new RegistersForm(dev, ACTION_TYPE.RT);

            regForm.GetInput();
        }

        private void btExit_Click(object sender, System.EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btLog_Click(object sender, System.EventArgs e)
        {
            txtLog.Clear();
        }

        public void LogFunc(string str)
        {
            if (txtLog != null)
                txtLog.AppendText(str + Environment.NewLine);
        }

        public void TraceLog(string str)
        {
            if (this.InvokeRequired)
                Invoke(new Log.TRACE_LOG(LogFunc), new object[]{str});
            else
                LogFunc(str);
        }

        public void ErrLog(string str)
        {
            if (this.InvokeRequired)
                Invoke(new Log.ERR_LOG(LogFunc), new object[]{str});
            else
                LogFunc(str);
        }
    }

    public class diag_lib
    {
        public static string PadBuffer(string str, uint fromIndex, uint toIndex)
        {
            for (uint i = fromIndex; i < toIndex; ++i)
                str += "0";

            return str;
        }

        public static string DisplayHexBuffer(object[] obj, DWORD dwBuffSize,
             WDC_ADDR_MODE mode)
        {
            StringBuilder display = new StringBuilder((int)dwBuffSize);

            switch(mode)
            {
            case WDC_ADDR_MODE.WDC_MODE_8:
                {
                    BYTE[] buff = (BYTE[])obj[0];

                    for (uint i = 0; i < dwBuffSize; ++i)
                    {
                            display.Append(buff[i].ToString("X2") + " ");
                    }
                    break;
                }
            case WDC_ADDR_MODE.WDC_MODE_16:
                {
                    WORD[] buff = (WORD[])obj[0];

                    for (int i = 0; i < dwBuffSize / 2; ++i)
                    {
                            display.Append(buff[i].ToString("X4") + " ");
                    }
                    break;
                }
            case WDC_ADDR_MODE.WDC_MODE_32:
                {
                    UINT32[] buff = (UINT32[])obj[0];

                    for (int i = 0; i < dwBuffSize / 4; ++i)
                    {
                            display.Append(buff[i].ToString("X8") + " ");
                    }
                    break;
                }
            case WDC_ADDR_MODE.WDC_MODE_64:
                {
                    UINT64[] buff = (UINT64[])obj[0];

                    for (int i = 0; i < dwBuffSize / 8; ++i)
                    {
                            display.Append(buff[i].ToString("X16") + " ");
                    }
                    break;
                }
            }

            return display.ToString();
        }

        public static string DisplayHexBuffer(byte[] buff, uint dwBuffSize)
        {
            return DisplayHexBuffer(new object[]{buff}, dwBuffSize,
                WDC_ADDR_MODE.WDC_MODE_8);
        }
    };

    /* Holds the information relevant for a DMA channel transfer */
    public class DmaInfo
    {
        private DmaBuffer m_dmaBuffer;
        private PLX_DMA_CHANNEL m_plxChannel;
        private IntPtr m_pBuffer;

        public DmaInfo(DmaBuffer dmaBuffer, PLX_DMA_CHANNEL plxChn,
            IntPtr pBuffer)
        {
            m_dmaBuffer = dmaBuffer;
            m_plxChannel = plxChn;
            m_pBuffer = pBuffer;
        }

        public DmaBuffer DmaBuffer
        {
            get
            {
                return m_dmaBuffer;
            }
            set
            {
                m_dmaBuffer = value;
            }
        }

        public WDC_DEVICE_HANDLE DeviceHandle
        {
            get
            {
                return m_dmaBuffer.DeviceHandle;
            }
        }

        public PLX_DMA_CHANNEL DmaChannel
        {
            get
            {
                return m_plxChannel;
            }
        }

        public IntPtr Buffer
        {
            get
            {
                return m_pBuffer;
            }
        }

        public void Close()
        {
            if (m_dmaBuffer is DmaBufferSG && m_pBuffer != IntPtr.Zero)
                Marshal.FreeHGlobal(m_pBuffer);
            m_dmaBuffer.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }

    public class DmaInfoList : ArrayList
    {
        public void AddItem(DmaBuffer dmaBuffer, PLX_DMA_CHANNEL plxChn,
            IntPtr pBuffer)
        {
            DmaInfo dmaInfo = new DmaInfo(dmaBuffer, plxChn, pBuffer);

            this.Add(dmaInfo);
        }

        public DmaInfo GetItem(WDC_DEVICE_HANDLE hDev, PLX_DMA_CHANNEL plxChn)
        {
            Exception excp = new Exception();

            foreach (DmaInfo dmaInfo in this)
            {
                if (dmaInfo.DeviceHandle == hDev &&
                    dmaInfo.DmaChannel == plxChn)
                {
                    return dmaInfo;
                }
            }

            throw excp;
        }

        public DmaInfoList GetItems(WDC_DEVICE_HANDLE hDev)
        {
            DmaInfoList dmaDevList = new DmaInfoList();

            foreach (DmaInfo dmaInfo in this)
            {
                if (dmaInfo.DeviceHandle == hDev)
                    dmaDevList.Add(dmaInfo);
            }

            return dmaDevList;
        }

        public bool IsDMAOpen(WDC_DEVICE_HANDLE hDev, PLX_DMA_CHANNEL plxChn)
        {
            try
            {
                return (GetItem(hDev, plxChn) != null);
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            foreach (DmaInfo dmaInfo in this)
                dmaInfo.Dispose();

            this.Clear();
        }
    }
}

