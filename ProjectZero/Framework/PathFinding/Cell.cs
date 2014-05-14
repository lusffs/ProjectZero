using System;

namespace ProjectZero.Framework.PathFinding
{
    public class Cell
    {
        [Obsolete("This property is only needed for pretty console printing purposes...")]
        public bool IsPartOfPath { get; set; }
        public bool IsStart { get; set; }
        public bool IsTarget { get; set; }
        public bool IsBlocked { get; set; }
        public int DistanceFromStart { get; set; }
    }
}
