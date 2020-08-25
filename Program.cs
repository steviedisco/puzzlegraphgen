using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PuzzleGraphGenerator.Helpers;
using PuzzleGraphGenerator.Models;

namespace PuzzleGraphGenerator
{
    class Program
    {
        static void Main()
        {
            var graph = CreateGraph(); 
            var serializer = new XmlSerializer(typeof(GraphContainer));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var writer = new NoTypeXmlWriter("./output.xgml", CodePagesEncodingProvider.Instance.GetEncoding(1252))
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };

            serializer.Serialize(writer, graph);
            writer.Close();

            Console.WriteLine("Generated Successfully");
        }

        private static GraphContainer CreateGraph()
        {
            var container = new GraphContainer();
            var graph = container.AddGraph();

            const int x = 100;
            const int max_nodes = 5;
            var starty = -100;
            var stepy = 80;

            for (int i = 0; i < max_nodes; i++)
            {
                var label = (i == max_nodes - 1) ? "Finish" : (i == 0) ? "Start" : $"Node {i}";
                var y = starty + (stepy * i);
                graph.AddNode(i, label).AddGraphics(x, y).AddLabelGraphics(label);
            }

            for (int i = 0; i < max_nodes - 1; i++)
            {
                graph.AddEdge(i, i + 1).AddEdgeGraphics();
            }

            return container;
        }
    }
}
