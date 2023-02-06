namespace DataAnalyser
{
	partial class Landing
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Landing));
			this.msg = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.linkLabel2 = new System.Windows.Forms.LinkLabel();
			this.linkLabel3 = new System.Windows.Forms.LinkLabel();
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
			// linkLabel1
			// 
			this.linkLabel1.Location = new System.Drawing.Point(84, 85);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(198, 29);
			this.linkLabel1.TabIndex = 25;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Hardware PreRequisites";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// linkLabel2
			// 
			this.linkLabel2.Location = new System.Drawing.Point(84, 143);
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new System.Drawing.Size(198, 29);
			this.linkLabel2.TabIndex = 26;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "Software PreRequisites";
			// 
			// linkLabel3
			// 
			this.linkLabel3.Location = new System.Drawing.Point(84, 198);
			this.linkLabel3.Name = "linkLabel3";
			this.linkLabel3.Size = new System.Drawing.Size(261, 29);
			this.linkLabel3.TabIndex = 27;
			this.linkLabel3.TabStop = true;
			this.linkLabel3.Text = "Intra Product Communication";
			// 
			// Landing
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::DataAnalyser.Properties.Resources.theme1;
			this.ClientSize = new System.Drawing.Size(626, 766);
			this.Controls.Add(this.linkLabel3);
			this.Controls.Add(this.linkLabel2);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.msg);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "Landing";
			this.Text = "LnW HawkEye";
			this.Load += new System.EventHandler(this.Landing_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label msg;
		private System.Windows.Forms.Label LblAssetType;
		private System.Windows.Forms.Label lblAstNum;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.LinkLabel linkLabel2;
		private System.Windows.Forms.LinkLabel linkLabel3;
	}
}

