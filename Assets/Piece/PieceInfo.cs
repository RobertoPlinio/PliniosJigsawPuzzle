using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    [CreateAssetMenu(fileName = "PieceInfo", menuName = "Piece/PieceInfo", order = 1)]
    public class PieceInfo : ScriptableObject
    {
        const string pieceResourcesPath = "Piece/Pieces";

        public Piece.Side left;
        public Piece.Side up;
        public Piece.Side right;
        public Piece.Side bottom;

        public Mesh equivalentMesh;
        public Object equivalentObj;

        public void UpdatePiece() {
            string key = string.Concat(left.ToString()[0], up.ToString()[0], right.ToString()[0], bottom.ToString()[0]);
            Debug.Log($"The key is {key}");
            equivalentMesh = (Mesh)AssetFinder.GetObjectFromAll(pieceResourcesPath, key, typeof(Mesh));
            equivalentObj = (Object)AssetFinder.GetObjectFromAll(pieceResourcesPath, key, typeof(Object));
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this.GetInstanceID()), $"Piece_{key}");
        }
    }
}