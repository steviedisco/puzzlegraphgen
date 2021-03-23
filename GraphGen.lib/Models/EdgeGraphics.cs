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

            AddGraphObject(Attribute.Create("fill", "String", "#000000"));
            AddGraphObject(Attribute.Create("targetArrow", "String", "standard"));
        }

        public static EdgeGraphics Create()
        {
            return new EdgeGraphics();
        }

        public Line AddLine()
        {
            return AddGraphObject(Line.Create()) as Line;
        }
    }
}
