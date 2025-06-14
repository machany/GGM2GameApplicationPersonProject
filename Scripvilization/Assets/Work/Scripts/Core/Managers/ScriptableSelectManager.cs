using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Core.Inputs;
using Assets.Work.Scripts.Executors;
using Assets.Work.Scripts.Sriptable;
using Assets.Work.Scripts.UI;
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
        private IExecutor Executor
        {
            get => _executor;
            set => _executor = value ?? defaultExecutor;
        }

        private IScriptable _defaultScriptable;
        private IScriptable _scriptable;
        private IScriptable Scriptable
        {
            get => _scriptable;
            set => _scriptable = value ?? _defaultScriptable;
        }
        private List<string> _scripts;

        private void Awake()
        {
            inputSO.OnMouseSelectedStatusEvent += HandleMouseClickEvent;
            scriptInputManager.OnSaveCommand += HandleSaveCommand;

            scriptInfoUI.OnAbortButtonClick += HandleAbortButtonClick;
            scriptInfoUI.OnExecuteButtonClick += HandleExecuteButtonClick;
            scriptInfoUI.OnStatusChangeButtonClick += HandleStatusChangeButtonClick;

            scriptInfoUI.OnScriptableNameChange += HandleScriptableNameChange;

            _defaultScriptable = defaultExecutor.GetComponent<IScriptable>();

            _scripts = new List<string>();
            _executor = defaultExecutor;
        }

        private void OnDestroy()
        {
            inputSO.OnMouseSelectedStatusEvent -= HandleMouseClickEvent;
            scriptInputManager.OnSaveCommand -= HandleSaveCommand;

            scriptInfoUI.OnAbortButtonClick -= HandleAbortButtonClick;
            scriptInfoUI.OnExecuteButtonClick -= HandleExecuteButtonClick;
            scriptInfoUI.OnStatusChangeButtonClick -= HandleStatusChangeButtonClick;

            scriptInfoUI.OnScriptableNameChange -= HandleScriptableNameChange;

            if (Executor != null)
            {
                Executor.OnCommandExecuted -= HandleCommandExecuted;
                Executor.OnCommandEndOrAbort -= HandleCommandEndOrAbort;
                Executor.OnCommandError -= HandleCommandError;
            }
        }

        private void HandleScriptableNameChange(string name)
        {
            if (Scriptable == null)
                return;

            if (EqualityComparer<IExecutor>.Default.Equals(Executor, defaultExecutor))
                return;

            Scriptable.ObjectName = name;
            scriptInfoUI.SetScriptableObjectName(Scriptable.ObjectName);
        }

        private void HandleAbortButtonClick()
        {
            Executor.Abort();
        }

        private void HandleExecuteButtonClick()
        {
            Executor.ExecuteCommands();
            scriptInputManager.SetText(Executor.Commands);
        }

        private void HandleStatusChangeButtonClick()
        {
            Executor.Repeat = !Executor.Repeat;
            scriptInfoUI.ChangeStatusText(false, Executor.Repeat);
        }

        private void HandleMouseClickEvent(bool status)
        {
            if (!status)
                return;

            if (!GetWorldHitInfo(out RaycastHit hitInfo))
            {
                scriptInputManager.Close();
                Scriptable?.UnSelected();
                Executor = defaultExecutor;
                scriptInfoUI.SetScriptableObjectName(defaultExecutorName);
                scriptInputManager.SetText(Executor.Commands);
                return;
            }

            // 기존에 선택된 오브젝트 선택취소 후 새 오브젝트 선택
            Scriptable?.UnSelected();
            Scriptable = hitInfo.collider.GetComponent<IScriptable>();

            if (Scriptable != null)
            {
                Scriptable?.Selected();
                scriptInfoUI.SetScriptableObjectName(Scriptable.ObjectName);
                stageEventChannel.InvokeEvent(StageEvents.SelectScriptable.Init(hitInfo.collider.transform.position));
            }

            Executor.OnCommandExecuted -= HandleCommandExecuted;
            Executor.OnCommandEndOrAbort -= HandleCommandEndOrAbort;
            Executor.OnCommandError -= HandleCommandError;

            Executor = hitInfo.collider.GetComponent<IExecutor>();
            scriptInputManager.SetText(Executor.Commands);

            Executor.OnCommandExecuted += HandleCommandExecuted;
            Executor.OnCommandEndOrAbort += HandleCommandEndOrAbort;
            Executor.OnCommandError += HandleCommandError;
            scriptInputManager.Open();
        }

        private void HandleCommandExecuted(int index)
        {
            SetLineColor(index, "green");
        }

        private void HandleCommandError(int index)
        {
            SetLineColor(index, "red");
        }

        private void SetLineColor(int index, string color)
        {
            if (Executor == null || index >= Executor.Commands.Length || index < 0)
                return;

            string[] lines = Executor.Commands.ToArray();

            for (int i = 0; i < lines.Length; ++i)
                lines[i] = Regex.Replace(lines[i], "<.*?>", "");

            lines[index] = $"<color={color}>{lines[index]}</color>";

            scriptInputManager.SetText(lines);
            scriptInfoUI.ChangeStatusText(true, Executor.Repeat);
        }

        private void HandleCommandEndOrAbort()
        {
            scriptInfoUI.ChangeStatusText(false, Executor.Repeat);

            if (Executor == null)
                return;

            string[] lines = Executor.Commands.ToArray();

            for (int i = 0; i < lines.Length; ++i)
                lines[i] = Regex.Replace(lines[i], "<.*?>", "");

            scriptInputManager.SetText(lines);
        }

        private void HandleSaveCommand(string[] scripts)
        {
            if (Executor == null)
                return;

            _scripts.Clear();

            for (int i = 0; i < scripts.Length; i++)
                if (!string.IsNullOrEmpty(scripts[i]))
                    _scripts.Add(scripts[i].Trim());

            Executor.Commands = _scripts.ToArray();
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
