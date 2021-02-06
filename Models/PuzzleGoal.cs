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

        public PuzzleResult Result { get; set; }

        public (double, double) Position { get; set; } = (0, 0);

        public PuzzleGoal(string title)
        {
            Title = title;
        }

        public PuzzleGoal(string title, string solved, PuzzleGoal nextPuzzle)
        {
            Title = title;
            Result = new PuzzleResult { PrizeName = solved, NextPuzzle = nextPuzzle };
        }

        public PuzzleGoal(string title, string solved, List<PuzzleGoal> nextPuzzles)
        {
            Title = title;
            Result = new PuzzleResult { PrizeName = solved, NextPuzzles = nextPuzzles };
        }
    }

    public class PuzzleStart : PuzzleGoal
    {
        public PuzzleStart(PuzzleGoal goal) : base("Start")
        {
            Result = new PuzzleResult { NextPuzzle = goal };
        }

        public PuzzleStart(List<PuzzleGoal> goals) : base("Start")
        {
            Result = new PuzzleResult { NextPuzzles = goals };
        }
    }
}
