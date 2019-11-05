namespace PVDataSampler
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_btOK = new System.Windows.Forms.Button();
            this.m_btCancel = new System.Windows.Forms.Button();
            this.m_cbGridCounter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_btOK
            // 
            this.m_btOK.Location = new System.Drawing.Point(166, 248);
            this.m_btOK.Name = "m_btOK";
            this.m_btOK.Size = new System.Drawing.Size(75, 23);
            this.m_btOK.TabIndex = 0;
            this.m_btOK.Text = "OK";
            this.m_btOK.UseVisualStyleBackColor = true;
            this.m_btOK.Click += new System.EventHandler(this.m_btOK_Click);
            // 
            // m_btCancel
            // 
            this.m_btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btCancel.Location = new System.Drawing.Point(247, 248);
            this.m_btCancel.Name = "m_btCancel";
            this.m_btCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btCancel.TabIndex = 1;
            this.m_btCancel.Text = "Abbrechen";
            this.m_btCancel.UseVisualStyleBackColor = true;
            // 
            // m_cbGridCounter
            // 
            this.m_cbGridCounter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cbGridCounter.FormattingEnabled = true;
            this.m_cbGridCounter.Location = new System.Drawing.Point(148, 12);
            this.m_cbGridCounter.Name = "m_cbGridCounter";
            this.m_cbGridCounter.Size = new System.Drawing.Size(121, 21);
            this.m_cbGridCounter.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Zähler des Versorgers:";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.m_btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btCancel;
            this.ClientSize = new System.Drawing.Size(334, 283);
            this.Controls.Add(this.m_cbGridCounter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_btCancel);
            this.Controls.Add(this.m_btOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Einstellungen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btOK;
        private System.Windows.Forms.Button m_btCancel;
        private System.Windows.Forms.ComboBox m_cbGridCounter;
        private System.Windows.Forms.Label label1;
    }
}