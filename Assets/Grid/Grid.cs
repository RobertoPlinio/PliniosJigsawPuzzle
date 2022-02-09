using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Grid : MonoBehaviour
{
    [Header("Debug options")]
    public bool showDebug = false;
    [Range(2, 50)] public int debug_width, debug_height;
    [Range(0.1f, 1)] public float debug_pieceSize = 1f;
    int TotalPieces => debug_height * debug_width;

    HashSet<GridSlot> slots = new HashSet<GridSlot>();
    bool isGridGenerated = false;

    private void OnValidate() {
        if (showDebug) DebugCalculateTotalPieces();
    }

    private void Start() {
        if (showDebug) {
            CalculateTotalPieces(debug_width, debug_height);
            GenerateGridSlots(debug_width, debug_height, debug_pieceSize);
        }
    }

    void GenerateGridSlots(int width, int height, float pieceSize) {
        int rows = 0;

        while (rows < height) {
            Vector3 gridPos = transform.TransformPoint(transform.right * pieceSize * 0.5f - transform.up * pieceSize * 0.5f - transform.up * rows * pieceSize);

            for (int i = 0; i < width; i++) {
                slots.Add(new GridSlot(gridPos));
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

            Handles.Label(transform.TransformPoint(transform.up * 0.5f), $"Total: {TotalPieces}");

            while (rows < debug_height) {
                Vector3 gridPos = transform.TransformPoint(transform.right * debug_pieceSize * 0.5f - transform.up * debug_pieceSize * 0.5f - transform.up * rows * debug_pieceSize);
                for (int i = 0; i < debug_width; i++) {
                    Gizmos.DrawIcon(gridPos, "Icon_JigsawPiece", true, Color.Lerp(startColor, endColor, ((float)i * (rows+1)) / TotalPieces));
                    gridPos += transform.right * debug_pieceSize;
                }
                rows++;
            }
        } else {
            foreach(GridSlot item in slots) {
                Gizmos.DrawIcon(item.position, "Icon_JigsawPiece", true, Color.yellow);
            }
        }
    }
    #endregion

    public struct GridSlot
    {
        public Vector3 position;
        public bool isAvailable;

        public GridSlot(Vector3 bornPosition) {
            isAvailable = true;
            position = bornPosition;
        }
    }
}
