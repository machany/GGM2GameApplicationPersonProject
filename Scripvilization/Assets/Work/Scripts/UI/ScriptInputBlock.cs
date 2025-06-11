using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Work.Scripts.UI
{
    public class ScriptInputBlock : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private int maxScriptLineLength;

        public Action<string[]> OnDeselectEvent;

        private int _caretPosition;
        private string _text;
        private bool _valueChangeLock;

        private void Awake()
        {
            inputField.onDeselect.AddListener(HandleDeselect);
            inputField.onValueChanged.AddListener(HandleValueChange);
            inputField.onSubmit.AddListener(ForceFocus);

            inputField.characterLimit = 0;
            inputField.lineLimit = maxScriptLineLength;
            inputField.resetOnDeActivation = false;
        }

        private void OnDestroy()
        {
            inputField.onDeselect.RemoveListener(HandleDeselect);
            inputField.onValueChanged.RemoveListener(HandleValueChange);
            inputField.onSubmit.RemoveListener(ForceFocus);
        }

        private void HandleValueChange(string value)
        {
            _caretPosition = inputField.caretPosition;
            _text = inputField.text;
        }

        private void HandleDeselect(string value)
            => OnDeselectEvent?.Invoke(StringToScript(value));

        private string[] StringToScript(string value)
            => value.Split('\n');

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

            Debug.Log(_caretPosition);
            string newText = _text.Insert(Mathf.Max(_caretPosition, _text.Length - 1), "\n");
            Debug.Log(newText);
            inputField.text = newText;
            inputField.ActivateInputField();

            inputField.caretPosition = _caretPosition + 1;

            _valueChangeLock = false;
        }
    }
}
