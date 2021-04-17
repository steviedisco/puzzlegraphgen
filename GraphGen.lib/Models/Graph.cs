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

        private static int _renameCount = 1;

        public static void Initialise(this Graph graph)
        {
            _plottedPositions = new Dictionary<int, Dictionary<int, int>>();
            _plottedGoals = new Dictionary<int, PuzzleGoal>();
        }

        public static void Position(this Graph graph, PuzzleGoal goal = null, int x = xStart, int y = yStart, PuzzleGoal start = null)
        {
            goal ??= graph.PuzzleStart;

            if (start is null) graph.Initialise();

            start ??= goal;

            if (!_plottedPositions.ContainsKey(y))
            {
                _plottedPositions.Add(y, new Dictionary<int, int>());
            }

            _plottedPositions[y][x] = goal.Id;

            goal.Position = (x, y);

            _plottedGoals[goal.Id] = goal;

            if (goal.Result != null)
            {
                foreach (var nextPuzzle in goal.Result.NextPuzzles)
                {
                    if (!_plottedGoals.ContainsKey(nextPuzzle.Id))
                    {
                        _plottedGoals[nextPuzzle.Id] = nextPuzzle;

                        var keys = _plottedPositions.Keys.Where(f => f > y);

                        foreach (var key in keys)
                        {
                            while (_plottedPositions[key].ContainsKey(x))
                            {
                                x += xStep;
                            }
                        }

                        foreach (var node in _plottedGoals.Values.Where(x => Math.Sign(x.Position.x) > 0 && x.Id != goal.Id))
                        {
                            var intersects = IntersectCheckX(goal, x, node);

                            if (intersects) x += xStep;
                        }

                        graph.Position(nextPuzzle, x, y + yStep, start);
                    }
                }
            }            
        }

        public static void Sort(this Graph graph, int direction = 1)
        {
            var allSorted = true;

            do
            {
                allSorted = true;

                foreach (var nextGoal in _plottedGoals.Values)
                {
                    allSorted &= !graph.DoSort(nextGoal, direction);
                }
            }
            while (!allSorted);
        }

        public static bool DoSort(this Graph graph, PuzzleGoal goal = null, int direction = 1)
        {
            var shifted = false;

            if (goal.Result == null) return false;

            foreach (var nextPuzzle in goal.Result.NextPuzzles)
            {
                var index = 0;

                if (!string.IsNullOrEmpty(goal.Result.PrizeName))
                {
                    if (nextPuzzle.Position.y <= goal.Position.y)
                    {
                        shifted = true;
                        graph.ShiftY(nextPuzzle);                        
                    }
                }
                else
                {
                    if (nextPuzzle.Position.y < goal.Position.y)
                    {
                        shifted = true;
                        graph.ShiftY(nextPuzzle);
                    }
                }

                if (shifted) break;

                if (index++ == 0)
                {
                    foreach (var position in _plottedPositions[nextPuzzle.Position.y])
                    {
                        var node = _plottedGoals[position.Value];

                        if (position.Key > nextPuzzle.Position.x && position.Key < goal.Position.x)
                        {
                            if (position.Value > 0)
                            {
                                shifted = true;
                                graph.ShiftY(nextPuzzle);
                            }
                        } 
                        else if (position.Key == goal.Position.x && nextPuzzle.Position.x < goal.Position.x)
                        {
                            if (position.Value > 0)
                            {
                                shifted = true;                                
                                graph.ShiftY(node);
                            }
                        }
                    }

                    foreach (var position in _plottedPositions[goal.Position.y])
                    {
                        if (position.Value != goal.Id && position.Key == nextPuzzle.Position.x && nextPuzzle.Position.y > goal.Position.x)
                        {
                            if (position.Value > 0)
                            {
                                var node = _plottedGoals[position.Value];
                                shifted = true;
                                graph.ShiftY(goal);
                            }
                        }
                    }

                    if (shifted) break;

                    foreach (var position in _plottedPositions[goal.Position.y])
                    {
                        if (position.Key > goal.Position.x && position.Key < nextPuzzle.Position.x)
                        {
                            if (position.Value > 0)
                            {
                                shifted = true;
                                graph.ShiftY(goal);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var position in _plottedPositions[goal.Position.y])
                    {
                        if (position.Key > nextPuzzle.Position.x && position.Key < goal.Position.x)
                        {
                            shifted = true;
                            graph.ShiftY(goal);
                        }
                    }
                }

                if (shifted) break;

                if (Math.Sign(goal.Position.x) > 0)
                {
                    foreach (var node in _plottedGoals.Values.Where(x => Math.Sign(x.Position.x) > 0 && x.Id != goal.Id))
                    {
                        var intersects = IntersectCheckX(goal, goal.Position.x, node);

                        if (intersects)
                        {
                            shifted = true;
                            graph.ShiftY(goal);
                            break;
                        }
                    }
                }                

                if (shifted) break;

                var rows = _plottedPositions.Where(x => x.Key > goal.Position.y && x.Key < nextPuzzle.Position.y).ToList();

                if (rows.Any(row => row.Value.Any(node => (node.Key * direction) > 0 && node.Key == goal.Position.x)))
                {
                    shifted = true;
                    graph.ShiftX(goal, direction);
                }

                if (shifted) break;

                if ((nextPuzzle.Position.x * direction) > 0 && (goal.Position.x * direction) > (nextPuzzle.Position.x * direction) && goal.Position.y < nextPuzzle.Position.y)
                {
                    var row = _plottedPositions.Where(x => x.Key == nextPuzzle.Position.y).FirstOrDefault();

                    if (row.Value.ContainsKey(goal.Position.x))
                    {
                        var node = _plottedGoals[_plottedPositions[nextPuzzle.Position.y][goal.Position.x]];

                        if (node != nextPuzzle)
                        {
                            shifted = true;
                            graph.ShiftX(node, direction);
                        }
                    }
                }

                if (shifted) break;                
            }

            return shifted;
        }

        public static bool Swap(this Graph graph, PuzzleGoal goal = null)
        {
            goal ??= graph.PuzzleStart;

            if (goal.Result == null) return false;

            var swapped = false;

            foreach (var nextPuzzle in goal.Result.NextPuzzles)
            {
                if (!goal.Swapped && goal.Position.x != 0)
                {
                    var swappable = false;

                    foreach (var node in _plottedGoals.Select(x => x.Value))
                    {
                        if (node.Result == null) continue;

                        foreach (var result in node.Result.NextPuzzles)
                        {
                            if (result == goal && node.Position.x == 0)
                            {
                                swappable = true;
                            }
                        }
                    }

                    if (!swappable) continue;

                    foreach (var node in _plottedGoals.Select(x => x.Value))
                    {
                        var intersects = (node.Position.x == goal.Position.x &&
                                          node.Position.y > goal.Position.y &&
                                          goal.Result.NextPuzzles.Any(x => x.Position.x < node.Position.x && x.Position.y > node.Position.y));

                        if (intersects)
                        {
                            swapped = true;
                            graph.SwapX(goal);
                            break;
                        }
                    }

                    if (swapped) break;

                    for (var y = 0; y <= nextPuzzle.Position.y; y += yStep)
                    {
                        if (!_plottedPositions.ContainsKey(y)) continue;

                        var nodes = _plottedPositions[y].Where(x => x.Key >= 0 && x.Key < goal.Position.x).ToList();

                        foreach (var node in nodes)
                        {
                            var source = _plottedGoals[node.Value];
                            if (source != nextPuzzle && source.Result != null)
                            {
                                foreach (var target in source.Result.NextPuzzles)
                                {
                                    if (target.Position.y > nextPuzzle.Position.y)
                                    {
                                        swapped = true;
                                        graph.SwapX(goal);
                                        break;
                                    }
                                }
                            }

                            if (swapped) break;
                        }

                        if (swapped) break;
                    }
                }

                if (swapped) break;

                if (!nextPuzzle.Swapped && goal.Position.x < 0)
                {
                    swapped = true;
                    graph.SwapX(nextPuzzle);
                    break;
                }

                if (swapped) break;

                swapped = graph.Swap(nextPuzzle);
            }

            return swapped;
        }

        public static void Rename(this Graph graph, PuzzleGoal goal = null)
        {
            if (goal == null) _renameCount = 1;

            goal ??= graph.PuzzleStart;

            if (!goal.Renamed && goal.Title.StartsWith("Puzzle"))
            {                
                goal.Title = $"Puzzle {_renameCount++}";
                goal.Renamed = true;
            }

            if (goal.Result == null) return;            

            foreach (var next in goal.Result.NextPuzzles.Reverse<PuzzleGoal>())
            {
                graph.Rename(next);
            }
        }

        public static void CompressY(this Graph graph)
        {
            var sorted = _plottedPositions.Keys.OrderBy(y => y).ToList();
            var removed = false;

            for (var i = 1; i < sorted.Count(); i++)
            {
                var diff = sorted[i] - sorted[i - 1];
                var row = _plottedPositions[sorted[i]];

                foreach (var id in row.Values)
                {
                    var goal = _plottedGoals[id];

                    if (diff > yStep)
                    {
                        removed = DoCompressY(sorted[i], goal);
                    }
                    else if (diff == yStep)
                    {                        
                        var newY = sorted[i - 1];
                        var intersects = false;

                        var goalParents = _plottedGoals.Select(x => x.Value).Where(x => x.Result != null && x.Result.NextPuzzles.Any(y => y == goal && Math.Sign(y.Position.x) == Math.Sign(goal.Position.x))).ToList();

                        foreach (var parent in goalParents)
                        {
                            foreach (var node in _plottedGoals.Where(x => x.Value.Position.y == newY &&
                                                                      x.Value.Id != goal.Id && (Math.Sign(goal.Position.x) == 0 || Math.Sign(x.Value.Position.x) == Math.Sign(goal.Position.x)))
                                                              .Select(x => x.Value))
                            {
                                intersects = (parent.Result.NextPuzzles.Any(x => x == node) &&
                                        ((parent.Position.x < goal.Position.x && parent.Position.x < node.Position.x) ||
                                         (node.Position.x > goal.Position.x && goal.Position.x > parent.Position.x)));

                                if (intersects) break;
                            }

                            if (intersects) break;
                        }

                        if (!intersects)
                        {
                            foreach (var node in _plottedGoals.Where(x => x.Value.Position.y == newY &&
                                                                      x.Value.Id != goal.Id && (Math.Sign(goal.Position.x) == 0 || Math.Sign(x.Value.Position.x) == Math.Sign(goal.Position.x)))
                                                          .Select(x => x.Value))
                            {
                                var nodeParents = _plottedGoals.Select(x => x.Value).Where(x => x.Result != null &&
                                                                                           x.Result.NextPuzzles.Any(y => y == node)).ToList();

                                foreach (var parent in nodeParents)
                                {
                                    intersects = (node.Position.y == goal.Position.y &&
                                                  ((parent.Position.x < goal.Position.x && goal.Position.x < node.Position.x) ||
                                                   (node.Position.x > goal.Position.x && goal.Position.x > parent.Position.x)));

                                    if (intersects) break;
                                }

                                if (intersects) break;
                            }
                        }

                        if (!intersects)
                        {
                            foreach (var node in _plottedGoals.Values.Where(x => Math.Sign(x.Position.x) == Math.Sign(goal.Position.x) && x.Id != goal.Id))
                            {
                                intersects = IntersectCheckY(goal, newY, node);

                                if (intersects) break;
                            }
                        }

                        if (_plottedPositions.ContainsKey(newY) &&
                            (!_plottedPositions[newY].ContainsKey(goal.Position.x)) &&
                            // (goal.Result != null && goal.Result.NextPuzzles.Count <= 1) &&
                            !goalParents.Any(x => x.Position.y >= newY) && !intersects)
                        {
                            removed = DoCompressY(sorted[i], goal);
                        }                        
                    }

                    if (removed) break;
                }            
            }

            if (removed) graph.CompressY();
        }

        public static bool DoCompressY(int rowKey, PuzzleGoal goal)
        {
            _plottedPositions[rowKey].Remove(goal.Position.x);

            goal.Position = (goal.Position.x, goal.Position.y - yStep);

            if (!_plottedPositions.ContainsKey(goal.Position.y))
            {
                _plottedPositions.Add(goal.Position.y, new Dictionary<int, int>());
            }

            _plottedPositions[goal.Position.y][goal.Position.x] = goal.Id;

            if (!_plottedPositions[rowKey].Any())
            {
                _plottedPositions.Remove(rowKey);
                return true;
            }

            return false;
        }

        public static void CompressX(this Graph graph, int direction = 1)
        {
            var allSorted = true;

            do
            {
                allSorted = true;

                foreach (var goal in _plottedGoals.Values.Where(x => Math.Sign(x.Position.x) == direction).OrderBy(x => x.Position.x).Reverse())
                {
                    allSorted &= !graph.DoCompressX(goal, direction);
                }
            }
            while (!allSorted);
        }

        public static bool DoCompressX(this Graph graph, PuzzleGoal goal, int direction = 1)
        {
            var intersects = false;
            var newX = goal.Position.x;

            if (goal.Title == "Puzzle 2") return false;

            do
            {
                newX = newX - (direction * xStep);

                if (newX == 0) return false;

                if (goal.Result != null)
                {
                    foreach (var node in _plottedGoals.Values.Where(x => (x.Position.x == 0 || Math.Sign(x.Position.x) == direction) && x.Id != goal.Id))
                    {
                        intersects = IntersectCheckX(goal, newX, node);

                        if (intersects) break;
                    }
                }

                if (!intersects)
                {
                    _plottedPositions[goal.Position.y].Remove(goal.Position.x);

                    goal.Position = (newX, goal.Position.y);

                    _plottedPositions[goal.Position.y][goal.Position.x] = goal.Id;

                    return true;
                }
            }
            while (true);
        }

        private static bool IntersectCheckX(PuzzleGoal goal, int newX, PuzzleGoal node)
        {
            bool intersects = (node.Position.x == newX &&
                                                  node.Position.y == goal.Position.y);
            intersects |= (node.Position.x == newX &&
                          node.Position.y < goal.Position.y &&
                          node.Result.NextPuzzles.Any(x => x.Position.x == newX && x.Position.y > goal.Position.y));

            intersects |= (node.Position.y == goal.Position.y &&
                          node.Position.x > goal.Position.x &&
                          node.Result.NextPuzzles.Any(x => x.Position.x == newX && x.Position.y > goal.Position.y));

            intersects |= (node.Position.x == newX &&
                          node.Position.y < goal.Position.y &&
                          node.Result.NextPuzzles.Any(x => x.Position.x > newX)); //  && x.Position.y == goal.Position.y

            return intersects;
        }

        private static bool IntersectCheckY(PuzzleGoal goal, int newY, PuzzleGoal node)
        {
            bool intersects = (node.Position.y == newY &&
                                                  node.Position.x == goal.Position.x);

            if (node.Result != null)
            {
                intersects |= (node.Position.y <= newY &&
                          node.Position.x < goal.Position.x &&
                          node.Result.NextPuzzles.Any(x => x.Position.y == newY && x.Position.x > goal.Position.x));

                intersects |= (node.Position.x == goal.Position.x &&
                              node.Position.y < goal.Position.y &&
                              node.Result.NextPuzzles.Any(x => x.Position.y == newY && x.Position.x > goal.Position.x));

                intersects |= (node.Position.y == newY &&
                              node.Position.x < goal.Position.x &&
                              node.Result.NextPuzzles.Any(x => x.Position.y > newY && x.Position.x == goal.Position.x));
            }            

            return intersects;
        }

        public static void SwapX(this Graph graph, PuzzleGoal goal)
        {
            _plottedPositions[goal.Position.y].Remove(goal.Position.x);

            goal.Position = (-goal.Position.x, goal.Position.y);

            _plottedPositions[goal.Position.y][goal.Position.x] = goal.Id;

            goal.Swapped = true;
        }

        public static void ShiftX(this Graph graph, PuzzleGoal goal, int direction = 1)
        {
            _plottedPositions[goal.Position.y].Remove(goal.Position.x);

            var newX = goal.Position.x + (direction * xStep);

            goal.Position = (newX, goal.Position.y);

            if (_plottedPositions.ContainsKey(goal.Position.y) &&
                _plottedPositions[goal.Position.y].ContainsKey(goal.Position.x) &&
                _plottedPositions[goal.Position.y][goal.Position.x] > 0)
            {
                var existing = _plottedGoals[_plottedPositions[goal.Position.y][goal.Position.x]];
                graph.ShiftX(existing, direction);
            }

            _plottedPositions[goal.Position.y][goal.Position.x] = goal.Id;
        }

        public static void ShiftY(this Graph graph, PuzzleGoal goal)
        {
            _plottedPositions[goal.Position.y].Remove(goal.Position.x);

            if (!_plottedPositions[goal.Position.y].Any())
            {
                _plottedPositions.Remove(goal.Position.y);
            }

            PuzzleGoal existing = null;

            goal.Position = (goal.Position.x, goal.Position.y + yStep);

            if (_plottedPositions.ContainsKey(goal.Position.y) &&
                _plottedPositions[goal.Position.y].ContainsKey(goal.Position.x) &&
                _plottedPositions[goal.Position.y][goal.Position.x] > 0)
            {
                existing = _plottedGoals[_plottedPositions[goal.Position.y][goal.Position.x]];
                graph.ShiftY(existing);
            }

            if (!_plottedPositions.ContainsKey(goal.Position.y))
            {
                _plottedPositions.Add(goal.Position.y, new Dictionary<int, int>());
            }

            _plottedPositions[goal.Position.y][goal.Position.x] = goal.Id;

            if (goal.Result == null) return;

            foreach (var next in goal.Result.NextPuzzles.Reverse<PuzzleGoal>())
            {
                if (next != existing)
                    graph.ShiftY(next);
            }
        }

        public static void Plot(this Graph graph, PuzzleGoal goal = null)
        {
            goal ??= graph.PuzzleStart;

            if (goal.Plotted) return;

            goal.Plotted = true;

            graph.AddNode(goal.Id, goal.Title)
                 .AddGraphics(goal.Position, nodeWidth, nodeHeight)
                 .AddLabelGraphics(goal.Title);

            if (goal.Result == null) return;

            var currentId = goal.Id;
            var currentPosition = (goal.Position.x, goal.Position.y);
             
            if (!string.IsNullOrEmpty(goal.Result.PrizeName))
            {
                currentPosition = (goal.Position.x, goal.Position.y + (yStep / 2));
                currentId = goal.Id + 1;

                graph.AddNode(currentId, goal.Result.PrizeName)
                        .AddGraphics(currentPosition, nodeWidth, nodeHeight / 2, true)
                        .AddLabelGraphics(goal.Result.PrizeName);

                graph.AddEdge(goal.Id, currentId)
                     .AddEdgeGraphics()
                     .AddLine();
            }

            foreach (var nextPuzzle in goal.Result.NextPuzzles)
            {
                graph.Plot(nextPuzzle);
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
                     .AddPoints(currentPosition, points, index++, nextPuzzle.Id);
            }            
        }
    }
}