using Microsoft.Xna.Framework;

namespace ProjectZero.Framework.PathFinding
{
    public class Cell
    {
        public bool IsStart { get; set; }
        public bool IsTarget { get; set; }
        public bool IsBlocked { get; set; }
        public int DistanceFromStart { get; set; }
        public int AbsDistFromTarget { get; set; }
        public Point? Previous { get; set; }
    }
}
