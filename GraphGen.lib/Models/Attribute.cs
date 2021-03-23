using System;
using System.Xml.Serialization;
using PuzzleGraphGenerator.Interfaces;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "attribute", Namespace = "Attribute")]
    [Serializable]
    public class Attribute : IGraphObject
    {
        [XmlAttribute("key")]
        public string Key { get; set; } = string.Empty;

        [XmlAttribute("type")]
        public string Type { get; set; } = string.Empty;

        [XmlText]
        public string Value { get; set; } = string.Empty;

        public static Attribute Create(
            string key, string type = null, object value = null) 
        {
            return new Attribute
            {
                Key = key,
                Type = type,
                Value = value?.ToString()
            };
        }
    }
}
