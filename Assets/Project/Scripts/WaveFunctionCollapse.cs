using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Project.Scripts
{
    public class WaveFunctionCollapse : MonoBehaviour
    {

        #region Attributes

        [SerializeField] private int width; // x
        [SerializeField] private int height; // y

        [SerializeField] private List<SquareData> squareData;
        [SerializeField] private List<Square> allPossibleSquares; // Debug Only

        [SerializeField] private Tilemap outputTilemap;
        
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

        #endregion Unity Lifecycle

        #region Private Functions

        #region Preprocessing

        private void InitializeWave()
        {
            _wave = new Wave();
            _wave.superPositions = new SuperPosition[width, height];
            for (int i = 0; i < _wave.superPositions.GetLength(0); i++)
            {
                for (int j = 0; j < _wave.superPositions.GetLength(1); j++)
                {
                    _wave.superPositions[i, j] = new SuperPosition(new List<Square>(allPossibleSquares));
                }
            }
            _waveVisualizer.wave = _wave;
        }

        private void ProcessSquareData()
        {
            allPossibleSquares = new List<Square>();
            // Create All Squares from SquareData
            foreach (SquareData squareDatum in squareData)
            {
                List<Square> squares = CreateSquares(squareDatum);
                allPossibleSquares.AddRange(squares);
            }
            // Calculate Valid Neighbors for Each Square
            foreach (Square square in allPossibleSquares)
            {
                Dictionary<Direction, List<Square>> validNeighbors = CalculateValidNeighbors(square, allPossibleSquares);
                square.validNeighbors = validNeighbors;
                square.constrainedNeighbors = validNeighbors;
            }

            for (int i = 0; i < allPossibleSquares.Count; i++)
            {
                MyData myData = allPossibleSquares[i].data;
                Tile tile = myData.tile;
                float rotation = allPossibleSquares[i].rotation;
                Vector3Int position = new Vector3Int(i, 0, 0);
                outputTilemap.SetTile(position, tile);
                outputTilemap.SetTransformMatrix(position, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
            }
        }

        private List<Square> CreateSquares(SquareData newSquareData)
        {
            List<Square> squares = new List<Square>();

            // Create Base Square
            Square square = new Square();
            square.data = newSquareData.data;
            square.weight = newSquareData.weight;
            square.pX = newSquareData.pX;
            square.nX = newSquareData.nX;
            square.pY = newSquareData.pY;
            square.nY = newSquareData.nY;
            square.rotation = 0;
            squares.Add(square);

            // Create Rotated Squares
            if (newSquareData.rotate90degrees)
            {
                Square square90 = new Square();
                square90.data = newSquareData.data;
                square90.weight = newSquareData.weight;
                square90.pX = newSquareData.pY;
                square90.nX = newSquareData.nY;
                square90.pY = newSquareData.nX;
                square90.nY = newSquareData.pX;
                square90.rotation = -90;
                squares.Add(square90);
            }
            if (newSquareData.rotate180degrees)
            {
                Square square180 = new Square();
                square180.data = newSquareData.data;
                square180.weight = newSquareData.weight;
                square180.pX = newSquareData.nX;
                square180.nX = newSquareData.pX;
                square180.pY = newSquareData.nY;
                square180.nY = newSquareData.pY;
                square180.rotation = -180;
                squares.Add(square180);
            }
            if (newSquareData.rotate270degrees)
            {
                Square square270 = new Square();
                square270.data = newSquareData.data;
                square270.weight = newSquareData.weight;
                square270.pX = newSquareData.nY;
                square270.nX = newSquareData.pY;
                square270.pY = newSquareData.pX;
                square270.nY = newSquareData.nX;
                square270.rotation = -270;
                squares.Add(square270);
            }

            return squares;
        }

        public Dictionary<Direction, List<Square>> CalculateValidNeighbors(Square square, List<Square> possibleSquares)
        {
            Dictionary<Direction, List<Square>> validNeighbors = new Dictionary<Direction, List<Square>>();
            validNeighbors.Add(Direction.pX, new List<Square>());
            validNeighbors.Add(Direction.nX, new List<Square>());
            validNeighbors.Add(Direction.pY, new List<Square>());
            validNeighbors.Add(Direction.nY, new List<Square>());

            foreach (Square possibleSquare in possibleSquares)
            {
                if (square.pX == possibleSquare.nX)
                {
                    if (!validNeighbors[Direction.pX].Contains(possibleSquare))
                    {
                        validNeighbors[Direction.pX].Add(possibleSquare);
                    }
                }
                if (square.nX == possibleSquare.pX)
                {
                    if (!validNeighbors[Direction.nX].Contains(possibleSquare))
                    {
                        validNeighbors[Direction.nX].Add(possibleSquare);
                    }
                }
                if (square.pY == possibleSquare.nY)
                {
                    if (!validNeighbors[Direction.pY].Contains(possibleSquare))
                    {
                        validNeighbors[Direction.pY].Add(possibleSquare);
                    }
                }
                if (square.nY == possibleSquare.pY)
                {
                    if (!validNeighbors[Direction.nY].Contains(possibleSquare))
                    {
                        validNeighbors[Direction.nY].Add(possibleSquare);
                    }
                }
            }

            return validNeighbors;
        }

        #endregion Preprocessing

        #region Wave Function Collapse Algorithm

        private int maxAttempts = 0;
        
        private void Solve()
        {
            Debug.Log("Solve");
            while (!_wave.IsCollapsed() && !_wave.IsInvalid() && maxAttempts < 1000)
            {
                maxAttempts++;
                Debug.Log("Solve: " + maxAttempts);
                Step();
            }
            Debug.Log("Solve Completed");
            Debug.Log("Wave is Collapsed: " + _wave.IsCollapsed());
            Debug.Log("Wave is Invalid: " + _wave.IsInvalid());
        }

        private void Iterate()
        {
            Debug.Log("Iterate");
            if (_wave.IsCollapsed())
            {
                Debug.Log("The Wave is Collapsed");
                return;
            }

            if (_wave.IsInvalid())
            {
                Debug.Log("The Wave is Invalid");
                return;
            }
            (int, int) lowestEntropyCoordinates = _wave.GetLowestEntropyCoordinates();
            int x = lowestEntropyCoordinates.Item1;
            int y = lowestEntropyCoordinates.Item2;
            _wave.Collapse(x, y);
            bool propogateSuccessful = _wave.Propagate(x, y);
            Debug.Log("Propogation Completed: " + propogateSuccessful);
        }

        private void Step()
        {
            Debug.Log("Step");
            Iterate();
            _waveVisualizer.DrawWave();
        }

        private void Reset()
        {
            InitializeWave();
            _waveVisualizer.ClearTilemap();
            maxAttempts = 0;
        }

        #endregion Wave Function Collapse Algorithm

        #endregion Private Functions

    }
}





