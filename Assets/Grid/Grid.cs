using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;

namespace JigsawPuzzle
{
    public class Grid : MonoBehaviour
    {
        [Header("Overriden if Puzzle Manager is present")]
        public Material[] matArray1;
        public Material[] matArray2;
        
        [Header("Debug options")]
        public bool showDebug = false;

        [ConditionalField(nameof(showDebug))]
        [Range(2, 50)] public int debug_width, debug_height;

        [ConditionalField(nameof(showDebug))]
        [Range(0.1f, 1)] public float debug_pieceSize = 1f;

        int debug_totalPieces => debug_height * debug_width;

        public GridSlot[,] slots;
        bool isGridGenerated = false;

        private void OnValidate() {
            if (showDebug) DebugCalculateTotalPieces();
        }

        private void Awake() {
            if (showDebug) {
                CalculateTotalPieces(debug_width, debug_height);
                GenerateGridSlots(debug_width, debug_height, debug_pieceSize);
            }
        }

        public void Init(int width, int height, float pieceSize = 1f) {
            GenerateGridSlots(width, height, pieceSize);
        }

        public HashSet<Piece> GeneratePieces(PuzzleManager manager) {
            HashSet<Piece> createdPieces = new HashSet<Piece>();
            int rows = slots.GetLength(0);
            int cols = slots.GetLength(1);

            Object[] prefabs = Resources.LoadAll("Piece/Presets");
                Material[] mats = matArray1;

            for (int c = 0; c < cols; c++) {
                for (int r = 0; r < rows; r++) {
                    string name = CreatePieceName(r, c);
                    GameObject piece = Instantiate(prefabs.First(x => x.name.Contains(name)) as GameObject, slots[r, c].position, Quaternion.identity);
                    Piece pieceComponent = piece.GetComponent<Piece>();
                    piece.GetComponent<MeshRenderer>().materials = mats; //Just temp fun visualization
                    mats = mats == matArray2 ? matArray1 : matArray2;
                    createdPieces.Add(pieceComponent);
                    slots[r, c].AssignPiece(pieceComponent);
                }
                mats = mats == matArray2 ? matArray1 : matArray2;
            }

            return createdPieces;

            string CreatePieceName(int x, int y) {
                char left = x < 1 ? 'E' : SolvePieceSide(slots[x - 1, y].GetPiece().right);
                char up = y < 1 ? 'E' : SolvePieceSide(slots[x, y - 1].GetPiece().bottom);
                char right = x >= rows - 1 ? 'E' : CreateRandomSide();
                char bottom = y >= cols - 1 ? 'E' : CreateRandomSide();

                return string.Concat(left, up, right, bottom);
            }

            char SolvePieceSide(Piece.Side previousPieceSlot) {
                switch (previousPieceSlot) {
                    case Piece.Side.Hole: return 'T';
                    case Piece.Side.Tab: return 'H';
                    default:
                        Debug.LogError("[GRID] Left piece has no valid side");
                        return default;
                }

            }

            char CreateRandomSide() {
                int result = Mathf.RoundToInt(Random.value);
                return result % 2 == 0 ? 'T' : 'H';
            }
        }

        void GenerateGridSlots(int width, int height, float pieceSize) {
            int rows = 0;
            slots = new GridSlot[width, height];
            while (rows < height) {
                Vector3 gridPos = transform.TransformPoint(-transform.up * rows * pieceSize);

                for (int i = 0; i < width; i++) {
                    slots[i, rows] = new GridSlot(gridPos, new Vector2(i, rows));
                    gridPos += transform.right * pieceSize;
                }
                rows++;
            }

            isGridGenerated = true;
        }

        #region Debug
        void CalculateTotalPieces(int w, int h) {
            Debug.Log($"Total pieces for ({w}, {h}): {w * h}");
        }

        [ContextMenu("(Debug) Calculate total pieces")]
        public void DebugCalculateTotalPieces() => CalculateTotalPieces(debug_width, debug_height);

        private void OnDrawGizmos() {
            if (!showDebug) return;

            if (!isGridGenerated) {
                Color startColor = Color.magenta;
                Color endColor = Color.blue;
                int rows = 0;

                Handles.Label(transform.TransformPoint(transform.up * 0.5f), $"Total: {debug_totalPieces}");

                while (rows < debug_height) {
                    Vector3 gridPos = transform.TransformPoint(-transform.up * rows * debug_pieceSize);
                    for (int i = 0; i < debug_width; i++) {
                        Gizmos.DrawIcon(gridPos, "Icon_JigsawPiece", true, Color.Lerp(startColor, endColor, ((float)i * (rows + 1)) / debug_totalPieces));
                        gridPos += transform.right * debug_pieceSize;
                    }
                    rows++;
                }
            }
        }
        #endregion

        public struct GridSlot
        {
            public Vector3 position;
            public Vector2 id;
            public bool isAvailable;
            Piece assignedPiece;

            public GridSlot(Vector3 bornPosition, Vector2 _id) {
                isAvailable = true;
                position = bornPosition;
                id = _id + Vector2.one;
                assignedPiece = null;
            }

            public void AssignPiece(Piece _piece) {
                assignedPiece = _piece;
            }

            public Piece GetPiece() => assignedPiece;
        }
    }
}