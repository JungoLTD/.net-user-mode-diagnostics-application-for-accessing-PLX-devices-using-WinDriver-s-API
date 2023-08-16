// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Jungo.wdapi_dotnet;
using Jungo.plx_lib;
using wdc_err = Jungo.wdapi_dotnet.WD_ERROR_CODES;
using DWORD = System.UInt32;
using BYTE = System.Byte;
using BOOL = System.Boolean;

namespace Jungo.PLX_Sample
{
    public class DMATransferForm : System.Windows.Forms.Form
    {
        private PLX_MasterDevice m_masterDevice;
        private DmaBuffer m_dmaBuffer;
        private PLX_DMA_CHANNEL m_dmaChannel;
        private COMPLETION m_compMethod;
        private BOOL m_direction;
        private DWORD m_dwBytes;
        private IntPtr m_pData;
        private BYTE[] m_buff;
        private System.Windows.Forms.Label lblComp;
        private System.Windows.Forms.ComboBox cmboCompMethod;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.Button btReadWrite;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.Button btLog;
        private System.ComponentModel.Container components = null;

        public DMATransferForm(PLX_MasterDevice dev, DmaBuffer dmaBuffer,
            IntPtr pUserBuffer, PLX_DMA_CHANNEL dmaChannel, BOOL direction)
        {
            InitializeComponent();

            m_masterDevice = dev;
            m_dmaBuffer = dmaBuffer;
            m_pData = pUserBuffer;
            m_dmaChannel = dmaChannel;
            m_direction = direction;
            txtData.ReadOnly  = m_direction;
            cmboCompMethod.Items.AddRange(new object[]{"Interrupt", "Polling"});
            cmboCompMethod.SelectedIndex = 1;
            btReadWrite.Text = (m_direction)? "Read" : "Write";
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

#region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lblComp = new System.Windows.Forms.Label();
            this.cmboCompMethod = new System.Windows.Forms.ComboBox();
            this.btExit = new System.Windows.Forms.Button();
            this.btReadWrite = new System.Windows.Forms.Button();
            this.lblData = new System.Windows.Forms.Label();
            this.txtData = new System.Windows.Forms.TextBox();
            this.btLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // lblComp
            this.lblComp.Location = new System.Drawing.Point(24, 8);
            this.lblComp.Name = "lblComp";
            this.lblComp.Size = new System.Drawing.Size(120, 23);
            this.lblComp.TabIndex = 2;
            this.lblComp.Text = "Completion Method";
            // cmboCompMethod
            this.cmboCompMethod.Location = new System.Drawing.Point(24, 32);
            this.cmboCompMethod.Name = "cmboCompMethod";
            this.cmboCompMethod.Size = new System.Drawing.Size(121, 21);
            this.cmboCompMethod.TabIndex = 3;
            // btExit
            this.btExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btExit.Location = new System.Drawing.Point(232, 192);
            this.btExit.Name = "btExit";
            this.btExit.TabIndex = 10;
            this.btExit.Text = "Exit";
            // btReadWrite
            this.btReadWrite.Location = new System.Drawing.Point(24, 192);
            this.btReadWrite.Name = "btReadWrite";
            this.btReadWrite.TabIndex = 8;
            this.btReadWrite.Text = "Read";
            this.btReadWrite.Click +=
                new System.EventHandler(this.btReadWrite_Click);
            // lblData
            this.lblData.Location = new System.Drawing.Point(24, 72);
            this.lblData.Name = "lblData";
            this.lblData.TabIndex = 7;
            this.lblData.Text = "Data:";
            // txtData
            this.txtData.AutoSize = false;
            this.txtData.Location = new System.Drawing.Point(24, 96);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtData.Size = new System.Drawing.Size(288, 80);
            this.txtData.TabIndex = 6;
            this.txtData.Text = "";
            this.txtData.Click += new System.EventHandler(this.txtData_Clicked);
            // btLog
            this.btLog.Location = new System.Drawing.Point(128, 192);
            this.btLog.Name = "btLog";
            this.btLog.TabIndex = 9;
            this.btLog.Text = "Clear Log";
            this.btLog.Click += new System.EventHandler(this.btLog_Click);
            // DMATransferForm
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btExit;
            this.ClientSize = new System.Drawing.Size(336, 229);
            this.Controls.Add(this.btLog);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.btReadWrite);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.lblComp);
            this.Controls.Add(this.cmboCompMethod);
            this.Name = "DMATransferForm";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DMATransferForm";
            this.ResumeLayout(false);
        }
#endregion

        public bool GetInput()
        {
            DialogResult result = DialogResult.Retry;
            while ((result = ShowDialog()) == DialogResult.Retry);

            return true;
        }

        private void btReadWrite_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.None;
            try
            {
                TranslateInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Input Entry Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.Retry;
            }
        }

        private void TranslateInput()
        {
            DWORD dwStatus = 0;
            BOOL bIsRead = m_direction;
            m_dwBytes = m_dmaBuffer.BuffSize;

            if ((uint)cmboCompMethod.SelectedIndex == 0xffffffff)
                throw new Exception("Select the Completion Method");
            m_compMethod = (cmboCompMethod.SelectedIndex == 0)?
                COMPLETION.INTERRUPT : COMPLETION.POLLING;

            m_buff = new byte[m_dwBytes];

            if (!bIsRead)
            {
                if (txtData.Text == "")
                {
                    throw new Exception("Please enter the data to be " +
                        "written");
                }

                string str = diag_lib.PadBuffer(txtData.Text,
                    (DWORD)txtData.Text.Length,(DWORD)2 * m_dwBytes);

                for (int i = 0; i < m_dwBytes; ++i)
                    m_buff[i] = Convert.ToByte(str.Substring(2 * i, 2), 16);

                Marshal.Copy(m_buff, 0 , m_pData, (int)m_dwBytes);
            }

            dwStatus = m_masterDevice.DMATransfer(m_dmaChannel, m_dmaBuffer,
                (m_compMethod == COMPLETION.INTERRUPT));

            if (dwStatus == (DWORD)wdc_err.WD_STATUS_SUCCESS)
            {
                if (m_compMethod != COMPLETION.POLLING)
                {
                    DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    if (bIsRead)
                        Marshal.Copy(m_pData, m_buff, 0, (int)m_dwBytes);
                }
            }

            TraceLog(bIsRead, (wdc_err)dwStatus);
        }

        private void txtData_Clicked(object sender, System.EventArgs e)
        {
            if (txtData.ReadOnly == false)
                txtData.Clear();
        }

        private void btLog_Click(object sender, System.EventArgs e)
        {
            txtData.Clear();
        }

        private void TraceLog(BOOL bIsRead, wdc_err status)
        {
            string sData = "";
            string sInfo = "";
            if (status == wdc_err.WD_STATUS_SUCCESS)
            {
                sData = (bIsRead? "R: " : "W: ")
                    + diag_lib.DisplayHexBuffer(m_buff, m_dwBytes);

                sInfo = " (" + m_masterDevice.ToString(false) + ")";
                Log.TraceLog("DMATransferForm: " + sData + sInfo);
            }
            else
            {
                sData = "failed to complete the DMA transfer. status 0x" +
                    status.ToString("X") + ": " + utils.Stat2Str((DWORD)status);
                sInfo = "(" + m_masterDevice.ToString(false) + ")";
                Log.ErrLog("DMATransferForm: " + sData + sInfo);
            }

            txtData.Text += sData + Environment.NewLine;
        }
    }
}

