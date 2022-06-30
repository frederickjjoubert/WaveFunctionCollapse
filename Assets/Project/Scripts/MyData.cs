using UnityEngine;
using UnityEngine.Tilemaps;

namespace Project.Scripts
{
    [CreateAssetMenu(fileName = "MyData", menuName = "WFC/MyData", order = 0)]
    public class MyData : ScriptableObject
    {

        // YOUR DATA HERE
        [SerializeField] public Tile tile;

    }
}
