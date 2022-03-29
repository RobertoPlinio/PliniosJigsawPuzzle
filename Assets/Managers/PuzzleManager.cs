using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEditor;
using MyBox.Internal;

namespace JigsawPuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        public Grid grid;
        public ImageHandler imgHandler;

        HashSet<Piece> piecesInPlay = new HashSet<Piece>();

        [Header("Debug")]
        public bool debug = false;

        [ConditionalField(nameof(debug)), Range(2, 50)]
        public int debug_width, debug_height;
        [ConditionalField(nameof(debug))]
        [Range(0.1f, 1)] public float debug_pieceSize = 1f;
        [ConditionalField(nameof(debug))] public Texture2D debug_image;
        [ConditionalField(nameof(debug))] public ImageHandler.AdjustmentMode debug_adjustmentMode = ImageHandler.AdjustmentMode.Stretch;

        private void Awake() {
            grid = GetComponentInChildren<Grid>();
            imgHandler = GetComponentInChildren<ImageHandler>();
        }

        private void Start() {
            Texture2D adjustedImage = default;

            if (debug) {
                InitDebug(ref adjustedImage);
            }

            piecesInPlay = grid.GeneratePieces(this);
            Texture2D[] slices = imgHandler.SliceImage(adjustedImage, 0.5f);

            MeshRenderer mr;
            int count = 0;

            foreach(var piece in piecesInPlay) {
                mr = piece.GetComponent<MeshRenderer>();
                mr.material.SetColor("_Color", Color.white);
                mr.material.mainTexture = slices[count++];
                //mr.material.mainTextureScale = Vector2.one * scale;
            }
        }

        private void InitDebug(ref Texture2D img) {
            grid.Init(debug_width, debug_height, debug_pieceSize);
            imgHandler.Init(debug_width, debug_height);
            img = imgHandler.AdjustImageToBounds(debug_image, debug_adjustmentMode);
        }

        private void OnDrawGizmos() {
            if (!debug || Application.isPlaying) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.TransformPoint(new Vector2(debug_width * 0.5f, debug_height * -0.5f)), new Vector2(debug_width, debug_height));
        }
    }
}