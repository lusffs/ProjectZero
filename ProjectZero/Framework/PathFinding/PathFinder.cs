using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ProjectZero.Framework.PathFinding
{
    // Fuck mocking?
    //       |
    //       v
    public static class PathFinder
    {

        private static int _numColumnsMinusOne;
        private static int _numRowsMinusOne;
        private static bool _targetHit;
        private static Point _target;

        // Coordinates are in pairs of "Y, X", i.e. "row, column"
        //
        // Possible usage:
        // 1. Query for shortest path when player is ready to start round.
        //    2.1 If null is returned, there is no path for enemies and the mob turn can't begin
        //    2.2 else, a path of coordinates in the grid is returned
        public static List<Point> GetShortestPath(Cell[,] grid, Point start, Point target)
        {
            Setup(grid, target);

            // Reset distances to cells
            ClearMap(grid);

            CalculatePaths(grid, start.Y, start.X);

            if (PathExists(grid, target))
            {
                return GetShortestPath(grid, target);
            }

            return null;
        }

        private static void Setup(Cell[,] grid, Point target)
        {
            _target = target;
            _targetHit = false;
            _numColumnsMinusOne = grid.GetLength(1) - 1;
            _numRowsMinusOne = grid.GetLength(0) - 1;
        }


        // Could possibly be useful if run when player tries to build in a location in order to
        // prevent a complete block of the target
        public static bool PathExists(Cell[,] grid, Point start, Point target)
        {
            Setup(grid, target);

            ClearMap(grid);
            CalculatePaths(grid, start.Y, start.X);
            return PathExists(grid, target);            
        }

        private static void ClearMap(Cell[,] grid)
        {
            for (var row = 0; row <= _numRowsMinusOne; row++)
            {
                for (var col = 0; col <= _numColumnsMinusOne; col++)
                {
                    grid[row, col].DistanceFromStart = 0;
                }
            }
        }

        private static bool PathExists(Cell[,] grid, Point target)
        {
            return grid[target.Y, target.X].DistanceFromStart > 0;
        }

        private static List<Point> GetShortestPath(Cell[,] grid, Point target)
        {
            var path = new List<Point> { target };

            while (true)
            {
                var cellPosition = GetBestConnection(grid, target.Y, target.X);

                if (grid[cellPosition.Y, cellPosition.X].IsStart)
                    break;

                target = new Point(cellPosition.X, cellPosition.Y);
                path.Add(target);


            }

            path.Reverse();
            return path;
        }

        private static void CalculatePaths(Cell[,] grid, int currentRowIndex, int currentColIndex)
        {
            var cell = grid[currentRowIndex, currentColIndex];
            if (cell.IsTarget)
            {
                _targetHit = true;
            }
            if (_targetHit)
            {
                return;
            }

            // Some A* heuristics.. 
            var openNeighbours = GetNextSteps(grid, currentRowIndex, currentColIndex).ToList();
            //target
            CalcAbsDistForCells(grid, openNeighbours, _target);
            openNeighbours = openNeighbours.OrderBy(x => grid[x.Y, x.X].AbsDistFromTarget).ToList();

            foreach (var openNeighbour in openNeighbours)
            {
                grid[openNeighbour.Y, openNeighbour.X].DistanceFromStart = cell.DistanceFromStart + 1;
                CalculatePaths(grid, openNeighbour.Y, openNeighbour.X);
            }
        }

        private static void CalcAbsDistForCells(Cell[,] grid, IEnumerable<Point> neighbours, Point target)
        {
            foreach (var neighbour in neighbours)
            {
                grid[neighbour.Y, neighbour.X].AbsDistFromTarget = grid[neighbour.Y, neighbour.X].DistanceFromStart + Math.Abs(neighbour.X - target.X) + Math.Abs(neighbour.Y - target.Y);
            }
        }

        private static Point GetBestConnection(Cell[,] grid, int rowIndex, int colIndex)
        {
            var validNeighbours = new List<Point>();

            if (IsLegalStep(grid, rowIndex - 0, colIndex - 1))
            {
                validNeighbours.Add(new Point(colIndex - 1, rowIndex));
            }
            if (IsLegalStep(grid, rowIndex - 1, colIndex - 0))
            {
                validNeighbours.Add(new Point(colIndex, rowIndex - 1));
            }
            if (IsLegalStep(grid, rowIndex + 1, colIndex - 0))
            {
                validNeighbours.Add(new Point(colIndex, rowIndex + 1));
            }
            if (IsLegalStep(grid, rowIndex - 0, colIndex + 1))
            {
                validNeighbours.Add(new Point(colIndex + 1, rowIndex));
            }

            var orderedNeighbours = validNeighbours.OrderBy(x => grid[x.Y, x.X].DistanceFromStart);

            
            return orderedNeighbours.First(y => grid[y.Y, y.X].DistanceFromStart > 0 || grid[y.Y, y.X].IsStart);
            
        }

        private static IEnumerable<Point> GetNextSteps(Cell[,] grid, int rowIndex, int colIndex)
        {
            var openNeighbours = new List<Point>();

            if (ShouldMove(grid, rowIndex - 0, colIndex - 1, grid[rowIndex, colIndex].DistanceFromStart))
            {
                openNeighbours.Add(new Point(colIndex - 1, rowIndex));
            }
            if (ShouldMove(grid, rowIndex - 1, colIndex - 0, grid[rowIndex, colIndex].DistanceFromStart))
            {
                openNeighbours.Add(new Point(colIndex, rowIndex - 1));
            }
            if (ShouldMove(grid, rowIndex + 1, colIndex - 0, grid[rowIndex, colIndex].DistanceFromStart))
            {
                openNeighbours.Add(new Point(colIndex, rowIndex + 1));
            }
            if (ShouldMove(grid, rowIndex - 0, colIndex + 1, grid[rowIndex, colIndex].DistanceFromStart))
            {
                openNeighbours.Add(new Point(colIndex + 1, rowIndex));
            }

            return openNeighbours;
        }

        private static bool ShouldMove(Cell[,] grid, int row, int col, int currentDistance)
        {
            if (!IsLegalStep(grid, row, col))
                return false;

            // Target is always possible next step, starting point is never
            var cell = grid[row, col];
            if (cell.IsTarget)
                return true;
            if (cell.IsStart)
                return false;

            // Cell can be reached with new lower amount of steps
            if (IsShorterPath(currentDistance, cell))
                return true;

            return false;
        }

        private static bool IsShorterPath(int currentDistance, Cell cell)
        {
            return (cell.DistanceFromStart == 0 || cell.DistanceFromStart > currentDistance + 1);
        }

        private static bool IsLegalStep(Cell[,] grid, int row, int col)
        {
            return CellWithinGridBounds(row, col) && !grid[row, col].IsBlocked;
        }

        private static bool CellWithinGridBounds(int row, int col)
        {
            if (row < 0 || col < 0 || (col > _numColumnsMinusOne || row > _numRowsMinusOne))
                return false;
            return true;
        }
    }
}
