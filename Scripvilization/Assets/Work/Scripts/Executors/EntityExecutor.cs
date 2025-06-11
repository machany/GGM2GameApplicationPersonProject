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
        [SerializeField] private EventChannelSO stageEventChannel;

        [Header("Command Setting")]
        [SerializeField] private int commandExecuteInterval = 1000; // 명령어 작동 간격
        [SerializeField] private bool ignoreAbortRepeat;

        // 생각해보니 시작상태에서 돌아다니는 얘가 필요할 듯
        [SerializeField] private bool _repeat; // 반복여부
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

        [SerializeField] private string[] _commands; // 명령어 목록
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
            _executing = false;
            _abort = false;

            stageEventChannel.AddListener<StageClearEvent>(HandleStageClearEvent);
        }

        private void OnDestroy()
        {
            stageEventChannel.RemoveListener<StageClearEvent>(HandleStageClearEvent);
        }

        private void Update()
        {
            if (Repeat && !_executing)
            {
                ExecuteCommands();
            }
        }

        public async void ExecuteCommands()
        {
            if (_executing || Commands == null || Commands.Length == 0)
                return;

            _abort = false;
            _executing = true;

            for (int i = 0; i < Commands.Length; i++)
            {
                await Task.Delay(commandExecuteInterval);

                // 명령 실행 전 중지
                if (_abort)
                {
                    Repeat = ignoreAbortRepeat || false;
                    _abort = false;
                    break;
                }

                commandExecuteChannel.InvokeEvent(CommandExecueteManageEvents.ExecuteCommandEvent.Initialize(Commands[i], scriptableOwner));
            }

            _executing = false;
        }

        public void Abort()
        {
            _abort = true;
        }

        private void HandleStageClearEvent(StageClearEvent @event)
        {
            Abort();
        }

#if UNITY_EDITOR

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
