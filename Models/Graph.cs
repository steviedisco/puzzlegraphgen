using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Graph")]
    [Serializable]
    public class Graph : Section
    {
        private Graph()
        {
            Name = "graph";

            AddGraphObject(Attribute.CreateAttribute("hierarchic", "int", 1));
            AddGraphObject(Attribute.CreateAttribute("label", "String", ""));
            AddGraphObject(Attribute.CreateAttribute("directed", "int", 1));
        }

        public static Graph CreateGraph()
        {
            return new Graph();
        }

        public Node AddNode(int id, string label)
        {
            return AddGraphObject(Node.CreateNode(id, label)) as Node;
        }

        public Edge AddEdge(int source, int target)
        {
            return AddGraphObject(Edge.CreateEdge(source, target)) as Edge;
        }
    }
}
