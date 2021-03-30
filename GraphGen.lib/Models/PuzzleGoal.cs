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

        public bool Hidden { get; set; } = false;

        public bool Sorted { get; set; } = false;

        public bool Linked { get; set; } = false;

        public bool Shifted { get; set; } = false;

        public PuzzleGoal(string title, bool hidden = false)
        {
            Title = title;
            Hidden = hidden;
        }

        public PuzzleGoal(string title, string solved, PuzzleGoal nextPuzzle, bool hidden = false) : this(title, hidden)
        {
            Result = new PuzzleResult { PrizeName = solved, NextPuzzle = nextPuzzle };
        }

        public PuzzleGoal(string title, string solved, List<PuzzleGoal> nextPuzzles, bool hidden = false) : this(title, hidden)
        {
            Result = new PuzzleResult { PrizeName = string.Empty, NextPuzzles = nextPuzzles };
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
