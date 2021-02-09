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

        public static void Initialise(this Graph graph)
        {
            plottedPositions = new Dictionary<int, List<int>>();
            plottedNodes = new List<int>();
        }

        public static int Plot(this Graph graph, PuzzleGoal goal, int x = xStart, int y = yStart, PuzzleGoal start = null)
        {
            if (start is null) graph.Initialise(); 

            start ??= goal;

            if (!plottedPositions.ContainsKey(y))
            {
                plottedPositions.Add(y, new List<int>());
            }

            if (goal.Result != null)
            {
                foreach (var nextPuzzle in goal.Result.NextPuzzles.Reverse<PuzzleGoal>())
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

                        x = Math.Max(x, graph.Plot(nextPuzzle, x, y + yStep, start));
                    }
                }
            }

            goal.Position = (xStart - x, y);

            graph.AddNode(goal.Id, goal.Title)
                 .AddGraphics(goal.Position, nodeWidth, nodeHeight)
                 .AddLabelGraphics(goal.Title);

            if (goal.Result != null)
            {
                var currentId = goal.Id;
                var currentPosition = goal.Position;

                if (!string.IsNullOrEmpty(goal.Result.PrizeName))
                {
                    currentId++;

                    currentPosition = (goal.Position.Item1, goal.Position.Item2 + (yStep / 2));

                    graph.AddNode(currentId, goal.Result.PrizeName)
                         .AddGraphics(currentPosition, nodeWidth, nodeHeight / 2, true)
                         .AddLabelGraphics(goal.Result.PrizeName);

                    graph.AddEdge(currentId - 1, currentId)
                         .AddEdgeGraphics()
                         .AddLine();
                }

                var points = new List<(double, double)>();
                var index = 0;

                foreach (var nextPuzzle in goal.Result.NextPuzzles.Reverse<PuzzleGoal>())
                {
                    points.Add(nextPuzzle.Position);
                }

                foreach (var nextPuzzle in goal.Result.NextPuzzles.Reverse<PuzzleGoal>())
                {
                    graph.AddEdge(currentId, nextPuzzle.Id)
                         .AddEdgeGraphics()
                         .AddLine()
                         .AddPoints(currentPosition, points, index, nextPuzzle.Id);

                    index++;
                }
            }

            if (goal == start)
            {
                graph.SortNodes(start);
                graph.BumpPoints(start);
                graph.SortOverlaps(start);
            }

            return x;
        }

        public static void SortNodes(this Graph graph, PuzzleGoal puzzle, double amount = 0)
        {
            if (puzzle.Result is null) return;
            var puzzleNode = graph.GetAttributeNode(puzzle.Id, "y");

            foreach (var next in puzzle.Result.NextPuzzles)
            {
                var nextNode = graph.GetAttributeNode(next.Id, "y");
                amount = BumpNode(puzzleNode, nextNode, amount, yStep, next.Id);

                var prizeNode = graph.GetAttributeNode(next.Id + 1, "y");
                amount = BumpNode(puzzleNode, prizeNode, amount, yStep + (yStep / 2), next.Id + 1);                

                graph.SortNodes(next, amount);
            }
        }

        public static void BumpPoints(this Graph graph, PuzzleGoal puzzle)
        {
            if (puzzle.Result is null) return;

            foreach (var next in puzzle.Result.NextPuzzles)
            {
                var nextNode = graph.GetAttributeNode(next.Id, "y");

                graph.BumpPointNode(puzzle.Id + 1, next.Id, nextNode);
                graph.BumpPoints(next);
            }
        }

        private static double BumpNode(Attribute puzzleNode, Attribute nextNode, double amount, double step, int id)
        {
            if (puzzleNode is null || nextNode is null) return amount;

            if (int.Parse(puzzleNode.Value) >= int.Parse(nextNode.Value))
            {
                amount = (int.Parse(puzzleNode.Value) - int.Parse(nextNode.Value)) + step;
            }

            if (amount > 0)
            {
                nextNode.Value = (int.Parse(nextNode.Value) + amount).ToString();
            }

            return amount;
        }

        private static void SortOverlaps(this Graph graph, PuzzleGoal puzzle, Dictionary<int, List<(int, int)>> allocated = null, PuzzleGoal start = null)
        {
            start ??= puzzle;
            allocated ??= new Dictionary<int, List<(int, int)>>();

            if (puzzle.Result is null) return;

            foreach (var next in puzzle.Result.NextPuzzles)
            {
                foreach (var edge in graph.Sections.Where(x => x is Edge && 
                    x.Attributes.Where(y => y.Key == "source" && y.Value == (puzzle.Id + 1).ToString()).Any() &&
                    x.Attributes.Where(y => y.Key == "target" && y.Value == next.Id.ToString()).Any()).Select(x => x as Edge))
                {
                    foreach (var graphic in edge.Sections.Where(x => x is EdgeGraphics).Select(x => x as EdgeGraphics))
                    {
                        foreach (var line in graphic.Sections.Where(x => x is Line).Select(x => x as Line))
                        {
                            var points = line.Sections.Where(x => x is Point).Select(x => x as Point).ToList();

                            if (points.Count() < 3) continue;

                            var x1 = int.Parse(points[1].Attributes.Where(x => x.Key == "x").First().Value);
                            var y1 = int.Parse(points[1].Attributes.Where(x => x.Key == "y").First().Value);
                            var x2 = int.Parse(points[2].Attributes.Where(x => x.Key == "x").First().Value);
                            var y2 = int.Parse(points[2].Attributes.Where(x => x.Key == "y").First().Value);

                            if (allocated.ContainsKey(y1))
                            {
                                if (x1 != x2 && y1 == y2 && allocated[y2].Any(x => x.Item2 < x2))
                                {
                                    var puzzleNode = graph.GetAttributeNode(puzzle.Id, "y");
                                    var nextNode = graph.GetAttributeNode(next.Id, "y");

                                    var amount = (int)BumpNode(puzzleNode, nextNode, yStep, yStep, next.Id);

                                    var prizeNode = graph.GetAttributeNode(next.Id + 1, "y");
                                    amount = (int)BumpNode(puzzleNode, prizeNode, amount, yStep + (yStep / 2), next.Id + 1);

                                    if (allocated.ContainsKey(y1 + amount))
                                    {
                                        allocated[y1 + amount].Add((x1, x2));
                                    }
                                    else
                                    {
                                        allocated.Add(y1 + amount, new List<(int, int)>() { (x1, x2) });
                                    }

                                    graph.SortNodes(next, amount);
                                }
                                else
                                {
                                    allocated[y1].Add((x1, x2));
                                }
                            }
                            else
                            {
                                allocated.Add(y1, new List<(int, int)>() { (x1, x2) });
                            }

                            graph.BumpPoints(start);
                        }
                    }
                }

                graph.SortOverlaps(next, allocated);
            }
        }

        private static Attribute GetAttributeNode(this Graph graph, int id, string key)
        {
            var section = graph.Sections.Where(x => x.Attributes.Where(y => y.Key == "id" && y.Value == id.ToString()).Any()).FirstOrDefault() as Node;
            if (section is null) return null;

            var graphic = section.Sections.Where(x => x is Graphics).FirstOrDefault() as Graphics;
            if (graphic is null) return null;

            return graphic.Attributes.Where(x => x.Key == key).FirstOrDefault();
        }

        private static void BumpPointNode(this Graph graph, int sourceId, int targetId, Attribute nextNode)
        {
            var edge = graph.Sections.Where(x => x.Attributes.Where(y => y.Key == "source" && y.Value == sourceId.ToString()).Any()).FirstOrDefault() as Edge;
            if (edge is null) return;

            var graphic = edge.Sections.Where(x => x is EdgeGraphics).FirstOrDefault() as EdgeGraphics;
            if (graphic is null) return;

            var line = graphic.Sections.Where(x => x is Line).FirstOrDefault() as Line;
            if (line is null) return;

            var points = line.Sections.Where(x => x is Point).Select(x => x as Point).ToList();
            if (!points.Any()) return;

            var point = points.Where(x => x.Attributes.Any(y => y.Key == "nextId" && y.Value == targetId.ToString())).FirstOrDefault();

            if (point != null)
            {
                point.Attributes.Where(x => x.Key == "y").First().Value = nextNode.Value;
            }
        }
    }
}