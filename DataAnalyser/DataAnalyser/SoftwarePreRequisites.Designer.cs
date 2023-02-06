namespace DataAnalyser
{
	partial class HardwarePreRequisites
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoftwarePreRequisites));
			this.msg = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
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
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(91, 74);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(162, 24);
			this.checkBox1.TabIndex = 24;
			this.checkBox1.Text = "Operating System";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(91, 117);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(71, 24);
			this.checkBox2.TabIndex = 25;
			this.checkBox2.Text = "RAM";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// checkBox3
			// 
			this.checkBox3.AutoSize = true;
			this.checkBox3.Location = new System.Drawing.Point(91, 164);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(157, 24);
			this.checkBox3.TabIndex = 26;
			this.checkBox3.Text = "Number of Drives";
			this.checkBox3.UseVisualStyleBackColor = true;
			// 
			// HardwarePreRequisites
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::DataAnalyser.Properties.Resources.theme1;
			this.ClientSize = new System.Drawing.Size(626, 766);
			this.Controls.Add(this.checkBox3);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.msg);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HardwarePreRequisites";
			this.Text = "LnW HawkEye";
			this.Load += new System.EventHandler(this.HardwarePreRequisites_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label msg;
		private System.Windows.Forms.Label LblAssetType;
		private System.Windows.Forms.Label lblAstNum;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.CheckBox checkBox3;
	}
}

