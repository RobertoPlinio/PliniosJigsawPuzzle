using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzle
{
    public class Grid_Debug : MonoBehaviour
    {
        public Vector2 gridDimensions;
        public ImageHandler imgHandler;
        public Texture2D testImage;
        public Piece[] gridPieces;

        Texture2D adjustedImage;
        [Range(-1,1)]
        public float scale;
        private void Awake() {
            gridPieces = GetComponentsInChildren<Piece>();
            imgHandler.Init((int)gridDimensions.x, (int)gridDimensions.y);
        }

        private void Start() {
            adjustedImage = imgHandler.AdjustImageToBounds(testImage);
            Texture2D[] slices = imgHandler.SliceImage(adjustedImage);

            MeshRenderer mr;

            for (int i = 0; i < gridPieces.Length; i++) {
                mr = gridPieces[i].GetComponent<MeshRenderer>();
                mr.material.SetColor("_Color", Color.white);
                mr.material.mainTexture = slices[i];
                mr.material.mainTextureScale = Vector2.one * scale;
            }
        }
    }
}