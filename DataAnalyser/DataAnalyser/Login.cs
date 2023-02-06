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
    public partial class Form1 : Form
    {
        List<int> missingRowIds = new List<int>();
        public static String STR_AM_OPTION_DATA  = "Asset Matrix OptionData";
        public static String STR_STATE = "State";
        public static String STR_SITE = "Site";
        public static String STR_ASSET_SUM = "Asset Matrix OptionData";
        public static String STR_COMP_SUM = "Asset Matrix OptionData";
        public static String STR_OPTION_CODE = "OPTION CODE";
        public static String STR_AM_OP_VAL = "AM OPTION VALUE";
        public static String STR_SDS_OP_VAL = "SDS OPTION VALUE";
        public static String STR_ERR_STAT = "Please select a state\n";
        public static String STR_ERR_AST_TYP = "Please select an asset type\n";
        public static String STR_SUC_OPT_DATA = "OptionData Succesfully Exported....";
        public static String STR_SUC_EXP = "SuccessFully Exported.!!!";
        public static String STR_NO_DATA = "No Data.Change Your search Criteria.";

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            msg.Visible = false;
        }

		private void button1_Click(object sender, EventArgs e)
		{
            if(this.userNameTextBox.Text =="admin" && this.passwordTextBox.Text == "admin")
			{
               
            }
            //Moving on to the next screen
            DataAnalyser.Form1 form1 = this;
            this.Hide();
            Landing frm2 = new Landing();
            //frm2.ShowDialog();
            frm2.Show();
        }
    }
}
