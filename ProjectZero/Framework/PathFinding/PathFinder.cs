using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectZero.GameSystem;

namespace ProjectZero.Framework.PathFinding
{
    public static class PathFinder
    {
        private static Cell[,] _grid;
        private static Point _target;
        private static Point _start;
        private static int _gridRowsMinusOne;
        private static int _gridColsMinusOne;

        public static List<Point> GetPath(Cell[,] grid, Point start, Point target)
        {            
            Setup(grid, start, target);
            var path = CalculatePath();
            return path;
        }

        public static bool PathExists(Cell[,] grid, Point start, Point target)
        {
            Setup(grid, start, target);
            var path = CalculatePath();
            return (path != null && path.Count > 0);
        }

        private static void Setup(Cell[,] grid, Point start, Point target)
        {
            _grid = grid;
            _target = target;
            _start = start;
            _gridRowsMinusOne = Map.Rows - 1;
            _gridColsMinusOne = Map.Columns - 1;

            for (var row = 0; row <= _gridRowsMinusOne; row++)
            {
                for (var col = 0; col <= _gridColsMinusOne; col++)
                {
                    grid[row, col].DistanceFromStart = 0;
                }
            }
        }

        private static List<Point> CalculatePath()
        {
            var reachable = new List<Point> { _start };
            var explored = new List<Cell>();

            while (reachable.Count > 0)
            {
                // Choose some node we know how to reach.
                var node = ChooseNode(reachable);

                // Target hit, return!
                if (_grid[node.Y, node.X].IsTarget)
                {
                    return BuildPath();
                }

                // Been there done that..
                reachable.Remove(reachable.First(n => n.X == node.X && n.Y == node.Y));
                explored.Add(_grid[node.Y, node.X]);

                // Where can we get from here that we haven't explored before?                
                var newReachable = GetNeighbours(node).Where(point => !explored.Contains(_grid[point.Y, point.X])).ToList();

                foreach (var point in newReachable)
                {
                    if (reachable.Count(p => p.X == point.X && p.Y == point.Y) == 0)
                    {
                        reachable.Add(new Point(point.X, point.Y));
                    }

                    if (_grid[node.Y, node.X].DistanceFromStart + 1 < _grid[point.Y, point.X].DistanceFromStart ||
                        _grid[point.Y, point.X].DistanceFromStart == 0)
                    {
                        _grid[point.Y, point.X].Previous = node;
                        _grid[point.Y, point.X].DistanceFromStart = _grid[node.Y, node.X].DistanceFromStart + 1;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<Point> GetNeighbours(Point node)
        {
            var neighbours = new List<Point>();
            var neighbour = new Point(node.X + 1, node.Y);
            if (IsLegalStep(neighbour))
            {
                neighbours.Add(new Point(neighbour.X, neighbour.Y));
            }
            neighbour = new Point(node.X - 1, node.Y);
            if (IsLegalStep(neighbour))
            {
                neighbours.Add(new Point(neighbour.X, neighbour.Y));
            }
            neighbour = new Point(node.X, node.Y + 1);
            if (IsLegalStep(neighbour))
            {
                neighbours.Add(new Point(neighbour.X, neighbour.Y));
            }
            neighbour = new Point(node.X, node.Y - 1);
            if (IsLegalStep(neighbour))
            {
                neighbours.Add(new Point(neighbour.X, neighbour.Y));
            }

            return neighbours;
        }

        private static Point ChooseNode(IEnumerable<Point> reachable)
        {
            var minCost = int.MaxValue;
            Point? bestNode = null;
            
            foreach (var node in reachable)
            {
                var costForNode = _grid[node.Y, node.X].DistanceFromStart;
                var costNodeToGoal = DistanceBetweenPoints(new Point(node.X, node.Y), _target);
                var totalCost = costForNode + costNodeToGoal;

                if (minCost > totalCost)
                {
                    minCost = totalCost;
                    bestNode = new Point(node.X, node.Y);
                }
            }
            if (!bestNode.HasValue)
            {
                throw new NoNullAllowedException("best node returned as null, not valid...");                
            }
            return bestNode.Value;
        }

        private static List<Point> BuildPath()
        {
            var path = new List<Point>();
            var nodeInPath = new Point(_target.X, _target.Y);
           
            while (true)
            {
                path.Add(nodeInPath);
                
                if (_grid[nodeInPath.Y, nodeInPath.X].IsStart)
                {
                    break;
                }
                nodeInPath = _grid[nodeInPath.Y, nodeInPath.X].Previous;
            }
            path.Reverse();

            return path;
        }

        private static int DistanceBetweenPoints(Point a, Point b)
        {
            return Math.Abs(a.Y - b.Y) + Math.Abs(a.X - b.X);
        }

        private static bool IsLegalStep(Point node)
        {
            return CellWithinGridBounds(node) && !_grid[node.Y, node.X].IsBlocked;
        }

        private static bool CellWithinGridBounds(Point node)
        {
            if (node.Y < 0 || node.X < 0 || (node.Y > _gridRowsMinusOne || node.X > _gridColsMinusOne))
                return false;
            return true;
        }
    }
}
