using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Sriptable;
using MethodArchiveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Work.Scripts.Core.Managers
{
    [DefaultExecutionOrder(-10)]
    public class CommandExecutor : MonoBehaviour // 유효성 검사, 명령의 대상 받기, 명령어 실행
    {
        [SerializeField] private ObjectFinder objectManagerFinder;
        [SerializeField] protected EventChannelSO commandExecuteChannel;
        [SerializeField] protected string objectCallMeKeyword;

        public Action<string> OnErrorEvent;

        private MethodArchive _methodArchive;
        private ObjectManager _objetManager;

        private void Awake()
        {
            _methodArchive = new MethodArchive();
            _methodArchive.ArchiveAllMethod();
            _objetManager = objectManagerFinder.GetObject<ObjectManager>();

            commandExecuteChannel.AddListener<ExecuteCommandEvent>(HandleExecuteCommand);
        }

        private void OnDestroy()
        {
            commandExecuteChannel.RemoveListener<ExecuteCommandEvent>(HandleExecuteCommand);
        }

        private void HandleExecuteCommand(ExecuteCommandEvent evt)
        {
            if (string.IsNullOrEmpty(evt.command) || evt.scriptable == null)
                return;

            string[] commands = evt.command.Split(' ');

            List<string> parameters = commands.ToList();

            string command = parameters[0];
            parameters.RemoveAt(0);

            if (EqualityComparer<string>.Default.Equals(parameters[0], objectCallMeKeyword))
                parameters[0] = evt.scriptable.ObjectName;

            TryInvoke(command, parameters.ToArray());
        }

        public bool TryInvoke(string command, params string[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                return false;

            if (!_objetManager.TryGetObject(parameters[0], out IScriptable scriptable))
                return false;

            if (parameters.Length <= 1)
                TryInvoke(scriptable.ObjectName, command, scriptable);
            else
            {
                List<object> param = parameters.ToList().Select(v => (object)v).ToList();
                param.RemoveAt(0);
                param.Insert(0, scriptable);

                TryInvoke(scriptable.ObjectName, command, param.ToArray());
            }
            // 배열로 넘기는 방식이 아닌, 하나하나 넘기는 방식은 없는 듯
            // 굳이 꼭 해야하면 동덕으로 델리케이트 만들어서 parameter수 만큼 동적으로 만드는 방식밖에 못 하는 듯

            return true;
        }

        public bool TryInvoke(string errorOwner, string command, params object[] parameters)
        {
            try
            {
                _methodArchive.Invoke(command, parameters);
                return true;
            }
            catch (MethodArchiveException ex)
            {
                OnErrorEvent?.Invoke(errorOwner);
                Debug.LogError($"{ex.message}\n{ex.exception.Message}");
                return false;
            }
        }

#if UNITY_EDITOR
        [Header("Test")]
        [SerializeField] private string command;
        [SerializeField] private string paremeter;

        [ContextMenu("Invoke Command")]
        private void TestInvokeCommand()
        {
            if (string.IsNullOrEmpty(command) || string.IsNullOrEmpty(paremeter))
                return;

            string[] paremeters = paremeter.Split(' ');

            TryInvoke(command, paremeters);
        }
#endif
    }
}
