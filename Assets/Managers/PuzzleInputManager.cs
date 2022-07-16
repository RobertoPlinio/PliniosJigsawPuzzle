using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JigsawPuzzle.Input
{
    public class PuzzleInputManager : MonoBehaviour
    {
        public static Action<Vector3> OnPressed;
        public static Action<Vector3> OnHold;
        public static Action OnReleased;

        public bool _previousHolding;
        private Vector3 _inputPosition;
        private InputModule _currentModule;

        private void Awake()
        {
            SelectInputMode();
            _previousHolding = false;
        }

        private void SelectInputMode()
        {
            if (_currentModule)
            {
                Destroy(_currentModule);
                _currentModule = null;
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            _currentModule = gameObject.AddComponent<StandaloneModule>();
#elif UNITY_ANDROID
            //android module
#endif

            _currentModule._manager = this;
        }

        private void Update()
        {
            if (_currentModule.GetHolding(out _inputPosition))
            {
                if (!_previousHolding)
                    OnPressed?.Invoke(_inputPosition);

                OnHold?.Invoke(_inputPosition);

                _previousHolding = true;
            }

            if (_currentModule.GetReleased())
            {
                if (_previousHolding)
                    OnReleased?.Invoke();

                _previousHolding = false;
            }
        }
    }
}