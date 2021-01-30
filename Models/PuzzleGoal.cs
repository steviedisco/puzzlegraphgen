using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGraphGenerator.Models
{
    public class PuzzleGoal
    {
        private static int nextId = 0;

        public int Id { get; } = nextId++;

        public string Title { get; set; }

        public virtual bool IsStart { get; } = false;

        public List<PuzzleResult> PuzzleResults { get; set; }

        public (double, double) Position { get; set; } = (0, 0);

        public PuzzleGoal()
        {
            PuzzleResults = new List<PuzzleResult>();
        }        
    }

    public class PuzzleStart : PuzzleGoal
    {
        public PuzzleStart(PuzzleGoal goal)
        {
            PuzzleResults.Clear();
            PuzzleResults.Add(new PuzzleResult { NextPuzzle = goal });
        }

        public PuzzleStart(List<PuzzleGoal> goals)
        {
            PuzzleResults.Clear();

            foreach (var goal in goals)
            {
                PuzzleResults.Add(new PuzzleResult { NextPuzzle = goal });
            }
        }
    }
}
