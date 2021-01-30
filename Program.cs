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
            var graph = CreateDOTTGraph(); 
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

        private static GraphContainer CreateDOTTGraph()
        {
            // work backwards from last puzzle to first
            var final_puzzle = new PuzzleGoal { Title = "Get the Super Battery" };

            // layer 4
            var gold_puzzle = new PuzzleGoal { Title = "Get the gold" };
            gold_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Gold", NextPuzzle = final_puzzle });

            var vinegar_puzzle = new PuzzleGoal { Title = "Get Vinegar" };
            vinegar_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Vinegar", NextPuzzle = final_puzzle });

            // layer 3
            var fire_puzzle = new PuzzleGoal { Title = "Build a fire in the fireplace" };
            fire_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "The Blanket", NextPuzzle = gold_puzzle });

            var denture_puzzle = new PuzzleGoal { Title = "Dentures to Laverne" };
            denture_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Access to History Room", NextPuzzle = final_puzzle });

            // layer 2
            var cigar_puzzle = new PuzzleGoal { Title = "Get exploding cigar" };
            cigar_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Exploding cigar lighter gun", NextPuzzle = fire_puzzle });

            var laverne_puzzle = new PuzzleGoal { Title = "Laverne access outside" };
            laverne_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Access Laverne's chron-o-john", NextPuzzle = denture_puzzle });

            // Start layer
            var dwayne_puzzle = new PuzzleGoal { Title = "Get into Dwayne's room" };
            dwayne_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Flag Gun", NextPuzzle = cigar_puzzle });

            var tree_puzzle = new PuzzleGoal { Title = "Get Laverne down from tree" };
            tree_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Laverne", NextPuzzle = laverne_puzzle });

            var getdenture_puzzle = new PuzzleGoal { Title = "Get Dentures" };
            getdenture_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Dentures", NextPuzzle = denture_puzzle });

            var makevinegar_puzzle = new PuzzleGoal { Title = "Make Vinegar" };
            makevinegar_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Vinegar in Time Capsule", NextPuzzle = vinegar_puzzle });

            var redEd_plans_puzzle = new PuzzleGoal { Title = "Give plans to Red Edison" };
            redEd_plans_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Red Edison can build Battery", NextPuzzle = final_puzzle });

            var start_puzzle = new PuzzleStart();
            start_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Dwayne Puzzle Start", NextPuzzle = dwayne_puzzle, IsStart = true });
            start_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Tree Puzzle Start", NextPuzzle = tree_puzzle, IsStart = true });
            start_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Denture Puzzle Start", NextPuzzle = getdenture_puzzle, IsStart = true });
            start_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Vinegar Puzzle Start", NextPuzzle = makevinegar_puzzle, IsStart = true });
            start_puzzle.PuzzleResults.Add(new PuzzleResult { Name = "Red Ed Puzzle Start", NextPuzzle = redEd_plans_puzzle, IsStart = true });

            var container = GraphContainer.Create();
            var graph = container.AddGraph();

            graph.InitialisePlot();
            graph.Plot(start_puzzle);            

            return container;
        }
    }
}
