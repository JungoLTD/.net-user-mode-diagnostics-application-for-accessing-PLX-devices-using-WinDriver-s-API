// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Jungo.wdapi_dotnet;
using Jungo.plx_lib;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using UINT64 = System.UInt64;
using DWORD = System.UInt32;
using UINT32 = System.UInt32;
using WORD = System.UInt16;
using BYTE = System.Byte;
using BOOL = System.Boolean;

namespace Jungo.PLX_Sample
{
    public class RegistersForm : System.Windows.Forms.Form
    {
        private Exception m_excp;
        private PLX_Device m_device;
        private WDC_REG[] m_regs;
        private RW m_direction;
        private DWORD m_dwOffset;
        private ACTION_TYPE m_regType;
        private BYTE m_bData;
        private WORD m_wData;
        private UINT32 m_u32Data;
        private UINT64 m_u64Data;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Label lblOffset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblRegs;
        private System.Windows.Forms.Button btReadAll;
        private System.Windows.Forms.ComboBox cmboRegs;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Button btRead;
        private System.Windows.Forms.Button btWrite;
        private System.Windows.Forms.Button btLog;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblInput;

        private System.ComponentModel.Container components = null;

        public RegistersForm(PLX_Device dev, ACTION_TYPE regType)
        {
            InitializeComponent();

            m_device = dev;
            m_regType = regType;

            switch(regType)
            {
            case ACTION_TYPE.CFG:
                {
                    this.Text = "Read/Write Configuration Space by Registers";
                    lblRegs.Visible = true;
                    lblRegs.Text = "Choose a Cfg Register";
                    cmboRegs.Visible = true;
                    WDC_REG[] regs = PLX_Regs.gPLX_CfgRegs;

                    for (int i = 0; i < regs.GetLength(0); ++i)
                    {
                        cmboRegs.Items.AddRange(new object[] {regs[i].sName +
                            " size: " + regs[i].dwSize.ToString("X") +
                            " - " + regs[i].sDesc});
                    }
                    break;
                }
            case ACTION_TYPE.RT:
                {
                    this.Text = "Read/Write RunTime Registers";
                    lblRegs.Visible = true;
                    lblRegs.Text = "Choose a RunTime Register";
                    cmboRegs.Visible = true;
                    WDC_REG[] regs = m_device.IsMaster ? PLX_Regs.gPLX_M_Regs :
                        PLX_Regs.gPLX_T_Regs;
                    for (int i = 0; i < regs.GetLength(0); ++i)
                    {
                        cmboRegs.Items.AddRange(new object[]{regs[i].sName +
                            " size: " + regs[i].dwSize.ToString("X") +
                            " - " + regs[i].sDesc});
                    }
                    break;
                }
            case ACTION_TYPE.EEPROM:
                {
                    this.Text = "Read/Write EEPROM";
                    lblRegs.Visible = false;
                    cmboRegs.Visible = false;

                    txtOffset = new System.Windows.Forms.TextBox();
                    txtOffset.Visible = true;
                    txtOffset.Location = new
                        System.Drawing.Point(txtInput.Left, cmboRegs.Top);
                    txtOffset.Name = "txtOffset";
                    txtOffset.Size = new System.Drawing.Size(txtInput.Width,
                        txtInput.Height);
                    txtOffset.TabIndex = 0;
                    txtOffset.Text = "";
                    txtOffset.Parent = this;

                    lblOffset = new Label();
                    string sOffMult = m_device.EEprom.IsVPD() ?
                        wdc_lib_consts.WDC_SIZE_32.ToString():
                        wdc_lib_consts.WDC_SIZE_16.ToString();
                    lblOffset.Text = "Enter offset (must be a multiple of " +
                        sOffMult + "):";
                    lblOffset.Visible = true;
                    lblOffset.Location = new
                        System.Drawing.Point(lblInput.Left, cmboRegs.Top);
                    lblOffset.Name = "lblOffset";
                    lblOffset.Size = new
                        System.Drawing.Size(lblInput.Width - 5,
                        lblInput.Height);
                    lblOffset.TabIndex = 19;
                    lblOffset.Parent = this;

                    label3 = new Label();
                    label3.Location = new
                        System.Drawing.Point(label2.Left, cmboRegs.Top);
                    label3.Name = "label3";
                    label3.Size = new System.Drawing.Size(16, 23);
                    label3.TabIndex = 20;
                    label3.Text = "0x";
                    label3.Parent = this;

                    break;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

#region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.cmboRegs = new System.Windows.Forms.ComboBox();
            this.txtData = new System.Windows.Forms.TextBox();
            this.lblRegs = new System.Windows.Forms.Label();
            this.btReadAll = new System.Windows.Forms.Button();
            this.btExit = new System.Windows.Forms.Button();
            this.btRead = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btWrite = new System.Windows.Forms.Button();
            this.btLog = new System.Windows.Forms.Button();
            this.lblInput = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // cmboRegs
            this.cmboRegs.Location = new System.Drawing.Point(40, 32);
            this.cmboRegs.Name = "cmboRegs";
            this.cmboRegs.Size = new System.Drawing.Size(360, 21);
            this.cmboRegs.TabIndex = 0;
            this.cmboRegs.Text = "---- ----";
            // txtData
            this.txtData.AutoSize = false;
            this.txtData.Location = new System.Drawing.Point(40, 112);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(360, 136);
            this.txtData.TabIndex = 8;
            this.txtData.Text = "";
            // lblRegs
            this.lblRegs.Location = new System.Drawing.Point(48, 8);
            this.lblRegs.Name = "lblRegs";
            this.lblRegs.Size = new System.Drawing.Size(256, 23);
            this.lblRegs.TabIndex = 9;
            this.lblRegs.Text = "Choose a RunTime Register";
            // btReadAll
            this.btReadAll.Location = new System.Drawing.Point(176, 264);
            this.btReadAll.Name = "btReadAll";
            this.btReadAll.TabIndex = 3;
            this.btReadAll.Text = "Read All ";
            this.btReadAll.Click +=
                new System.EventHandler(this.btReadAll_Click);
            // btExit
            this.btExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btExit.Location = new System.Drawing.Point(432, 192);
            this.btExit.Name = "btExit";
            this.btExit.TabIndex = 6;
            this.btExit.Text = "Exit";
            // btRead
            this.btRead.Location = new System.Drawing.Point(48, 264);
            this.btRead.Name = "btRead";
            this.btRead.TabIndex = 2;
            this.btRead.Text = "Read";
            this.btRead.Click += new System.EventHandler(this.btRead_Click);
            // txtInput
            this.txtInput.Location = new System.Drawing.Point(256, 72);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(144, 20);
            this.txtInput.TabIndex = 1;
            this.txtInput.Text = "";
            // btWrite
            this.btWrite.Location = new System.Drawing.Point(304, 264);
            this.btWrite.Name = "btWrite";
            this.btWrite.TabIndex = 4;
            this.btWrite.Text = "Write";
            this.btWrite.Click += new System.EventHandler(this.btWrite_Click);
            // btLog
            this.btLog.Location = new System.Drawing.Point(432, 136);
            this.btLog.Name = "btLog";
            this.btLog.TabIndex = 5;
            this.btLog.Text = "Clear Log";
            this.btLog.Click += new System.EventHandler(this.btLog_Click);
            // lblInput
            this.lblInput.Location = new System.Drawing.Point(40, 72);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(208, 23);
            this.lblInput.TabIndex = 17;
            this.lblInput.Text = "Enter Input in hex (Write Transactions):";
            // label2
            this.label2.Location = new System.Drawing.Point(240, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 23);
            this.label2.TabIndex = 18;
            this.label2.Text = "0x";
            // RegistersForm
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btExit;
            this.ClientSize = new System.Drawing.Size(520, 301);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblInput);
            this.Controls.Add(this.btLog);
            this.Controls.Add(this.btWrite);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btRead);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.btReadAll);
            this.Controls.Add(this.lblRegs);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.cmboRegs);
            this.Name = "RegistersForm";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registers";
            this.ResumeLayout(false);

        }
