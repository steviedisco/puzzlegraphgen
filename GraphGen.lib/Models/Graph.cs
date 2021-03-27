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
        [XmlIgnore]
        public PuzzleStart PuzzleStart { get; private set; }

        private Graph() {}

        private Graph(PuzzleStart puzzleStart)
        {
            Name = "graph";

            AddGraphObject(Attribute.Create("hierarchic", "int", 1));
            AddGraphObject(Attribute.Create("label", "String", ""));
            AddGraphObject(Attribute.Create("directed", "int", 1));

            PuzzleStart = puzzleStart;
        }

        public static Graph Create(PuzzleStart puzzleStart)
        {
            return new Graph(puzzleStart);
        }

        public Node AddNode(int id, string label)
        {
            return AddGraphObject(Node.Create(id, label)) as Node;
        }

        public Edge AddEdge(int source, int target)
        {
            return AddGraphObject(Edge.Create(source, target)) as Edge;
        }

        public void RemoveNode(int id)
        {
            var node = Nodes.Where(x => x is Node && (x as Node).Attributes.Where(y => y.Key == "id" && y.Value == id.ToString()).Any()).Select(x => x as Node).FirstOrDefault();
            if (node != null) RemoveGraphObject(node);
        }

        public void RemoveEdges(int id)
        {
            Nodes.Where(x => x is Edge && (x as Edge)
                 .Attributes.Where(y => y.Key == "source" && y.Value == id.ToString()).Any()).ToList()
                 .ForEach(x => {

                     var edge = Nodes.Where(x => x is Edge && (x as Edge)
                                     .Attributes.Where(y => y.Key == "target" && y.Value == id.ToString()).Any()).FirstOrDefault();

                     if (edge != null)
                     {
                         (edge as Edge).Attributes.Where(y => y.Key == "target").First().Value = (x as Edge).Attributes.Where(z => z.Key == "target").First().Value;
                         RemoveGraphObject(x);
                     }                     
                 });
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

        public static int Plot(this Graph graph, PuzzleGoal goal = null, int x = xStart, int y = yStart, PuzzleGoal start = null)
        {
            goal ??= graph.PuzzleStart; 

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

                        var keys = plottedPositions.Keys.Where(f => f >= y);

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
                graph.SortNodes();
                graph.SortOverlaps();

                do
                {
                    graph.ResetElbows();
                } while (!graph.CheckNodes());

                graph.HideNodes();
            }

            return x;
        }

        public static bool CheckNodes(this Graph graph, PuzzleGoal puzzle = null)
        {
            puzzle ??= graph.PuzzleStart;

            var output = true;

            if (puzzle.Result is null) return output;
            var puzzleNode = graph.GetAttributeNode(puzzle.Id, "y");

            // find all nodes on the same y axis value
            var nodes = graph.GetNodesOnY(int.Parse(puzzleNode.Value));

            // find any elbows on the same y axis value
            var elbows = graph.GetElbowsOnY(int.Parse(puzzleNode.Value));

            // see if elbow is to the left of any of them
            foreach (var elbow in elbows)
            {
                var elbowX = int.Parse(elbow.Attributes.Where(x => x.Key == "x").First().Value);

                foreach (var node in nodes)
                {
                    var nodeX = int.Parse(node.Sections.First().Attributes.Where(x => x.Key == "x").First().Value);

                    if (elbowX <= nodeX)
                    {
                        ShiftNode(node, yStep);

                        // TODO
                        //var prizeId = graph.GetAttributeNode(int.Parse(node.Attributes.Where(x => x.Key == "id").First().Value) + 1, "y");
                        //var prizeNode = graph.Sections.Where(x => x.Name == "node" && x.Attributes.Where(y => y.Key == "id" && y.Value == prizeId.Value).Any()).First() as Node;

                        //ShiftNode(prizeNode, yStep + (yStep / 2));
                    }
                }
            }

            // if not, bump and return false

            foreach (var next in puzzle.Result.NextPuzzles.Reverse<PuzzleGoal>())
            {
                output &= graph.CheckNodes(next);
            }

            return output;
        }

        private static void ShiftNode(Node node, double step)
        {
            var yAttribute = node.Sections.Where(x => x.Name == "graphics").First()
                                 .Attributes.Where(x => x.Key == "y").First();

            yAttribute.Value = (int.Parse(yAttribute.Value) + step).ToString();
        }

        private static List<Node> GetNodesOnY(this Graph graph, int yAxis)
        {
            var sections = graph.Sections.Where(x => x.Name == "node" && (x.Sections.Where(y => y.Name == "graphics").First().Attributes.Where(y => y.Key == "y" && y.Value == yAxis.ToString()).Any())).ToList() as List<Section>;
            return sections.Select(x => x as Node).ToList();
        }

        private static List<Point> GetElbowsOnY(this Graph graph, int yAxis)
        {
            var output = new List<Point>();

            graph.Sections.Where(x => x.Name == "edge").ToList()
                .ForEach(section => {
                    var elbow = section.Sections.Where(y => y.Name == "graphics").First()
                        .Sections.First()
                        .Sections.Where(y => y.Name == "point" &&
                            y.Attributes.Where(z => z.Key == "nextId").Any() &&
                            y.Attributes.Where(z => z.Key == "y" && z.Value == yAxis.ToString()).Any()).FirstOrDefault() as Point;

                    if (elbow != null) output.Add(elbow);
                });                

            return output;
        }

        public static void HideNodes(this Graph graph, PuzzleGoal puzzle = null)
        {
            puzzle ??= graph.PuzzleStart;

            if (puzzle.Result is null) return;

            if (puzzle.Hidden)
            {
                graph.RemoveEdges(puzzle.Id);
                graph.RemoveNode(puzzle.Id);
            }

            foreach (var next in puzzle.Result.NextPuzzles)
            {                
                graph.HideNodes(next);
            }
        }

        public static void SortNodes(this Graph graph, PuzzleGoal puzzle = null, double amount = 0)
        {
            puzzle ??= graph.PuzzleStart;

            if (puzzle.Result is null) return;
            var puzzleNode = graph.GetAttributeNode(puzzle.Id, "y");

            foreach (var next in puzzle.Result.NextPuzzles.Reverse<PuzzleGoal>())
            {
                var nextNode = graph.GetAttributeNode(next.Id, "y");
                amount = BumpNode(puzzleNode, nextNode, 0, yStep);

                var prizeNode = graph.GetAttributeNode(next.Id + 1, "y");
                amount = BumpNode(puzzleNode, prizeNode, 0, yStep + (yStep / 2));                

                graph.SortNodes(next, amount);
            }
        }        

        private static double BumpNode(Attribute puzzleNode, Attribute nextNode, double amount, double step)
        {
            if (puzzleNode is null || nextNode is null) return amount;

            if (int.Parse(puzzleNode.Value) > int.Parse(nextNode.Value))
            {
                amount = (int.Parse(puzzleNode.Value) - int.Parse(nextNode.Value)) + step;
            }

            if (amount > 0)
            {
                nextNode.Value = (int.Parse(nextNode.Value) + amount).ToString();
            }

            return amount;
        }

        public static void ResetElbows(this Graph graph, PuzzleGoal puzzle = null)
        {
            puzzle ??= graph.PuzzleStart;

            if (puzzle.Result is null || puzzle.Sorted) return;

            foreach (var next in puzzle.Result.NextPuzzles)
            {
                puzzle.Sorted = true;

                graph.BumpPointNode(puzzle.Id, puzzle.Id + 1);
                graph.BumpPointNode(puzzle.Id + 1, next.Id);
                graph.BumpPointNode(puzzle.Id, next.Id);                

                graph.ResetElbows(next);
            }
        }

        private static void SortOverlaps(this Graph graph, PuzzleGoal puzzle = null, Dictionary<int, List<(int, int)>> allocated = null, PuzzleGoal start = null)
        {
            puzzle ??= graph.PuzzleStart;
            start ??= puzzle;
            allocated ??= new Dictionary<int, List<(int, int)>>();

            if (puzzle.Result is null) return;

            foreach (var next in puzzle.Result.NextPuzzles.Reverse<PuzzleGoal>())
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

                            if (points.Count < 3) continue;

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

                                    var amount = (int)BumpNode(puzzleNode, nextNode, 0, yStep);

                                    var prizeNode = graph.GetAttributeNode(next.Id + 1, "y");
                                    amount = (int)BumpNode(puzzleNode, prizeNode, 0, yStep + (yStep / 2));

                                    if (allocated.ContainsKey(y1 + amount))
                                    {
                                        allocated[y1 + amount].Add((x1, x2));
                                    }
                                    else
                                    {
                                        allocated.Add(y1 + amount, new List<(int, int)>() { (x1, x2) });
                                    }
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

        private static void BumpPointNode(this Graph graph, int sourceId, int targetId)
        {
            if (graph.Sections.Where(x => x.Attributes.Where(y => y.Key == "id" && y.Value == sourceId.ToString()).Any()).FirstOrDefault() is not Node startNode) return;

            var source = graph.Sections.Where(x => x.Attributes.Where(y => y.Key == "source" && y.Value == sourceId.ToString()).Any()).ToList();

            if (source.Where(x => x.Attributes.Where(y => y.Key == "target" && y.Value == targetId.ToString()).Any()).FirstOrDefault() is not Edge edge) return;

            if (edge.Sections.Where(x => x is EdgeGraphics).FirstOrDefault() is not EdgeGraphics graphic) return;

            if (graphic.Sections.Where(x => x is Line).FirstOrDefault() is not Line line) return;

            if (graph.GetAttributeNode(targetId, "x") is not Attribute nextNodeX) return;
            if (graph.GetAttributeNode(targetId, "y") is not Attribute nextNodeY) return;

            var points = line.Sections.Where(x => x is Point).Select(x => x as Point).ToList();
            if (!points.Any()) return;

            if (graph.GetAttributeNode(sourceId, "x") is not Attribute startPointX) return;
            if (graph.GetAttributeNode(sourceId, "y") is not Attribute startPointY) return;

            var midPoint = points[1];
            var midPointX = midPoint.Attributes.Where(x => x.Key == "x").First();
            var midPointY = midPoint.Attributes.Where(x => x.Key == "y").First();

            if (int.Parse(startPointX.Value) == int.Parse(midPointX.Value))
            {
                if (int.Parse(midPointX.Value) == int.Parse(nextNodeX.Value))
                {
                    midPointY.Value = (int.Parse(startPointY.Value) + ((int.Parse(nextNodeY.Value) - int.Parse(startPointY.Value)) / 2)).ToString();
                }
                else if (int.Parse(nextNodeX.Value) > int.Parse(midPointX.Value))
                {
                    midPointY.Value = nextNodeY.Value;
                }
            }
            else if (int.Parse(midPointX.Value) > int.Parse(startPointX.Value))
            {
                midPointX.Value = nextNodeX.Value;
                midPointY.Value = startPointY.Value;
            }
        }
    }
}