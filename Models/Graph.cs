using System;
using System.Collections.Generic;
using System.Linq;
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

    public static class GraphExtensions
    {
        private const int xStep = 170;
        private const int xStart = -400;

        private const int yStep = 120;
        private const int yStart = -400;

        private const int nodeWidth = 150;
        private const int nodeHeight = 50;

        private static Dictionary<int, List<int>> plottedNodes = new Dictionary<int, List<int>>();

        public static void PlotGraph(this Graph graph, PuzzleGoal goal, int x = xStart, int y = yStart)
        {
            var currentX = x;

            if (!plottedNodes.ContainsKey(y))
            {
                plottedNodes.Add(y, new List<int>());
            }

            foreach (var nextResult in goal.PuzzleResults)
            {                
                if (nextResult.NextPuzzle.Id == 0)
                {
                    nextResult.NextPuzzle.Id = PuzzleGoal.GetNextId();

                    var keys = plottedNodes.Keys.Where(x => x >= y);

                    foreach (var key in keys)
                    {
                        while (plottedNodes[key].Contains(x))
                        {
                            x += xStep;
                        }
                    }

                    plottedNodes[y].Add(x);

                    graph.PlotGraph(nextResult.NextPuzzle, x, y + yStep);
                }
            }

            if (!(goal is PuzzleStart))
            {
                graph.AddNode(goal.Id, goal.Title).AddGraphics(currentX, y, nodeWidth, nodeHeight).AddLabelGraphics(goal.Title);

                foreach (var nextResult in goal.PuzzleResults)
                {
                    graph.AddEdge(goal.Id, nextResult.NextPuzzle.Id).AddEdgeGraphics();
                }
        }
    }
}
