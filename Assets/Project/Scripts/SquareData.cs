using UnityEngine;

// [CustomEditor(typeof(SquareData))]
// public class SquareDataEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         SquareData squareData = (SquareData)target;
//         if (DrawDefaultInspector())
//         {
//             // Do Nothing
//         }

//         if (GUILayout.Button("CalculateValidNeighbors"))
//         {
//             // squareData.CalculateValidNeighbors();
//         }
//     }
// }

namespace Project.Scripts
{
    [CreateAssetMenu(fileName = "WFCTile", menuName = "WFC/Square", order = 0)]
    public class SquareData : ScriptableObject
    {

        [SerializeField] public float weight = 1f;

        [SerializeField] public MyData data;

        [SerializeField] public Connector pX;
        [SerializeField] public Connector nX;
        [SerializeField] public Connector pY;
        [SerializeField] public Connector nY;

        [SerializeField] public bool rotate90degrees = true;
        [SerializeField] public bool rotate180degrees = true;
        [SerializeField] public bool rotate270degrees = true;

    }
}