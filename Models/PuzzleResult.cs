using System.Collections.Generic;

namespace PuzzleGraphGenerator.Models
{
    public class PuzzleResult
    {
        public string Name { get; set; }

        public PuzzleGoal NextPuzzle 
        { 
            set
            {
                NextPuzzles.Clear();
                NextPuzzles.Add(value);
            }
        }

        public List<PuzzleGoal> NextPuzzles { get; set; } = new List<PuzzleGoal>();
    }

    public class KeyDialog : PuzzleResult { }
    public class KeyItem : PuzzleResult { }
    public class KeyEvent : PuzzleResult { }
}
