namespace WordPad
{
    partial class ReplaceForm
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
            this.tbOldString = new System.Windows.Forms.TextBox();
            this.tbNewString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxReplaceAll = new System.Windows.Forms.CheckBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnCancelReplace = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbOldString
            // 
            this.tbOldString.Location = new System.Drawing.Point(52, 36);
            this.tbOldString.Name = "tbOldString";
            this.tbOldString.Size = new System.Drawing.Size(198, 20);
            this.tbOldString.TabIndex = 0;
            // 
            // tbNewString
            // 
            this.tbNewString.Location = new System.Drawing.Point(52, 88);
            this.tbNewString.Name = "tbNewString";
            this.tbNewString.Size = new System.Drawing.Size(198, 20);
            this.tbNewString.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Что менять:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "На что менять:";
            // 
            // checkBoxReplaceAll
            // 
            this.checkBoxReplaceAll.AutoSize = true;
            this.checkBoxReplaceAll.Location = new System.Drawing.Point(39, 126);
            this.checkBoxReplaceAll.Name = "checkBoxReplaceAll";
            this.checkBoxReplaceAll.Size = new System.Drawing.Size(97, 17);
            this.checkBoxReplaceAll.TabIndex = 4;
            this.checkBoxReplaceAll.Text = "Заменить все";
            this.checkBoxReplaceAll.UseVisualStyleBackColor = true;
            // 
            // btnReplace
            // 
            this.btnReplace.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnReplace.Location = new System.Drawing.Point(161, 155);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 5;
            this.btnReplace.Text = "Заменить";
            this.btnReplace.UseVisualStyleBackColor = true;
            // 
            // btnCancelReplace
            // 
            this.btnCancelReplace.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelReplace.Location = new System.Drawing.Point(242, 155);
            this.btnCancelReplace.Name = "btnCancelReplace";
            this.btnCancelReplace.Size = new System.Drawing.Size(75, 23);
            this.btnCancelReplace.TabIndex = 6;
            this.btnCancelReplace.Text = "Отмена";
            this.btnCancelReplace.UseVisualStyleBackColor = true;
            // 
            // ReplaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 190);
            this.Controls.Add(this.btnCancelReplace);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.checkBoxReplaceAll);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbNewString);
            this.Controls.Add(this.tbOldString);
            this.Name = "ReplaceForm";
            this.Text = "ReplaceForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbOldString;
        private System.Windows.Forms.TextBox tbNewString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxReplaceAll;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnCancelReplace;
    }
}