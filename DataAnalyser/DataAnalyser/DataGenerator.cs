using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataAnalyser
{
    public static class DataGenerator
    {
        public static DataConfigurations DataConfigurations
        {
            get;
            set;
        }
        public static QueryConfiguration QueryConfiguration
        {
            get;
            set;
        }
        public static SDSConfig selectedState
        {
            get;
            set;
        }
        public static string selectedSite
        {
            get;
            set;
        }
        public static AssetTypes AssetTypesFromXML
        {
            get;
            set;
        }

        public static ComponentTypes ComponentTypesFromXML
        {
            get;
            set;
        }


        public static List<AssetType> AssetTypeLst
        {
            get;
            set;
        }
        public static AssetType selectedAssetType
        {
            get;
            set;
        }
        public static string AssetNumber
        {
            get;
            set;
        }
        public static string Key
        {
            get;
            set;
        }
        public static Dictionary<string, string> AssetToKeyId
        {
            get;
            set;
        }
        static string AMServer = string.Empty;
        static string AM_Db_name = string.Empty;
        static string AMun = string.Empty;
        static string AMPwd = string.Empty;

        static string PVServer = string.Empty;
        static string PV_Db_name = string.Empty;
        static string PVun = string.Empty;
        static string PVPwd = string.Empty;

        static string SDSServer = string.Empty;
        static string SDS_Db_name = string.Empty;
        static string SDSun = string.Empty;
        static string SDSPwd = string.Empty;

        static string BCCServer = string.Empty;
        static string BCC_Db_name = string.Empty;
        static string BCCun = string.Empty;
        static string BCCPwd = string.Empty;

        static string EBSServer = string.Empty;
        static string EBS_Db_name = string.Empty;
        static string EBSun = string.Empty;
        static string EBSPwd = string.Empty;

        static string AMCommandstr = string.Empty;
        static string PVCommandstr = string.Empty;
        static string SDSCommandstr = string.Empty;
        static string EBSCommandstr = string.Empty;
        static string BCCCommandstr = string.Empty;

        static int AMColumnNumber = 0;
        static int SDSColumnNumber = 0;
        public static List<int> missingRowIds = new List<int>();
        public static String AM_STR = "AM";
        public static String SDS_STR = "SDS";
        public static String PV_STR = "PV";
        public static String LINE_ADDRESS = "SDS Line Address";
        public static String EBS_STR = "EBS";
        public static String BCC_STR = "BCC";
        public static String STR_ASSET_SUM = "Asset Mismatch Summary";
        public static String STR_COMP_SUM = "Component Mismatch Summary";
        public static String STR_AST_OPT_SUM = "Asset Option Mismatch Summary";
        static Dictionary<string, string> assetsToNumberLineAddress = new Dictionary<string, string>();
        public static void ReadQueryConfiguration()
        {
            string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string filePath = appPath + @"QueryConfiguration.xml";
            XmlSerializer _xmlserializer = new XmlSerializer(typeof(QueryConfiguration));
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                QueryConfiguration = (QueryConfiguration)_xmlserializer.Deserialize(stream);
                stream.Close();
            }
        }
        public static void ReadDataConfiguration()
        {
            string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string filePath = appPath + @"DataConfiguration.xml";
            XmlSerializer _xmlserializer = new XmlSerializer(typeof(DataConfigurations));
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                DataConfigurations = (DataConfigurations)_xmlserializer.Deserialize(stream);
                stream.Close();
            }
        }

        public static void CreateAssetTypeList()
        {
            AssetTypeLst = new List<AssetType>();
            string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string filePath = appPath + @"AssetTypeConfiguration.xml";
            XmlSerializer _xmlserializer = new XmlSerializer(typeof(AssetTypeConfiguration));
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                AssetTypeConfiguration assetTypeConfigured = (AssetTypeConfiguration)_xmlserializer.Deserialize(stream);
                AssetTypesFromXML = assetTypeConfigured.AssetTypes;
                stream.Close();
            }

            foreach (PerAssetType perAsset in AssetTypesFromXML)
            {
                AssetTypeLst.Add(new AssetType { TypeName = perAsset.Value, AmTypeId = perAsset.AmTypeId, PvTypeId = perAsset.PvTypeId });
            }
        }

        public static void CreateComponentTypeList()
        {
            AssetTypeLst = new List<AssetType>();
            string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string filePath = appPath + @"ComponentTypeConfiguration.xml";
            XmlSerializer _xmlserializer = new XmlSerializer(typeof(ComponentTypeConfiguration));
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                ComponentTypeConfiguration componentTypeConfigured = (ComponentTypeConfiguration)_xmlserializer.Deserialize(stream);
                ComponentTypesFromXML = componentTypeConfigured.ComponentTypes;
                stream.Close();
            }

            foreach (PerComponentType perComponent in ComponentTypesFromXML)
            {
                AssetTypeLst.Add(new AssetType { TypeName = perComponent.Value, AmTypeId = perComponent.AmTypeId, isComp = true });
            }
        }

        public static void SetDbDetails(DbDetails input, string Product)
        {
            switch (Product)
            {
                case "AM":
                    AMServer = input.ServerName;
                    AM_Db_name = input.DataBase;
                    AMun = input.UserId;
                    AMPwd = input.Password;
                    break;
                case "PV":
                    PVServer = input.ServerName;
                    PV_Db_name = input.DataBase;
                    PVun = input.UserId;
                    PVPwd = input.Password;
                    break;
                case "SDS":
                    SDSServer = input.ServerName;
                    SDS_Db_name = input.DataBase;
                    SDSun = input.UserId;
                    SDSPwd = input.Password;
                    break;
                case "BCC":
                    BCCServer = input.ServerName;
                    BCC_Db_name = input.DataBase;
                    BCCun = input.UserId;
                    BCCPwd = input.Password;
                    break;
                case "EBS":
                    EBSServer = input.ServerName;
                    EBS_Db_name = input.DataBase;
                    EBSun = input.UserId;
                    EBSPwd = input.Password;
                    break;
            }
        }

        public static string FrameConnString(string server, string database, string un, string Pwd)
        {
            string ConnectionString = "Server=" + server + ";Database=" + database + ";User id=" + un + ";Password=" + Pwd + ";Trusted_Connection=False;Connect Timeout=3000";
            return ConnectionString;
        }

        public static void FrameCommands(string selectedComp = "", string listofsiteids = "")
        {
            if (QueryConfiguration == null || QueryConfiguration.Queries == null || QueryConfiguration.Queries.Count == 0)
                return;


            AMCommandstr = SDSCommandstr = PVCommandstr = string.Empty;
            if (selectedComp == "Options")
            {
                AMCommandstr = FrameQuery("sp_GetAssetOptions", Key);
                SDSCommandstr = FrameQuery("Q_GetSDSAssetOptions", selectedSite, AssetNumber);
            }
            else if (selectedComp == "OptionsList")
            {
                if (!string.IsNullOrEmpty(selectedSite))
                {
                    AMCommandstr = FrameQuery("sp_GetAssetList", selectedAssetType.AmTypeId, selectedSite);
                }
            }
            else if (selectedComp == "")
            {
                AMCommandstr = FrameQuery("sp_GetAssets", selectedAssetType.AmTypeId, selectedSite);
                if (selectedAssetType.PvTypeId == "-1")
                {
                    PVCommandstr = FrameQuery("Q_GetAssets_Progressive", selectedSite);
                }
                else if (selectedAssetType.PvTypeId == "-2")
                {
                    if (selectedSite == null)
                        selectedSite = string.Empty;
                    AMCommandstr = FrameQuery("sp_GetSlotLinks", selectedSite);
                    PVCommandstr = FrameQuery("Q_PV_GetSlotLinks", selectedSite);
                }
                else
                {
                    PVCommandstr = FrameQuery("Q_PV_GetAssets", selectedAssetType.PvTypeId, selectedSite);
                }

                if (selectedAssetType.TypeName == "Slot")
                {
                    SDSCommandstr = FrameQuery("Q_SDS_GetAssets", selectedSite);
                    EBSCommandstr = FrameQuery("Q_EBS_GetAssets", selectedSite);
                    BCCCommandstr = FrameQuery("Q_BCC_GetAssets", selectedSite);
                }
                else if (selectedAssetType.PvTypeId == "-1")
                {
                    SDSCommandstr = FrameQuery("Q_SDS_GetAssets_Progressive", selectedSite);
                }
                else if (selectedAssetType.PvTypeId == "-2")
                {
                    SDSCommandstr = FrameQuery("Q_SDS_GetAssets_SlotLink", selectedSite);
                }
                else
                    SDSCommandstr = FrameQuery("Q_SDS_GetAssets_NonSlot", selectedAssetType.PvTypeId, selectedSite);
            }
            else
            {
                switch (selectedComp)
                {

                    case "Area/Zone/Bank":
                        AMCommandstr = FrameQuery("sp_GetAZBOptions", listofsiteids);
                        SDSCommandstr = FrameQuery("Q_SDS_GetAZBOptions", listofsiteids);
                        PVCommandstr = FrameQuery("Q_PV_GetAZBOptions", listofsiteids);
                        AMColumnNumber = 0;
                        SDSColumnNumber = 0;
                        break;
                    case "Manufacturer":
                        AMCommandstr = FrameQuery("sp_GetComponentOptions", selectedAssetType.AmTypeId);
                        SDSCommandstr = FrameQuery("Q_SDS_GetComponentOptions", null);
                        AMColumnNumber = 1;
                        SDSColumnNumber = 1;
                        break;
                    case "Model Type":
                        AMCommandstr = FrameQuery("sp_GetComponentOptionsMDLTYPE", selectedAssetType.AmTypeId);
                        SDSCommandstr = FrameQuery("Q_SDS_GetModelType", null);
                        AMColumnNumber = 1;
                        SDSColumnNumber = 0;
                        break;
                    case "Model":
                        AMCommandstr = FrameQuery("sp_ModelNames", null);
                        SDSCommandstr = FrameQuery("Q_SDS_Model", null);
                        AMColumnNumber = 0;
                        SDSColumnNumber = 0;
                        break;
                    case "Denomination":
                        AMCommandstr = FrameQuery("sp_GetDenomOrPOSOptions", selectedAssetType.AmTypeId);
                        SDSCommandstr = FrameQuery("Q_SDS_GetDenom", null);
                        AMColumnNumber = 0;
                        SDSColumnNumber = 0;
                        break;
                }
            }
        }

        public static string FrameQuery(string key, params object[] args)
        {
            string Query = string.Empty;
            if (QueryConfiguration.Queries.Any(x => x.Key == key))
            {
                Query = QueryConfiguration.Queries.FirstOrDefault(x => x.Key == key).Value;
                if (args != null && args.Length > 0)
                    Query = string.Format(Query, args);
            }
            return Query;
        }
        public static DataSet ConnectToDB(string Product)
        {
            DataSet Ds = new DataSet();
            using (SqlConnection conn = new SqlConnection())
            {
                string command = string.Empty;
                switch (Product)
                {
                    case "AM":
                        conn.ConnectionString = FrameConnString(AMServer, AM_Db_name, AMun, AMPwd);
                        command = AMCommandstr;
                        break;
                    case "PV":
                        conn.ConnectionString = FrameConnString(PVServer, PV_Db_name, PVun, PVPwd);
                        command = PVCommandstr;
                        break;
                    case "SDS":
                        conn.ConnectionString = FrameConnString(SDSServer, SDS_Db_name, SDSun, SDSPwd);
                        command = SDSCommandstr;
                        break;
                    case "EBS":
                        conn.ConnectionString = FrameConnString(EBSServer, EBS_Db_name, EBSun, EBSPwd);
                        command = EBSCommandstr;
                        break;
                    case "BCC":
                        conn.ConnectionString = FrameConnString(BCCServer, BCC_Db_name, BCCun, BCCPwd);
                        command = BCCCommandstr;
                        break;
                }

                //  Open Connection
                try
                {
                    if (string.IsNullOrEmpty(command))
                        return Ds;
                    conn.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                SqlCommand DataCommand = new SqlCommand(command, conn);
                DataCommand.CommandTimeout = 0;
                SqlDataAdapter sqlAdaptor = new SqlDataAdapter();
                sqlAdaptor.SelectCommand = DataCommand;
                sqlAdaptor.Fill(Ds);
                try
                {
                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    if (null != conn)
                    {
                        conn.Close();
                    }

                }
            }

            return Ds;
        }

        public static DataTable MismatchAssets(DataTable AM, DataTable PV, DataTable SDS, DataTable BCC, DataTable EBS, int rowcount = 0)
        {
            DataTable AssetSummary = new DataTable(STR_ASSET_SUM);
            bool isSlot = false;
            if (DataGenerator.selectedAssetType.TypeName == "Slot")
            {
                isSlot = true;
            }
            AssetSummary.Columns.Add("Site Number", typeof(string));
            AssetSummary.Columns.Add(AM_STR, typeof(string));
            AssetSummary.Columns.Add(SDS_STR, typeof(string));
            AssetSummary.Columns.Add(PV_STR, typeof(string));
            if (isSlot)
            {
                AssetSummary.Columns.Add(BCC_STR, typeof(string));
                AssetSummary.Columns.Add(EBS_STR, typeof(string));
                AssetSummary.Columns.Add(LINE_ADDRESS, typeof(string));
            }

            Dictionary<DataTable, int> List = new Dictionary<DataTable, int>();
            List.Add(AM, AM.Rows.Count);
            List.Add(PV, PV.Rows.Count);
            List.Add(SDS, SDS.Rows.Count);
            if (isSlot)
            {
                List.Add(BCC, BCC.Rows.Count);
                List.Add(EBS, EBS.Rows.Count);
            }

            var sortedDict = from entry in List orderby entry.Value descending select entry;



            Dictionary<string, List<string>> assets1 = CreateDataDic(sortedDict.ElementAt(0).Key, ref assetsToNumberLineAddress);
            Dictionary<string, List<string>> assets2 = CreateDataDic(sortedDict.ElementAt(1).Key, ref assetsToNumberLineAddress);
            Dictionary<string, List<string>> assets3 = CreateDataDic(sortedDict.ElementAt(2).Key, ref assetsToNumberLineAddress);
            Dictionary<string, List<string>> assets4 = null;
            Dictionary<string, List<string>> assets5 = null;
            if (isSlot)
            {
                assets4 = CreateDataDic(sortedDict.ElementAt(3).Key, ref assetsToNumberLineAddress);
                assets5 = CreateDataDic(sortedDict.ElementAt(4).Key, ref assetsToNumberLineAddress);
            }

            List<String> rowValues = new List<String>();

            doComparison(rowcount, AssetSummary, assets1, assets2, assets3, assets4, assets5, rowValues);
            doComparison(rowcount, AssetSummary, assets2, assets1, assets3, assets4, assets5, rowValues);
            doComparison(rowcount, AssetSummary, assets3, assets1, assets2, assets4, assets5, rowValues);
            if (isSlot)
            {
                doComparison(rowcount, AssetSummary, assets4, assets1, assets2, assets3, assets5, rowValues);
                doComparison(rowcount, AssetSummary, assets5, assets1, assets2, assets3, assets4, rowValues);
            }
            return AssetSummary;
        }

        private static void doComparison(int rowcount, DataTable AssetSummary, Dictionary<string, List<string>> assets1,
            Dictionary<string, List<string>> assets2, Dictionary<string, List<string>> assets3,
            Dictionary<string, List<string>> assets4, Dictionary<string, List<string>> assets5, List<String> rowValues)
        {

            if (assets1.Count > 0)
            {
                foreach (string Ritem in assets1.ElementAt(0).Value)
                {
                    String s;
                    bool missing = false;
                    if (!string.IsNullOrEmpty(Ritem))
                    {
                        DataRow R = AssetSummary.NewRow();
                        s = selectedSite + Ritem;
                        R["Site Number"] = selectedSite;
                        R[assets1.ElementAt(0).Key] = Ritem;
                        if (assets2.Count > 0)
                        {
                            if (!(assets2.ElementAt(0).Value.Contains(Ritem)))
                            {
                                R[assets2.ElementAt(0).Key] = "";
                                int count = AssetSummary.Rows.Count + rowcount;
                                missing = true;
                            }
                            else
                            {

                                R[assets2.ElementAt(0).Key] = Ritem;
                            }
                        }
                        else
                        {
                            missing = true;
                        }

                        if (assets3.Count > 0)
                        {
                            if (!(assets3.ElementAt(0).Value.Contains(Ritem)))
                            {
                                R[assets3.ElementAt(0).Key] = "";
                                int count = AssetSummary.Rows.Count + rowcount;
                                if (!missing) missing = true;
                            }
                            else
                            {

                                R[assets3.ElementAt(0).Key] = Ritem;
                            }
                        }
                        else
                        {
                            missing = true;
                        }

                        if (null != assets4 && assets4.Count > 0)
                        {
                            if (!(assets4.ElementAt(0).Value.Contains(Ritem)))
                            {
                                R[assets4.ElementAt(0).Key] = "";
                                int count = AssetSummary.Rows.Count + rowcount;
                                if (!missing) missing = true;
                            }
                            else
                            {

                                R[assets4.ElementAt(0).Key] = Ritem;
                            }
                        }
                        else if (null != assets4)
                        {
                            missing = true;
                        }

                        if (null != assets5 && assets5.Count > 0)
                        {
                            if (!(assets5.ElementAt(0).Value.Contains(Ritem)))
                            {
                                R[assets5.ElementAt(0).Key] = "";
                                int count = AssetSummary.Rows.Count + rowcount;
                                if (!missing) missing = true;
                            }
                            else
                            {

                                R[assets5.ElementAt(0).Key] = Ritem;
                            }
                        }
                        else if (null != assets5)
                        {
                            missing = true;
                        }

                        if (missing)
                        {

                            if (assets1.ElementAt(0).Key == SDS_STR && DataGenerator.selectedAssetType.TypeName == "Slot")
                            {
                                R[LINE_ADDRESS] = assetsToNumberLineAddress[Ritem];
                            }
                            if (!rowValues.Contains(s))
                                AssetSummary.Rows.Add(R);

                            rowValues.Add(s);

                        }
                    }
                }
            }
        }

        public static void MismatchComponents(DataTable AM, DataTable SDS, DataTable PV, ref DataSet ExportDs, string ComponentName = "", int rowcount = 0)
        {
            DataTable AssetSummary = new DataTable();

            if (selectedAssetType.PvTypeId == "-2")
                AssetSummary.TableName = STR_ASSET_SUM;
            else
                AssetSummary.TableName = STR_COMP_SUM;

            Dictionary<DataTable, int> List = new Dictionary<DataTable, int>();
            List.Add(AM, AM.Rows.Count);
            List.Add(SDS, SDS.Rows.Count);

            var sortedDict = from entry in List orderby entry.Value descending select entry;
            Dictionary<string, List<string>> assets1 = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> assets2 = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> assets3 = new Dictionary<string, List<string>>();
            if (ComponentName.Equals("Area/Zone/Bank") || (selectedAssetType.PvTypeId == "-2"))
            {
                assets1 = CreateDataDicAZB(AM);
                assets2 = CreateDataDicAZB(SDS);
                assets3 = CreateDataDicAZB(PV);

                List<String> lstOfCaption = new List<String>();
                List<String> summaryColumns = new List<String>();
                lstOfCaption.Add(AM.TableName);
                lstOfCaption.Add(SDS.TableName);
                lstOfCaption.Add(PV.TableName);

                if (assets1.Count > 0)
                {
                    foreach (String perSheetName in lstOfCaption)
                    {
                        foreach (DataColumn item in AM.Columns)
                        {
                            String colName = perSheetName + item.Caption;
                            if (!AssetSummary.Columns.Contains(colName))
                            {
                                AssetSummary.Columns.Add(colName, typeof(string));
                                if (perSheetName.Equals(AM.TableName))
                                    summaryColumns.Add(item.Caption);
                            }
                        }
                    }
                }
                List<String> rowValues = new List<String>();

                doCompComparison(assets1, assets2, assets3, AssetSummary, ref summaryColumns, rowValues);
                doCompComparison(assets2, assets3, assets1, AssetSummary, ref summaryColumns, rowValues);
                doCompComparison(assets3, assets2, assets1, AssetSummary, ref summaryColumns, rowValues);
                ExportDs.Tables.Add(AssetSummary);
            }
            else
            {
                AssetSummary.Columns.Add(AM_STR, typeof(string));
                AssetSummary.Columns.Add(SDS_STR, typeof(string));
                int index0ColNumber = 0;
                int index1ColNumber = 0;
                if (sortedDict.ElementAt(0).Key.TableName.Equals(SDS_STR))
                {
                    index0ColNumber = SDSColumnNumber;
                    index1ColNumber = AMColumnNumber;
                }
                else
                {
                    index0ColNumber = AMColumnNumber;
                    index1ColNumber = SDSColumnNumber;
                }
                assets1 = CreateDataDic(sortedDict.ElementAt(0).Key, index0ColNumber);
                assets2 = CreateDataDic(sortedDict.ElementAt(1).Key, index1ColNumber);
                if (assets1.Count > 0 && assets2.Count > 0)
                {
                    foreach (string Ritem in assets1.ElementAt(0).Value)
                    {
                        if (!string.IsNullOrEmpty(Ritem))
                        {
                            DataRow R = AssetSummary.NewRow();
                            R[assets1.ElementAt(0).Key] = Ritem;

                            if (!(assets2.ElementAt(0).Value.Contains(Ritem)))
                            {
                                R[assets2.ElementAt(0).Key] = "";
                                if (!missingRowIds.Contains(AssetSummary.Rows.Count + rowcount)) missingRowIds.Add(AssetSummary.Rows.Count + rowcount);
                            }
                            else
                            {
                                R[assets2.ElementAt(0).Key] = Ritem;
                            }
                            AssetSummary.Rows.Add(R);
                        }
                    }
                    foreach (string Ritem in assets2.ElementAt(0).Value)
                    {
                        if (!string.IsNullOrEmpty(Ritem))
                        {
                            DataRow R = AssetSummary.NewRow();
                            R[assets2.ElementAt(0).Key] = Ritem;

                            if (!(assets1.ElementAt(0).Value.Contains(Ritem)))
                            {
                                R[assets1.ElementAt(0).Key] = "";
                                if (!missingRowIds.Contains(AssetSummary.Rows.Count + rowcount)) missingRowIds.Add(AssetSummary.Rows.Count + rowcount);
                                AssetSummary.Rows.Add(R);
                            }
                        }
                    }
                    ExportDs.Tables.Add(AssetSummary);
                }
            }
        }

        private static void doCompComparison(Dictionary<string, List<string>> assets1, Dictionary<string, List<string>> assets2, Dictionary<string, List<string>> assets3,
            DataTable AssetSummary, ref List<String> summaryColumns, List<String> rowValues)
        {
            if (assets1.Count > 0)
            {
                

                foreach (string Ritem in assets1.ElementAt(0).Value)
                {
                    DataRow R = null;
                    bool existingRow = false;
                    if (rowValues.Contains(Ritem))
                    {
                        int idx = rowValues.IndexOf(Ritem);
                        R = AssetSummary.Rows[idx];
                        AssetSummary.Rows.RemoveAt(idx);
                        rowValues.Remove(Ritem);
                        rowValues.Add(Ritem);
                    }
                    else
                    {
                        rowValues.Add(Ritem);
                    }


                    String s;
                    bool missing = false;
                    if (!string.IsNullOrEmpty(Ritem))
                    {
                        if (R == null)
                            R = AssetSummary.NewRow();

                        setRowValue(assets1, summaryColumns, Ritem, R);

                        if (assets2.Count > 0)
                        {
                            if (!(assets2.ElementAt(0).Value.Contains(Ritem)))
                            {
                                setEmptyRowValue(assets2, summaryColumns, R);

                                missing = true;
                            }
                            else
                            {
                                setRowValue(assets2, summaryColumns, Ritem, R);

                            }
                        }
                        else
                        {
                            missing = true;
                        }

                        if (assets3.Count > 0)
                        {
                            if (!(assets3.ElementAt(0).Value.Contains(Ritem)))
                            {
                                setEmptyRowValue(assets3, summaryColumns, R);

                                missing = true;
                            }
                            else
                            {
                                setRowValue(assets3, summaryColumns, Ritem, R);

                            }
                        }
                        else
                        {
                            missing = true;
                        }

                        if (missing)
                        {
                            AssetSummary.Rows.Add(R);
                        }




                    }
                }
            }

        }

        private static void setRowValue(Dictionary<string, List<string>> assets1, List<String> summaryColumns, string Ritem, DataRow R)
        {
            if (summaryColumns.Count > 0)
            {
                int i = 0;
                foreach (String perColumn in summaryColumns)
                {
                    R[assets1.ElementAt(0).Key + perColumn] = Ritem.Split(',')[i];
                    i++;
                }
            }
        }
        private static void setEmptyRowValue(Dictionary<string, List<string>> assets1, List<String> summaryColumns, DataRow R)
        {
            if (summaryColumns.Count > 0)
            {
                foreach (String perColumn in summaryColumns)
                {
                    R[assets1.ElementAt(0).Key + perColumn] = "";
                }
            }
        }


        private static Dictionary<string, List<string>> CreateDataDic(DataTable inputTable, ref  Dictionary<string, string> assetsToNumberLineAddress)
        {
            Dictionary<string, List<string>> assets = new Dictionary<string, List<string>>();
            if (inputTable.TableName == SDS_STR)
                assetsToNumberLineAddress = new Dictionary<string, string>();
            bool ismultipleCols = (inputTable.Columns.Count > 2) ? true : false;

            foreach (DataRow Ritem in inputTable.Rows)
            {
                if (assets.Keys.Contains(inputTable.TableName))
                {
                    if (ismultipleCols && DataGenerator.selectedAssetType.TypeName != "Slot")
                    {
                        string data = string.Empty;
                        foreach (var citem in Ritem.ItemArray)
                        {
                            data = data + ConvertToIntString(citem.ToString()) + "-->";
                        }

                    }
                    else
                    {
                        assets[inputTable.TableName].Add(ConvertToIntString(Ritem[1].ToString()));
                        if (inputTable.TableName == SDS_STR && DataGenerator.selectedAssetType.TypeName == "Slot")
                        {
                            assetsToNumberLineAddress.Add(ConvertToIntString(Ritem[1].ToString()), ConvertToIntString(Ritem[2].ToString()));
                        }
                    }
                }
                else
                {
                    assets.Add(inputTable.TableName, new List<string>());
                    assets[inputTable.TableName].Add(ConvertToIntString(Ritem[1].ToString()));
                    if (inputTable.TableName == SDS_STR && DataGenerator.selectedAssetType.TypeName == "Slot")
                    {
                        assetsToNumberLineAddress.Add(ConvertToIntString(Ritem[1].ToString()), ConvertToIntString(Ritem[2].ToString()));
                    }
                }
            }
            return assets;

        }
        private static Dictionary<string, List<string>> CreateDataDic(DataTable inputTable, int columnId)
        {
            Dictionary<string, List<string>> assets = new Dictionary<string, List<string>>();


            foreach (DataRow item in inputTable.Rows)
            {
                if (assets.Keys.Contains(inputTable.TableName))
                {
                    assets[inputTable.TableName].Add(ConvertToIntString(item[columnId].ToString()));
                }
                else
                {
                    assets.Add(inputTable.TableName, new List<string>());
                    assets[inputTable.TableName].Add(ConvertToIntString(item[columnId].ToString()));
                }
            }
            return assets;

        }
        private static Dictionary<string, List<string>> CreateDataDicAZB(DataTable inputTable)
        {
            Dictionary<string, List<string>> assets = new Dictionary<string, List<string>>();


            foreach (DataRow item in inputTable.Rows)
            {
                if (assets.Keys.Contains(inputTable.TableName))
                {
                    assets[inputTable.TableName].Add(String.Join(",", item.ItemArray.ToArray().ToList()));
                }
                else
                {
                    assets.Add(inputTable.TableName, new List<string>());
                    assets[inputTable.TableName].Add(String.Join(",", item.ItemArray.ToArray().ToList()));
                }
            }
            return assets;

        }

        public static void CreateExcelFromDataTable(DataSet DataSet, string missingRowIdsCompare = "")
        {
            string templatePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Excels\";
            if (!Directory.Exists(templatePath))
                Directory.CreateDirectory(templatePath);

            try
            {

                using (DataSet ds = DataSet)
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        using (ExcelPackage xp = new ExcelPackage())
                        {
                            foreach (DataTable dt in ds.Tables)
                            {
                                if (DataGenerator.selectedAssetType.TypeName != "Slot" && (dt.TableName == "BCC" || dt.TableName == "EBS"))
                                    continue;

                                String sheetName = dt.TableName;
                                ExcelWorksheet ws = xp.Workbook.Worksheets.Add(sheetName);
                                int rowstart = 1;
                                int colstart = 1;
                                int rowend = rowstart;
                                int colend = colstart + dt.Columns.Count - 1;
                                if (colend == 0)
                                {
                                    continue;
                                }
                                ws.Cells[rowstart, colstart, rowend, colend].Style.Font.Bold = true;

                                rowend = rowstart + dt.Rows.Count;
                                ws.Cells[rowstart, colstart].LoadFromDataTable(dt, true);

                                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                                ws.Cells[rowstart, colstart, rowend, colend].Style.Border.Top.Style =
                                ws.Cells[rowstart, colstart, rowend, colend].Style.Border.Bottom.Style =
                                ws.Cells[rowstart, colstart, rowend, colend].Style.Border.Left.Style =
                                ws.Cells[rowstart, colstart, rowend, colend].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                ws.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                //Freeze top pane/row.
                                ws.View.FreezePanes(2, 1);
                                int rowid = 0;
                                if (dt.TableName == STR_ASSET_SUM || dt.TableName == STR_COMP_SUM)
                                {
                                    foreach (int id in missingRowIds)
                                    {
                                        rowid = id + 2;
                                        ws.Cells[rowid, 1, rowid, dt.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        ws.Cells[rowid, 1, rowid, dt.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                        ws.Cells[rowid, 1, rowid, dt.Columns.Count].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                    }
                                    xp.Workbook.Worksheets[dt.TableName].Select();
                                    WriteLogFile.WriteLog("Console", "Successfully Created the work book");
                                }
                                else if (dt.TableName == STR_AST_OPT_SUM)
                                {
                                    //Todo: Highlight SDS Sync Options
                                    String SyncOptions = ConfigurationManager.AppSettings.Get("SYNCOPTIONS");
                                    if (!string.IsNullOrEmpty(SyncOptions))
                                    {
                                        List<String> syncOptList = SyncOptions.Split(',').ToList();
                                        int CodeColid = ws.Cells["1:1"].First(c => c.Value.ToString() == "OPTION CODE").Start.Column;
                                        int ValueColid = ws.Cells["1:1"].First(c => c.Value.ToString() == "SDS OPTION VALUE").Start.Column;
                                        int rowCount = ws.Dimension.End.Row;

                                        foreach (ExcelRangeBase cell in ws.Cells[1, CodeColid, rowCount, CodeColid])
                                        {
                                            if (string.IsNullOrEmpty(cell.Text)) continue;
                                            var text = cell.Text;
                                            if (syncOptList.Contains(text))
                                            {
                                                Color colFromHex = System.Drawing.Color.Yellow;
                                                ws.Cells[cell.Start.Row, ValueColid, cell.Start.Row, ValueColid].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                                ws.Cells[cell.Start.Row, ValueColid, cell.Start.Row, ValueColid].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                            }
                                        }
                                    }

                                    xp.Workbook.Worksheets[dt.TableName].Select();
                                    WriteLogFile.WriteLog("Console", "Successfully Created the work book");
                                }
                            }

                            string fileName = string.Format("Data_{0}_{1}{2}{3}_{4}{5}{6}.xlsx", selectedAssetType.TypeName.Replace('/', '_'), System.DateTime.UtcNow.Year, System.DateTime.UtcNow.Month, System.DateTime.UtcNow.Day, System.DateTime.UtcNow.Hour, System.DateTime.UtcNow.Minute, System.DateTime.UtcNow.Second);

                            xp.SaveAs(new FileInfo(templatePath + fileName));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static string ConvertToIntString(string input)
        {
            Int64 result = 0;
            if (Int64.TryParse(input, out result))
            {
                return result.ToString();
            }
            else if (!selectedAssetType.isComp && (input.ElementAt(0) == 'A' || input.ElementAt(0) == 'Z' || input.ElementAt(0) == 'X' || input.ElementAt(0) == 'P'
                || input.ElementAt(0) == 'K' || input.ElementAt(0) == 'D' || input.ElementAt(0) == 'R' || input.ElementAt(0) == 'C' || input.ElementAt(0) == 'I'
                || input.ElementAt(0) == 'H' || input.ElementAt(0) == 'M') && (selectedAssetType.TypeName != "Slot" && selectedAssetType.TypeName != "Progressive Pool"))
            {
                return ConvertToIntString(input.Replace("A", "").Replace("Z", "").Replace("X", "").Replace("P", "").Replace("K", "").Replace("D", "").Replace("R", "").Replace("C", "").Replace("I", "").Replace("H", "").Replace("M", ""));
            }
            else
            {
                return input;
            }
        }
    }

    public class AssetType
    {
        public string TypeName
        {
            get;
            set;
        }
        public string AmTypeId
        {
            get;
            set;
        }
        public string PvTypeId
        {
            get;
            set;
        }
        public bool isComp
        {
            get;
            set;
        }
        public string EBSTypeId
        {
            get;
            set;
        }
        public string BCCTypeId
        {
            get;
            set;
        }
    }
}
