using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Project.Scripts
{
    public class WaveFunctionCollapse : MonoBehaviour
    {
        #region Attributes

        [SerializeField] private int _width = 10; // x
        [SerializeField] private int _height = 10; // y

        [SerializeField] private List<SquareData> _squareData;
        [SerializeField] private HashSet<Square> _allPossibleSquares; // Debug Only

        [SerializeField] private Tilemap _outputTilemap;

        private Wave _wave;
        private WaveVisualizer _waveVisualizer;

        #endregion Attributes

        #region Unity Lifecycle

        private void Awake()
        {
            _waveVisualizer = GetComponent<WaveVisualizer>();
        }

        private void Start()
        {
            ProcessSquareData();
            InitializeWave();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Step();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Solve();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            _wave.OnRequestDraw -= HandleDrawRequest;
        }

        #endregion Unity Lifecycle

        #region Private Functions

        #region Handler Functions

        private void HandleDrawRequest()
        {
            Debug.Log("HandleDrawRequest");
            _waveVisualizer.DrawWave();
        }

        #endregion Handler Functions

        #region Preprocessing

        private void InitializeWave()
        {
            _wave = new Wave();
            _wave.SuperPositions = new SuperPosition[_width, _height];
            for (int i = 0; i < _wave.SuperPositions.GetLength(0); i++)
            {
                for (int j = 0; j < _wave.SuperPositions.GetLength(1); j++)
                {
                    _wave.SuperPositions[i, j] = new SuperPosition(new HashSet<Square>(_allPossibleSquares));
                }
            }

            _wave.OnRequestDraw += HandleDrawRequest;
            _waveVisualizer.Wave = _wave;
        }

        private void ProcessSquareData()
        {
            _allPossibleSquares = new HashSet<Square>();
            // Create All Squares from SquareData
            foreach (SquareData squareDatum in _squareData)
            {
                List<Square> newSquares = CreateSquares(squareDatum);
                _allPossibleSquares.UnionWith(newSquares);
            }

            // Calculate Valid Neighbors for Each Square
            foreach (Square square in _allPossibleSquares)
            {
                Dictionary<Direction, HashSet<Square>>
                    validNeighbors = CalculateValidNeighbors(square, _allPossibleSquares);
                square.pYValidNeighbors = validNeighbors[Direction.pY];
                square.nYValidNeighbors = validNeighbors[Direction.nY];
                square.pXValidNeighbors = validNeighbors[Direction.pX];
                square.nXValidNeighbors = validNeighbors[Direction.nX];
            }

            // DEBUG: Draw out all created squares to visually inspect the rotations look correct.
            // for (int i = 0; i < _allPossibleSquares.Count; i++)
            // {
            //     MyData myData = _allPossibleSquares[i].data;
            //     Tile tile = myData.tile;
            //     float rotation = _allPossibleSquares[i].rotation;
            //     Vector3Int position = new Vector3Int(i, 0, 0);
            //     _outputTilemap.SetTile(position, tile);
            //     _outputTilemap.SetTransformMatrix(position, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
            // }
        }

        private List<Square> CreateSquares(SquareData newSquareData)
        {
            List<Square> squares = new List<Square>();

            // Create Base Square
            Square square = new Square();
            square.data = newSquareData.data;
            square.weight = newSquareData.weight;
            square.pXConnector = newSquareData.pX;
            square.nXConnector = newSquareData.nX;
            square.pYConnector = newSquareData.pY;
            square.nYConnector = newSquareData.nY;
            square.rotation = 0;
            squares.Add(square);

            // Create Rotated Squares
            if (newSquareData.rotate90degrees)
            {
                Square square90 = new Square();
                square90.data = newSquareData.data;
                square90.weight = newSquareData.weight;
                square90.pXConnector = newSquareData.pY;
                square90.nXConnector = newSquareData.nY;
                square90.pYConnector = newSquareData.nX;
                square90.nYConnector = newSquareData.pX;
                square90.rotation = -90;
                squares.Add(square90);
            }

            if (newSquareData.rotate180degrees)
            {
                Square square180 = new Square();
                square180.data = newSquareData.data;
                square180.weight = newSquareData.weight;
                square180.pXConnector = newSquareData.nX;
                square180.nXConnector = newSquareData.pX;
                square180.pYConnector = newSquareData.nY;
                square180.nYConnector = newSquareData.pY;
                square180.rotation = -180;
                squares.Add(square180);
            }

            if (newSquareData.rotate270degrees)
            {
                Square square270 = new Square();
                square270.data = newSquareData.data;
                square270.weight = newSquareData.weight;
                square270.pXConnector = newSquareData.nY;
                square270.nXConnector = newSquareData.pY;
                square270.pYConnector = newSquareData.pX;
                square270.nYConnector = newSquareData.nX;
                square270.rotation = -270;
                squares.Add(square270);
            }

            return squares;
        }

        public Dictionary<Direction, HashSet<Square>> CalculateValidNeighbors(Square square,
            HashSet<Square> possibleSquares)
        {
            Dictionary<Direction, HashSet<Square>> validNeighbors = new Dictionary<Direction, HashSet<Square>>
            {
                { Direction.pX, new HashSet<Square>() },
                { Direction.nX, new HashSet<Square>() },
                { Direction.pY, new HashSet<Square>() },
                { Direction.nY, new HashSet<Square>() }
            };

            foreach (Square possibleSquare in possibleSquares)
            {
                if (square.pXConnector == possibleSquare.nXConnector)
                {
                    validNeighbors[Direction.pX].Add(possibleSquare);
                }

                if (square.nXConnector == possibleSquare.pXConnector)
                {
                    validNeighbors[Direction.nX].Add(possibleSquare);
                }

                if (square.pYConnector == possibleSquare.nYConnector)
                {
                    validNeighbors[Direction.pY].Add(possibleSquare);
                }

                if (square.nYConnector == possibleSquare.pYConnector)
                {
                    validNeighbors[Direction.nY].Add(possibleSquare);
                }
            }

            return validNeighbors;
        }

        #endregion Preprocessing

        #region Wave Function Collapse Algorithm

        private void Solve()
        {
            Debug.Log("Solve");
            bool solveable = true;
            while (!_wave.IsCollapsed() && solveable)
            {
                solveable = Step();
            }

            Debug.Log("Solve Completed");
            Debug.Log("Wave is Collapsed: " + _wave.IsCollapsed());
            Debug.Log("Wave is Valid: " + !_wave.IsInvalid());
        }

        private bool Step()
        {
            bool stepSuccessful = Iterate();
            _waveVisualizer.DrawWave();
            return stepSuccessful;
        }

        private bool Iterate()
        {
            (int, int) lowestEntropyCoordinates = _wave.GetLowestEntropyCoordinates();
            int x = lowestEntropyCoordinates.Item1;
            int y = lowestEntropyCoordinates.Item2;
            _wave.Collapse(x, y);
            bool propagateSuccessful = _wave.Propagate(x, y);
            return propagateSuccessful;
        }

        private void Reset()
        {
            InitializeWave();
            _waveVisualizer.ClearTilemap();
            _waveVisualizer.DrawWave();
        }

        #endregion Wave Function Collapse Algorithm

        #endregion Private Functions
    }
}