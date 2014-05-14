using System;
using System.Collections.Generic;
using System.Linq;

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
        public static List<Tuple<int, int>> GetShortestPath(Cell[,] grid, Tuple<int, int> start, Tuple<int, int> target)
        {
            CalculatePaths(grid, start.Item1, start.Item2);

            if (PathExists(grid, target))
            {
                return GetShortestPath(grid, target);
            }
            
            return null;
        }

        // Could possibly be useful if run when player tries to build in a location in order to
        // prevent a complete block of the target
        public static bool PathExists(Cell[,] grid, Tuple<int, int> start, Tuple<int, int> target)
        {
            CalculatePaths(grid, start.Item1, start.Item2);
            return PathExists(grid, target);            
        }

        private static bool PathExists(Cell[,] grid, Tuple<int, int> target)
        {
            return grid[target.Item1, target.Item2].DistanceFromStart > 0;
        }

        private static List<Tuple<int, int>> GetShortestPath(Cell[,] grid, Tuple<int, int> target)
        {
            var path = new List<Tuple<int, int>> { target };

            while (true)
            {
                var cellPosition = GetBestConnection(grid, target.Item1, target.Item2);

                if (grid[cellPosition.Item1, cellPosition.Item2].IsStart)
                    break;

                target = new Tuple<int, int>(cellPosition.Item1, cellPosition.Item2);
                path.Add(target);

                // This row isn't really needed for anything. See Cell.IsParthOfPath obsolete message.
                grid[cellPosition.Item1, cellPosition.Item2].IsPartOfPath = true;
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
                grid[openNeighbour.Item1, openNeighbour.Item2].DistanceFromStart = cell.DistanceFromStart + 1;
                CalculatePaths(grid, openNeighbour.Item1, openNeighbour.Item2);
            }
        }

        private static Tuple<int, int> GetBestConnection(Cell[,] grid, int rowIndex, int colIndex)
        {
            var validNeighbours = new List<Tuple<int, int>>();

            if (IsLegalStep(grid, rowIndex - 0, colIndex - 1))
            {
                validNeighbours.Add(new Tuple<int, int>(rowIndex, colIndex - 1));
            }
            if (IsLegalStep(grid, rowIndex - 1, colIndex - 0))
            {
                validNeighbours.Add(new Tuple<int, int>(rowIndex - 1, colIndex));
            }
            if (IsLegalStep(grid, rowIndex + 1, colIndex - 0))
            {
                validNeighbours.Add(new Tuple<int, int>(rowIndex + 1, colIndex));
            }
            if (IsLegalStep(grid, rowIndex - 0, colIndex + 1))
            {
                validNeighbours.Add(new Tuple<int, int>(rowIndex, colIndex + 1));
            }

            return validNeighbours.OrderBy(x => grid[x.Item1, x.Item2].DistanceFromStart).First();
        }

        private static IEnumerable<Tuple<int, int>> GetNextSteps(Cell[,] grid, int rowIndex, int colIndex)
        {
            var openNeighbours = new List<Tuple<int, int>>();

            if (ShouldMove(grid, rowIndex - 0, colIndex - 1, grid[rowIndex, colIndex].DistanceFromStart))
            {
                openNeighbours.Add(new Tuple<int, int>(rowIndex, colIndex - 1));
            }
            if (ShouldMove(grid, rowIndex - 1, colIndex - 0, grid[rowIndex, colIndex].DistanceFromStart))
            {
                openNeighbours.Add(new Tuple<int, int>(rowIndex - 1, colIndex));
            }
            if (ShouldMove(grid, rowIndex + 1, colIndex - 0, grid[rowIndex, colIndex].DistanceFromStart))
            {
                openNeighbours.Add(new Tuple<int, int>(rowIndex + 1, colIndex));
            }
            if (ShouldMove(grid, rowIndex - 0, colIndex + 1, grid[rowIndex, colIndex].DistanceFromStart))
            {
                openNeighbours.Add(new Tuple<int, int>(rowIndex, colIndex + 1));
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
