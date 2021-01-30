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

            AddGraphObject(Attribute.Create("text", "String", label));
            AddGraphObject(Attribute.Create("fontSize", "int", 12));
            AddGraphObject(Attribute.Create("fontName", "String", "Calibri"));
            AddGraphObject(Attribute.Create("model"));
        }

        public static LabelGraphics Create(string label)
        {
            return new LabelGraphics(label);
        }
    }
}
