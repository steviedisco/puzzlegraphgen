using System;
using System.Collections.Generic;
using System.Text;

namespace PuzzleGraphGenerator.Models
{
    public class PuzzleResult
    {
        public string Name { get; set; }

        public PuzzleGoal NextPuzzle { get; set; }

        public bool IsStart { get; set; } = false;
    }

    public class KeyDialog : PuzzleResult { }
    public class KeyItem : PuzzleResult { }
    public class KeyEvent : PuzzleResult { }
}
