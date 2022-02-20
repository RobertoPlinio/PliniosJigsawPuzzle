using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        public Grid grid;
        public ImageSlicer imgSlicer;
        private void Awake() {
            grid = GetComponentInChildren<Grid>();
            imgSlicer = GetComponentInChildren<ImageSlicer>();
        }

        private void Start() {
            grid.Init();
            imgSlicer.Init(grid.debug_width, grid.debug_height);

            foreach (var slot in grid.slots) {
                var newPiece = GameObject.CreatePrimitive(PrimitiveType.Quad).AddComponent<Piece>();
                newPiece.IsCorner = IsCorner(slot.id);
                newPiece.transform.position = slot.position;
                newPiece.name = $"Piece ({slot.id.x},{slot.id.y})";
            }
        }

        bool IsCorner (Vector2 id) {
            if(grid.showDebug) {
                return id.x == 1 || id.x == grid.debug_width
                    || id.y == 1 || id.y == grid.debug_height;
            }

            return default;
        }
    }
}