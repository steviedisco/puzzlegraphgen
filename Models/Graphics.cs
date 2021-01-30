using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Graphics")]
    [Serializable]
    public class Graphics : Section
    {
        private Graphics() { }

        private Graphics(double x, double y, double w, double h)
        {
            Name = "graphics";

            AddGraphObject(Attribute.Create("x", "double", x));
            AddGraphObject(Attribute.Create("y", "double", y));
            AddGraphObject(Attribute.Create("w", "double", w));
            AddGraphObject(Attribute.Create("h", "double", h));
            AddGraphObject(Attribute.Create("type", "String", "roundrectangle"));
            AddGraphObject(Attribute.Create("raisedBorder", "boolean", false));
            AddGraphObject(Attribute.Create("fill", "String", "#FFCC00"));
            AddGraphObject(Attribute.Create("outline", "String", "#000000"));
        }

        public static Graphics Create(double x, double y, double w, double h)
        {
            return new Graphics(x, y, w, h);
        }
    }
}
