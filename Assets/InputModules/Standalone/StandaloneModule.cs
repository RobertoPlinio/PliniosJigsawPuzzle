using System.Collections;
using System.Collections.Generic;
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

            //if (_justReleased)
            //{
            //    _justReleased = false;
            //    return true;
            //}
            //else return false;
        }
    }
}