using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "EdgeGraphics")]
    [Serializable]
    public class EdgeGraphics : Section
    {
        private EdgeGraphics()
        {
            Name = "graphics";

            AddGraphObject(Attribute.CreateAttribute("fill", "String", "#000000"));
            AddGraphObject(Attribute.CreateAttribute("targetArrow", "String", "standard"));
        }

        public static EdgeGraphics CreateEdgeGraphics()
        {
            return new EdgeGraphics();
        }
    }
}
