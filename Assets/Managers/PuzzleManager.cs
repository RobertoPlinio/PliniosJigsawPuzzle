using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEditor;

namespace JigsawPuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        public Grid grid;
        public ImageHandler imgHandler;

        [Range(0.1f, 1)] public float pieceSlotDistanceTolerance = 0.4f;
        HashSet<Piece> piecesInPlay = new HashSet<Piece>();

        [Header("Debug")]
        public bool debug = false;

        [ConditionalField(nameof(debug)), Range(2, 50)]
        public int debug_width, debug_height;
        [ConditionalField(nameof(debug))]
        [Range(0.1f, 1)] public float debug_pieceSize = 1f;
        [ConditionalField(nameof(debug))] public Texture2D debug_image;
        [ConditionalField(nameof(debug))] public ImageHandler.AdjustmentMode debug_adjustmentMode = ImageHandler.AdjustmentMode.Stretch;

        private void Awake()
        {
            grid = GetComponentInChildren<Grid>();
            imgHandler = GetComponentInChildren<ImageHandler>();
        }

        private void Start()
        {
            Texture2D adjustedImage = default;

            if (debug)
            {
                InitDebug(ref adjustedImage);
            }

            piecesInPlay = grid.GeneratePieces(this);
            Texture2D[] slices = imgHandler.SliceImage(adjustedImage, 0.5f);

            MeshRenderer mr;
            int count = 0;

            foreach (var piece in piecesInPlay)
            {
                mr = piece.GetComponent<MeshRenderer>();
                mr.material.SetColor("_Color", Color.white);
                mr.material.mainTexture = slices[count++];
                //mr.material.mainTextureScale = Vector2.one * scale;
            }
        }

        public void OnPieceUpdatedPosition(Piece piece)
        {
            if (!grid.GetGridSlot(piece, out Grid.GridSlot gridSlot))
            {
                if (debug) Debug.Log($"[PUZZLE] Could not find slot for piece {piece.name}");
                return;
            }

            float pieceSlotDistance = (piece.transform.position - gridSlot.position).magnitude;
            bool isPieceInSlot = pieceSlotDistance <= pieceSlotDistanceTolerance;

            if (debug)
            {
                if (isPieceInSlot) Debug.Log($"[PUZZLE]<color=green> Piece {piece.name} is in correct slot!</color>");
                else Debug.Log($"[PUZZLE]<color=red> Piece {piece.name} is NOT in correct slot!</color> {pieceSlotDistance}");
            }
        }

        #region Debug

        private void InitDebug(ref Texture2D img)
        {
            grid.Init(debug_width, debug_height, debug_pieceSize);
            imgHandler.Init(debug_width, debug_height);
            img = imgHandler.AdjustImageToBounds(debug_image, debug_adjustmentMode);
        }

        private void OnDrawGizmos()
        {
            if (!debug || Application.isPlaying) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.TransformPoint(new Vector2(debug_width * 0.5f, debug_height * -0.5f)), new Vector2(debug_width, debug_height));
        }

        [ButtonMethod]
        public void ResetTabletop()
        {
            foreach (var piece in piecesInPlay)
            {
                Destroy(piece.gameObject);
            }

            piecesInPlay.Clear();
            Start();
        }

        #endregion
    }
}