using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzle.Input
{
    public abstract class InputModule : MonoBehaviour
    {
        public PuzzleInputManager _manager;
        public abstract bool GetHolding(out Vector3 pixelCoordinates);
        public abstract bool GetReleased();
        public abstract bool GetZooming(out float zoomMagnitude);
    }
}
