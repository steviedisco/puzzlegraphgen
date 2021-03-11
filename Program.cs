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
            var graph = CreateFullDOTTGraph(); 
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
            var final = new PuzzleGoal("Get the Super Battery");

            // layer 4
            var gold = new PuzzleGoal("Get the gold", "Gold", final);
            var vinegar = new PuzzleGoal("Get Vinegar", "Vinegar", final);

            // layer 3
            var fire = new PuzzleGoal("Build a fire in the fireplace", "The Blanket", gold);
            var denture = new PuzzleGoal("Dentures to Laverne", "Access to History Room", vinegar);

            // layer 2
            var cigar = new PuzzleGoal("Get exploding cigar", "Exploding cigar lighter gun", fire);
            var laverne = new PuzzleGoal("Laverne access outside", "Access Laverne's chron-o-john", denture);

            // Start layer
            var dwayne = new PuzzleGoal("Get into Dwayne's room", "Flag Gun", cigar);
            var tree = new PuzzleGoal("Get Laverne down from tree", "Laverne", laverne);
            var getdenture = new PuzzleGoal("Get Dentures", "Dentures", denture);
            var makevinegar = new PuzzleGoal("Make Vinegar", "Vinegar in Time Capsule", vinegar);
            var redEd = new PuzzleGoal("Give plans to Red Edison", "Red Edison can build Battery", final);

            var start = new PuzzleStart(new List<PuzzleGoal>() { dwayne, tree, getdenture, makevinegar, redEd });

            var container = GraphContainer.Create();
            var graph = container.AddGraph();
            
            graph.Plot(start);            

            return container;
        }

        private static GraphContainer CreateMidDOTTGraph()
        {
            var win = new PuzzleGoal("Win the Game");

            var tentacleBoss = new PuzzleGoal("Knock out ten Tentacles", "TentacleBoss", win);
            var bowlingBall = new PuzzleGoal("Get Bowling Ball", "Bowling Ball", tentacleBoss);
            var shrunkenKids = new PuzzleGoal("Back to the Present", "Shrunken Kids", bowlingBall);

            var poweredJohn = new PuzzleGoal("Power Hoagie's Chron-o-John", "Hoagie's Chron-o-John Powered", shrunkenKids);
            var chargedBattery = new PuzzleGoal("Charge Battery", "Charged Super Battery", poweredJohn);

            var kite = new PuzzleGoal("Get Kite", "Kite", chargedBattery);

            var labCoat = new PuzzleGoal("Get Lab Coat", "Lab Coat", kite);
            var franklinRoom = new PuzzleGoal("Start Storm", "Franklin in his Room", kite);

            var soap = new PuzzleGoal("Get Soap", "Soap", franklinRoom);

            var superBattery = new PuzzleGoal("Get the Super Battery", "Super Battery", chargedBattery);

            var gold = new PuzzleGoal("Get the Gold", "Gold", superBattery);
            var buildBattery = new PuzzleGoal("Give plans to Red Edison", "Red Edison can Build Battery", superBattery);
            var vinegar = new PuzzleGoal("Get Vinegar", "Vinegar", superBattery);

            var blanket = new PuzzleGoal("Build a fire in the fireplace", "The Blanket", gold);
            var cigar = new PuzzleGoal("Get exploding cigar", "Exploding Cigar Lighter Gun", blanket);
            var flagGun = new PuzzleGoal("Get into Dwayne's room", "Flag Gun", cigar);

            var secretLab1 = new PuzzleGoal("Secret Lab 1", "Help Wanted Sign", labCoat);
            var hoagie1 = new PuzzleGoal("Battery Plans 1", "Hoagie", labCoat);

            var bucketWaterBrush = new PuzzleGoal("Battery Plans 2", "Bucket, Water, Brush", franklinRoom);

            var hoagie2 = new PuzzleGoal("Battery Plans 3", "Hoagie", soap);

            var chatteringTeeth = new PuzzleGoal("Get Chattering Teeth", "Chattering Teeth", blanket);
            var patentLetter = new PuzzleGoal("Battery Plans 4", "Hoagie Patent Letter", flagGun);

            var oil = new PuzzleGoal("Battery Plans 5", "Oil", superBattery);

            var plans = new PuzzleGoal("Battery Plans 6", "The Super Battery Plans", buildBattery);            

            var wine = new PuzzleGoal("Battery Plans 7", "Wine", vinegar);

            var plansParent = new PuzzleGoal("Find Super Battery Plans", string.Empty,
                new List<PuzzleGoal> { hoagie1, bucketWaterBrush, hoagie2, patentLetter, oil, plans, wine });

            var secretLab2 = new PuzzleGoal("Secret Lab 2", "Access to Living Room", chatteringTeeth);
            var secretLab3 = new PuzzleGoal("Secret Lab 3", "Access to Secret Lab", plansParent);

            var labParent = new PuzzleGoal("Find Dr. Fred's Secret Lab", string.Empty,
                new List<PuzzleGoal> { secretLab1, secretLab2, secretLab3 });

            var begin = new PuzzleGoal("Begin Game", string.Empty, labParent);

            var start = new PuzzleStart(begin);

            var container = GraphContainer.Create();
            var graph = container.AddGraph();

            graph.Plot(start);

            return container;
        }

        private static GraphContainer CreateFullDOTTGraph()
        {
            var win = new PuzzleGoal("Win the Game");

            var tentacleBoss = new PuzzleGoal("Knock out ten Tentacles", "TentacleBoss", win);
            var bowlingBall = new PuzzleGoal("Get Bowling Ball", "Bowling Ball", tentacleBoss);
            var shrunkenKids = new PuzzleGoal("Back to the Present", "Shrunken Kids", bowlingBall);

            var poweredJohn = new PuzzleGoal("Power Hoagie's Chron-o-John", "Hoagie's Chron-o-John Powered", shrunkenKids);
            var chargedBattery = new PuzzleGoal("Charge Battery", "Charged Super Battery", poweredJohn);

            var kite = new PuzzleGoal("Get Kite", "Kite", chargedBattery);

            var labCoat = new PuzzleGoal("Get Lab Coat", "Lab Coat", kite);
            var franklinRoom = new PuzzleGoal("Start Storm", "Franklin in his Room", kite);

            var soap = new PuzzleGoal("Get Soap", "Soap", franklinRoom);

            var superBattery = new PuzzleGoal("Get the Super Battery", "Super Battery", chargedBattery);

            var gold = new PuzzleGoal("Get the Gold", "Gold", superBattery);
            var buildBattery = new PuzzleGoal("Give plans to Red Edison", "Red Edison can Build Battery", superBattery);
            var vinegar = new PuzzleGoal("Get Vinegar", "Vinegar", superBattery);

            var blanket = new PuzzleGoal("Build a fire in the fireplace", "The Blanket", gold);
            var cigar = new PuzzleGoal("Get exploding cigar", "Exploding Cigar Lighter Gun", blanket);
            var flagGun = new PuzzleGoal("Get into Dwayne's room", "Flag Gun", cigar);

            var secretLab1 = new PuzzleGoal("Secret Lab 1", "Help Wanted Sign", labCoat, hidden: true);
            var hoagie1 = new PuzzleGoal("Battery Plans 1", "Hoagie", labCoat, hidden: true);

            var bucketWaterBrush = new PuzzleGoal("Battery Plans 2", "Bucket, Water, Brush", franklinRoom, hidden: true);

            var hoagie2 = new PuzzleGoal("Battery Plans 3", "Hoagie", soap, hidden: true);

            var chatteringTeeth = new PuzzleGoal("Get Chattering Teeth", "Chattering Teeth", blanket);
            var patentLetter = new PuzzleGoal("Battery Plans 4", "Hoagie Patent Letter", flagGun, hidden: true);

            var oil = new PuzzleGoal("Battery Plans 5", "Oil", superBattery, hidden: true);

            var plans = new PuzzleGoal("Battery Plans 6", "The Super Battery Plans", buildBattery, hidden: true);

            var wine = new PuzzleGoal("Battery Plans 7", "Wine", vinegar, hidden: true);

            var plansParent = new PuzzleGoal("Find Super Battery Plans", string.Empty,
                new List<PuzzleGoal> { hoagie1, bucketWaterBrush, hoagie2, patentLetter, oil, plans, wine });

            var secretLab2 = new PuzzleGoal("Secret Lab 2", "Access to Living Room", chatteringTeeth, hidden: true);
            var secretLab3 = new PuzzleGoal("Secret Lab 3", "Access to Secret Lab", plansParent, hidden: true);

            var labParent = new PuzzleGoal("Find Dr. Fred's Secret Lab", string.Empty,
                new List<PuzzleGoal> { secretLab1, secretLab2, secretLab3 });

            var begin = new PuzzleGoal("Begin Game", string.Empty, labParent);

            var start = new PuzzleStart(begin);

            var container = GraphContainer.Create();
            var graph = container.AddGraph();

            graph.Plot(start);

            return container;
        }

        #endregion

        private static GraphContainer CreateBranchingGraph()
        {
            var final = new PuzzleGoal("Final puzzle");

            var a = new PuzzleGoal("Branch A", "Item 6", final);
            var b = new PuzzleGoal("Branch B", "Item 7", final);
            var c = new PuzzleGoal("Branch C", "Item 8", final);

            var puzzleA = new PuzzleGoal("Do a puzzle", "Item 3", a);
            var puzzleB = new PuzzleGoal("Do another puzzle", "Item 4", new List<PuzzleGoal>() { b, c });

            var d = new PuzzleGoal ("Branch D", "Item 9", final);
            var e = new PuzzleGoal ("Branch E", "Item 10", final);

            var puzzleC = new PuzzleGoal("It's another puzzle", "Item 5", new List<PuzzleGoal>() { d, e });

            var second = new PuzzleGoal("Second puzzle", "Item 2", new List<PuzzleGoal>() { puzzleA, puzzleB, puzzleC });
            var first = new PuzzleGoal("First puzzle", "Item 1", second);

            var start = new PuzzleStart(first);

            var container = GraphContainer.Create();
            var graph = container.AddGraph();

            graph.Plot(start);

            return container;
        }
    }
}