#endregion

        public bool GetInput()
        {
            DialogResult result = DialogResult.Retry;

            while ((result = ShowDialog()) == DialogResult.Retry);

            return true;
        }

        private void btClick(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.None;

            try
            {
                if (m_regType == ACTION_TYPE.EEPROM)
                    TranslateInput_EEPROM();
                else
                    TranslateInput_Regs();
            }
            catch
            {
                MessageBox.Show(m_excp.Message, "Input Entry Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.Retry;
                return;
            }

            if (m_regType == ACTION_TYPE.EEPROM)
                ReadWriteEEPROM();
            else
            {
                if (m_direction == RW.READ_ALL)
                    ReadAllRegs();
                else
                    ReadWriteReg();
            }
        }

        private void btRead_Click(object sender, System.EventArgs e)
        {
            m_direction = RW.READ;
            btClick(sender, e);
        }

        private void btWrite_Click(object sender, System.EventArgs e)
        {
            m_direction = RW.WRITE;
            btClick(sender, e);
        }

        private void btReadAll_Click(object sender, System.EventArgs e)
        {
            m_direction = RW.READ_ALL;
            btClick(sender, e);
        }

        private void TranslateInput_Regs()
        {
            if (m_direction != RW.READ_ALL)
            {
                m_regs = new WDC_REG[1];
                m_excp = new Exception("Select a Register");
                int iSelectedReg = cmboRegs.SelectedIndex;
                if ((uint)iSelectedReg == 0xffffffff)
                    throw m_excp;

                m_regs[0] = (m_regType == ACTION_TYPE.CFG) ?
                    PLX_Regs.gPLX_CfgRegs[iSelectedReg]:
                    (m_device.IsMaster) ?
                    PLX_Regs.gPLX_M_Regs[iSelectedReg]:
                    PLX_Regs.gPLX_T_Regs[iSelectedReg];

                if (m_direction == RW.WRITE)
                {
                    m_excp = new Exception("Enter the data to write. "
                        + "Entered value should be a hex number");
                    if (txtInput.Text == "")
                        throw m_excp;
                    DWORD dwSize = m_regs[0].dwSize;
                    switch(dwSize)
                    {
                    case wdc_lib_consts.WDC_SIZE_8:
                        {
                            m_bData = Convert.ToByte(txtInput.Text, 16);
                            break;
                        }
                    case wdc_lib_consts.WDC_SIZE_16:
                        {
                            m_wData = Convert.ToUInt16(txtInput.Text, 16);
                            break;
                        }
                    case wdc_lib_consts.WDC_SIZE_32:
                        {
                            m_u32Data = Convert.ToUInt32(txtInput.Text, 16);
                            break;
                        }
                    case wdc_lib_consts.WDC_SIZE_64:
                        {
                            m_u64Data = Convert.ToUInt64(txtInput.Text, 16);
                            break;
                        }
                    }
                }
            }
            else
            {
                m_regs = (m_regType == ACTION_TYPE.CFG) ?
                    PLX_Regs.gPLX_CfgRegs: (m_device.IsMaster) ?
                    PLX_Regs.gPLX_M_Regs: PLX_Regs.gPLX_T_Regs;
            }
        }

        private BOOL IsLegalDirection(int iRegIndex)
        {
            WDC_REG reg = m_regs[iRegIndex];

            if (((RW.READ == m_direction || RW.READ_ALL == m_direction) &&
                (WDC_DIRECTION.WDC_WRITE == reg.direction)) ||
                ((RW.WRITE == m_direction) &&
                (WDC_DIRECTION.WDC_READ == reg.direction)))
                return false;

            return true;
        }

        private void ReadWriteReg()
        {
            WDC_REG reg = m_regs[0];
            DWORD dwStatus = (DWORD)wdc_err.WD_STATUS_SUCCESS;
            BOOL bIsRead = (m_direction == RW.READ);

            if (!IsLegalDirection(0))
            {
                txtData.Text += "you have chosen to " + (bIsRead ?
                    "read from" : "write to") + " a register which is " +
                    (bIsRead ? "write-only" : "read-only") +
                    Environment.NewLine;
                return;
            }

            switch (reg.dwSize)
            {
            case wdc_lib_consts.WDC_SIZE_8:
                {
                    if (RW.READ == m_direction)
                        dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                            wdc_lib_decl.WDC_PciReadCfg8(m_device.Handle,
                                reg.dwOffset, ref m_bData) :
                            wdc_lib_decl.WDC_ReadAddr8(m_device.Handle,
                                reg.dwAddrSpace, reg.dwOffset, ref m_bData);
                    else
                    {
                        dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                            wdc_lib_decl.WDC_PciWriteCfg8(m_device.Handle,
                                reg.dwOffset, m_bData) :
                            wdc_lib_decl.WDC_WriteAddr8(m_device.Handle,
                                reg.dwAddrSpace, reg.dwOffset, m_bData);
                    }
                    break;
                }
            case wdc_lib_consts.WDC_SIZE_16:
                {
                    if (RW.READ == m_direction)
                        dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                            wdc_lib_decl.WDC_PciReadCfg16(m_device.Handle,
                                reg.dwOffset, ref m_wData) :
                            wdc_lib_decl.WDC_ReadAddr16(m_device.Handle,
                                reg.dwAddrSpace, reg.dwOffset, ref m_wData);
                    else
                    {
                        dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                            wdc_lib_decl.WDC_PciWriteCfg16(m_device.Handle,
                                reg.dwOffset, m_wData) :
                            wdc_lib_decl.WDC_WriteAddr16(m_device.Handle,
                                reg.dwAddrSpace, reg.dwOffset, m_wData);
                    }
                    break;
                }
            case wdc_lib_consts.WDC_SIZE_32:
                {
                    if (RW.READ == m_direction)
                        dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                            wdc_lib_decl.WDC_PciReadCfg32(m_device.Handle,
                                reg.dwOffset, ref m_u32Data) :
                            wdc_lib_decl.WDC_ReadAddr32(m_device.Handle,
                                reg.dwAddrSpace, reg.dwOffset, ref m_u32Data);
                    else
                    {
                        dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                            wdc_lib_decl.WDC_PciWriteCfg32(m_device.Handle,
                                reg.dwOffset, m_u32Data) :
                            wdc_lib_decl.WDC_WriteAddr32(m_device.Handle,
                                reg.dwAddrSpace, reg.dwOffset, m_u32Data);
                    }
                    break;
                }
            case wdc_lib_consts.WDC_SIZE_64:
                {
                    if (RW.READ == m_direction)
                        dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                            wdc_lib_decl.WDC_PciReadCfg64(m_device.Handle,
                                reg.dwOffset, ref m_u64Data) :
                            wdc_lib_decl.WDC_ReadAddr64(m_device.Handle,
                                reg.dwAddrSpace, reg.dwOffset, ref m_u64Data);
                    else
                    {
                        dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                            wdc_lib_decl.WDC_PciWriteCfg64(m_device.Handle,
                                reg.dwOffset, m_u64Data) :
                            wdc_lib_decl.WDC_WriteAddr64(m_device.Handle,
                                reg.dwAddrSpace, reg.dwOffset, m_u64Data);
                    }
                    break;
                }
            }
            TraceLog((((DWORD)wdc_err.WD_STATUS_SUCCESS == dwStatus) ?
                (bIsRead ? "read " : "wrote ") + "0x" +
                ((reg.dwSize == wdc_lib_consts.WDC_SIZE_8) ?
                m_bData.ToString("X2"):
                ((reg.dwSize == wdc_lib_consts.WDC_SIZE_16) ?
                m_wData.ToString("X4") :
                ((reg.dwSize == wdc_lib_consts.WDC_SIZE_32) ?
                m_u32Data.ToString("X8") : m_u64Data.ToString("X16")))) +
                (bIsRead ? " from " : " to ") + "register " + reg.sName :
                "failed to complete the transaction on register" + reg.sName),
                    (wdc_err)dwStatus);
        }

        private void ReadAllRegs()
        {
            WDC_REG reg = m_regs[0];
            DWORD dwStatus = (DWORD)wdc_err.WD_STATUS_SUCCESS;

            TraceLog("displaying all registers (" + m_device.ToString(false) +
                ")", (wdc_err)dwStatus);
            for (int i = 0; i < m_regs.GetLength(0); ++i)
            {
                reg = m_regs[i];
                if (!IsLegalDirection(i))
                {
                    txtData.Text = "register " + reg.sName + "is write-only" +
                        Environment.NewLine;
                }
                else
                {
                    switch (reg.dwSize)
                    {
                    case wdc_lib_consts.WDC_SIZE_8:
                        {
                            dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                                wdc_lib_decl.WDC_PciReadCfg8(m_device.Handle,
                                    reg.dwOffset, ref m_bData) :
                                wdc_lib_decl.WDC_ReadAddr8(m_device.Handle,
                                    reg.dwAddrSpace, reg.dwOffset, ref m_bData);
                            break;
                        }
                    case wdc_lib_consts.WDC_SIZE_16:
                        {
                            dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                                wdc_lib_decl.WDC_PciReadCfg16(m_device.Handle,
                                    reg.dwOffset, ref m_wData) :
                                wdc_lib_decl.WDC_ReadAddr16(m_device.Handle,
                                    reg.dwAddrSpace, reg.dwOffset, ref m_wData);
                            break;
                        }
                    case wdc_lib_consts.WDC_SIZE_32:
                        {
                            dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                                wdc_lib_decl.WDC_PciReadCfg32(m_device.Handle,
                                    reg.dwOffset, ref m_u32Data) :
                                wdc_lib_decl.WDC_ReadAddr32(m_device.Handle,
                                    reg.dwAddrSpace, reg.dwOffset,
                                    ref m_u32Data);
                            break;
                        }
                    case wdc_lib_consts.WDC_SIZE_64:
                        {
                            dwStatus = (m_regType == ACTION_TYPE.CFG) ?
                                wdc_lib_decl.WDC_PciReadCfg64(m_device.Handle,
                                    reg.dwOffset, ref m_u64Data) :
                                wdc_lib_decl.WDC_ReadAddr64(m_device.Handle,
                                    reg.dwAddrSpace, reg.dwOffset,
                                    ref m_u64Data);
                            break;
                        }
                    }
                    TraceLog((((DWORD)wdc_err.WD_STATUS_SUCCESS == dwStatus) ?
                        "read from register " + reg.sName + " 0x" +
                        ((reg.dwSize == wdc_lib_consts.WDC_SIZE_8) ?
                        m_bData.ToString("X2"):
                        ((reg.dwSize == wdc_lib_consts.WDC_SIZE_16) ?
                        m_wData.ToString("X4") :
                        ((reg.dwSize == wdc_lib_consts.WDC_SIZE_32) ?
                        m_u32Data.ToString("X8") :
                        m_u64Data.ToString("X16")))) :
                        "failed to complete the transaction on register " +
                        reg.sName), (wdc_err)dwStatus);
                }
            }
        }

        private void TranslateInput_EEPROM()
        {
            bool bIsVPD = m_device.EEprom.IsVPD();
            if (m_direction != RW.READ_ALL)
            {
                DWORD dwMult = (bIsVPD ? wdc_lib_consts.WDC_SIZE_32 :
                    wdc_lib_consts.WDC_SIZE_16);
                m_excp = new Exception("the offset entered is not a multiple " +
                    "of " + dwMult.ToString());
                m_dwOffset = (DWORD)Convert.ToByte(txtOffset.Text, 16);
                if ((m_dwOffset % dwMult) != 0)
                    throw m_excp;

                if (m_direction == RW.WRITE)
                {
                    m_excp = new Exception("Enter the data to write. "
                        + "Entered value should be a hex number");

                    if (txtInput.Text == "")
                        throw m_excp;

                    if (bIsVPD)
                        m_u32Data = Convert.ToUInt32(txtInput.Text, 16);
                    else
                        m_wData = Convert.ToUInt16(txtInput.Text, 16);
                }
            }
        }

        private void ReadWriteEEPROM()
        {
            bool bIsVPD = m_device.EEprom.IsVPD();
            DWORD dwStatus = (DWORD)wdc_err.WD_STATUS_SUCCESS;
            string str;

            switch(m_direction)
            {
            case RW.READ:
                {
                    dwStatus = bIsVPD ? m_device.EEprom.EEPROM_Read(m_dwOffset,
                        ref m_u32Data):
                        ((EEPROM_RT)m_device.EEprom).EEPROM_Read16(m_dwOffset,
                            ref m_wData);
                    if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                    {
                        str = "read 0x" + (bIsVPD ? m_u32Data.ToString("X") :
                            m_wData.ToString("X")) + " from offset "
                            + m_dwOffset.ToString("X");
                    }
                    else
                        str = "Failed reading from offset " +
                            m_dwOffset.ToString("X");

                    TraceLog(str, (wdc_err)dwStatus);
                    break;
                }
            case RW.WRITE:
                {
                    dwStatus = bIsVPD ? m_device.EEprom.EEPROM_Write(m_dwOffset,
                        m_u32Data) :
                        ((EEPROM_RT)m_device.EEprom).EEPROM_Write16(m_dwOffset,
                            m_wData);
                    if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
                    {
                        str = "Wrote 0x" + (bIsVPD ? m_u32Data.ToString("X") :
                            m_wData.ToString("X")) + " to offset " +
                            m_dwOffset.ToString("X");
                    }
                    else
                    {
                        str = "Failed writing to offset " +
                            m_dwOffset.ToString("X");
                    }

                    TraceLog(str, (wdc_err)dwStatus);
                    break;
                }
            case RW.READ_ALL:
                {
                    string sDisplay = "displaying EEPROM (" +
                        m_device.ToString(false) + ")" ;

                    for (DWORD dwOffset = 0; dwOffset < 0xff; dwOffset += 4)
                    {
                        if ((dwOffset % 0x10) == 0)
                        {
                            TraceLog(sDisplay, (wdc_err)dwStatus);
                            sDisplay = dwOffset.ToString("X2") + ": " ;
                        }
                        dwStatus = m_device.EEprom.EEPROM_Read(dwOffset,
                            ref m_u32Data);
                        if ((DWORD)wdc_err.WD_STATUS_SUCCESS != dwStatus)
                        {
                            sDisplay += Environment.NewLine + "Error " +
                                "occurred while reading offset 0x" +
                                dwOffset.ToString("X") + "of the serial " +
                                "EEPROM. Error 0x" + dwStatus.ToString("X") +
                                ": " + utils.Stat2Str((DWORD)dwStatus);
                            break;
                        }
                        sDisplay += m_u32Data.ToString("X8") + "  ";
                    }
                    TraceLog(sDisplay, (wdc_err)dwStatus);
                    break;
                }
            }
        }

        private void btLog_Click(object sender, System.EventArgs e)
        {
            txtData.Clear();
        }

        private void TraceLog(string str, wdc_err status)
        {
            txtData.Text += str + Environment.NewLine;

            string sForm = (m_regType == ACTION_TYPE.EEPROM) ?
                "EEPROM R/W Form: " : (m_regType == ACTION_TYPE.CFG) ?
                "CFG Registers R/W Form: " : "RT Registers R/W Form: ";

            if (m_direction == RW.READ_ALL)
            {
                Log.TraceLog(sForm + str);
                return;
            }

            string sMsg = sForm + str + " (" + m_device.ToString(false) + ")";

            if (status == wdc_err.WD_STATUS_SUCCESS)
                Log.TraceLog(sMsg);
            else
                Log.ErrLog(sMsg);
        }
    }
}

