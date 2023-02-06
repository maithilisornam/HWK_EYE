using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataAnalyser
{
    public partial class Landing : Form
    {
        List<int> missingRowIds = new List<int>();

        public Landing()
        {
            InitializeComponent();
           
        }


		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            
            this.Hide();
            HardwarePreRequisites hrdWare = new HardwarePreRequisites();

            hrdWare.Show();
        }

		private void Landing_Load(object sender, EventArgs e)
		{

		}
	}
}