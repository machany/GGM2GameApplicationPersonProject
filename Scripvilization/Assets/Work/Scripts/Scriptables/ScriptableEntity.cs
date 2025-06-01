using AgamaLibrary.DataStructures;
using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Entities;
using Assets.Work.Scripts.Sriptable;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Scriptables
{
    public class ScriptableEntity : Entity, IScriptable
    {
        [Header("Default Setting")]
        [SerializeField] protected EventChannelSO objectMangeEventChannel;
        [SerializeField] protected EventChannelSO commandExecuteChannel;
        [SerializeField] protected int commandExecuteInterval = 1000; // 명령어 작동 간격

        public GameObject Object => gameObject; // 자기자신
        [SerializeField] private string _objectName;
        public string ObjectName
        {
            get => _objectName;
            set
            {
                objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(false, _objectName, this)); // 기존 등록 해제 후
                _objectName = value.Replace(" ", "");
                objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(true, _objectName, this)); // 새로 등록
            }
        }

        public bool Repeat { get; set; } // 반복여부
        protected string[] _commands; // 명령어 목록
        public string[] Commands
        {
            get => _commands;
            set
            {
                _commands = value;
                Abort(); // 명령어 변경시 즉시 중단.
            }
        }

        protected bool _abort; // 중단 됨
        protected bool _executing; // 현재 명령어 사이클이 돌고 있는지 여부

        protected override void Awake()
        {
            base.Awake();

            Repeat = false;
            _abort = false;
            _executing = false;
        }

        protected virtual void OnEnable()
        {
            objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(true, ObjectName, this));
        }

        protected virtual void OnDisable()
        {
            objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(false, ObjectName, this));
        }

        protected virtual void Update()
        {
            if (Repeat && !_executing)
                ExecuteCommands();
        }

        protected virtual async void ExecuteCommands()
        {
            if (_executing || Commands == null)
                return;

            _executing = true;
            for (int i = 0; i < Commands.Length; i++)
            {
                if (_abort)
                {
                    _abort = false;
                    break;
                }

                commandExecuteChannel.InvokeEvent(CommandExecueteManageEvents.ExecuteCommandEvent.Initialize(Commands[i], this));
                await Task.Delay(commandExecuteInterval);
            }
            _executing = false;
        }

        public virtual void Execute()
        {

        }

        public virtual void Abort()
        {
            _abort = true;
        }

        public virtual void Complete()
        {

        }

#if UNITY_EDITOR

        [Header("Test")]
        [SerializeField] private string objectNameTest;
        [SerializeField] private bool repeatTest;
        [SerializeField] private string[] commandsTest;

        [ContextMenu("SetOptions Option")]
        private void SetOptions()
        {
            ObjectName = objectNameTest;
            Repeat = repeatTest;
            Commands = commandsTest;
        }

        [ContextMenu("Excute Comannd")]
        private void ExecuteCommand()
        {
            ExecuteCommands();
        }

#endif
    }
}
