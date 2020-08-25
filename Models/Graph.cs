using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Graph")]
    [Serializable]
    public class Graph : Section
    {
        public Graph()
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
    }
}
