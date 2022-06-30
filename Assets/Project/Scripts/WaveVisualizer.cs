using UnityEngine;
using UnityEngine.Tilemaps;

namespace Project.Scripts
{
    public class WaveVisualizer : MonoBehaviour
    {

        public Wave wave;

        [SerializeField] private Tilemap tilemap;

        public void DrawWave()
        {
            ClearTilemap();
            // Rotate Tiles on placement
            // https://forum.unity.com/threads/rotating-tiles-in-unity-with-code.583132/
            SuperPosition[,] superPositions = wave.superPositions;
            for (int i = 0; i < superPositions.GetLength(0); i++)
            {
                for (int j = 0; j < superPositions.GetLength(1); j++)
                {
                    Square square = superPositions[i, j].squares[0];
                    Vector3Int position = new Vector3Int(i, j, 0);
                    float rotation = square.rotation;
                    MyData data = square.data;
                    Tile tile = data.tile;
                    tilemap.SetTile(position, tile);
                    tilemap.SetTransformMatrix(position, Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation)));
                }
            }
        }

        public void ClearTilemap()
        {
            tilemap.ClearAllTiles();
        }

    }
}