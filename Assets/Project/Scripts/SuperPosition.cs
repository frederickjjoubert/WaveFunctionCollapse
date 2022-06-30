using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Project.Scripts
{
    public class SuperPosition
    {

        public enum CollapseMethod
        {
            random,
            weighted
        }

        private CollapseMethod _collapseMethod = CollapseMethod.random;

        public List<Square> squares;

        public SuperPosition(List<Square> squares)
        {
            this.squares = squares;
        }

        public int GetCount()
        {
            return squares.Count;
        }

        public float Entropy()
        {
            // Sums are over the weights of each remaining
            // allowed tile type for the square whose
            // entropy we are calculating.
            // float shannon_entropy_for_square = log(sum(weight)) - (sum(weight * log(weight)) / sum(weight))
            return squares.Count; // temporary
        }

        public bool IsInvalid()
        {
            return squares.Count == 0;
        }

        public bool IsCollapsed()
        {
            return squares.Count == 1;
        }

        public void Collapse()
        {
            switch (_collapseMethod)
            {
                case CollapseMethod.random:
                    CollapseRandom();
                    break;
                case CollapseMethod.weighted:
                    CollapseWeighted();
                    break;
            }
        }

        private void CollapseRandom()
        {
            Square square = GetRandomSquare();
            squares = new List<Square>();
            squares.Add(square);
        }

        private void CollapseWeighted()
        {
            Square square = GetWeightedSquare();
            squares = new List<Square>();
            squares.Add(square);
        }

        private Square GetRandomSquare()
        {
            System.Random prng = new Random(System.DateTime.Now.GetHashCode());
            // return squares[UnityEngine.Random.Range(0, squares.Count)];
            return squares[prng.Next(0, squares.Count)];
        }

        // I haven't tested this yet, pray for me!
        private Square GetWeightedSquare()
        {
            float totalWeight = 0;
            foreach (Square square in squares)
            {
                totalWeight += square.weight;
            }
            float randomWeight = UnityEngine.Random.Range(0, totalWeight);
            float currentWeight = 0;
            foreach (Square square in squares)
            {
                currentWeight += square.weight;
                if (currentWeight >= randomWeight)
                {
                    return square;
                }
            }
            return null;
        }


    }
}