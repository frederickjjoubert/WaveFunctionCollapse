using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Project.Scripts
{
    public class SuperPosition
    {
        public enum CollapseMethod
        {
            Random,
            Weighted
        }

        #region Attributes

        public HashSet<Square> Squares;

        private CollapseMethod _collapseMethod = CollapseMethod.Weighted;

        #endregion Attributes

        #region Constructors

        public SuperPosition(HashSet<Square> squares)
        {
            Squares = squares;
        }

        #endregion Constructors

        #region Public Functions

        public int GetCount()
        {
            return Squares.Count;
        }

        public float Entropy()
        {
            // Sums are over the weights of each remaining
            // allowed tile type for the square whose
            // entropy we are calculating.
            // float shannon_entropy_for_square = log(sum(weight)) - (sum(weight * log(weight)) / sum(weight))
            return Squares.Count; // temporary
        }

        public bool IsInvalid()
        {
            return Squares.Count == 0;
        }

        public bool IsCollapsed()
        {
            return Squares.Count == 1;
        }

        public void Collapse()
        {
            switch (_collapseMethod)
            {
                case CollapseMethod.Random:
                    CollapseRandom();
                    break;
                case CollapseMethod.Weighted:
                    CollapseWeighted();
                    break;
            }
        }

        #endregion Public Functions

        #region Private Functions

        private void CollapseRandom()
        {
            Square square = GetRandomSquare();
            Squares = new HashSet<Square> { square };
        }

        private void CollapseWeighted()
        {
            Square square = GetWeightedSquare();
            Squares = new HashSet<Square> { square };
        }

        private Square GetRandomSquare()
        {
            Random prng = new Random(System.DateTime.Now.GetHashCode());
            int randomIndex = prng.Next(0, Squares.Count);
            return Squares.ElementAt(randomIndex);
        }


        // I haven't tested this yet, pray for me!
        private Square GetWeightedSquare()
        {
            float totalWeight = 0;
            foreach (Square square in Squares)
            {
                totalWeight += square.weight;
            }

            float randomWeight = UnityEngine.Random.Range(0, totalWeight);
            float currentWeight = 0;
            foreach (Square square in Squares)
            {
                currentWeight += square.weight;
                if (currentWeight >= randomWeight)
                {
                    return square;
                }
            }

            return null;
        }

        #endregion Private Functions
    }
}