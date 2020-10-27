using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGraphGenerator.Models
{
    public class PuzzleGoal
    {
        public int Id { get; set; } = 0;

        public string Title { get; set; }

        public virtual bool IsStart { get; } = false;

        public List<PuzzleResult> PuzzleResults { get; set; }

        private static int nextId = 0;

        public PuzzleGoal()
        {
            PuzzleResults = new List<PuzzleResult>();
        }

        public static int GetNextId()
        {
            nextId++;
            return nextId;
        }
    }

    public class PuzzleStart : PuzzleGoal
    {
        public override bool IsStart { get; } = true;
    }
}
