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
        public override bool IsStart { get; } = true;
    }
}
