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
    [XmlRoot("QueryConfiguration")]
    public class QueryConfiguration : ISerializable
    {
        [XmlArray("Queries")]
        [XmlArrayItem("Query")]
        public Queries Queries
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
    public class Query
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }
        [XmlText]
        public string Value { get; set; }
    }

    public class Queries : List<Query> { } 

    
}
