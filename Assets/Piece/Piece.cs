using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    public class Piece : MonoBehaviour
    {
        const string pieceResourcesPath = "Piece/Pieces";
        const string piecePresetResourcesPath = "Piece/Presets";
        public enum Side
        {
            Edge,
            Tab,
            Hole
        }

        public Side left;
        public Side up;
        public Side right;
        public Side bottom;

        public bool IsCorner;

        private string pieceName {
            get { return string.Concat(left.ToString()[0], up.ToString()[0], right.ToString()[0], bottom.ToString()[0]); }
            set { }
        }

        public void Init() {
            UpdatePiece();
        }

        [ContextMenu("Update Piece")]
        public void UpdatePiece() {
            bool isNotAPrefab = PrefabUtility.GetPrefabInstanceStatus(this) == PrefabInstanceStatus.NotAPrefab;

            IsCorner = right == Side.Edge || left == Side.Edge
                    || bottom == Side.Edge || up == Side.Edge;

            if (isNotAPrefab && !Application.isPlaying) {

                UnityEngine.Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);

                foreach (GameObject o in objs) {
                    MeshFilter mf = o.GetComponent<MeshFilter>();
                    if (!mf.sharedMesh || mf.sharedMesh.name != o.name) {
                        mf.sharedMesh = (Mesh)AssetFinder.GetObjectFromAll(pieceResourcesPath, pieceName, typeof(Mesh));

                        Debug.Log(mf.sharedMesh.name);
                        string assetPath = AssetDatabase.GetAssetPath(this);
                        if (!string.IsNullOrWhiteSpace(assetPath)) {
                            string result = AssetDatabase.RenameAsset(assetPath, $"Piece_{pieceName}");
                            if (!string.IsNullOrWhiteSpace(result))
                                Debug.Log(result);

                        }
                    }

                    EditorUtility.SetDirty(this);
                    AssetDatabase.Refresh();
                }
            } else {

                MeshFilter mf = GetComponent<MeshFilter>();
                try {
                    if (Application.isPlaying) {
                        if (mf.mesh.name != name) {
                            mf.mesh = (Mesh)AssetFinder.GetObjectFromAll(pieceResourcesPath, pieceName, typeof(Mesh));
                        }
                    } else {
                        if (mf.sharedMesh.name != name) {
                            mf.sharedMesh = (Mesh)AssetFinder.GetObjectFromAll(pieceResourcesPath, pieceName, typeof(Mesh));
                        }
                    }
                } catch (Exception e) {
                    Debug.Log($"[PIECE] Error: {e.Message}", gameObject);
                }

                name = "Piece_" + pieceName;
            }
        }
    }
}
