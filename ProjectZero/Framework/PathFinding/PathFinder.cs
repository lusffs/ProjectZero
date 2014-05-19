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
        // Coordinates are in pairs of "Y, X", i.e. "row, column"
        //
        // Possible usage:
        // 1. Query for shortest path when player is ready to start round.
        //    2.1 If null is returned, there is no path for enemies and the mob turn can't begin
        //    2.2 else, a path of coordinates in the grid is returned
        public static List<Point> GetShortestPath(Cell[,] grid, Point start, Point target)
        {
            // Reset distances to cells
            ClearMap(grid);

            CalculatePaths(grid, start.Y, start.X);

            if (PathExists(grid, target))
            {
                return GetShortestPath(grid, target);
            }
            
            return null;
        }

        // Could possibly be useful if run when player tries to build in a location in order to
        // prevent a complete block of the target
        public static bool PathExists(Cell[,] grid, Point start, Point target)
        {
            ClearMap(grid);
            CalculatePaths(grid, start.Y, start.X);
            return PathExists(grid, target);            
        }

        private static void ClearMap(Cell[,] grid)
        {
            for (var row = 0; row < grid.GetLength(0); row++)
            {
                for (var col = 0; col < grid.GetLength(1); col++)
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

                // This row isn't really needed for anything. See Cell.IsParthOfPath obsolete message.
                grid[cellPosition.Y, cellPosition.X].IsPartOfPath = true;
            }

            path.Reverse();
            return path;
        }

        private static void CalculatePaths(Cell[,] grid, int currentRowIndex, int currentColIndex)
        {
            var cell = grid[currentRowIndex, currentColIndex];
            var openNeighbours = GetNextSteps(grid, currentRowIndex, currentColIndex);

            foreach (var openNeighbour in openNeighbours)
            {
                grid[openNeighbour.Y, openNeighbour.X].DistanceFromStart = cell.DistanceFromStart + 1;
                CalculatePaths(grid, openNeighbour.Y, openNeighbour.X);
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

            return validNeighbours.OrderBy(x => grid[x.Y, x.X].DistanceFromStart).First();
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
            return CellWithinGridBounds(grid, row, col) && !grid[row, col].IsBlocked;
        }

        private static bool CellWithinGridBounds(Cell[,] grid, int row, int col)
        {
            if (row < 0 || col < 0 || (col > grid.GetLength(1) - 1 || row > grid.GetLength(0) - 1))
                return false;
            return true;
        }
    }
}
