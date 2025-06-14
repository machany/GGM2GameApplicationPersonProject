using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Work.Scripts.Core.Inputs
{
    [CreateAssetMenu(fileName = "InputSO", menuName = "SO/Input", order = 0)]
    public class InputSO : ScriptableObject, Controlls.IPlayerActions, Controlls.IScriptActions
    {
        #region Init

        private Controlls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controlls();
                _controls.Player.SetCallbacks(this);
                _controls.Script.SetCallbacks(this);
            }
            SetPlayerEnabled(true);
            SetScriptEnabled(false);
        }

        private void OnDisable()
        {
            SetPlayerEnabled(false);
            SetScriptEnabled(false);
        }

        public void SetPlayerEnabled(bool enabled)
        {
            if (enabled)
                _controls.Player.Enable();
            else
                _controls.Player.Disable();
        }

        public void SetScriptEnabled(bool enabled)
        {
            if (enabled)
                _controls.Script.Enable();
            else
                _controls.Script.Disable();
        }

        #endregion

        #region Player Input

        public Vector2 MoveDirection { get; private set; }
        public Vector2 MousePosition { get; private set; }
        public float TurnValue { get; private set; }

        public event Action<Vector2> OnMoveChangeEvent;
        public event Action<Vector2> OnZoomDeltaValueChangeEvent;

        public event Action<float> OnTurnChangeEvent;
        public event Action<bool> OnResetStatusEvent;

        public event Action<Vector2> OnMousePositionChangeEvent;
        public event Action<bool> OnMouseSelectedStatusEvent;
        public event Action<bool> OnMouseOptionClickStatusEvent;
        public event Action<Vector2> OnMouseMoveEvent;

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();
            OnMoveChangeEvent?.Invoke(MoveDirection);
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            OnZoomDeltaValueChangeEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnTurn(InputAction.CallbackContext context)
        {
            TurnValue = context.ReadValue<Vector2>().x;
            OnTurnChangeEvent?.Invoke(TurnValue);
        }

        public void OnReset(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnResetStatusEvent?.Invoke(true);

            if (context.canceled)
                OnResetStatusEvent?.Invoke(false);
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
            OnMousePositionChangeEvent?.Invoke(MousePosition);
        }

        public void OnMouseSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnMouseSelectedStatusEvent?.Invoke(true);

            if (context.canceled)
                OnMouseSelectedStatusEvent?.Invoke(false);
        }

        public void OnMouseOption(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnMouseOptionClickStatusEvent?.Invoke(true);

            if (context.canceled)
                OnMouseOptionClickStatusEvent?.Invoke(false);
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
            OnMouseMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        #endregion

        #region Script

        public event Action OnMouseClickEvent;
        public event Action OnToggleScriptUIEvent;

        public void OnMouseClick(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnMouseClickEvent?.Invoke();
        }

        public void OnToggleScriptUI(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnToggleScriptUIEvent?.Invoke();
        }

        #endregion
    }
}
