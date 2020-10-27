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

            AddGraphObject(Attribute.CreateAttribute("x", "double", x));
            AddGraphObject(Attribute.CreateAttribute("y", "double", y));
            AddGraphObject(Attribute.CreateAttribute("w", "double", w));
            AddGraphObject(Attribute.CreateAttribute("h", "double", h));
            AddGraphObject(Attribute.CreateAttribute("type", "String", "roundrectangle"));
            AddGraphObject(Attribute.CreateAttribute("raisedBorder", "boolean", false));
            AddGraphObject(Attribute.CreateAttribute("fill", "String", "#FFCC00"));
            AddGraphObject(Attribute.CreateAttribute("outline", "String", "#000000"));
        }

        public static Graphics CreateGraphics(double x, double y, double w, double h)
        {
            return new Graphics(x, y, w, h);
        }
    }
}
