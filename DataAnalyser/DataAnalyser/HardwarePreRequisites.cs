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
    public partial class HardwarePreRequisites : Form
    {
        List<int> missingRowIds = new List<int>();
        List<HardwarePreRequisiteDetails> lstHardware = new List<HardwarePreRequisiteDetails>();
        String PROP_OS = "Operating System";
        String PROP_RAM = "RAM Size";
        String PROP_NO_OF_DRIVES = "No. of Drives";
        class HardwarePreRequisiteDetails
		{
            public HardwarePreRequisiteDetails(int ID, String hrdwrPropName,  String minimumReq)
			{
                this.ID = ID;
                this.hrdwrPropName = hrdwrPropName;
                this.minimumReq = minimumReq;
			}
            public int ID { get; set; }
            public String hrdwrPropName { get; set; }

            public String minimumReq { get; set; }
		}

        private void GetSystemInfo()
        {
            long memKb;
            DataAnalyser.Program.GetPhysicallyInstalledSystemMemory(out memKb);
            
            StringBuilder systemInfo = new StringBuilder(string.Empty);

            HardwarePreRequisiteDetails hrdwrOs = new HardwarePreRequisiteDetails(1, PROP_OS, Environment.OSVersion.ToString());
            HardwarePreRequisiteDetails hrdwrRAM = new HardwarePreRequisiteDetails(2, PROP_RAM, (memKb / 1024 / 1024).ToString());
            HardwarePreRequisiteDetails hrdwrDrives = new HardwarePreRequisiteDetails(3, PROP_NO_OF_DRIVES, GetNumberOfDrives().ToString());
            
            lstHardware.Add(hrdwrOs);
            lstHardware.Add(hrdwrRAM);
            lstHardware.Add(hrdwrDrives);

            systemInfo.AppendFormat("Processor Architecture:  {0}n", Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));
            systemInfo.AppendFormat("Processor Model:  {0}n", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"));
            systemInfo.AppendFormat("Processor Level:  {0}n", Environment.GetEnvironmentVariable("PROCESSOR_LEVEL"));
            systemInfo.AppendFormat("SystemDirectory:  {0}n", Environment.SystemDirectory);
            systemInfo.AppendFormat("ProcessorCount:  {0}n", Environment.ProcessorCount);
            systemInfo.AppendFormat("UserDomainName:  {0}n", Environment.UserDomainName);
            systemInfo.AppendFormat("UserName: {0}n", Environment.UserName);
            //Drives
            systemInfo.AppendFormat("LogicalDrives:n");
            
            systemInfo.AppendFormat("Version:  {0}", Environment.Version);

        }

		private int GetNumberOfDrives()
		{
            int noOfDrives = 0;
            foreach (System.IO.DriveInfo DriveInfo1 in System.IO.DriveInfo.GetDrives())
            {
                try
                {
                    noOfDrives = noOfDrives+1;
                    //systemInfo.AppendFormat("t Drive: {0}ntt VolumeLabel: " +
                    //    "{1}ntt DriveType: {2}ntt DriveFormat: {3}ntt " +
                    //    "TotalSize: {4}ntt AvailableFreeSpace: {5}n",
                    //    DriveInfo1.Name, DriveInfo1.VolumeLabel, DriveInfo1.DriveType,
                    //    DriveInfo1.DriveFormat, DriveInfo1.TotalSize, DriveInfo1.AvailableFreeSpace);
                }
                catch
                {
                }
            }
            return noOfDrives;
        }

        public HardwarePreRequisites()
        {
            InitializeComponent();
            GetSystemInfo();


            for(int i = 0; i< lstHardware.Count; i++)
			{
				if (lstHardware[i].hrdwrPropName.Equals(PROP_OS))
				{
                    if(lstHardware[i].minimumReq == "Microsoft Windows NT 6.2.9200.0"){
                        this.checkBox1.Checked = true;
                    }
                    else
                    {
                        this.checkBox1.Checked = false;
                        this.checkBox1.Text = this.checkBox1.Text + lstHardware[i].minimumReq;
                    }

                }
                if (lstHardware[i].hrdwrPropName.Equals(PROP_RAM))
                {
                    if (lstHardware[i].minimumReq == "8")
                    {
                        this.checkBox2.Checked = true;
                    }
                    else
                    {
                        this.checkBox2.Checked = false;
                        this.checkBox2.Text = this.checkBox2.Text + lstHardware[i].minimumReq;
                    }
                }
				if (lstHardware[i].hrdwrPropName.Equals(PROP_NO_OF_DRIVES))
				{
                    if (lstHardware[i].minimumReq == "3")
                    {
                        this.checkBox3.Checked = true;
                    }
                    else
                    {
                        this.checkBox3.Checked = false;
                        this.checkBox3.Text = this.checkBox3.Text + lstHardware[i].minimumReq;
                    }

                }
            }
            

        }

	}
}