using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Node")]
    [Serializable]
    public class Node : Section
    {
        private Node()
        {
        }

        private Node(int id, string label)
        {
            Name = "node";

            AddGraphObject(Attribute.Create("id", "int", id));
            AddGraphObject(Attribute.Create("label", "String", label));
        }

        public static Node Create(int id, string label)
        {
            return new Node(id, label);
        }

        public Node AddGraphics((double x, double y) position, double w = 60, double h = 40, bool isLabel = false)
        {
            AddGraphObject(Graphics.Create(position.x, position.y, w, h, isLabel));

            return this;
        }

        public Node AddLabelGraphics(string label)
        {
            AddGraphObject(LabelGraphics.Create(label));

            return this;
        }
    }
}
