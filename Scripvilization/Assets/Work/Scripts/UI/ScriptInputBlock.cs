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

        private void Awake()
        {
            inputField.onDeselect.AddListener(HandleDeselect);
            inputField.onEndEdit.AddListener(ForceFocus);

            inputField.characterLimit = 0;
            inputField.lineLimit = maxScriptLineLength;
            inputField.resetOnDeActivation = false;

            inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
        }

        private void OnDestroy()
        {
            inputField.onDeselect.RemoveListener(HandleDeselect);
            inputField.onEndEdit.RemoveListener(ForceFocus);
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

        private async void ForceFocus(string value)
        {
            await Task.Yield();

            int caretPos = inputField.caretPosition;

            string text = inputField.text;
            string newText = text.Insert(caretPos, "\n");

            inputField.text = newText;
            inputField.ActivateInputField();

            inputField.caretPosition = caretPos + 1;
        }
    }
}
