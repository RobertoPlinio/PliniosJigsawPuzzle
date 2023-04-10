using UnityEngine;

namespace JigsawPuzzle.Input
{
    public class StandaloneModule : InputModule
    {
        public override bool GetHolding(out Vector3 pixelCoordinates)
        {
            pixelCoordinates = UnityEngine.Input.mousePosition;
            return UnityEngine.Input.GetMouseButton(0);
        }

        public override bool GetReleased()
        {
            return UnityEngine.Input.GetMouseButtonUp(0);
        }

        public override bool GetZooming(out float zoomMagnitude)
        {
            zoomMagnitude = UnityEngine.Input.mouseScrollDelta.y;
            return zoomMagnitude != 0f;
        }
    }
}