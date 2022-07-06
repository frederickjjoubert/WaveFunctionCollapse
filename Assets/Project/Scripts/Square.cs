using System;
using System.Collections.Generic;

namespace Project.Scripts
{
    [Serializable]
    public class Square
    {
        public MyData Data; // TODO: Make this a uuid, so that we don't need to worry about the size of MyData
        public float Weight = 1f;
        public Connector PxConnector;
        public Connector NxConnector;
        public Connector PyConnector;
        public Connector NyConnector;
        public float Rotation;
        public HashSet<Square> PyValidNeighbors = new HashSet<Square>();
        public HashSet<Square> NyValidNeighbors = new HashSet<Square>();
        public HashSet<Square> PxValidNeighbors = new HashSet<Square>();
        public HashSet<Square> NxValidNeighbors = new HashSet<Square>();

        #region Public Functions

        public void EvaluatePossibleNeighbors(HashSet<Square> possibleSquares)
        {
            // Clear Previous Valid Neighbors
            PyValidNeighbors = new HashSet<Square>();
            NyValidNeighbors = new HashSet<Square>();
            PxValidNeighbors = new HashSet<Square>();
            NxValidNeighbors = new HashSet<Square>();

            // Evaluate Possible Neighbors
            foreach (Square possibleSquare in possibleSquares)
            {
                // Check if the Square above fits our PX Connector
                if (PyConnector == possibleSquare.NyConnector)
                {
                    PyValidNeighbors.Add(possibleSquare);
                }

                // Check if the Square below fits our PX Connector
                if (NyConnector == possibleSquare.PyConnector)
                {
                    NyValidNeighbors.Add(possibleSquare);
                }

                // Check if the Square to the right fits our PX Connector
                if (PxConnector == possibleSquare.NxConnector)
                {
                    PxValidNeighbors.Add(possibleSquare);
                }

                // Check if the Square to the left fits our PX Connector
                if (NxConnector == possibleSquare.PxConnector)
                {
                    NxValidNeighbors.Add(possibleSquare);
                }
            }
        }

        #endregion Public Functions
    }
}