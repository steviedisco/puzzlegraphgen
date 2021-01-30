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

            AddGraphObject(Attribute.Create("hierarchic", "int", 1));
            AddGraphObject(Attribute.Create("label", "String", ""));
            AddGraphObject(Attribute.Create("directed", "int", 1));
        }

        public static Graph Create()
        {
            return new Graph();
        }

        public Node AddNode(int id, string label)
        {
            return AddGraphObject(Node.Create(id, label)) as Node;
        }

        public Edge AddEdge(int source, int target)
        {
            return AddGraphObject(Edge.Create(source, target)) as Edge;
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

        private static Dictionary<int, List<int>> plottedPositions;
        private static List<int> plottedNodes;

        public static void InitialisePlot(this Graph graph)
        {
            plottedPositions = new Dictionary<int, List<int>>();
            plottedNodes = new List<int>();
        }

        public static void Plot(this Graph graph, PuzzleGoal goal, int x = xStart, int y = yStart)
        {
            if (!plottedPositions.ContainsKey(y))
            {
                plottedPositions.Add(y, new List<int>());
            }

            foreach (var nextResult in goal.PuzzleResults)
            {
                foreach (var nextPuzzle in nextResult.NextPuzzles)
                {
                    if (!plottedNodes.Contains(nextPuzzle.Id))
                    {
                        plottedNodes.Add(nextPuzzle.Id);

                        var keys = plottedPositions.Keys.Where(x => x >= y);

                        foreach (var key in keys)
                        {
                            while (plottedPositions[key].Contains(x))
                            {
                                x += xStep;
                            }
                        }

                        plottedPositions[y].Add(x);

                        graph.Plot(nextPuzzle, x, y + yStep);
                    }
                }
            }

            if (!(goal is PuzzleStart))
            {
                goal.Position = (x, y);

                graph.AddNode(goal.Id, goal.Title)
                     .AddGraphics(goal.Position, nodeWidth, nodeHeight)
                     .AddLabelGraphics(goal.Title);

                foreach (var nextResult in goal.PuzzleResults)
                {
                    foreach (var nextPuzzle in nextResult.NextPuzzles)
                    {
                        graph.AddEdge(goal.Id, nextPuzzle.Id)
                         .AddEdgeGraphics()
                         .AddLine()
                         .AddPoints(goal.Position, nextPuzzle.Position);
                    }
                }
            }
        }        
    }
}