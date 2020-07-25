using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "attribute")]
    [Serializable]
    public class Attribute
    {
        [XmlAttribute("key")]
        public string Key { get; set; } = string.Empty;

        [XmlAttribute("type")]
        public string Type { get; set; } = string.Empty;

        [XmlText]
        public string Value { get; set; } = string.Empty;

        public static Attribute CreateAttribute(
            string key, string type = null, object value = null) 
        {
            return new Attribute
            {
                Key = key,
                Type = type,
                Value = value != null ? value.ToString() : null
            };
        }
    }
}
