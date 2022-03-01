using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzle
{
    public class Piece : MonoBehaviour
    {
        public enum Side
        {
            Edge,
            Tab,
            Hole
        }

        public bool IsCorner;
        public PieceInfo info;

        [ContextMenu("Debug Init")]
        void DebugInit() => Init(info);

        public void Init(PieceInfo _info) {
            info = _info;

            IsCorner = info.right == Side.Edge || info.left == Side.Edge
                || info.bottom == Side.Edge || info.up == Side.Edge;

            GetComponent<MeshFilter>().mesh = info.equivalentMesh;

            MeshRenderer mr = GetComponent<MeshRenderer>();
            if(mr.materials.Length < 2) {
                Material[] mats = new Material[] { mr.material, mr.material };
                mr.materials = mats;
            }
        }
    }
}
