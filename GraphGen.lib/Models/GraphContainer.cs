using PuzzleGraphGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "GraphContainer")]
    [Serializable]
    public class GraphContainer : Section
    {
        private static Random _rng = new Random();
        private static int _maxBranches = 3;
        private static int _maxDepth = 4;
        private static int _puzzleCount = 0;
        private static int _skipOdds = 5;

        private GraphContainer()
        {
            Name = "xgml";

            AddGraphObject(Attribute.Create("Creator", "String", "yFiles"));
            AddGraphObject(Attribute.Create("Version", "String", 2.17));
        }

        public static GraphContainer Create()
        {
            return new GraphContainer();
        }

        public Graph CreateGraph(PuzzleStart puzzleStart)
        {
            return AddGraphObject(Graph.Create(puzzleStart)) as Graph;
        }

        public static GraphContainer Generate(int seed = -1, int maxDepth = 4, int maxBranches = 3, int skipOdds = 5)
        {
            seed = seed > -1 ? seed : new Random((int)System.DateTime.Now.Ticks).Next();

            _maxDepth = maxDepth;
            _maxBranches = maxBranches;
            _skipOdds = skipOdds;

            _rng = new Random(seed);

            PuzzleGoal.ResetIdCounter();

            var last = new PuzzleGoal("Last Puzzle", "", new List<PuzzleGoal>() { new PuzzleGoal("End Game") });

            var allPuzzles = GenerateGoals(nextPuzzle: last);

            var first = new PuzzleGoal("First puzzle", "", allPuzzles[allPuzzles.Count]);
            var start = new PuzzleStart(first);

            var container = Create();
            var graph = container.CreateGraph(start);

            graph.Position();
            graph.Rename();

            while (graph.Sort());
            
            graph.CompressY();
            graph.CompressX();
            graph.CompressX(-1);
            graph.Plot();

            return container;
        }

        public static string GenerateXML(int seed = -1, int maxDepth = 4, int maxBranches = 3, int skipOdds = 5)
        {
            _puzzleCount = 0;

            var graph = Generate(seed, maxDepth, maxBranches, skipOdds);

            return Serialize(graph);
        }

        private static string Serialize(GraphContainer graph)
        {
            var serializer = new XmlSerializer(typeof(GraphContainer));
            var ms = new MemoryStream();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);            

            var writer = new NoTypeXmlWriter(ms, CodePagesEncodingProvider.Instance.GetEncoding(1252))
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };

            serializer.Serialize(writer, graph);
            writer.Close();

            writer = new NoTypeXmlWriter("output.xgml", CodePagesEncodingProvider.Instance.GetEncoding(1252))
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };

            serializer.Serialize(writer, graph);
            writer.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        private static Dictionary<int, List<PuzzleGoal>> GenerateGoals(            
            int depth = 1,
            PuzzleGoal nextPuzzle = null,
            Dictionary<int, List<PuzzleGoal>> allPuzzles = null)
        {
            allPuzzles ??= new Dictionary<int, List<PuzzleGoal>>();

            if (depth > _maxDepth)
            {
                return allPuzzles;
            }

            if (!allPuzzles.ContainsKey(depth))
            {
                allPuzzles.Add(depth, new List<PuzzleGoal>());
            }

            var branches = _rng.Next(1, _maxBranches);

            for (var branch = 1; branch <= branches; branch++)
            {
                PuzzleGoal puzzle;

                if (nextPuzzle != null)
                {
                    nextPuzzle.Linked = true;
                    puzzle = new PuzzleGoal($"Puzzle {++_puzzleCount}", $"Item {_puzzleCount}", nextPuzzle);
                }
                else
                {
                    var nextPuzzles = new List<PuzzleGoal>();

                    if (depth > 1)
                    {
                        var pickRandom = _rng.Next(0, _skipOdds);

                        if (pickRandom == 0)
                        {
                            var randomDepth = _rng.Next(1, depth - 1);
                            var inBranches = _rng.Next(1, Math.Min(allPuzzles[randomDepth].Count, _maxBranches));

                            for (var inBranch = 0; inBranch < inBranches; inBranch++)
                            {
                                var randomPuzzleIndex = _rng.Next(0, allPuzzles[randomDepth].Count - 1);

                                allPuzzles[randomDepth][randomPuzzleIndex].Linked = true;
                                nextPuzzles.Add(allPuzzles[randomDepth][randomPuzzleIndex]);
                            }
                        }                        
                        else
                        {
                            var inBranches = _rng.Next(1, Math.Min(allPuzzles[depth - 1].Count, _maxBranches));

                            for (var inBranch = 0; inBranch < inBranches; inBranch++)
                            {
                                var randomPuzzleIndex = _rng.Next(0, allPuzzles[depth - 1].Count - 1);

                                allPuzzles[depth - 1][randomPuzzleIndex].Linked = true;
                                nextPuzzles.Add(allPuzzles[depth - 1][randomPuzzleIndex]);
                            }
                        }
                    }

                    puzzle = new PuzzleGoal($"Puzzle {++_puzzleCount}", $"Item {_puzzleCount}", nextPuzzles);
                }                

                allPuzzles[depth].Add(puzzle);                
            }

            allPuzzles = GenerateGoals(depth + 1, allPuzzles: allPuzzles);

            for (var i = depth - 1; i < depth && depth > 1; i++)
            {
                var unLinked = allPuzzles[i].Where(x => !x.Linked).ToList();

                foreach (var puzzle in unLinked)
                {
                    var randomParent = _rng.Next(0, allPuzzles[depth].Count - 1);
                    allPuzzles[depth][randomParent].Result.NextPuzzles.Add(puzzle);
                }                    
            }

            return allPuzzles;
        }

        #region dig example

        public static string DigExample()
        {
            var graph = CreateDigGraph();

            return Serialize(graph);
        }


        public static GraphContainer CreateDigGraph()
        {
            var end = new PuzzleGoal("End Game");

            var killGuard = new PuzzleGoal("Kill guard", "Save everyone", end);

            var openEye = new PuzzleGoal("Open eye", "Guard", killGuard);

            var completeStrangeDevice = new PuzzleGoal("Complete strange device", "Powered eye", openEye);

            var crystal = new PuzzleGoal("Crystal", "Crystal", completeStrangeDevice, hidden: true);
            var eyePart = new PuzzleGoal("Eye part", "Eye part", completeStrangeDevice, hidden: true);

            var fixCrystalMachine = new PuzzleGoal("Fix crystal machine", "", new List<PuzzleGoal> { crystal, eyePart });

            var useMapPanel = new PuzzleGoal("Use map panel", "Eye part", fixCrystalMachine);

            var persuadeCreature = new PuzzleGoal("Persuade creature", "Creator's engraving", useMapPanel);

            var reviveCreature = new PuzzleGoal("Revive creature", "", persuadeCreature);

            var completeEnergyLines = new PuzzleGoal("Complete energy lines", "Completed eye", openEye);

            var activateLightBridge = new PuzzleGoal("Activate light bridge", "Light bridge", completeEnergyLines);

            var accessCathedralSpire1 = new PuzzleGoal("Access cathedral spire", "Access cathedral spire", persuadeCreature, hidden: true);
            var accessCathedralSpire2 = new PuzzleGoal("Access cathedral spire", "Access cathedral spire", activateLightBridge, hidden: true);

            var completeAlcove = new PuzzleGoal("Complete alcove", "", new List<PuzzleGoal> { accessCathedralSpire1, accessCathedralSpire2 });

            var freeBrink = new PuzzleGoal("Free brink", "Brink freed", completeAlcove);

            var fourPlates = new PuzzleGoal("Four plates", "Four plates", completeAlcove, hidden: true);
            var brinkTrapped = new PuzzleGoal("Brink trapped", "Brink trapped", freeBrink, hidden: true);

            var collectFourPlates = new PuzzleGoal("Collect four plates", "", new List<PuzzleGoal> { fourPlates, brinkTrapped });

            var accessHiddenIsland = new PuzzleGoal("Access hidden island", "Plate", collectFourPlates);

            var crystals = new PuzzleGoal("", "Crystals", reviveCreature, hidden: true);
            var tablet = new PuzzleGoal("", "Tablet", accessHiddenIsland, hidden: true);

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

            var openMishapedDoor = new PuzzleGoal("Open mishaped door", "", new List<PuzzleGoal> { accessMapSpire1, accessMapSpire2 });

            var fixOpenDoor1 = new PuzzleGoal("", "Sceptres", alignMoons, hidden: true);
            var fixOpenDoor2 = new PuzzleGoal("", "Plate", collectFourPlates, hidden: true);
            var fixOpenDoor3 = new PuzzleGoal("", "Green rod", openMishapedDoor, hidden: true);

            var fixOpenDoor = new PuzzleGoal("Fix/Open door", "", new List<PuzzleGoal> { fixOpenDoor1, fixOpenDoor2, fixOpenDoor3 });

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

            var jumpGap = new PuzzleGoal("Jump gap", "", new List<PuzzleGoal> { jumpGap1, jumpGap2, jumpGap3, jumpGap4, jumpGap5, jumpGap6, jumpGap7 });

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

            graph.Position();
            graph.Rename();

            while (graph.Sort()) ;

            graph.CompressY();
            graph.CompressX();
            graph.CompressX(-1);
            graph.Plot();

            return container;
        }

        #endregion 
    }
}
