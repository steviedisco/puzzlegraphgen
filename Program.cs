using System;
using System.Collections.Generic;
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
            var graph = CreateBranchingGraph(); 
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

        #region test stuff

        //private static GraphContainer CreateSimpleGraph()
        //{
        //    var container = GraphContainer.Create();
        //    var graph = container.AddGraph();

        //    const int x = 100;
        //    const int max_nodes = 5;
        //    var starty = -100;
        //    var stepy = 80;

        //    for (int i = 0; i < max_nodes; i++)
        //    {
        //        var label = (i == max_nodes - 1) ? "Finish" : (i == 0) ? "Start" : $"Node {i}";
        //        var y = starty + (stepy * i);
        //        graph.AddNode(i, label).AddGraphics(x, y).AddLabelGraphics(label);
        //    }

        //    for (int i = 0; i < max_nodes - 1; i++)
        //    {
        //        graph.AddEdge(i, i + 1).AddEdgeGraphics();
        //    }

        //    return container;
        //}

        //private static GraphContainer Create2PathGraph()
        //{
        //    var container = GraphContainer.Create();
        //    var graph = container.AddGraph();

        //    const int max_nodes = 3;
        //    const int offsetx = 100;
        //    const int stepy = 80;
        //    const int chara = 65;
        //    var starty = -220;

        //    graph.AddNode(0, "Start").AddGraphics(0, starty).AddLabelGraphics("Start");
        //    graph.AddNode(1, "Puzzle 1").AddGraphics(0, starty + stepy).AddLabelGraphics("Puzzle 1");

        //    graph.AddEdge(0, 1).AddEdgeGraphics();

        //    var label = string.Empty;
        //    var id = 2;
        //    var previd = 1;
        //    var joinids = new int[2];

        //    for (int i = -1; i <= 1; i+=2)
        //    {
        //        previd = 1;

        //        for (int j = 2; j < 2 + max_nodes; j++)
        //        {
        //            var prefix = (char)(chara + ((i + 1) / 2));
        //            var y = starty + (stepy * j);
        //            label = $"Puzzle {prefix}{j}";

        //            graph.AddNode(id, label).AddGraphics(i * offsetx, y).AddLabelGraphics(label);
        //            graph.AddEdge(previd, id).AddEdgeGraphics();
        //            previd = id;
        //            id++;
        //        }

        //        joinids[(i + 1) / 2] = previd;
        //    }

        //    label = $"Puzzle {max_nodes + 2}";
        //    graph.AddNode(id, label).AddGraphics(0, starty + (stepy * (max_nodes + 2))).AddLabelGraphics(label);
        //    graph.AddEdge(joinids[0], id).AddEdgeGraphics();
        //    graph.AddEdge(joinids[1], id).AddEdgeGraphics();
        //    previd = id++;

        //    graph.AddNode(id, "Finish").AddGraphics(0, starty + (stepy * (max_nodes + 3))).AddLabelGraphics("Finish");
        //    graph.AddEdge(previd, id).AddEdgeGraphics();

        //    return container;
        //}

        //private static GraphContainer Create3PathGraph()
        //{
        //    var container = GraphContainer.Create();
        //    var graph = container.AddGraph();

        //    const int max_nodes = 3;
        //    const int offsetx = 150;
        //    const int stepy = 80;
        //    const int chara = 65;
        //    var starty = -220;

        //    graph.AddNode(0, "Start").AddGraphics(0, starty).AddLabelGraphics("Start");
        //    graph.AddNode(1, "Puzzle 1").AddGraphics(0, starty + stepy).AddLabelGraphics("Puzzle 1");

        //    graph.AddEdge(0, 1).AddEdgeGraphics();

        //    var label = string.Empty;
        //    var id = 2;
        //    var previd = 1;
        //    var joinids = new int[3];

        //    for (int i = -1; i <= 1; i++)
        //    {
        //        previd = 1;

        //        for (int j = 2; j < 2 + max_nodes; j++)
        //        {
        //            var prefix = (char)(chara + (i + 1));
        //            var y = starty + (stepy * j);
        //            label = $"Puzzle {prefix}{j}";

        //            graph.AddNode(id, label).AddGraphics(i * offsetx, y).AddLabelGraphics(label);
        //            graph.AddEdge(previd, id).AddEdgeGraphics();
        //            previd = id;
        //            id++;
        //        }

        //        joinids[(i + 1)] = previd;
        //    }

        //    label = $"Puzzle {max_nodes + 2}";
        //    graph.AddNode(id, label).AddGraphics(0, starty + (stepy * (max_nodes + 2))).AddLabelGraphics(label);
        //    graph.AddEdge(joinids[0], id).AddEdgeGraphics();
        //    graph.AddEdge(joinids[1], id).AddEdgeGraphics();
        //    graph.AddEdge(joinids[2], id).AddEdgeGraphics();
        //    previd = id++;

        //    graph.AddNode(id, "Finish").AddGraphics(0, starty + (stepy * (max_nodes + 3))).AddLabelGraphics("Finish");
        //    graph.AddEdge(previd, id).AddEdgeGraphics();

        //    return container;
        //}

        #endregion

        #region dott

        private static GraphContainer CreateDOTTGraph()
        {
            // work backwards from last puzzle to first
            var final = new PuzzleGoal { Title = "Get the Super Battery" };

            // layer 4
            var gold = new PuzzleGoal { Title = "Get the gold" };
            gold.PuzzleResults.Add(new PuzzleResult { Name = "Gold", NextPuzzle = final });

            var vinegar = new PuzzleGoal { Title = "Get Vinegar" };
            vinegar.PuzzleResults.Add(new PuzzleResult { Name = "Vinegar", NextPuzzle = final });

            // layer 3
            var fire = new PuzzleGoal { Title = "Build a fire in the fireplace" };
            fire.PuzzleResults.Add(new PuzzleResult { Name = "The Blanket", NextPuzzle = gold });

            var denture = new PuzzleGoal { Title = "Dentures to Laverne" };
            denture.PuzzleResults.Add(new PuzzleResult { Name = "Access to History Room", NextPuzzle = vinegar });

            // layer 2
            var cigar = new PuzzleGoal { Title = "Get exploding cigar" };
            cigar.PuzzleResults.Add(new PuzzleResult { Name = "Exploding cigar lighter gun", NextPuzzle = fire });

            var laverne = new PuzzleGoal { Title = "Laverne access outside" };
            laverne.PuzzleResults.Add(new PuzzleResult { Name = "Access Laverne's chron-o-john", NextPuzzle = denture });

            // Start layer
            var dwayne = new PuzzleGoal { Title = "Get into Dwayne's room" };
            dwayne.PuzzleResults.Add(new PuzzleResult { Name = "Flag Gun", NextPuzzle = cigar });

            var tree = new PuzzleGoal { Title = "Get Laverne down from tree" };
            tree.PuzzleResults.Add(new PuzzleResult { Name = "Laverne", NextPuzzle = laverne });

            var getdenture = new PuzzleGoal { Title = "Get Dentures" };
            getdenture.PuzzleResults.Add(new PuzzleResult { Name = "Dentures", NextPuzzle = denture });

            var makevinegar = new PuzzleGoal { Title = "Make Vinegar" };
            makevinegar.PuzzleResults.Add(new PuzzleResult { Name = "Vinegar in Time Capsule", NextPuzzle = vinegar });

            var redEd = new PuzzleGoal { Title = "Give plans to Red Edison" };
            redEd.PuzzleResults.Add(new PuzzleResult { Name = "Red Edison can build Battery", NextPuzzle = final });

            var start = new PuzzleStart(new List<PuzzleGoal>() { dwayne, tree, getdenture, makevinegar, redEd });

            var container = GraphContainer.Create();
            var graph = container.AddGraph();

            graph.InitialisePlot();
            graph.Plot(start);            

            return container;
        }

        #endregion

        private static GraphContainer CreateBranchingGraph()
        {
            // work backwards from last puzzle to first
            var a = new PuzzleGoal { Title = "Branch A" };
            var b = new PuzzleGoal { Title = "Branch B" };
            var c = new PuzzleGoal { Title = "Branch C" };

            var puzzle = new PuzzleGoal { Title = "Do a puzzle" };
            puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Solved", NextPuzzles = {a, b, c} });

            var start = new PuzzleStart(puzzle);

            var container = GraphContainer.Create();
            var graph = container.AddGraph();

            graph.InitialisePlot();
            graph.Plot(start);

            return container;
        }
    }
}
