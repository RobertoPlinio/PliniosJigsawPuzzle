using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager = JigsawPuzzle.Input.PuzzleInputManager;

namespace JigsawPuzzle
{
    public class PieceHandler : MonoBehaviour
    {
        public Camera mainCamera;
        private Piece _piece;

        private void OnEnable()
        {
            SubscribeToManagerEvents();
        }

        private void OnDisable()
        {
            UnSubscribeToManagerEvents();
        }

        private void OnPressed(Vector3 pixelPos)
        {
            Ray cameraDir = mainCamera.ScreenPointToRay(pixelPos);

            if (!Physics.Raycast(cameraDir, out RaycastHit hitInfo, 20f)) return;

            _piece = hitInfo.collider.GetComponent<Piece>();
        }

        private void OnHolding(Vector3 pixelPos)
        {
            Ray cameraDir = mainCamera.ScreenPointToRay(pixelPos);
            Debug.DrawRay(cameraDir.origin, cameraDir.direction * 20f);

            if (!_piece) return;

            Vector3 piecePos = cameraDir.origin + cameraDir.direction * 10;
            
            _piece.transform.position = piecePos;

        }

        private void OnReleased()
        {
            if (!_piece) return;

            _piece.UpdatePiecePosition();
            _piece = null;
        }

        private void SubscribeToManagerEvents()
        {
            Manager.OnPressed += OnPressed;
            Manager.OnHold += OnHolding;
            Manager.OnReleased += OnReleased;
        }

        private void UnSubscribeToManagerEvents()
        {
            Manager.OnPressed -= OnPressed;
            Manager.OnHold -= OnHolding;
            Manager.OnReleased -= OnReleased;
        }
    }
}
