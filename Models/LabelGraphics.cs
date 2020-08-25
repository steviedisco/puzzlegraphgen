using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "LabelGraphics")]
    [Serializable]
    public class LabelGraphics : Section
    {
        private LabelGraphics() { }

        private LabelGraphics(string label)
        {
            Name = "LabelGraphics";

            AddGraphObject(Attribute.CreateAttribute("text", "String", label));
            AddGraphObject(Attribute.CreateAttribute("fontSize", "int", 12));
            AddGraphObject(Attribute.CreateAttribute("fontName", "String", "Calibri"));
            AddGraphObject(Attribute.CreateAttribute("model"));
        }

        public static LabelGraphics CreateLabelGraphics(string label)
        {
            return new LabelGraphics(label);
        }
    }
}
