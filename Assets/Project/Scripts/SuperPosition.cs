using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = System.Random;

namespace Project.Scripts
{
    public class SuperPosition
    {
        #region Enums

        public enum CollapseMethod
        {
            Random,
            Weighted
        }

        #endregion Enums

        #region Attributes

        #region Properties

        public HashSet<Square> Squares
        {
            get => _squares;
            set
            {
                _squares = value;
                UpdateSuperPosition();
            }
        }

        public bool IsCollapsed
        {
            get => _isCollapsed;
            private set => _isCollapsed = value;
        }

        public bool IsValid
        {
            get => _isValid;
            private set => _isValid = value;
        }

        public float Entropy
        {
            get => _entropy;
            private set => _entropy = value;
        }

        #endregion Properties

        #region Fields

        private HashSet<Square> _squares;
        private CollapseMethod _collapseMethod = CollapseMethod.Weighted;
        private bool _isCollapsed = false;
        private bool _isValid = true;
        private float _entropy = 0;

        #endregion Fields

        #endregion Attributes

        #region Constructors

        public SuperPosition(HashSet<Square> squares, CollapseMethod collapseMethod = CollapseMethod.Weighted)

        {
            Squares = squares;
            _collapseMethod = collapseMethod;
            UpdateSuperPosition();
        }

        #endregion Constructors

        #region Public Functions

        public float GetEntropy()
        {
            // Sums are over the weights of each remaining
            // allowed tile type for the square whose
            // entropy we are calculating.
            // float shannon_entropy_for_square = log(sum(weight)) - (sum(weight * log(weight)) / sum(weight))
            return Squares.Count; // temporary
        }

        public bool GetIsInvalid()
        {
            return Squares.Count == 0;
        }

        public bool GetIsCollapsed()
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

            UpdateSuperPosition();
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
                totalWeight += square.Weight;
            }

            float randomWeight = UnityEngine.Random.Range(0, totalWeight);
            float currentWeight = 0;
            foreach (Square square in Squares)
            {
                currentWeight += square.Weight;
                if (currentWeight >= randomWeight)
                {
                    return square;
                }
            }

            return null;
        }

        private void UpdateEntropy()
        {
            _entropy = GetEntropy();
        }

        private void UpdateIsCollapsed()
        {
            _isCollapsed = GetIsCollapsed();
        }

        private void UpdateIsValid()
        {
            _isValid = !GetIsInvalid();
        }

        private void UpdateSuperPosition()
        {
            UpdateEntropy();
            UpdateIsCollapsed();
            UpdateIsValid();
        }

        #endregion Private Functions
    }
}