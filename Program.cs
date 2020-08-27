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
            var graph = Create3PathGraph(); 
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

        private static GraphContainer CreateSimpleGraph()
        {
            var container = GraphContainer.CreateGraphContainer();
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

        private static GraphContainer Create2PathGraph()
        {
            var container = GraphContainer.CreateGraphContainer();
            var graph = container.AddGraph();

            const int max_nodes = 3;
            const int offsetx = 100;
            const int stepy = 80;
            const int chara = 65;
            var starty = -220;

            graph.AddNode(0, "Start").AddGraphics(0, starty).AddLabelGraphics("Start");
            graph.AddNode(1, "Puzzle 1").AddGraphics(0, starty + stepy).AddLabelGraphics("Puzzle 1");

            graph.AddEdge(0, 1).AddEdgeGraphics();

            var label = string.Empty;
            var id = 2;
            var previd = 1;
            var joinids = new int[2];

            for (int i = -1; i <= 1; i+=2)
            {
                previd = 1;

                for (int j = 2; j < 2 + max_nodes; j++)
                {
                    var prefix = (char)(chara + ((i + 1) / 2));
                    var y = starty + (stepy * j);
                    label = $"Puzzle {prefix}{j}";

                    graph.AddNode(id, label).AddGraphics(i * offsetx, y).AddLabelGraphics(label);
                    graph.AddEdge(previd, id).AddEdgeGraphics();
                    previd = id;
                    id++;
                }

                joinids[(i + 1) / 2] = previd;
            }

            label = $"Puzzle {max_nodes + 2}";
            graph.AddNode(id, label).AddGraphics(0, starty + (stepy * (max_nodes + 2))).AddLabelGraphics(label);
            graph.AddEdge(joinids[0], id).AddEdgeGraphics();
            graph.AddEdge(joinids[1], id).AddEdgeGraphics();
            previd = id++;

            graph.AddNode(id, "Finish").AddGraphics(0, starty + (stepy * (max_nodes + 3))).AddLabelGraphics("Finish");
            graph.AddEdge(previd, id).AddEdgeGraphics();

            return container;
        }

        private static GraphContainer Create3PathGraph()
        {
            var container = GraphContainer.CreateGraphContainer();
            var graph = container.AddGraph();

            const int max_nodes = 3;
            const int offsetx = 150;
            const int stepy = 80;
            const int chara = 65;
            var starty = -220;

            graph.AddNode(0, "Start").AddGraphics(0, starty).AddLabelGraphics("Start");
            graph.AddNode(1, "Puzzle 1").AddGraphics(0, starty + stepy).AddLabelGraphics("Puzzle 1");

            graph.AddEdge(0, 1).AddEdgeGraphics();

            var label = string.Empty;
            var id = 2;
            var previd = 1;
            var joinids = new int[3];

            for (int i = -1; i <= 1; i++)
            {
                previd = 1;

                for (int j = 2; j < 2 + max_nodes; j++)
                {
                    var prefix = (char)(chara + (i + 1));
                    var y = starty + (stepy * j);
                    label = $"Puzzle {prefix}{j}";

                    graph.AddNode(id, label).AddGraphics(i * offsetx, y).AddLabelGraphics(label);
                    graph.AddEdge(previd, id).AddEdgeGraphics();
                    previd = id;
                    id++;
                }

                joinids[(i + 1)] = previd;
            }

            label = $"Puzzle {max_nodes + 2}";
            graph.AddNode(id, label).AddGraphics(0, starty + (stepy * (max_nodes + 2))).AddLabelGraphics(label);
            graph.AddEdge(joinids[0], id).AddEdgeGraphics();
            graph.AddEdge(joinids[1], id).AddEdgeGraphics();
            graph.AddEdge(joinids[2], id).AddEdgeGraphics();
            previd = id++;

            graph.AddNode(id, "Finish").AddGraphics(0, starty + (stepy * (max_nodes + 3))).AddLabelGraphics("Finish");
            graph.AddEdge(previd, id).AddEdgeGraphics();

            return container;
        }
    }
}
