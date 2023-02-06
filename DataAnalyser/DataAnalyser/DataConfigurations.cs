using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataAnalyser
{
    [Serializable]
    [XmlRoot("DataConfigurations")]
    public class DataConfigurations : ISerializable
    {
        [XmlElement("AssetMatrixConfig")]
        public AssetMatrixConfig AssetMatrixConfig
        {
            get;
            set;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class AssetMatrixConfig
    {
        [XmlElement("DbDetails")]
        public DbDetails DbDetails
        {
            get;
            set;
        }
         [XmlElement("EBSDbDetails")]
         public DbDetails EBSDbDetails
        {
            get;
            set;
        }
      
        [XmlElement("BCCDbDetails")]
        public DbDetails BCCDbDetails
        {
            get;
            set;
        }
            
        [XmlElement("SDSConfigs")]
        public List<SDSConfig> SDSConfigs
        {
            get;
            set;
        }
    }
        
    [Serializable]
    public class SDSConfig
    {
        [XmlAttribute("State")]
        public string State
        {
            get;
            set;
        }

        [XmlElement("DbDetails")]
        public DbDetails DbDetails
        {
            get;
            set;
        }

        [XmlElement("PowerVenueConfigs")]
        public List<PowerVenueConfig> PowerVenueConfigs
        {
            get;
            set;
        }
    }

    [Serializable]
    public class PowerVenueConfig
    {
        [XmlAttribute("Site")]
        public string Site
        {
            get;
            set;
        }

        [XmlElement("DbDetails")]
        public DbDetails DbDetails
        {
            get;
            set;
        }
    }

  

    [Serializable]
    public class DbDetails
    {
        [XmlAttribute("ServerName")]
        public string ServerName
        {
            get;
            set;
        }

        [XmlAttribute("DataBase")]
        public string DataBase
        {
            get;
            set;
        }

        [XmlAttribute("UserId")]
        public string UserId
        {
            get;
            set;
        }

        [XmlAttribute("Password")]
        public string Password
        {
            get;
            set;
        }
    }

}
