using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Scriptables;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Executors
{
    public class EntityExecutor : MonoBehaviour, IExecutor
    {
        [Header("Default Setting")]
        // IScriptable로 가져오고 싶은데 인터페이스는 직렬화 안 되니, EntityExcutor니까 ScriptableEntity에 의존 해도 괜찮을 듯.
        [SerializeField] private ScriptableEntity scriptableOwner;
        [SerializeField] private EventChannelSO commandExecuteChannel;

        [Header("Command Setting")]
        [SerializeField] private int commandExecuteInterval = 1000; // 명령어 작동 간격

        [SerializeField] // 이거 테스트용입니다. 변수는 테스트용이 아녜요.
        private bool _repeat; // 반복여부
        public bool Repeat
        {
            get => _repeat;
            set
            {
                _abort = false;

                _repeat = value;
                // 반복은 활성상태 상관없이 중단없음.
            }
        }

        private string[] _commands; // 명령어 목록
        public string[] Commands
        {
            get => _commands;
            set
            {
                _commands = value;
                Abort(); // 명령어 변경시 즉시 중단.
            }
        }

        private bool _abort; // 중단 됨
        private bool _executing; // 현재 명령어 사이클이 돌고 있는지 여부

        private void Awake()
        {
            Repeat = false;
            _executing = false;
            _abort = false;
        }

        private void Update()
        {
            if (Repeat && !_executing)
            {
                ExecuteCommands();
            }
        }

        private async void ExecuteCommands()
        {
            if (_executing || Commands == null)
                return;

            _abort = false;
            _executing = true;

            for (int i = 0; i < Commands.Length; i++)
            {
                // 명령 실행 전 중지
                if (_abort)
                {
                    Repeat = false;
                    _abort = false;
                    break;
                }

                commandExecuteChannel.InvokeEvent(CommandExecueteManageEvents.ExecuteCommandEvent.Initialize(Commands[i], scriptableOwner));
                await Task.Delay(commandExecuteInterval);
            }

            _executing = false;
        }

        public void Abort()
        {
            _abort = true;
        }

#if UNITY_EDITOR

        [Header("Test")]
        [SerializeField] private string[] commandsTest;

        [ContextMenu("Apply Comannd")]
        private void ApplyCommandTest()
        {
            Commands = commandsTest;
        }

        [ContextMenu("Excute Comannd")]
        private void ExecuteCommandTest()
        {
            ExecuteCommands();
        }

        [ContextMenu("Abort")]
        private void AbortTest()
        {
            Abort();
        }

#endif
    }
}
