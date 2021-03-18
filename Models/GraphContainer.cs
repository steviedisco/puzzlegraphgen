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

            AddGraphObject(Attribute.Create("Creator", "String", "yFiles"));
            AddGraphObject(Attribute.Create("Version", "String", 2.17));
        }

        public static GraphContainer Create()
        {
            return new GraphContainer();
        }

        public Graph CreateGraph(PuzzleStart puzzleStart)
        {
            return AddGraphObject(Graph.Create(puzzleStart)) as Graph;
        }
    }
}
