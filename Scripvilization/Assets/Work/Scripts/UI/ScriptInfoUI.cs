using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Work.Scripts.UI
{
    public class ScriptInfoUI : MonoBehaviour
    {
        [SerializeField] private StatusText statusText;
        [SerializeField] private TMP_InputField scriptableNameInputField;

        public event Action<string> OnScriptableNameChange;

        public event Action OnStatusChangeButtonClick;
        public event Action OnExecuteButtonClick;
        public event Action OnAbortButtonClick;

        private string _scriptableName;

        private void Awake()
        {
            scriptableNameInputField.onSubmit.AddListener(HandleSubmit); 
            scriptableNameInputField.onDeselect.AddListener(HandleDeselect); 
        }

        public void SetScriptableObjectName(string name)
            => _scriptableName = scriptableNameInputField.text = name;

        private void HandleSubmit(string value)
            => OnScriptableNameChange?.Invoke(value);

        private void HandleDeselect(string value)
            => scriptableNameInputField.text = _scriptableName;

        public void HandleStatusChangeButtonClick()
            => OnStatusChangeButtonClick?.Invoke();
        public void HandleExecuteButtonClick()
            => OnExecuteButtonClick?.Invoke();
        public void HandleAbortButtonClick()
            => OnAbortButtonClick?.Invoke();

        public void ChangeStatusText(bool isExecut, bool isRepeat)
            => statusText.SetText(isExecut, isRepeat);
    }
}
