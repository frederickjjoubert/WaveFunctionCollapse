using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Project.Scripts
{
    public class WaveVisualizer : MonoBehaviour
    {
        #region Attributes

        public Wave Wave;

        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tile unchangedTile;
        [SerializeField] private Tile changedTile;
        [SerializeField] private Tile errorTile;

        #endregion Attributes

        #region Public Functions

        public void DrawWave()
        {
            ClearTilemap();
            // Rotate Tiles on placement
            // https://forum.unity.com/threads/rotating-tiles-in-unity-with-code.583132/
            SuperPosition[,] superPositions = Wave.SuperPositions;
            for (int i = 0; i < superPositions.GetLength(0); i++)
            {
                for (int j = 0; j < superPositions.GetLength(1); j++)
                {
                    Vector3Int position = new Vector3Int(i, j, 0);
                    Tile tile = GetTile(superPositions[i, j]);
                    tilemap.SetTile(position, tile);
                    float rotation = superPositions[i, j].Squares.ElementAt(0).Rotation;
                    tilemap.SetTransformMatrix(position, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
                }
            }
        }

        public void ClearTilemap()
        {
            tilemap.ClearAllTiles();
        }

        #endregion Public Functions

        #region Private Functions

        private Tile GetTile(SuperPosition superPosition)
        {
            if (superPosition.Squares.Count == 16) return unchangedTile;

            if (superPosition.Squares.Count > 1) return changedTile;

            if (superPosition.Squares.Count == 1)
            {
                Square square = superPosition.Squares.ElementAt(0);
                MyData data = square.Data;
                Tile tile = data.tile;
                return tile;
            }

            if (superPosition.Squares.Count == 0) return errorTile;

            // This should never be called...
            return errorTile;
        }

        #endregion Private Functions
    }
}