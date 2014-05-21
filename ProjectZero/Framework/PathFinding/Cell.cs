using System;

namespace ProjectZero.Framework.PathFinding
{
    public class Cell
    {
        public bool IsStart { get; set; }
        public bool IsTarget { get; set; }
        public bool IsBlocked { get; set; }
        public int DistanceFromStart { get; set; }
        public decimal AbsDistFromTarget { get; set; }
    }
}
