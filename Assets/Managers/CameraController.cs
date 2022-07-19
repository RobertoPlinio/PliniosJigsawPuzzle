using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using InputManger = JigsawPuzzle.Input.PuzzleInputManager;

namespace JigsawPuzzle
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;

        public Camera mainCamera;
        public float cameraDistance = 10f;
        [MinMaxRange(0.1f, 3f)]
        public RangedFloat zoomLevel = new RangedFloat(0.1f, 1.5f);
        public float zoomSpeed = 1f;

        private float _defaultCameraFOV, minFOV, maxFOV;

        private void Awake()
        {
            if (Instance && Instance != this) Destroy(Instance.gameObject);
            Instance = this;

            if (!mainCamera) mainCamera = FindObjectOfType<Camera>();

            zoomSpeed = Mathf.Max(zoomSpeed, 0.1f);
        }

        #region Subscribing to input manager
        private void OnEnable()
        {
            InputManger.OnZoom += ZoomCamera;
        }

        private void OnDestroy()
        {
            InputManger.OnZoom -= ZoomCamera;
        }
        #endregion

        public void UpdateCameraPos(Vector3 centerPos, Vector3 edgePos)
        {
            //Not perfect since I'm working with diagonals but its good enough

            Vector3 cameraPos = centerPos + Vector3.forward * -cameraDistance;

            float edgeDist = (cameraPos - edgePos).magnitude;
            float FOV_radians = Mathf.Acos(cameraDistance / edgeDist) * 2;
            float FOV_degrees = FOV_radians * Mathf.Rad2Deg;

            mainCamera.transform.position = cameraPos;
            _defaultCameraFOV = FOV_degrees;
            minFOV = _defaultCameraFOV * zoomLevel.Min;
            maxFOV = _defaultCameraFOV * zoomLevel.Max;
            SetCameraFOV(FOV_degrees);
        }

        private void SetCameraFOV(float value) => mainCamera.fieldOfView = value;

        private void ZoomCamera(float zoomDelta)
        {
            float newFOV = Mathf.Clamp(mainCamera.fieldOfView - zoomDelta * zoomSpeed, minFOV, maxFOV);
            SetCameraFOV(newFOV);
        }

        private void Update()
        {
            float upInput = UnityEngine.Input.GetKey(KeyCode.W) ? 1f : 0f;
            float downInput = UnityEngine.Input.GetKey(KeyCode.S) ? 1f : 0f;
            float leftInput = UnityEngine.Input.GetKey(KeyCode.A) ? 1f : 0f;
            float rightInput = UnityEngine.Input.GetKey(KeyCode.D) ? 1f : 0f;

            mainCamera.transform.position += (mainCamera.transform.up * upInput +
                mainCamera.transform.up * downInput * -1f +
                mainCamera.transform.right * leftInput * -1f +
                mainCamera.transform.right * rightInput)
                * Time.deltaTime;
        }
    }
}