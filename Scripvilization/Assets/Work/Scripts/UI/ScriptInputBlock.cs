using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Assets.Work.Scripts.UI
{
    public class ScriptInputBlock : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        public event Action<string> OnSaveTextEvent;
        public event Action OnSelectedEvent;

        private bool _valueChangeLock;

        private void Awake()
        {
            inputField.onSelect.AddListener(HandleSelect);
            inputField.onDeselect.AddListener(HandleDeselect);
            inputField.onSubmit.AddListener(ForceFocus);

            inputField.characterLimit = 0;
            inputField.resetOnDeActivation = false;
        }

        private void OnDestroy()
        {
            inputField.onSelect.RemoveListener(HandleSelect);
            inputField.onDeselect.RemoveListener(HandleDeselect);
            inputField.onSubmit.RemoveListener(ForceFocus);
        }

        private void HandleDeselect(string value)
            => OnSaveTextEvent?.Invoke(inputField.text);
        private void HandleSelect(string value)
            => OnSelectedEvent?.Invoke();

        public void SetText(string[] scripts)
        {
            string text = string.Join("\n", scripts);
            inputField.text = text;
        }

        private void ForceFocus(string value)
        {
            if (_valueChangeLock)
                return;

            _valueChangeLock = true;

            OnSaveTextEvent?.Invoke(inputField.text);

            string newText = inputField.text.Insert(Mathf.Min(inputField.caretPosition, inputField.text.Length), "\n");
            inputField.text = newText;
            inputField.ActivateInputField();

            inputField.caretPosition = inputField.caretPosition + 1;

            _valueChangeLock = false;
        }
    }
}
