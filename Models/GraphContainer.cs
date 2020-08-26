using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "GraphContainer")]
    [Serializable]
    public class GraphContainer : Section
    {
        private GraphContainer()
        {
            Name = "xgml";

            AddGraphObject(Attribute.CreateAttribute("Creator", "String", "yFiles"));
            AddGraphObject(Attribute.CreateAttribute("Version", "String", 2.17));
        }

        public static GraphContainer CreateGraphContainer()
        {
            return new GraphContainer();
        }

        public Graph AddGraph()
        {
            return AddGraphObject(Graph.CreateGraph()) as Graph;
        }
    }
}
