using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JigsawPuzzle
{
    [CustomEditor(typeof(Piece))]
    public class PieceEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("UpdateAsset")) {
                Piece p = (Piece)target;
                p.UpdatePiece();
            }
        }
    }
}