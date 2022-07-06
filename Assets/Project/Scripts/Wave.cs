using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts
{
    public class Wave
    {
        #region Events

        public Action OnRequestDraw;

        #endregion Events

        #region Attributes

        public SuperPosition[,] SuperPositions;

        #endregion Attributes

        #region Public Functions

        public bool IsCollapsed()
        {
            for (int i = 0; i < SuperPositions.GetLength(0); i++)
            {
                for (int j = 0; j < SuperPositions.GetLength(1); j++)
                {
                    SuperPosition superPosition = SuperPositions[i, j];
                    if (!superPosition.IsCollapsed)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsValid()
        {
            for (int i = 0; i < SuperPositions.GetLength(0); i++)
            {
                for (int j = 0; j < SuperPositions.GetLength(1); j++)
                {
                    SuperPosition superPosition = SuperPositions[i, j];
                    if (!superPosition.IsValid)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public (int, int) GetLowestEntropyCoordinates()
        {
            float lowestEntropy = float.MaxValue;

            List<(int, int)> coordinates = new List<(int, int)>();

            for (int i = 0; i < SuperPositions.GetLength(0); i++)
            {
                for (int j = 0; j < SuperPositions.GetLength(1); j++)
                {
                    SuperPosition superPosition = SuperPositions[i, j];
                    if (superPosition.IsCollapsed) continue;
                    if (superPosition.Entropy == lowestEntropy)
                    {
                        coordinates.Add((i, j));
                    }
                    else if (superPosition.Entropy < lowestEntropy)
                    {
                        coordinates.Clear();
                        coordinates.Add((i, j));
                        lowestEntropy = superPosition.Entropy;
                    }
                }
            }

            int index = UnityEngine.Random.Range(0, coordinates.Count - 1);
            return coordinates[index];
        }

        public void Collapse(int x, int y)
        {
            SuperPositions[x, y].Collapse();
        }

        public bool Propagate(int x, int y)
        {
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((x, y));
            int iMax = SuperPositions.GetLength(0);
            int jMax = SuperPositions.GetLength(1);
            while (stack.Count > 0)
            {
                (int, int) coordinates = stack.Pop();
                int i = coordinates.Item1;
                int j = coordinates.Item2;
                HashSet<Square> squares = SuperPositions[i, j].Squares;
                if (squares.Count == 0) return false;

                HashSet<Square> pYAllValidNeighbors = new HashSet<Square>();
                HashSet<Square> nYAllValidNeighbors = new HashSet<Square>();
                HashSet<Square> pXAllValidNeighbors = new HashSet<Square>();
                HashSet<Square> nXAllValidNeighbors = new HashSet<Square>();

                foreach (Square square in squares)
                {
                    // Get the Valid Neighbors that are allowed above the current square.
                    HashSet<Square> pYValidNeighbors = new HashSet<Square>(square.PyValidNeighbors);
                    foreach (Square validNeighbor in pYValidNeighbors)
                    {
                        pYAllValidNeighbors.Add(validNeighbor);
                    }

                    // Get the Valid Neighbors that are allowed below the current square.
                    HashSet<Square> validDownNeighbors = new HashSet<Square>(square.NyValidNeighbors);
                    foreach (Square validNeighbor in validDownNeighbors)
                    {
                        nYAllValidNeighbors.Add(validNeighbor);
                    }

                    // Get the Valid Neighbors that are allowed to the right of the current square.
                    HashSet<Square> validRightNeighbors = new HashSet<Square>(square.PxValidNeighbors);
                    foreach (Square validNeighbor in validRightNeighbors)
                    {
                        pXAllValidNeighbors.Add(validNeighbor);
                    }

                    // Get the Valid Neighbors that are allowed to the left of the current square.
                    HashSet<Square> validLeftNeighbors = new HashSet<Square>(square.NxValidNeighbors);
                    foreach (Square validNeighbor in validLeftNeighbors)
                    {
                        nXAllValidNeighbors.Add(validNeighbor);
                    }
                }

                // Check Up
                if (j < jMax - 1)
                {
                    (int, int) neighborsCoordinates = (i, j + 1);
                    int nI = neighborsCoordinates.Item1;
                    int nJ = neighborsCoordinates.Item2;
                    HashSet<Square> pYCurrentNeighbors = new HashSet<Square>(SuperPositions[nI, nJ].Squares);
                    HashSet<Square> pYConstrainedNeighbors = Constrain(pYCurrentNeighbors, pYAllValidNeighbors);
                    if (pYConstrainedNeighbors.Count == 0) return false; // DEBUG ONLY, THIS SHOULDN'T HAPPEN!!!
                    bool setsAreEqual = pYCurrentNeighbors.SetEquals(pYConstrainedNeighbors);
                    if (!setsAreEqual)
                    {
                        SuperPositions[nI, nJ].Squares = pYConstrainedNeighbors;
                        if (!stack.Contains(neighborsCoordinates))
                        {
                            stack.Push(neighborsCoordinates);
                        }
                    }
                }

                // Check Down
                if (j > 0)
                {
                    (int, int) neighborsCoordinates = (i, j - 1);
                    int nI = neighborsCoordinates.Item1;
                    int nJ = neighborsCoordinates.Item2;
                    HashSet<Square> nYCurrentNeighbors = new HashSet<Square>(SuperPositions[nI, nJ].Squares);
                    HashSet<Square> nYConstrainedNeighbors = Constrain(nYCurrentNeighbors, nYAllValidNeighbors);
                    if (nYConstrainedNeighbors.Count == 0) return false; // DEBUG ONLY, THIS SHOULDN'T HAPPEN!!!
                    bool setsAreEqual = nYCurrentNeighbors.SetEquals(nYConstrainedNeighbors);
                    if (!setsAreEqual)
                    {
                        SuperPositions[nI, nJ].Squares = nYConstrainedNeighbors;
                        if (!stack.Contains(neighborsCoordinates))
                        {
                            stack.Push(neighborsCoordinates);
                        }
                    }
                }

                // Check Left
                if (i > 0)
                {
                    (int, int) neighborsCoordinates = (i - 1, j);
                    int nI = neighborsCoordinates.Item1;
                    int nJ = neighborsCoordinates.Item2;
                    HashSet<Square> nXCurrentNeighbors = new HashSet<Square>(SuperPositions[nI, nJ].Squares);
                    HashSet<Square> nXConstrainedNeighbors = Constrain(nXCurrentNeighbors, nXAllValidNeighbors);
                    if (nXConstrainedNeighbors.Count == 0) return false; // DEBUG ONLY, THIS SHOULDN'T HAPPEN!!!
                    bool setsAreEqual = nXCurrentNeighbors.SetEquals(nXConstrainedNeighbors);
                    if (!setsAreEqual)
                    {
                        SuperPositions[nI, nJ].Squares = nXConstrainedNeighbors;
                        if (!stack.Contains(neighborsCoordinates))
                        {
                            stack.Push(neighborsCoordinates);
                        }
                    }
                }

                // Check Right
                if (i < iMax - 1)
                {
                    (int, int) neighborsCoordinates = (i + 1, j);
                    int nI = neighborsCoordinates.Item1;
                    int nJ = neighborsCoordinates.Item2;
                    HashSet<Square> pXCurrentNeighbors = new HashSet<Square>(SuperPositions[nI, nJ].Squares);
                    HashSet<Square> pXConstrainedNeighbors = Constrain(pXCurrentNeighbors, pXAllValidNeighbors);
                    if (pXConstrainedNeighbors.Count == 0) return false; // DEBUG ONLY, THIS SHOULDN'T HAPPEN!!!
                    bool setsAreEqual = pXCurrentNeighbors.SetEquals(pXConstrainedNeighbors);
                    if (!setsAreEqual)
                    {
                        SuperPositions[nI, nJ].Squares = pXConstrainedNeighbors;
                        if (!stack.Contains(neighborsCoordinates))
                        {
                            stack.Push(neighborsCoordinates);
                        }
                    }
                }
            }

            return true;
        }

        #endregion Public Functions


        private HashSet<Square> Constrain(HashSet<Square> currentNeighbors, HashSet<Square> validNeighbors)
        {
            HashSet<Square> constrainedSquares = new HashSet<Square>();
            foreach (Square neighbor in currentNeighbors)
            {
                if (validNeighbors.Contains(neighbor))
                {
                    constrainedSquares.Add(neighbor);
                }
            }

            return constrainedSquares;
        }
    }
}