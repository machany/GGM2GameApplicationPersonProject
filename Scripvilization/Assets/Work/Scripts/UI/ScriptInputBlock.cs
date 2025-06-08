using System;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Work.Scripts.UI
{
    public class ScriptInputBlock : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private int maxTextCount;

        public Action<string> OnSubmintEvent;
        public Action<string> OnDeselectEvent;

        private void Awake()
        {
            inputField.onValueChanged.AddListener(HandleScriptChange);
            inputField.onSubmit.AddListener(HandleScriptSubmit);
            inputField.onDeselect.AddListener(HandleDeselect);
        }

        private void OnDestroy()
        {
            inputField.onValueChanged.RemoveListener(HandleScriptChange);
            inputField.onSubmit.RemoveListener(HandleScriptSubmit);
            inputField.onDeselect.RemoveListener(HandleDeselect);
        }

        public void HandleScriptChange(string value)
        {
            if (value.Length >= maxTextCount)
                value = value.Substring(0, maxTextCount);
            inputField.text = value;
        }

        public void HandleScriptSubmit(string value)
        {
            OnSubmintEvent?.Invoke(value);
        }

        public void HandleDeselect(string value)
        {
            OnDeselectEvent?.Invoke(value);
        }

        public void Select()
        {
            inputField.Select();
            inputField.ActivateInputField();
        }

        public void SetText(string text)
        {
            if (text.Length >= maxTextCount)
                text = text.Substring(0, maxTextCount);
            inputField.text = text;
        }
    }
}
