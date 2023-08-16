// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using DWORD = System.UInt32;
using WORD = System.UInt16;
using BOOL = System.Boolean;
using Jungo.wdapi_dotnet;
using Jungo.plx_lib;

namespace Jungo.PLX_Sample
{
    public class OpenDMAForm : System.Windows.Forms.Form
    {
        private Exception m_excp;
        private uint m_u32LocalAddr;
        private RW m_direction;
        private ALLOCATION_TYPE m_wAllocType;
        private DWORD m_dwBytes;
        private BOOL m_bAutoInc;
        private System.Windows.Forms.ComboBox cmboAllocType;
        private System.Windows.Forms.TextBox txtLocalAddr;
        private System.Windows.Forms.TextBox txtBuffSize;
        private System.Windows.Forms.Label lblDmaOpen;
        private System.Windows.Forms.ComboBox cmboAction;
        private System.Windows.Forms.Label lblAllocType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btSubmit;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkBoxInc;

        private System.ComponentModel.Container components = null;

        public OpenDMAForm(PLX_DMA_CHANNEL channel)
        {
            InitializeComponent();

            lblDmaOpen.Text = (channel == 0)?
                "Open DMA - Channel 0":
                "Open DMA - Channel 1";

            cmboAllocType.Items.AddRange(new object[]{"Scatter Gather",
                "Contiguous", "Transaction SG", "Transaction Contig"});
            cmboAction.Items.AddRange(new object[]{"Read", "Write"});

            cmboAllocType.SelectedIndex = 0;
            cmboAction.SelectedIndex = 0;
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
            this.cmboAllocType = new System.Windows.Forms.ComboBox();
            this.txtLocalAddr = new System.Windows.Forms.TextBox();
            this.txtBuffSize = new System.Windows.Forms.TextBox();
            this.lblDmaOpen = new System.Windows.Forms.Label();
            this.cmboAction = new System.Windows.Forms.ComboBox();
            this.lblAllocType = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btSubmit = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkBoxInc = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // cmboAllocType
            this.cmboAllocType.Location = new System.Drawing.Point(24, 48);
            this.cmboAllocType.Name = "cmboAllocType";
            this.cmboAllocType.Size = new System.Drawing.Size(121, 21);
            this.cmboAllocType.TabIndex = 0;
            // txtLocalAddr
            this.txtLocalAddr.Location = new System.Drawing.Point(40, 144);
            this.txtLocalAddr.Name = "txtLocalAddr";
            this.txtLocalAddr.TabIndex = 3;
            this.txtLocalAddr.Text = "";
            // txtBuffSize
            this.txtBuffSize.Location = new System.Drawing.Point(40, 192);
            this.txtBuffSize.Name = "txtBuffSize";
            this.txtBuffSize.TabIndex = 4;
            this.txtBuffSize.Text = "";
            // lblDmaOpen
            this.lblDmaOpen.Location = new System.Drawing.Point(56, 8);
            this.lblDmaOpen.Name = "lblDmaOpen";
            this.lblDmaOpen.Size = new System.Drawing.Size(128, 23);
            this.lblDmaOpen.TabIndex = 5;
            this.lblDmaOpen.Text = "Open DMA - Channel 0";
            // cmboAction
            this.cmboAction.ItemHeight = 13;
            this.cmboAction.Location = new System.Drawing.Point(24, 96);
            this.cmboAction.Name = "cmboAction";
            this.cmboAction.Size = new System.Drawing.Size(121, 21);
            this.cmboAction.TabIndex = 2;
            // lblAllocType
            this.lblAllocType.Location = new System.Drawing.Point(24, 32);
            this.lblAllocType.Name = "lblAllocType";
            this.lblAllocType.Size = new System.Drawing.Size(128, 16);
            this.lblAllocType.TabIndex = 6;
            this.lblAllocType.Text = "Allocation type";
            // label3
            this.label3.Location = new System.Drawing.Point(24, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Read/Write";
            // label4
            this.label4.Location = new System.Drawing.Point(24, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "DMA local Address (hex)";
            // label5
            this.label5.Location = new System.Drawing.Point(24, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "DMA buffer size (hex)";
            // btSubmit
            this.btSubmit.Location = new System.Drawing.Point(184, 56);
            this.btSubmit.Name = "btSubmit";
            this.btSubmit.TabIndex = 11;
            this.btSubmit.Text = "Submit";
            this.btSubmit.Click += new System.EventHandler(this.btSubmit_Click);
            // btCancel
            this.btCancel.DialogResult =
                System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(184, 96);
            this.btCancel.Name = "btCancel";
            this.btCancel.TabIndex = 12;
            this.btCancel.Text = "Cancel";
            // groupBox1
            this.groupBox1.Location = new System.Drawing.Point(168, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(112, 88);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // label1
            this.label1.Location = new System.Drawing.Point(24, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 23);
            this.label1.TabIndex = 14;
            this.label1.Text = "0x";
            // label2
            this.label2.Location = new System.Drawing.Point(24, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 23);
            this.label2.TabIndex = 15;
            this.label2.Text = "0x";
            // chkBoxInc
            this.chkBoxInc.Size = new Size(100, 40);
            this.chkBoxInc.Location = new System.Drawing.Point(176, 160);
            this.chkBoxInc.Name = "chkBoxInc";
            this.chkBoxInc.TabIndex = 16;
            this.chkBoxInc.Text = "AutoIncrement Address";
            this.chkBoxInc.Checked = true;

            // OpenDMAForm
            this.AcceptButton = this.btSubmit;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(296, 229);
            this.Controls.Add(this.chkBoxInc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSubmit);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblAllocType);
            this.Controls.Add(this.lblDmaOpen);
            this.Controls.Add(this.txtBuffSize);
            this.Controls.Add(this.txtLocalAddr);
            this.Controls.Add(this.cmboAction);
            this.Controls.Add(this.cmboAllocType);
            this.Controls.Add(this.groupBox1);
            this.Name = "OpenDMAForm";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpenDMA_Form";
            this.ResumeLayout(false);

        }
#endregion

        public bool GetInput(ref uint u32LocalAddr, ref RW direction,
            ref ALLOCATION_TYPE wAllocType, ref DWORD dwBytes, ref BOOL
            bAutoInc)
        {
            DialogResult result = DialogResult.Retry;

            while ((result = ShowDialog()) == DialogResult.Retry);

            if (result != DialogResult.OK)
                return false;

            u32LocalAddr = m_u32LocalAddr;
            direction = m_direction;
            wAllocType = m_wAllocType;
            dwBytes = m_dwBytes;
            bAutoInc = m_bAutoInc;

            return true;
        }

        private void btSubmit_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            try
            {
                TranslateInput();
            }
            catch
            {
                MessageBox.Show(m_excp.Message , "Input Entry Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.Retry;
            }
        }

        private void TranslateInput()
        {
            m_excp = new Exception("Select Allocation Type");
            if ((uint)cmboAllocType.SelectedIndex == 0xffffffff)
                throw m_excp;
            m_wAllocType = (ALLOCATION_TYPE)cmboAllocType.SelectedIndex;

            m_excp = new Exception("Select DMA direction");
            if ((uint)cmboAction.SelectedIndex == 0xffffffff)
                throw m_excp;
            m_direction = (cmboAction.SelectedIndex == 0)?
                RW.READ : RW.WRITE;

            m_excp = new Exception("Enter local DMA address. " +
                "Entered value should be a hex number");
            m_u32LocalAddr = (DWORD)Convert.ToInt32(txtLocalAddr.Text, 16);

            m_excp = new Exception("Enter the DMA buffer's size in bytes. " +
                "Entered value should be a hex number");
            m_dwBytes = (DWORD)Convert.ToInt32(txtBuffSize.Text, 16);

            m_bAutoInc = chkBoxInc.Checked;
        }
    }
}

