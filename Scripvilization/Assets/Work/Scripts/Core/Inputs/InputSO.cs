using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Work.Scripts.Core.Inputs
{
    [CreateAssetMenu(fileName = "InputSO", menuName = "SO/Input", order = 0)]
    public class InputSO : ScriptableObject, Controlls.IPlayerActions
    {
        #region Init

        private Controlls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controlls();
                _controls.Player.SetCallbacks(this);
            }
            SetEnabled(true);
        }

        private void OnDisable()
        {
            SetEnabled(false);
        }

        public void SetEnabled(bool enabled)
        {
            if (enabled)
                _controls.Player.Enable();
            else
                _controls.Player.Disable();
        }

        #endregion

        #region Player Input

        public Vector2 MoveDirection { get; private set; }

        public event Action<Vector2> OnMouseMoveEvent;
        public event Action<bool> OnMouseSelectedEvent;

        public event Action<Vector2> OnMoveEvent;
        public event Action<Vector2> OnZoomDeltaValueChangeEvent;

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            OnMouseMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMouseSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnMouseSelectedEvent.Invoke(true);

            if (context.canceled)
                OnMouseSelectedEvent.Invoke(false);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();
            OnMoveEvent?.Invoke(MoveDirection);
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            OnZoomDeltaValueChangeEvent?.Invoke(context.ReadValue<Vector2>());
        }

        #endregion
    }
}
