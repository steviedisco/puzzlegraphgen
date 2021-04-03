﻿using System;
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

                        graph.Position(nextPuzzle, x, y + yStep, start);
                    }
                }
            }            
        }

        public static void Sort(this Graph graph, PuzzleGoal goal = null)
        {
            goal ??= graph.PuzzleStart;

            if (goal.Result == null) return;

            foreach (var nextPuzzle in goal.Result.NextPuzzles)
            {
                var index = 0;

                if (!string.IsNullOrEmpty(goal.Result.PrizeName))
                {
                    if (nextPuzzle.Position.Item2 <= goal.Position.Item2)
                    {
                        graph.Shift(nextPuzzle);
                    }
                }
                else
                {
                    if (nextPuzzle.Position.Item2 < goal.Position.Item2)
                    {
                        graph.Shift(nextPuzzle);
                    }
                }

                if (index++ == 0)
                {
                    foreach (var position in _plottedPositions[nextPuzzle.Position.Item2])
                    {
                        if (position.Key > nextPuzzle.Position.Item1 && position.Key < goal.Position.Item1)
                        {
                            if (position.Value > 0)
                            {
                                graph.Shift(nextPuzzle);
                            }                            
                        }
                    }
                }
                else
                {
                    foreach (var position in _plottedPositions[goal.Position.Item2])
                    {
                        if (position.Key > nextPuzzle.Position.Item1 && position.Key < goal.Position.Item1)
                        {
                            graph.Shift(goal);
                        }
                    }
                }

                graph.Sort(nextPuzzle);
            }
        }


        public static void Compress(this Graph graph)
        {
            var sorted = _plottedPositions.Keys.OrderBy(y => y).ToList();
            var removed = false;

            for (var i = 1; i < sorted.Count(); i++)
            {
                var diff = sorted[i] - sorted[i - 1];

                if (diff > yStep)
                {
                    var row = _plottedPositions[sorted[i]];                    

                    foreach (var id in row.Values)
                    {
                        var goal = _plottedGoals[id];

                        _plottedPositions[sorted[i]].Remove(goal.Position.Item1);

                        goal.Position = (goal.Position.Item1, goal.Position.Item2 - yStep);

                        if (!_plottedPositions.ContainsKey(goal.Position.Item2))
                        {
                            _plottedPositions.Add(goal.Position.Item2, new Dictionary<int, int>());
                        }

                        _plottedPositions[goal.Position.Item2][goal.Position.Item1] = goal.Id;

                        if (!_plottedPositions[sorted[i]].Any())
                        {
                            _plottedPositions.Remove(sorted[i]);
                            removed = true;
                            break;
                        }                           
                    }

                    if (removed) break;
                }
            }

            if (removed) graph.Compress();
        }


        public static void Shift(this Graph graph, PuzzleGoal goal = null)
        {
            _plottedPositions[goal.Position.Item2].Remove(goal.Position.Item1);

            if (!_plottedPositions[goal.Position.Item2].Any())
            {
                _plottedPositions.Remove(goal.Position.Item2);
            }

            PuzzleGoal existing = null;

            goal.Position = (goal.Position.Item1, goal.Position.Item2 + yStep);

            if (_plottedPositions.ContainsKey(goal.Position.Item2) &&
                _plottedPositions[goal.Position.Item2].ContainsKey(goal.Position.Item1) &&
                _plottedPositions[goal.Position.Item2][goal.Position.Item1] > 0)
            {
                existing = _plottedGoals[_plottedPositions[goal.Position.Item2][goal.Position.Item1]];
                graph.Shift(existing);
            }

            if (!_plottedPositions.ContainsKey(goal.Position.Item2))
            {
                _plottedPositions.Add(goal.Position.Item2, new Dictionary<int, int>());
            }

            _plottedPositions[goal.Position.Item2][goal.Position.Item1] = goal.Id;

            if (goal.Result == null) return;

            foreach (var next in goal.Result.NextPuzzles.Reverse<PuzzleGoal>())
            {
                if (next != existing)
                    graph.Shift(next);
            }
        }

        public static void Plot(this Graph graph, PuzzleGoal goal = null)
        {
            goal ??= graph.PuzzleStart;

            graph.AddNode(goal.Id, goal.Title)
                 .AddGraphics(goal.Position, nodeWidth, nodeHeight)
                 .AddLabelGraphics(goal.Title);

            if (goal.Result == null) return;

            var currentId = goal.Id;
            var currentPosition = goal.Position;            
             
            if (!string.IsNullOrEmpty(goal.Result.PrizeName))
            {
                currentPosition = (goal.Position.Item1, goal.Position.Item2 + (yStep / 2));
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