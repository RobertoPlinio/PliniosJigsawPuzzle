using JigsawPuzzle;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    [CustomEditor(typeof(PieceInfo))]
    public class PieceInfoEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            PieceInfo script = (PieceInfo)target;

            if (GUILayout.Button("Update piece")) script.UpdatePiece();
        }
    }
}