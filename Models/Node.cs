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

            AddGraphObject(Attribute.CreateAttribute("id", "int", id));
            AddGraphObject(Attribute.CreateAttribute("label", "String", label));
        }

        public static Node CreateNode(int id, string label)
        {
            return new Node(id, label);
        }

        public Node AddGraphics(double x, double y, double w = 60, double h = 40)
        {
            AddGraphObject(Graphics.CreateGraphics(x, y, w, h));

            return this;
        }

        public Node AddLabelGraphics(string label)
        {
            AddGraphObject(LabelGraphics.CreateLabelGraphics(label));

            return this;
        }
    }
}
