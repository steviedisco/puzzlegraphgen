using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Edge")]
    [Serializable]
    public class Edge : Section
    {
        private Edge()
        {
        }

        private Edge(int source, int target)
        {
            Name = "edge";

            AddGraphObject(Attribute.CreateAttribute("source", "int", source));
            AddGraphObject(Attribute.CreateAttribute("target", "int", target));
        }

        public static Edge CreateEdge(int source, int target)
        {
            return new Edge(source, target);
        }

        public Edge AddEdgeGraphics()
        {
            AddGraphObject(EdgeGraphics.CreateEdgeGraphics());

            return this;
        }
    }
}
