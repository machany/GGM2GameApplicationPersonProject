using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Sriptable;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Executors
{
    // 동작이 다른 executor를 상정하고 인터페이스와 분리하였었음.
    public abstract class Executor : MonoBehaviour, IExecutor
    {
        [Header("Command Setting")]
        [SerializeField] protected EventChannelSO commandExecuteChannel;
        [SerializeField] protected int commandExecuteInterval = 1000; // 명령어 작동 간격
        [SerializeField] protected string[] _commands; // 명령어 목록
        public string[] Commands
        {
            get => _commands;
            set
            {
                _commands = value;
                Abort(); // 명령어 변경시 즉시 중단.
            }
        }

        [Header("Repeat Setting")]
        // 생각해보니 시작상태에서 돌아다니는 얘가 필요할 듯
        [SerializeField] protected bool _repeat; // 반복여부
        public bool Repeat
        {
            get => _repeat;
            set
            {
                _repeat = value;
                // 반복은 활성상태 상관없이 중단없음.
                if (_repeat == true)
                    _abort = false;
            }
        }
        [SerializeField] protected bool ignoreAbortToChangeRepeat;

        public bool CommandLock { get; set; }

        protected bool _abort; // 중단 됨
        protected bool _executing; // 현재 명령어 사이클이 돌고 있는지 여부

        protected IScriptable _scriptableOwner = null;

        public event Action<int> OnCommandExecuted;
        public event Action OnCommandEndOrAbort;

        protected virtual void Awake()
        {
            _executing = false;
            _abort = false;
        }

        protected virtual void Update()
        {
            if (Repeat && !_executing)
                ExecuteCommands();
        }

        public async void ExecuteCommands()
        {
            if (_executing || Commands == null || Commands.Length == 0)
                return;

            _abort = false;
            _executing = true;

            for (int i = 0; i < Commands.Length; i++)
            {
                // 명령 실행 전 중지
                if (_abort)
                {
                    Repeat = ignoreAbortToChangeRepeat || false;
                    _abort = false;
                    OnCommandEndOrAbort?.Invoke();
                    break;
                }

                commandExecuteChannel.InvokeEvent(CommandExecueteManageEvents.ExecuteCommandEvent.Initialize(Commands[i], _scriptableOwner));
                OnCommandExecuted?.Invoke(i);

                await Task.Delay(commandExecuteInterval);
            }

            OnCommandEndOrAbort?.Invoke();
            _executing = false;
        }

        public void Abort()
            => _abort = true;

#if UNITY_EDITOR

        [ContextMenu("Excute Comannd")]
        protected virtual void ExecuteCommandTest()
        {
            ExecuteCommands();
        }

        [ContextMenu("Abort")]
        protected virtual void AbortTest()
        {
            Abort();
        }

#endif
    }
}
