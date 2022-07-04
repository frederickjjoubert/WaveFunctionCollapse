using System;
using System.Collections.Generic;

namespace Project.Scripts
{
    [Serializable]
    public class Square
    {
        public MyData data;
        public float weight = 1f;
        public Connector pXConnector;
        public Connector nXConnector;
        public Connector pYConnector;
        public Connector nYConnector;
        public float rotation;
        public HashSet<Square> pYValidNeighbors;
        public HashSet<Square> nYValidNeighbors;
        public HashSet<Square> pXValidNeighbors;
        public HashSet<Square> nXValidNeighbors;
    }
}