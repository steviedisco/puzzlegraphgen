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
        private const int xStep = 200;
        private const int xStart = 0;

        private const int yStep = 150;
        private const int yStart = 0;

        private const int nodeWidth = 150;
        private const int nodeHeight = 50;

        private static Dictionary<int, List<int>> plottedPositions;
        private static List<int> plottedNodes;

        public static void InitialisePlot(this Graph graph)
        {
            plottedPositions = new Dictionary<int, List<int>>();
            plottedNodes = new List<int>();
        }

        public static int Plot(this Graph graph, PuzzleGoal goal, int x = xStart, int y = yStart)
        {
            if (!plottedPositions.ContainsKey(y))
            {
                plottedPositions.Add(y, new List<int>());
            }

            if (goal.PuzzleResult != null)
            {
                foreach (var nextPuzzle in goal.PuzzleResult.NextPuzzles)
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

                        x = Math.Max(x, graph.Plot(nextPuzzle, x, y + yStep));
                    }
                }
            }            

            if (!(goal is PuzzleStart))
            {
                goal.Position = (x, y);

                graph.AddNode(goal.Id, goal.Title)
                     .AddGraphics(goal.Position, nodeWidth, nodeHeight)
                     .AddLabelGraphics(goal.Title);                

                if (goal.PuzzleResult != null)
                {
                    var currentId = goal.Id;
                    var currentPosition = goal.Position;

                    if (!string.IsNullOrEmpty(goal.PuzzleResult.PrizeName))
                    {
                        currentId++;

                        currentPosition = (goal.Position.Item1, goal.Position.Item2 + (yStep / 2));

                        graph.AddNode(currentId, goal.PuzzleResult.PrizeName)
                             .AddGraphics(currentPosition, nodeWidth, nodeHeight / 2, true)
                             .AddLabelGraphics(goal.PuzzleResult.PrizeName);

                        graph.AddEdge(currentId - 1, currentId)
                             .AddEdgeGraphics()
                             .AddLine();
                    }

                    var points = new List<(double, double)>();
                    var index = 0;

                    foreach (var nextPuzzle in goal.PuzzleResult.NextPuzzles)
                    {
                        points.Add(nextPuzzle.Position);
                    }

                    foreach (var nextPuzzle in goal.PuzzleResult.NextPuzzles)
                    {
                        graph.AddEdge(currentId, nextPuzzle.Id)
                             .AddEdgeGraphics()
                             .AddLine()
                             .AddPoints(currentPosition, points, index);

                        index++;
                    }
                }                
            }

            return x;
        }        
    }
}