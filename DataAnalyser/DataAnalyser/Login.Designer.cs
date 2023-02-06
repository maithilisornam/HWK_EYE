namespace DataAnalyser
{
    partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.msg = new System.Windows.Forms.Label();
			this.passwordLbl = new System.Windows.Forms.Label();
			this.lblUserName = new System.Windows.Forms.Label();
			this.userNameTextBox = new System.Windows.Forms.TextBox();
			this.passwordTextBox = new System.Windows.Forms.MaskedTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// msg
			// 
			this.msg.AutoSize = true;
			this.msg.BackColor = System.Drawing.Color.Transparent;
			this.msg.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.msg.ForeColor = System.Drawing.Color.Yellow;
			this.msg.Location = new System.Drawing.Point(138, 574);
			this.msg.Name = "msg";
			this.msg.Size = new System.Drawing.Size(0, 32);
			this.msg.TabIndex = 22;
			// 
			// passwordLbl
			// 
			this.passwordLbl.AutoSize = true;
			this.passwordLbl.BackColor = System.Drawing.Color.Transparent;
			this.passwordLbl.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.passwordLbl.ForeColor = System.Drawing.Color.White;
			this.passwordLbl.Location = new System.Drawing.Point(154, 255);
			this.passwordLbl.Name = "passwordLbl";
			this.passwordLbl.Size = new System.Drawing.Size(129, 32);
			this.passwordLbl.TabIndex = 17;
			this.passwordLbl.Text = "Password :";
			// 
			// lblUserName
			// 
			this.lblUserName.AutoSize = true;
			this.lblUserName.BackColor = System.Drawing.Color.Transparent;
			this.lblUserName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblUserName.ForeColor = System.Drawing.Color.White;
			this.lblUserName.Location = new System.Drawing.Point(140, 198);
			this.lblUserName.Name = "lblUserName";
			this.lblUserName.Size = new System.Drawing.Size(159, 32);
			this.lblUserName.TabIndex = 16;
			this.lblUserName.Text = "User Name* :";
			// 
			// userNameTextBox
			// 
			this.userNameTextBox.Location = new System.Drawing.Point(314, 204);
			this.userNameTextBox.Name = "userNameTextBox";
			this.userNameTextBox.Size = new System.Drawing.Size(149, 26);
			this.userNameTextBox.TabIndex = 23;
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.Location = new System.Drawing.Point(314, 255);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.Size = new System.Drawing.Size(149, 26);
			this.passwordTextBox.TabIndex = 24;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(243, 329);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(103, 40);
			this.button1.TabIndex = 25;
			this.button1.Text = "Login";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::DataAnalyser.Properties.Resources.theme1;
			this.ClientSize = new System.Drawing.Size(626, 766);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.passwordTextBox);
			this.Controls.Add(this.userNameTextBox);
			this.Controls.Add(this.lblUserName);
			this.Controls.Add(this.passwordLbl);
			this.Controls.Add(this.msg);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "Form1";
			this.Text = "LnW HawkEye";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label msg;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label LblAssetType;
        private System.Windows.Forms.Label passwordLbl;
        private System.Windows.Forms.Label lblAstNum;
		private System.Windows.Forms.TextBox userNameTextBox;
		private System.Windows.Forms.MaskedTextBox passwordTextBox;
		private System.Windows.Forms.Button button1;
	}
}

