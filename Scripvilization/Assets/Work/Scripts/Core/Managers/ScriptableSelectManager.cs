using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Core.Inputs;
using Assets.Work.Scripts.Executors;
using Assets.Work.Scripts.Sriptable;
using Assets.Work.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Work.Scripts.Core.Managers
{
    public class ScriptableSelectManager : MonoBehaviour
    {
        [Header("Select Setting")]
        [SerializeField] private InputSO inputSO;
        [SerializeField] private LayerMask checkTarget;
        [SerializeField] private EventChannelSO stageEventChannel;

        [Header("Default Setting")]
        // IExecutor가 직열화 안 됨. 따라서 이렇게 사용함.
        [SerializeField] private Executor defaultExecutor;
        [SerializeField] private string defaultExecutorName;
        [SerializeField] private ScriptInputUIManager scriptInputManager;

        [Header("UI Setting")]
        [SerializeField] private ScriptInfoUI scriptInfoUI;

        private IExecutor _executor;
        private IScriptable _scriptable;
        private List<string> _scripts;

        private void Awake()
        {
            inputSO.OnMouseSelectedStatusEvent += HandleMouseClickEvent;
            scriptInputManager.OnSaveCommand += HandleSaveCommand;

            scriptInfoUI.OnAbortButtonClick += HandleAbortButtonClick;
            scriptInfoUI.OnExecuteButtonClick += HandleExecuteButtonClick;
            scriptInfoUI.OnStatusChangeButtonClick += HandleStatusChangeButtonClick;

            scriptInfoUI.OnScriptableNameChange += HandleScriptableNameChange;

            _scripts = new List<string>();
        }

        

        private void OnDestroy()
        {
            inputSO.OnMouseSelectedStatusEvent -= HandleMouseClickEvent;
            scriptInputManager.OnSaveCommand -= HandleSaveCommand;

            scriptInfoUI.OnAbortButtonClick -= HandleAbortButtonClick;
            scriptInfoUI.OnExecuteButtonClick -= HandleExecuteButtonClick;
            scriptInfoUI.OnStatusChangeButtonClick -= HandleStatusChangeButtonClick;

            scriptInfoUI.OnScriptableNameChange -= HandleScriptableNameChange;

            if (_executor != null)
            {
                _executor.OnCommandExecuted -= HandleCommandExecuted;
                _executor.OnCommandEndOrAbort -= HandleCommandEndOrAbort;
            }
        }

        private void HandleScriptableNameChange(string name)
        {
            if (_scriptable == null)
                return;

            if (EqualityComparer<IExecutor>.Default.Equals(_executor, defaultExecutor))
                return;

            _scriptable.ObjectName = name;
            scriptInfoUI.SetScriptableObjectName(_scriptable.ObjectName);
        }

        private void HandleAbortButtonClick()
        {
            _executor?.Abort();
        }

        private void HandleExecuteButtonClick()
        {
            _executor?.ExecuteCommands();
        }

        private void HandleStatusChangeButtonClick()
        {
            if (_executor != null)
            {
                _executor.Repeat = !_executor.Repeat;
                scriptInfoUI.ChangeStatusText(false, _executor.Repeat);
            }
        }

        private void HandleMouseClickEvent(bool status)
        {
            if (!status)
                return;

            if (!GetWorldHitInfo(out RaycastHit hitInfo))
            {
                scriptInputManager.Close();
                _scriptable?.UnSelected();
                _executor = defaultExecutor;
                scriptInfoUI.SetScriptableObjectName(defaultExecutorName);
                return;
            }

            // 기존에 선택된 오브젝트 선택취소 후 새 오브젝트 선택
            _scriptable?.UnSelected();
            _scriptable = hitInfo.collider.GetComponent<IScriptable>();

            if (_scriptable != null)
            {
                _scriptable?.Selected();
                scriptInfoUI.SetScriptableObjectName(_scriptable.ObjectName);
                stageEventChannel.InvokeEvent(StageEvents.SelectScriptable.Init(hitInfo.collider.transform.position));
            }

            if (_executor != null)
            {
                _executor.OnCommandExecuted -= HandleCommandExecuted;
                _executor.OnCommandEndOrAbort -= HandleCommandEndOrAbort;
            }

            _executor = hitInfo.collider.GetComponent<IExecutor>();
            scriptInputManager.SetText(_executor.Commands);

            if (_executor != null)
            {
                _executor.OnCommandExecuted += HandleCommandExecuted;
                _executor.OnCommandEndOrAbort += HandleCommandEndOrAbort;
                scriptInputManager.Open();
            }
        }

        private void HandleCommandExecuted(int index)
        {
            if (_executor == null || index >= _executor.Commands.Length || index < 0)
                return;

            string[] lines = _executor.Commands.ToArray();

            for (int i = 0; i < lines.Length; ++i)
                lines[i] = Regex.Replace(lines[i], "<.*?>", "");

            lines[index] = $"<color=red>{lines[index]}</color>";

            scriptInputManager.SetText(lines);
            scriptInfoUI.ChangeStatusText(true, _executor.Repeat);
        }

        private void HandleCommandEndOrAbort()
        {
            scriptInfoUI.ChangeStatusText(false, _executor.Repeat);

            if (_executor == null)
                return;

            string[] lines = _executor.Commands.ToArray();

            for (int i = 0; i < lines.Length; ++i)
                lines[i] = Regex.Replace(lines[i], "<.*?>", "");

            scriptInputManager.SetText(lines);
        }

        private void HandleSaveCommand(string[] scripts)
        {
            if (_executor == null)
                return;

            _scripts.Clear();

            for (int i = 0; i < scripts.Length; i++)
                if (!string.IsNullOrEmpty(scripts[i]))
                    _scripts.Add(scripts[i].Trim());

            _executor.Commands = _scripts.ToArray();
        }

        public bool GetWorldHitInfo(out RaycastHit hitInfo)
        {
            Camera mainCam = Camera.main;
            Debug.Assert(mainCam != null, "No main camera in this scene.");

            Ray cameraRay = mainCam.ScreenPointToRay(inputSO.MousePosition);
            if (Physics.Raycast(cameraRay, out hitInfo, mainCam.farClipPlane, checkTarget))
                return true;

            return false;
        }
    }
}
