using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        public Grid grid;

        private void Awake() {
            grid = GetComponentInChildren<Grid>();
        }

        private void Start() {
            grid.Init();

            foreach(var slot in grid.slots) {
                GameObject newPiece = GameObject.CreatePrimitive(PrimitiveType.Quad);
                newPiece.AddComponent<Piece>();
                newPiece.transform.position = slot.position;
                newPiece.name = $"Piece ({slot.id.x},{slot.id.y})";
            }
        }
    }
}