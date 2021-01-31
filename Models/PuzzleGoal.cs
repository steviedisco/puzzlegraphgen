using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleGraphGenerator.Models
{
    public class PuzzleGoal
    {
        private static int nextId = 0;

        public int Id { get; } = nextId += 2;

        public string Title { get; set; }

        public virtual bool IsStart { get; } = false;

        public PuzzleResult PuzzleResult { get; set; }

        public (double, double) Position { get; set; } = (0, 0);

        public PuzzleGoal(string title)
        {
            Title = title;
        }

        public PuzzleGoal(string title, string solved, PuzzleGoal nextPuzzle)
        {
            Title = title;
            PuzzleResult = new PuzzleResult { PrizeName = solved, NextPuzzle = nextPuzzle };
        }

        public PuzzleGoal(string title, string solved, List<PuzzleGoal> nextPuzzles)
        {
            Title = title;
            PuzzleResult = new PuzzleResult { PrizeName = solved, NextPuzzles = nextPuzzles };
        }
    }

    public class PuzzleStart : PuzzleGoal
    {
        public PuzzleStart(PuzzleGoal goal) : base("Start")
        {
            PuzzleResult = new PuzzleResult { NextPuzzle = goal };
        }

        public PuzzleStart(List<PuzzleGoal> goals) : base("Start")
        {
            PuzzleResult = new PuzzleResult { NextPuzzles = goals };
        }
    }
}
