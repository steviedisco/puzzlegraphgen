using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlRoot("section")]
    [Serializable]
    public class Graph : Section
    {
        public Graph()
        {
            Name = "xgml";

            AddAttribute(Attribute.CreateAttribute("Creator", "String", "yFiles"));
            AddAttribute(Attribute.CreateAttribute("Version", "String", 2.17));

            var section = AddSection(CreateSection("graph"));
            section.AddAttribute(Attribute.CreateAttribute("hierarchic", "int", 1));
            section.AddAttribute(Attribute.CreateAttribute("label", "String", ""));
            section.AddAttribute(Attribute.CreateAttribute("directed", "int", 1));
        }
    }
}
