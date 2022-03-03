using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        public Grid grid;
        public ImageSlicer imgSlicer;

        HashSet<Piece> piecesInPlay = new HashSet<Piece>();

        private void Awake() {
            grid = GetComponentInChildren<Grid>();
            imgSlicer = GetComponentInChildren<ImageSlicer>();
        }

        private void Start() {
            //grid.Init();
            piecesInPlay = grid.GeneratePieces(this);
            imgSlicer.Init(grid.debug_width, grid.debug_height);
        }
    }
}