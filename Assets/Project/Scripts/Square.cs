using System;
using System.Collections.Generic;

namespace Project.Scripts
{
    [Serializable]
    public class Square
    {

        public MyData data;
        public float weight = 1f;
        public Connector pX;
        public Connector nX;
        public Connector pY;
        public Connector nY;
        public float rotation;
        public Dictionary<Direction, List<Square>> validNeighbors;
        public Dictionary<Direction, List<Square>> constrainedNeighbors;

    }
}

