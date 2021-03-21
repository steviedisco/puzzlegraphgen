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
            var graph = CreateFullDigGraph(); 
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

        //private static GraphContainer CreateDOTTGraph()
        //{
        //    // work backwards from last puzzle to first
        //    var final = new PuzzleGoal("Get the Super Battery");

        //    // layer 4
        //    var gold = new PuzzleGoal("Get the gold", "Gold", final);
        //    var vinegar = new PuzzleGoal("Get Vinegar", "Vinegar", final);

        //    // layer 3
        //    var fire = new PuzzleGoal("Build a fire in the fireplace", "The Blanket", gold);
        //    var denture = new PuzzleGoal("Dentures to Laverne", "Access to History Room", vinegar);

        //    // layer 2
        //    var cigar = new PuzzleGoal("Get exploding cigar", "Exploding cigar lighter gun", fire);
        //    var laverne = new PuzzleGoal("Laverne access outside", "Access Laverne's chron-o-john", denture);

        //    // Start layer
        //    var dwayne = new PuzzleGoal("Get into Dwayne's room", "Flag Gun", cigar);
        //    var tree = new PuzzleGoal("Get Laverne down from tree", "Laverne", laverne);
        //    var getdenture = new PuzzleGoal("Get Dentures", "Dentures", denture);
        //    var makevinegar = new PuzzleGoal("Make Vinegar", "Vinegar in Time Capsule", vinegar);
        //    var redEd = new PuzzleGoal("Give plans to Red Edison", "Red Edison can build Battery", final);

        //    var start = new PuzzleStart(new List<PuzzleGoal>() { dwayne, tree, getdenture, makevinegar, redEd });

        //    var container = GraphContainer.Create();
        //    var graph = container.CreateGraph(start);

        //    graph.Plot();            

        //    return container;
        //}

        //private static GraphContainer CreateMidDOTTGraph()
        //{
        //    var win = new PuzzleGoal("Win the Game");

        //    var tentacleBoss = new PuzzleGoal("Knock out ten Tentacles", "TentacleBoss", win);
        //    var bowlingBall = new PuzzleGoal("Get Bowling Ball", "Bowling Ball", tentacleBoss);
        //    var shrunkenKids = new PuzzleGoal("Back to the Present", "Shrunken Kids", bowlingBall);

        //    var poweredJohn = new PuzzleGoal("Power Hoagie's Chron-o-John", "Hoagie's Chron-o-John Powered", shrunkenKids);
        //    var chargedBattery = new PuzzleGoal("Charge Battery", "Charged Super Battery", poweredJohn);

        //    var kite = new PuzzleGoal("Get Kite", "Kite", chargedBattery);

        //    var labCoat = new PuzzleGoal("Get Lab Coat", "Lab Coat", kite);
        //    var franklinRoom = new PuzzleGoal("Start Storm", "Franklin in his Room", kite);

        //    var soap = new PuzzleGoal("Get Soap", "Soap", franklinRoom);

        //    var superBattery = new PuzzleGoal("Get the Super Battery", "Super Battery", chargedBattery);

        //    var gold = new PuzzleGoal("Get the Gold", "Gold", superBattery);
        //    var buildBattery = new PuzzleGoal("Give plans to Red Edison", "Red Edison can Build Battery", superBattery);
        //    var vinegar = new PuzzleGoal("Get Vinegar", "Vinegar", superBattery);

        //    var blanket = new PuzzleGoal("Build a fire in the fireplace", "The Blanket", gold);
        //    var cigar = new PuzzleGoal("Get exploding cigar", "Exploding Cigar Lighter Gun", blanket);
        //    var flagGun = new PuzzleGoal("Get into Dwayne's room", "Flag Gun", cigar);

        //    var secretLab1 = new PuzzleGoal("Secret Lab 1", "Help Wanted Sign", labCoat);
        //    var hoagie1 = new PuzzleGoal("Battery Plans 1", "Hoagie", labCoat);

        //    var bucketWaterBrush = new PuzzleGoal("Battery Plans 2", "Bucket, Water, Brush", franklinRoom);

        //    var hoagie2 = new PuzzleGoal("Battery Plans 3", "Hoagie", soap);

        //    var chatteringTeeth = new PuzzleGoal("Get Chattering Teeth", "Chattering Teeth", blanket);
        //    var patentLetter = new PuzzleGoal("Battery Plans 4", "Hoagie Patent Letter", flagGun);

        //    var oil = new PuzzleGoal("Battery Plans 5", "Oil", superBattery);

        //    var plans = new PuzzleGoal("Battery Plans 6", "The Super Battery Plans", buildBattery);            

        //    var wine = new PuzzleGoal("Battery Plans 7", "Wine", vinegar);

        //    var plansParent = new PuzzleGoal("Find Super Battery Plans", string.Empty,
        //        new List<PuzzleGoal> { hoagie1, bucketWaterBrush, hoagie2, patentLetter, oil, plans, wine });

        //    var secretLab2 = new PuzzleGoal("Secret Lab 2", "Access to Living Room", chatteringTeeth);
        //    var secretLab3 = new PuzzleGoal("Secret Lab 3", "Access to Secret Lab", plansParent);

        //    var labParent = new PuzzleGoal("Find Dr. Fred's Secret Lab", string.Empty,
        //        new List<PuzzleGoal> { secretLab1, secretLab2, secretLab3 });

        //    var begin = new PuzzleGoal("Begin Game", string.Empty, labParent);

        //    var start = new PuzzleStart(begin);

        //    var container = GraphContainer.Create();
        //    var graph = container.CreateGraph(start);

        //    graph.Plot();

        //    return container;
        //}

        #endregion

        #region dig

        private static GraphContainer CreateFullDigGraph()
        {
            var end = new PuzzleGoal("End Game");

            var killGuard = new PuzzleGoal("Kill guard", "Save everyone", end);

            var openEye = new PuzzleGoal("Open eye", "Guard", killGuard);

            var completeStrangeDevice = new PuzzleGoal("Complete strange device", "Powered eye", openEye);
            
            var crystal = new PuzzleGoal("Crystal", "Crystal", completeStrangeDevice, hidden:true);
            var eyePart = new PuzzleGoal("Eye part", "Eye part", completeStrangeDevice, hidden:true);

            var fixCrystalMachine = new PuzzleGoal("Fix crystal machine", "", new List<PuzzleGoal> { crystal, eyePart });

            var useMapPanel = new PuzzleGoal("Use map panel", "Eye part", fixCrystalMachine);            

            var persuadeCreature = new PuzzleGoal("Persuade creature", "Creator's engraving", useMapPanel);

            var reviveCreature = new PuzzleGoal("Revive creature", "", persuadeCreature);

            var completeEnergyLines = new PuzzleGoal("Complete energy lines", "Completed eye", openEye);

            var activateLightBridge = new PuzzleGoal("Activate light bridge", "Light bridge", completeEnergyLines);

            var completeAlcove = new PuzzleGoal("Complete alcove", "Access cathedral spire", new List<PuzzleGoal> { persuadeCreature, activateLightBridge });

            var freeBrink = new PuzzleGoal("Free brink", "Brink freed", completeAlcove);

            var fourPlates = new PuzzleGoal("Four plates", "Four plates", completeAlcove, hidden:true);
            var brinkTrapped = new PuzzleGoal("Brink trapped", "Brink trapped", freeBrink, hidden:true);

            var collectFourPlates = new PuzzleGoal("Collect four plates", "", new List<PuzzleGoal> { fourPlates, brinkTrapped });            

            var accessHiddenIsland = new PuzzleGoal("Access hidden island", "Plate", collectFourPlates);            

            var crystals = new PuzzleGoal("", "Crystals", reviveCreature, hidden:true);
            var tablet = new PuzzleGoal("", "Tablet", accessHiddenIsland, hidden:true);

            var saveRobbins = new PuzzleGoal("Save robbins", "", new List<PuzzleGoal> { crystals, tablet });

            var distractMonster = new PuzzleGoal("Distract monster", "Rock by waterfall", saveRobbins);

            var recruitBrink = new PuzzleGoal("Recruit brink", "Brink in position", distractMonster);

            var distractBrink = new PuzzleGoal("Distract brink", "Stolen crystals", recruitBrink);

            var reviveCreature2 = new PuzzleGoal("Revive creature", "Flashlight", distractBrink);

            var openPyramid = new PuzzleGoal("Open pyramid", "Crystal", reviveCreature2);

            var openDoor = new PuzzleGoal("Open door", "Access door", openPyramid);

            var killGuard2 = new PuzzleGoal("Kill guard", "Access door", openDoor);

            var moveSlab = new PuzzleGoal("Move slab", "Crystal", killGuard2);

            var revealPassage = new PuzzleGoal("Reveal passage", "", moveSlab);

            var mapPanel = new PuzzleGoal("Map panel", "Discover secret under tomb", moveSlab);

            var activateLightBridge2 = new PuzzleGoal("Activate light bridge", "Light bridge", completeEnergyLines);

            var turnOnLights = new PuzzleGoal("Turn on lights", "Light source under earth", revealPassage);

            var openShutter = new PuzzleGoal("Open shutter", "Let light through", revealPassage);

            var alignMoons = new PuzzleGoal("Align moons", "Light slab", moveSlab);

            var accessMapSpire1 = new PuzzleGoal("", "Access map spire", mapPanel, hidden: true);
            var accessMapSpire2 = new PuzzleGoal("", "Access map spire", activateLightBridge2, hidden: true);

            var openMishapedDoor = new PuzzleGoal("Open mishaped door", "", new List<PuzzleGoal> { accessMapSpire1, accessMapSpire2});

            var fixOpenDoor1 = new PuzzleGoal("", "Sceptres", alignMoons, hidden: true);
            var fixOpenDoor2 = new PuzzleGoal("", "Plate", collectFourPlates, hidden: true);
            var fixOpenDoor3 = new PuzzleGoal("", "Green rod", openMishapedDoor, hidden: true);

            var fixOpenDoor = new PuzzleGoal("Fix/Open door", "Plate", new List<PuzzleGoal> { fixOpenDoor1, fixOpenDoor2, fixOpenDoor3 });

            var findRatsLair = new PuzzleGoal("Find rat's lair", "Door piece", fixOpenDoor);

            var bugRat = new PuzzleGoal("Bug rat", "Alien device", findRatsLair);

            var fashionTrap = new PuzzleGoal("Fashion trap", "Rat", bugRat);

            var activateLightBridge3 = new PuzzleGoal("Activate light bridge", "Light bridge", completeEnergyLines);

            var discoverChamber = new PuzzleGoal("Discover chamber", "", new List<PuzzleGoal> { turnOnLights, openShutter });

            var digOutPlate = new PuzzleGoal("Dig out plate", "", discoverChamber);

            var shovel = new PuzzleGoal("Shovel", "Shovel", digOutPlate, hidden: true);
            var accessLightBridge = new PuzzleGoal("Access light bridge", "Access light bridge", activateLightBridge2, hidden: true);

            var accessTombSpire = new PuzzleGoal("Access tomb spire", "", new List<PuzzleGoal> { shovel, accessLightBridge });

            var openStarDoor = new PuzzleGoal("Open Star door", "Access tram", accessTombSpire);

            var energiseCrystals = new PuzzleGoal("Energise crystals", "Power tram", accessTombSpire);

            var passAirlock = new PuzzleGoal("Pass airlock", "Access command centre", energiseCrystals);

            var openReturnPath = new PuzzleGoal("Open return path");

            var jumpGap1 = new PuzzleGoal("", "Blue rod", energiseCrystals, hidden: true);
            var jumpGap2 = new PuzzleGoal("", "Rod", openShutter, hidden: true);
            var jumpGap3 = new PuzzleGoal("", "Shovel", openReturnPath, hidden: true);
            var jumpGap4 = new PuzzleGoal("", "Panel cover", fixOpenDoor, hidden: true);
            var jumpGap5 = new PuzzleGoal("", "Access cave", findRatsLair, hidden: true);
            var jumpGap6 = new PuzzleGoal("", "Trap parts", fashionTrap, hidden: true);
            var jumpGap7 = new PuzzleGoal("", "Access lightbridge", activateLightBridge3, hidden: true);

            var jumpGap = new PuzzleGoal("Jump gap", "Panel cover", new List<PuzzleGoal> { jumpGap1, jumpGap2, jumpGap3, jumpGap4, jumpGap5, jumpGap6, jumpGap7 });

            var openDiamondDoor = new PuzzleGoal("Open diamond door", "Access planetarium spire", jumpGap);

            var orangeRod = new PuzzleGoal("", "Orange rod", openDiamondDoor, hidden: true);
            var plate2 = new PuzzleGoal("", "Plate", collectFourPlates, hidden: true);

            var killMonsterToAccessWater = new PuzzleGoal("Kill monster to access water", "", new List<PuzzleGoal> { orangeRod, plate2 });

            var reviveBoobyTrapTurtle = new PuzzleGoal("Revive/boobytrap turtle", "Boobytrapped turtle", killMonsterToAccessWater);

            var openWeakenedDoor = new PuzzleGoal("Open weakened door", "Canister", reviveBoobyTrapTurtle);

            var reviveBrink = new PuzzleGoal("Revive Brink", "", openWeakenedDoor);

            var activateLightBridge4 = new PuzzleGoal("Activate light bridge", "Light bridge", completeEnergyLines);

            var openTriangleDoor1 = new PuzzleGoal("", "Red rod", openStarDoor, hidden: true);
            var openTriangleDoor2 = new PuzzleGoal("", "Red rod", mapPanel, hidden: true);
            var openTriangleDoor3 = new PuzzleGoal("", "Glowing crystals", reviveBrink, hidden: true);
            var openTriangleDoor4 = new PuzzleGoal("", "Fossil", reviveBoobyTrapTurtle, hidden: true);
            var openTriangleDoor5 = new PuzzleGoal("", "Access museum spire", activateLightBridge4, hidden: true);

            var openTriangleDoor = new PuzzleGoal("Jump gap", "Panel cover", new List<PuzzleGoal> { openTriangleDoor1, openTriangleDoor2, openTriangleDoor3, openTriangleDoor4, openTriangleDoor5 });

            var powerMishapedDoorPanel = new PuzzleGoal("Power mishaped door panel", "Powered door", openMishapedDoor);

            var restorePower1 = new PuzzleGoal("", "Power doors", openTriangleDoor, hidden: true);
            var restorePower2 = new PuzzleGoal("", "Revealed power line", powerMishapedDoorPanel, hidden: true);

            var restorePower = new PuzzleGoal("Restore Power", "", new List<PuzzleGoal> { restorePower1, restorePower2 });

            var removeDoorPanel = new PuzzleGoal("Remove door panel", "Broken controls", powerMishapedDoorPanel);

            var plate1 = new PuzzleGoal("Plate", "Plate", collectFourPlates, hidden: true);
            var blueCrystal = new PuzzleGoal("Blue crystal", "Blue crystal", turnOnLights, hidden: true);
            var accessTunnel = new PuzzleGoal("Access tunnel", "Access tunnel", passAirlock, hidden: true);
            var purpleRod = new PuzzleGoal("Purple rod", "Purple rod", openTriangleDoor, hidden: true);
            var accessPowerSource = new PuzzleGoal("Access power source", "Access power source", restorePower, hidden: true);
            var accessNexus = new PuzzleGoal("Access nexus", "Access nexus", removeDoorPanel, hidden: true);

            var findNexus = new PuzzleGoal("Find Nexus", "", new List<PuzzleGoal> { plate1, blueCrystal, accessTunnel, purpleRod, accessPowerSource, accessNexus }); 

            var goldRod = new PuzzleGoal("Gold rod", "Gold rod", openPyramid, hidden: true);
            var goldEngravedRod = new PuzzleGoal("Gold engraved rod", "Gold engraved rod", openDoor, hidden: true);
            var hole = new PuzzleGoal("Hole", "Hole", findNexus, hidden: true);
            var wire = new PuzzleGoal("Wire", "Wire", powerMishapedDoorPanel, hidden: true);

            var pullWire = new PuzzleGoal("Pull wire", "", new List<PuzzleGoal> { goldRod, goldEngravedRod, hole, wire });

            var findSmallMound = new PuzzleGoal("Find small mound", "Bracelet", bugRat);

            var tusk = new PuzzleGoal("Tusk", "Tusk", removeDoorPanel, hidden: true);
            var jawBone = new PuzzleGoal("Jawbone", "Jawbone", freeBrink, hidden: true);

            var digGrave = new PuzzleGoal("Dig grave", "", new List<PuzzleGoal> { tusk, jawBone }); 

            var accessWreck = new PuzzleGoal("Access wreck", "Access wreck", pullWire, hidden: true);
            var alienDevice = new PuzzleGoal("Alien device", "Alien device", findSmallMound, hidden: true);
            var shovel2 = new PuzzleGoal("Shovel", "Shovel", digGrave, hidden: true);

            var travelToAlienPlanet = new PuzzleGoal("Travel to alien planet", "", new List<PuzzleGoal> { accessWreck, alienDevice, shovel2 }); 

            var accessAsteroidInterior = new PuzzleGoal("Access asteroid interior", "Metal plates", travelToAlienPlanet);

            var plantBombAlpha = new PuzzleGoal("Plant bomb alpha", "", accessAsteroidInterior);
            var plantBombBeta = new PuzzleGoal("Plant bomb beta", "", accessAsteroidInterior);

            var releasePigA1 = new PuzzleGoal("", "Bomb", plantBombAlpha, hidden: true);
            var releasePigA2 = new PuzzleGoal("", "Digger", plantBombAlpha, hidden: true);
            var releasePigA3 = new PuzzleGoal("", "Arming key", plantBombAlpha, hidden: true);

            var releasePigB1 = new PuzzleGoal("", "Bomb", plantBombBeta, hidden: true);
            var releasePigB2 = new PuzzleGoal("", "Shovel", plantBombBeta, hidden: true);
            var releasePigB3 = new PuzzleGoal("", "Arming key", plantBombBeta, hidden: true);

            var releasePig = new PuzzleGoal("Release pig", "", new List<PuzzleGoal> { releasePigA1, releasePigA2, releasePigA3, releasePigB1, releasePigB2, releasePigB3 });


            var theDigBegins = new PuzzleGoal("The Dig begins", "", releasePig);

            var start = new PuzzleStart(theDigBegins);

            var container = GraphContainer.Create();
            var graph = container.CreateGraph(start);

            graph.Plot();

            return container;
        }

        #endregion

        //private static GraphContainer CreateBranchingGraph()
        //{
        //    var final = new PuzzleGoal("Final puzzle");

        //    var a = new PuzzleGoal("Branch A", "Item 6", final);
        //    var b = new PuzzleGoal("Branch B", "Item 7", final);
        //    var c = new PuzzleGoal("Branch C", "Item 8", final);

        //    var puzzleA = new PuzzleGoal("Do a puzzle", "Item 3", a);
        //    var puzzleB = new PuzzleGoal("Do another puzzle", "Item 4", new List<PuzzleGoal>() { b, c });

        //    var d = new PuzzleGoal ("Branch D", "Item 9", final);
        //    var e = new PuzzleGoal ("Branch E", "Item 10", final);

        //    var puzzleC = new PuzzleGoal("It's another puzzle", "Item 5", new List<PuzzleGoal>() { d, e });

        //    var second = new PuzzleGoal("Second puzzle", "Item 2", new List<PuzzleGoal>() { puzzleA, puzzleB, puzzleC });
        //    var first = new PuzzleGoal("First puzzle", "Item 1", second);

        //    var start = new PuzzleStart(first);

        //    var container = GraphContainer.Create();
        //    var graph = container.CreateGraph(start);

        //    graph.Plot();

        //    return container;
        //}
    }
}
