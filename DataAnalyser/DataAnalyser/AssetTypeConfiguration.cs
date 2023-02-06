using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataAnalyser
{

    [XmlRoot("AssetTypeConfiguration")]
    public class AssetTypeConfiguration
    {

        [XmlArray("AssetTypes")]
        [XmlArrayItem("PerAssetType")]
        public AssetTypes AssetTypes { get; set; }
    }

public class AssetTypes : List<PerAssetType> {  }


public class PerAssetType
{
    [XmlText]
    public string Value { get; set; }

    [XmlAttribute]
    public string AssetTypeName { get; set; }

    [XmlAttribute]
    public string AmTypeId { get; set; }

    [XmlAttribute]
    public string PvTypeId { get; set; }

}

}
