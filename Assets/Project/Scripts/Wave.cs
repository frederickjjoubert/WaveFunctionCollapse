using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts
{
    public class Wave
    {

        private int MaxIterations = 1000;

        public SuperPosition[,] superPositions;

        public bool IsCollapsed()
        {
            for (int i = 0; i < superPositions.GetLength(0); i++)
            {
                for (int j = 0; j < superPositions.GetLength(1); j++)
                {
                    SuperPosition superPosition = superPositions[i, j];
                    if (!superPosition.IsCollapsed())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsInvalid()
        {
            for (int i = 0; i < superPositions.GetLength(0); i++)
            {
                for (int j = 0; j < superPositions.GetLength(1); j++)
                {
                    SuperPosition superPosition = superPositions[i, j];
                    if (superPosition.IsInvalid())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public (int, int) GetLowestEntropyCoordinates()
        {
            float lowestEntropy = float.MaxValue;
            int x = 0;
            int y = 0;
            for (int i = 0; i < superPositions.GetLength(0); i++)
            {
                for (int j = 0; j < superPositions.GetLength(1); j++)
                {
                    SuperPosition superPosition = superPositions[i, j];
                    if (superPosition.IsCollapsed()) continue;
                    if (superPosition.Entropy() < lowestEntropy)
                    {
                        lowestEntropy = superPosition.Entropy();
                        x = i;
                        y = j;
                    }
                }
            }

            return (x, y);
        }

        public void Collapse(int x, int y)
        {
            superPositions[x, y].Collapse();
        }

        public bool Propagate(int x, int y)
        {
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((x, y));
            int iMax = superPositions.GetLength(0);
            int jMax = superPositions.GetLength(1);
            int iterations = 0;
            while (stack.Count > 0 && iterations < MaxIterations)
            {
                iterations++;
                (int, int) coordinates = stack.Pop();
                int i = coordinates.Item1;
                int j = coordinates.Item2;
                // Check Each Tile Super Position Possible Neighbors
                List<Square> squares = superPositions[x, y].squares;
                if (squares.Count == 0) return false;

                List<Square> allValidUpNeighbors = new List<Square>();
                List<Square> allValidDownNeighbors = new List<Square>();
                List<Square> allValidLeftNeighbors = new List<Square>();
                List<Square> allValidRightNeighbors = new List<Square>();

                foreach (Square square in squares)
                {
                    // Get the Valid Neighbors that are allowed above the current square.
                    List<Square> validUpNeighbors = square.constrainedNeighbors[Direction.pY];
                    foreach (Square validNeighbor in validUpNeighbors)
                    {
                        if (!allValidUpNeighbors.Contains(validNeighbor))
                        {
                            allValidUpNeighbors.Add(validNeighbor);
                        }
                    }
                    // Get the Valid Neighbors that are allowed below the current square.
                    List<Square> validDownNeighbors = square.constrainedNeighbors[Direction.nY];
                    foreach (Square validNeighbor in validDownNeighbors)
                    {
                        if (!allValidDownNeighbors.Contains(validNeighbor))
                        {
                            allValidDownNeighbors.Add(validNeighbor);
                        }
                    }
                    // Get the Valid Neighbors that are allowed to the left of the current square.
                    List<Square> validLeftNeighbors = square.constrainedNeighbors[Direction.nX];
                    foreach (Square validNeighbor in validLeftNeighbors)
                    {
                        if (!allValidLeftNeighbors.Contains(validNeighbor))
                        {
                            allValidLeftNeighbors.Add(validNeighbor);
                        }
                    }
                    // Get the Valid Neighbors that are allowed to the right of the current square.
                    List<Square> validRightNeighbors = square.constrainedNeighbors[Direction.pX];
                    foreach (Square validNeighbor in validRightNeighbors)
                    {
                        if (!allValidRightNeighbors.Contains(validNeighbor))
                        {
                            allValidRightNeighbors.Add(validNeighbor);
                        }
                    }
                }

                // Check Up
                if (j < jMax - 1)
                {
                    (int, int) neighborsCoordinates = (i, j + 1);
                    int nI = neighborsCoordinates.Item1;
                    int nJ = neighborsCoordinates.Item2;
                    List<Square> neighboringSquares = superPositions[nI, nJ].squares;
                    List<Square> constrainedNeighboringSquares = Constrain(allValidUpNeighbors, neighboringSquares);
                    if (constrainedNeighboringSquares.Count == 0) return false;
                    if (neighboringSquares.Count != constrainedNeighboringSquares.Count)
                    {
                        if (!stack.Contains(neighborsCoordinates)) stack.Push(neighborsCoordinates);
                        superPositions[nI, nJ].squares = constrainedNeighboringSquares;
                    }
                }

                // Check Down
                if (j > 0)
                {
                    (int, int) neighborsCoordinates = (i, j - 1);
                    int nI = neighborsCoordinates.Item1;
                    int nJ = neighborsCoordinates.Item2;
                    List<Square> neighboringSquares = superPositions[nI, nJ].squares;
                    List<Square> constrainedNeighboringSquares = Constrain(allValidDownNeighbors, neighboringSquares);
                    if (constrainedNeighboringSquares.Count == 0) return false;
                    if (neighboringSquares.Count != constrainedNeighboringSquares.Count)
                    {
                        if (!stack.Contains(neighborsCoordinates)) stack.Push(neighborsCoordinates);
                        superPositions[nI, nJ].squares = constrainedNeighboringSquares;
                    }
                }

                // Check Left
                if (i > 0)
                {
                    (int, int) neighborsCoordinates = (i - 1, j);
                    int nI = neighborsCoordinates.Item1;
                    int nJ = neighborsCoordinates.Item2;
                    List<Square> neighboringSquares = superPositions[nI, nJ].squares;
                    List<Square> constrainedNeighboringSquares = Constrain(allValidLeftNeighbors, neighboringSquares);
                    if (constrainedNeighboringSquares.Count == 0) return false;
                    if (neighboringSquares.Count != constrainedNeighboringSquares.Count)
                    {
                        if (!stack.Contains(neighborsCoordinates)) stack.Push(neighborsCoordinates);
                        superPositions[nI, nJ].squares = constrainedNeighboringSquares;
                    }
                }

                // Check Right
                if (i < iMax - 1)
                {
                    (int, int) neighborsCoordinates = (i + 1, j);
                    int nI = neighborsCoordinates.Item1;
                    int nJ = neighborsCoordinates.Item2;
                    List<Square> neighboringSquares = superPositions[nI, nJ].squares;
                    List<Square> constrainedNeighboringSquares = Constrain(allValidRightNeighbors, neighboringSquares);
                    if (constrainedNeighboringSquares.Count == 0) return false;
                    if (neighboringSquares.Count != constrainedNeighboringSquares.Count)
                    {
                        if (!stack.Contains(neighborsCoordinates)) stack.Push(neighborsCoordinates);
                        superPositions[nI, nJ].squares = constrainedNeighboringSquares;
                    }
                }
            }

            return true;
        }

        private List<Square> Constrain(List<Square> validNeighbors, List<Square> currentNeighbors)
        {
            List<Square> constrainedSquares = new List<Square>();
            foreach (Square neighbor in currentNeighbors)
            {
                if (validNeighbors.Contains(neighbor))
                {
                    constrainedSquares.Add(neighbor);
                }
            }

            if (constrainedSquares.Count == 0)
            {
                Debug.Log("Constrained Squares is Empty!!!");
            }

            return constrainedSquares;
        }
    }
}