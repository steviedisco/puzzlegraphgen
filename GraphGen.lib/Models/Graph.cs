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

        private static Dictionary<int, Dictionary<int, int>> _plottedPositions;
        private static Dictionary<int, PuzzleGoal> _plottedGoals;

        public static void Initialise(this Graph graph)
        {
            _plottedPositions = new Dictionary<int, Dictionary<int, int>>();
            _plottedGoals = new Dictionary<int, PuzzleGoal>();
        }

        public static int Plot(this Graph graph, PuzzleGoal goal = null, int x = xStart, int y = yStart, PuzzleGoal start = null)
        {
            goal ??= graph.PuzzleStart; 

            if (start is null) graph.Initialise(); 

            start ??= goal;

            if (!_plottedPositions.ContainsKey(y))
            {
                _plottedPositions.Add(y, new Dictionary<int, int>());
            }

            if (goal.Result != null)
            {
                foreach (var nextPuzzle in goal.Result.NextPuzzles.Reverse<PuzzleGoal>())
                {
                    if (!_plottedGoals.ContainsKey(nextPuzzle.Id))
                    {
                        _plottedGoals[nextPuzzle.Id] = nextPuzzle;

                        var keys = _plottedPositions.Keys.Where(f => f >= y);

                        foreach (var key in keys)
                        {
                            while (_plottedPositions[key].ContainsKey(x))
                            {
                                x += xStep;
                            }
                        }

                        _plottedPositions[y][x] = nextPuzzle.Id;

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
                    currentPosition = (goal.Position.Item1, goal.Position.Item2 + (yStep / 2));

                    graph.AddNode(++currentId, goal.Result.PrizeName)
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
                    var elbow = graph.AddEdge(currentId, nextPuzzle.Id)
                                 .AddEdgeGraphics()
                                 .AddLine()
                                 .AddPoints(currentPosition, points, index++, nextPuzzle.Id);

                    if (_plottedPositions.ContainsKey((int)elbow.Item1) &&
                        _plottedPositions[(int)elbow.Item1].ContainsKey((int)elbow.Item2))
                    {
                        if ((int)currentPosition.Item1 == (int)elbow.Item1)
                        {
                            var shiftid = _plottedPositions[(int)elbow.Item2][(int)elbow.Item1];                            
                            graph.ShiftNodes(shiftid);
                            graph.ResetElbows();
                        }
                    }

                    ////////////////// working on this
                    var elbowNodes = _plottedPositions[(int)elbow.Item2];

                    foreach (var elbowNode in elbowNodes)
                    {
                        var node = _plottedGoals[elbowNode.Value];

                        if (node.Position.Item1 >= currentPosition.Item1 && node.Position.Item1 <= points[points.Count - 1].Item1)
                        {
                            graph.ShiftNodes(elbowNode.Value);
                            graph.ResetElbows();
                        }
                    }
                    ///////////////////

                    if (nextPuzzle.Result == null || string.IsNullOrEmpty(goal.Result.PrizeName)) continue;

                    foreach (var nextPuzzle2 in goal.Result.NextPuzzles.Reverse<PuzzleGoal>())
                    {
                        var nextPlotted = _plottedGoals[nextPuzzle2.Id];

                        if ((int)currentPosition.Item1 == (int)elbow.Item1)
                        {
                            if (nextPuzzle2.Position.Item2 <= goal.Position.Item2)
                            {                                
                                graph.ShiftNodes(nextPuzzle2.Id);
                                graph.ResetElbows();
                            }
                        }
                    }                    
                }

            }            

            return x;
        }

        public static void ShiftNodes(this Graph graph, int puzzleId)
        {            
            var yNode = graph.GetAttributeNode(puzzleId, "y");
            var yPrizeNode = graph.GetAttributeNode(puzzleId + 1, "y");

            yNode.Value = (int.Parse(yNode.Value) + yStep).ToString();

            if (yPrizeNode != null) yPrizeNode.Value = (int.Parse(yNode.Value) + yStep + (yStep / 2)).ToString();            

            var puzzle = _plottedGoals[puzzleId];

            _plottedPositions[(int)puzzle.Position.Item2].Remove((int)puzzle.Position.Item1);

            puzzle.Position = (puzzle.Position.Item1, puzzle.Position.Item2 + yStep);

            if (!_plottedPositions.ContainsKey((int)puzzle.Position.Item2)) _plottedPositions[(int)puzzle.Position.Item2] = new Dictionary<int, int>();

            _plottedPositions[(int)puzzle.Position.Item2][(int)puzzle.Position.Item1] = puzzleId;

            if (puzzle.Result is null) return;

            foreach (var next in puzzle.Result.NextPuzzles.Reverse<PuzzleGoal>())
            {
                graph.ShiftNodes(next.Id);
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

        public static void ResetElbows(this Graph graph, PuzzleGoal puzzle = null)
        {
            puzzle ??= graph.PuzzleStart;

            if (puzzle.Result is null) return;

            foreach (var next in puzzle.Result.NextPuzzles)
            {

                graph.BumpPointNode(puzzle.Id, puzzle.Id + 1);
                graph.BumpPointNode(puzzle.Id + 1, next.Id);
                graph.BumpPointNode(puzzle.Id, next.Id);

                graph.ResetElbows(next);
            }
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
            else if (int.Parse(midPointX.Value) < int.Parse(startPointX.Value))
            {
                midPointX.Value = nextNodeX.Value;
                midPointY.Value = startPointY.Value;
            }
        }

        //private static void BumpNode(Attribute puzzleNode, Attribute nextNode, double amount, double step, bool hasPrize)
        //{
        //    if (puzzleNode is null || nextNode is null) return;

        //    if (int.Parse(puzzleNode.Value) > int.Parse(nextNode.Value) ||
        //        (hasPrize && int.Parse(puzzleNode.Value) >= int.Parse(nextNode.Value)))
        //    {
        //        nextNode.Value = (int.Parse(puzzleNode.Value) + step).ToString();
        //    }
        //}

        #region first_attempt

        //public static bool CheckNodes(this Graph graph, PuzzleGoal puzzle = null)
        //{
        //    puzzle ??= graph.PuzzleStart;

        //    var output = true;

        //    if (puzzle.Result is null) return output;
        //    var puzzleNode = graph.GetAttributeNode(puzzle.Id, "y");

        //    // find all nodes on the same y axis value
        //    var nodesY = graph.GetNodesOnAxis("y", "x", int.Parse(puzzleNode.Value));

        //    // find any elbows on the same y axis value
        //    var elbowsY = graph.GetElbowsOnAxis("y", "x", int.Parse(puzzleNode.Value));

        //    // see if elbow is to the left of any of them
        //    foreach (var elbow in elbowsY)
        //    {
        //        var elbowX = int.Parse(elbow.Attributes.Where(x => x.Key == "x").First().Value);
        //        var elbowY = int.Parse(elbow.Attributes.Where(x => x.Key == "y").First().Value);
        //        var target = graph.GetTargetFromElbow(elbowX, elbowY);

        //        foreach (var node in nodesY)
        //        {
        //            var id = int.Parse(node.Attributes.Where(x => x.Key == "id").First().Value);
        //            var nextid = int.Parse(elbow.Attributes.Where(x => x.Key == "nextId").First().Value);
        //            var nodeX = int.Parse(node.Sections.First().Attributes.Where(x => x.Key == "x").First().Value);

        //            if (elbowX <= nodeX && id != puzzle.Id)
        //            {
        //                ShiftNode(node, "y", yStep);

        //                var prizeId = int.Parse(node.Attributes.Where(x => x.Key == "id").First().Value) + 1;
        //                var prizeNode = graph.Sections.Where(x => x.Name == "node" && x.Attributes.Where(y => y.Key == "id" && y.Value == prizeId.ToString()).Any()).FirstOrDefault() as Node;

        //                if (prizeNode != null)
        //                {
        //                    ShiftNode(prizeNode, "y", yStep + (yStep / 2));

        //                    if (int.Parse(node.Sections.First().Attributes.Where(x => x.Key == "y").First().Value) >=
        //                        int.Parse(prizeNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value))
        //                    {
        //                        prizeNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value =
        //                            int.Parse(node.Sections.First().Attributes.Where(x => x.Key == "y").First().Value + (yStep / 2)).ToString();
        //                    }                            
        //                }

        //                var nextNode = graph.Sections.Where(x => x.Name == "node" && x.Attributes.Where(y => y.Key == "id" && y.Value == nextid.ToString()).Any()).FirstOrDefault() as Node;

        //                if (nextNode != null)
        //                {
        //                    if (prizeNode != null)
        //                    {
        //                        if (int.Parse(prizeNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value) >=
        //                            int.Parse(nextNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value))
        //                        {
        //                            nextNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value =
        //                                (int.Parse(prizeNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value) + (yStep / 2)).ToString();

        //                            var prizeId2 = nextid + 1;
        //                            var prizeNode2 = graph.Sections.Where(x => x.Name == "node" && x.Attributes.Where(y => y.Key == "id" && y.Value == prizeId2.ToString()).Any()).FirstOrDefault() as Node;

        //                            if (prizeNode2 != null)
        //                            {
        //                                prizeNode2.Sections.First().Attributes.Where(x => x.Key == "y").First().Value =
        //                                    (int.Parse(nextNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value) + (yStep / 2)).ToString();
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (int.Parse(node.Sections.First().Attributes.Where(x => x.Key == "y").First().Value) >=
        //                            int.Parse(nextNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value))
        //                        {
        //                            nextNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value =
        //                                (int.Parse(node.Sections.First().Attributes.Where(x => x.Key == "y").First().Value) + yStep).ToString();

        //                            var prizeId2 = nextid + 1;
        //                            var prizeNode2 = graph.Sections.Where(x => x.Name == "node" && x.Attributes.Where(y => y.Key == "id" && y.Value == prizeId2.ToString()).Any()).FirstOrDefault() as Node;

        //                            if (prizeNode2 != null)
        //                            {
        //                                prizeNode2.Sections.First().Attributes.Where(x => x.Key == "y").First().Value =
        //                                    (int.Parse(nextNode.Sections.First().Attributes.Where(x => x.Key == "y").First().Value) + (yStep / 2)).ToString();
        //                            }
        //                        }
        //                    }
        //                }

        //                output = false;
        //                break;
        //            }
        //            if (!output) break;
        //        }
        //    }

        //    // find all nodes on the same x axis value
        //    var nodesX = graph.GetNodesOnAxis("x", "y", int.Parse(puzzleNode.Value));

        //    // find any elbows on the same x axis value
        //    var elbowsX = graph.GetElbowsOnAxis("x", "y", int.Parse(puzzleNode.Value));

        //    // see if elbow is to the left of any of them
        //    foreach (var elbow in elbowsX)
        //    {
        //        var elbowX = int.Parse(elbow.Attributes.Where(x => x.Key == "x").First().Value);
        //        var elbowY = int.Parse(elbow.Attributes.Where(x => x.Key == "y").First().Value);
        //        var target = graph.GetTargetFromElbow(elbowX, elbowY);

        //        foreach (var node in nodesX)
        //        {
        //            var id = int.Parse(node.Attributes.Where(x => x.Key == "id").First().Value);
        //            var nextid = int.Parse(elbow.Attributes.Where(x => x.Key == "nextId").First().Value);
        //            var nodeY = int.Parse(node.Sections.First().Attributes.Where(x => x.Key == "y").First().Value);

        //            if (elbowY <= nodeY && nextid != target && id != puzzle.Id)
        //            {
        //                ShiftNode(node, "x", xStep);

        //                var prizeId = int.Parse(node.Attributes.Where(x => x.Key == "id").First().Value) + 1;
        //                var prizeNode = graph.Sections.Where(x => x.Name == "node" && x.Attributes.Where(y => y.Key == "id" && y.Value == prizeId.ToString()).Any()).FirstOrDefault() as Node;

        //                if (prizeNode != null)
        //                {
        //                    ShiftNode(prizeNode, "x", xStep);
        //                }

        //                output = false;
        //                break;
        //            }
        //            if (!output) break;
        //        }
        //    }

        //    foreach (var next in puzzle.Result.NextPuzzles.Reverse<PuzzleGoal>())
        //    {
        //        output &= graph.CheckNodes(next);
        //    }

        //    return output;
        //}

        //private static void BumpPrize(Graph graph, Node node, Node nextNode)
        //{            
        //}

        //private static void ShiftNode(Node node, string axis, double step)
        //{
        //    var attribute = node.Sections.Where(x => x.Name == "graphics").First()
        //                         .Attributes.Where(x => x.Key == axis).First();

        //    attribute.Value = (int.Parse(attribute.Value) + step).ToString();
        //}

        //private static List<Node> GetNodesOnAxis(this Graph graph, string axis, string sortAxis, int value)
        //{
        //    var sections = graph.Sections.Where(x => x.Name == "node" && (x.Sections.Where(y => y.Name == "graphics").First().Attributes.Where(y => y.Key == axis && y.Value == value.ToString()).Any())).ToList() as List<Section>;
        //    return sections.Select(x => x as Node).OrderBy(x => x.Sections.First().Attributes.Where(y => y.Key == sortAxis).First().Value).ToList();
        //}

        //private static List<Point> GetElbowsOnAxis(this Graph graph, string axis, string sortAxis, int value)
        //{
        //    var output = new List<Point>();

        //    graph.Sections.Where(x => x.Name == "edge").ToList()
        //        .ForEach(section => {
        //            var elbow = section.Sections.First() .Sections.First()
        //                .Sections.Where(y => y.Name == "point" &&
        //                    y.Attributes.Where(z => z.Key == "nextId").Any() &&
        //                    y.Attributes.Where(z => z.Key == axis && z.Value == value.ToString()).Any()).FirstOrDefault() as Point;

        //            if (elbow != null) output.Add(elbow);
        //        });                

        //    return output;
        //}

        //private static int GetTargetFromElbow(this Graph graph, int xAxis, int yAxis)
        //{
        //    var edge = graph.Sections.Where(x => x.Name == "edge").ToList().Where(x =>
        //                    x.Sections.First().Sections.First().Sections.Where(y => y.Name == "point" &&
        //                    y.Attributes.Where(z => z.Key == "nextId").Any() &&
        //                    y.Attributes.Where(z => z.Key == "x" && z.Value == xAxis.ToString()).Any() &&
        //                    y.Attributes.Where(z => z.Key == "y" && z.Value == yAxis.ToString()).Any()).Any()).First() as Edge;

        //    return int.Parse(edge.Attributes.Where(x => x.Key == "target").First().Value);
        //}

        //public static void HideNodes(this Graph graph, PuzzleGoal puzzle = null)
        //{
        //    puzzle ??= graph.PuzzleStart;

        //    if (puzzle.Result is null) return;

        //    if (puzzle.Hidden)
        //    {
        //        graph.RemoveEdges(puzzle.Id);
        //        graph.RemoveNode(puzzle.Id);
        //    }

        //    foreach (var next in puzzle.Result.NextPuzzles)
        //    {                
        //        graph.HideNodes(next);
        //    }
        //}

        //public static void SortNodes(this Graph graph, PuzzleGoal puzzle = null)
        //{
        //    puzzle ??= graph.PuzzleStart;

        //    if (puzzle.Result is null) return;
        //    var puzzleNode = graph.GetAttributeNode(puzzle.Id, "y");

        //    foreach (var next in puzzle.Result.NextPuzzles.Reverse<PuzzleGoal>())
        //    {
        //        var nextNode = graph.GetAttributeNode(next.Id, "y");
        //        var puzprizeNode = graph.GetAttributeNode(puzzle.Id + 1, "y");
        //        var prizeNode = graph.GetAttributeNode(next.Id + 1, "y");

        //        BumpNode(puzzleNode, nextNode, 0, yStep, prizeNode != null);                
        //        BumpNode(prizeNode, puzprizeNode, 0, yStep + (yStep / 2), prizeNode != null);

        //        graph.SortNodes(next);
        //    }
        //}                

        //public static void ResetElbows(this Graph graph, PuzzleGoal puzzle = null)
        //{
        //    puzzle ??= graph.PuzzleStart;

        //    if (puzzle.Result is null) return;

        //    foreach (var next in puzzle.Result.NextPuzzles)
        //    {

        //        graph.BumpPointNode(puzzle.Id, puzzle.Id + 1);
        //        graph.BumpPointNode(puzzle.Id + 1, next.Id);
        //        graph.BumpPointNode(puzzle.Id, next.Id);                

        //        graph.ResetElbows(next);
        //    }
        //}

        //private static void SortOverlaps(this Graph graph, PuzzleGoal puzzle = null, Dictionary<int, List<(int, int)>> allocated = null, PuzzleGoal start = null)
        //{
        //    puzzle ??= graph.PuzzleStart;
        //    start ??= puzzle;
        //    allocated ??= new Dictionary<int, List<(int, int)>>();

        //    if (puzzle.Result is null) return;

        //    foreach (var next in puzzle.Result.NextPuzzles.Reverse<PuzzleGoal>())
        //    {
        //        foreach (var edge in graph.Sections.Where(x => x is Edge && 
        //            x.Attributes.Where(y => y.Key == "source" && y.Value == (puzzle.Id + 1).ToString()).Any() &&
        //            x.Attributes.Where(y => y.Key == "target" && y.Value == next.Id.ToString()).Any()).Select(x => x as Edge))
        //        {
        //            foreach (var graphic in edge.Sections.Where(x => x is EdgeGraphics).Select(x => x as EdgeGraphics))
        //            {
        //                foreach (var line in graphic.Sections.Where(x => x is Line).Select(x => x as Line))
        //                {
        //                    var points = line.Sections.Where(x => x is Point).Select(x => x as Point).ToList();

        //                    if (points.Count < 3) continue;

        //                    var x1 = int.Parse(points[1].Attributes.Where(x => x.Key == "x").First().Value);
        //                    var y1 = int.Parse(points[1].Attributes.Where(x => x.Key == "y").First().Value);
        //                    var x2 = int.Parse(points[2].Attributes.Where(x => x.Key == "x").First().Value);
        //                    var y2 = int.Parse(points[2].Attributes.Where(x => x.Key == "y").First().Value);

        //                    if (allocated.ContainsKey(y1))
        //                    {
        //                        if ((x1 != x2 && y1 == y2 && allocated[y2].Any(x => x.Item2 < x2) || 
        //                            (x1 != x2 && y1 == y2 && allocated[y2].Any(x => x.Item1 < y1))))
        //                        {
        //                            var puzzleNode = graph.GetAttributeNode(puzzle.Id, "y");
        //                            var nextNode = graph.GetAttributeNode(next.Id, "y");
        //                            var prizeNode = graph.GetAttributeNode(next.Id + 1, "y");

        //                            BumpNode(puzzleNode, nextNode, 0, yStep, (prizeNode != null));                                    
        //                            BumpNode(puzzleNode, prizeNode, 0, yStep + (yStep / 2), (prizeNode != null));

        //                            if (allocated.ContainsKey(y1 + 0))
        //                            {
        //                                allocated[y1 + 0].Add((x1, x2));
        //                            }
        //                            else
        //                            {
        //                                allocated.Add(y1 + 0, new List<(int, int)>() { (x1, x2) });
        //                            }
        //                        }                                
        //                        else
        //                        {
        //                            allocated[y1].Add((x1, x2));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        allocated.Add(y1, new List<(int, int)>() { (x1, x2) });
        //                    }
        //                }
        //            }
        //        }

        //        graph.SortOverlaps(next, allocated);
        //    }
        //}        

        //private static Attribute GetAttributeNode(this Graph graph, int id, string key)
        //{
        //    var section = graph.Sections.Where(x => x.Attributes.Where(y => y.Key == "id" && y.Value == id.ToString()).Any()).FirstOrDefault() as Node;
        //    if (section is null) return null;

        //    var graphic = section.Sections.Where(x => x is Graphics).FirstOrDefault() as Graphics;
        //    if (graphic is null) return null;

        //    return graphic.Attributes.Where(x => x.Key == key).FirstOrDefault();
        //}

        //private static void BumpPointNode(this Graph graph, int sourceId, int targetId)
        //{
        //    if (graph.Sections.Where(x => x.Attributes.Where(y => y.Key == "id" && y.Value == sourceId.ToString()).Any()).FirstOrDefault() is not Node startNode) return;

        //    var source = graph.Sections.Where(x => x.Attributes.Where(y => y.Key == "source" && y.Value == sourceId.ToString()).Any()).ToList();

        //    if (source.Where(x => x.Attributes.Where(y => y.Key == "target" && y.Value == targetId.ToString()).Any()).FirstOrDefault() is not Edge edge) return;

        //    if (edge.Sections.Where(x => x is EdgeGraphics).FirstOrDefault() is not EdgeGraphics graphic) return;

        //    if (graphic.Sections.Where(x => x is Line).FirstOrDefault() is not Line line) return;

        //    if (graph.GetAttributeNode(targetId, "x") is not Attribute nextNodeX) return;
        //    if (graph.GetAttributeNode(targetId, "y") is not Attribute nextNodeY) return;

        //    var points = line.Sections.Where(x => x is Point).Select(x => x as Point).ToList();
        //    if (!points.Any()) return;

        //    if (graph.GetAttributeNode(sourceId, "x") is not Attribute startPointX) return;
        //    if (graph.GetAttributeNode(sourceId, "y") is not Attribute startPointY) return;

        //    var midPoint = points[1];
        //    var midPointX = midPoint.Attributes.Where(x => x.Key == "x").First();
        //    var midPointY = midPoint.Attributes.Where(x => x.Key == "y").First();

        //    if (int.Parse(startPointX.Value) == int.Parse(midPointX.Value))
        //    {
        //        if (int.Parse(midPointX.Value) == int.Parse(nextNodeX.Value))
        //        {
        //            midPointY.Value = (int.Parse(startPointY.Value) + ((int.Parse(nextNodeY.Value) - int.Parse(startPointY.Value)) / 2)).ToString();
        //        }
        //        else if (int.Parse(nextNodeX.Value) > int.Parse(midPointX.Value))
        //        {
        //            midPointY.Value = nextNodeY.Value;
        //        }
        //    }
        //    else if (int.Parse(midPointX.Value) > int.Parse(startPointX.Value))
        //    {
        //        midPointX.Value = nextNodeX.Value;
        //        midPointY.Value = startPointY.Value;
        //    }
        //    else if (int.Parse(midPointX.Value) < int.Parse(startPointX.Value))
        //    {
        //        midPointX.Value = nextNodeX.Value;
        //        midPointY.Value = startPointY.Value;
        //    }
        //}

        #endregion
    }
}