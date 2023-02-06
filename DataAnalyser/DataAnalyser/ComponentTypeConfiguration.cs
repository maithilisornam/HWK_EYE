using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataAnalyser
{

    [XmlRoot("ComponentTypeConfiguration")]
    public class ComponentTypeConfiguration
    {

        [XmlArray("ComponentTypes")]
        [XmlArrayItem("PerComponentType")]
        public ComponentTypes ComponentTypes { get; set; }
    }

    public class ComponentTypes : List<PerComponentType> { }


    public class PerComponentType
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
