using System.Collections.Generic;
using UnityEngine;
using MyBox;

namespace JigsawPuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        public Grid grid;
        public ImageHandler imgHandler;

        private int gridWidth, gridHeight;
        private Texture2D originalImage;
        private ImageHandler.AdjustmentMode adjustmentMode = ImageHandler.AdjustmentMode.Stretch;
        private float pieceSize = 1f;
        [Range(0.1f, 1)] public float pieceSlotDistanceTolerance = 0.4f;
        List<Piece> piecesInPlay = new List<Piece>();

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
            if (debug) InitDebug();

            grid.Init(gridWidth, gridHeight, pieceSize);
            imgHandler.Init(gridWidth, gridHeight);
            Texture2D adjustedImage = imgHandler.AdjustImageToBounds(originalImage, adjustmentMode);

            piecesInPlay = grid.GeneratePieces(this);
            Texture2D[] slices = imgHandler.SliceImage(adjustedImage, 0.5f);

            MeshRenderer mr;
            int count = 0;

            foreach (var piece in piecesInPlay)
            {
                mr = piece.GetComponent<MeshRenderer>();
                mr.material.SetColor("_Color", Color.white);
                mr.material.mainTexture = slices[count++];
            }

            Vector3 gridDimensions = grid.GetGridDimensions();
            gridDimensions.z = piecesInPlay[0].transform.position.z;

            CameraController.Instance.UpdateCameraPos(gridDimensions * 0.5f, piecesInPlay[0].transform.position);
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

            if (isPieceInSlot)
            {
                piece.SlotPiece(gridSlot.position);
            }

            if (debug)
            {
                if (isPieceInSlot) Debug.Log($"[PUZZLE]<color=green> Piece {piece.name} is in correct slot!</color>");
                else Debug.Log($"[PUZZLE]<color=red> Piece {piece.name} is NOT in correct slot!</color> {pieceSlotDistance}");
            }
        }

        public float GetPieceSize() => pieceSize;

        #region Debug

        private void InitDebug()
        {
            gridWidth = debug_width;
            gridHeight = debug_height;
            pieceSize = debug_pieceSize;
            originalImage = debug_image;
            adjustmentMode = debug_adjustmentMode;
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