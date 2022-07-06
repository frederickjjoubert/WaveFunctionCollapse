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
        [SerializeField] private Tilemap _outputTilemap;

        private Wave _wave;
        private WaveVisualizer _waveVisualizer;
        private HashSet<Square> _allPossibleSquares;

        #endregion Attributes

        #region Unity Event Functions

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

        #endregion Unity Event Functions

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
            // Create All Possible Squares from SquareData
            foreach (SquareData squareDatum in _squareData)
            {
                List<Square> newSquares = CreateSquares(squareDatum);
                _allPossibleSquares.UnionWith(newSquares);
            }

            // Evaluate Possible Neighbors for Each Square to determine Valid Neighbors.
            foreach (Square square in _allPossibleSquares)
            {
                square.EvaluatePossibleNeighbors(_allPossibleSquares);
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
            square.Data = newSquareData.data;
            square.Weight = newSquareData.weight;
            square.PxConnector = newSquareData.pX;
            square.NxConnector = newSquareData.nX;
            square.PyConnector = newSquareData.pY;
            square.NyConnector = newSquareData.nY;
            square.Rotation = 0;
            squares.Add(square);

            // Create Rotated Squares
            if (newSquareData.rotate90degrees)
            {
                Square square90 = new Square();
                square90.Data = newSquareData.data;
                square90.Weight = newSquareData.weight;
                square90.PxConnector = newSquareData.pY;
                square90.NxConnector = newSquareData.nY;
                square90.PyConnector = newSquareData.nX;
                square90.NyConnector = newSquareData.pX;
                square90.Rotation = -90;
                squares.Add(square90);
            }

            if (newSquareData.rotate180degrees)
            {
                Square square180 = new Square();
                square180.Data = newSquareData.data;
                square180.Weight = newSquareData.weight;
                square180.PxConnector = newSquareData.nX;
                square180.NxConnector = newSquareData.pX;
                square180.PyConnector = newSquareData.nY;
                square180.NyConnector = newSquareData.pY;
                square180.Rotation = -180;
                squares.Add(square180);
            }

            if (newSquareData.rotate270degrees)
            {
                Square square270 = new Square();
                square270.Data = newSquareData.data;
                square270.Weight = newSquareData.weight;
                square270.PxConnector = newSquareData.nY;
                square270.NxConnector = newSquareData.pY;
                square270.PyConnector = newSquareData.pX;
                square270.NyConnector = newSquareData.nX;
                square270.Rotation = -270;
                squares.Add(square270);
            }

            return squares;
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
            Debug.Log("Wave is Valid: " + _wave.IsValid());
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