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

            AddGraphObject(Attribute.Create("source", "int", source));
            AddGraphObject(Attribute.Create("target", "int", target));
        }

        public static Edge Create(int source, int target)
        {
            return new Edge(source, target);
        }

        public EdgeGraphics AddEdgeGraphics()
        {
            return AddGraphObject(EdgeGraphics.Create()) as EdgeGraphics;
        }
    }
}
