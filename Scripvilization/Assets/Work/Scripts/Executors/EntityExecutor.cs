using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Scriptables;
using UnityEngine;

namespace Assets.Work.Scripts.Executors
{
    public class EntityExecutor : Executor
    {
        [Header("Default Setting")]
        // IScriptable로 가져오고 싶은데 인터페이스는 직렬화 안 되니, EntityExcutor니까 ScriptableEntity에 의존 해도 괜찮을 듯.
        [SerializeField] private ScriptableEntity scriptableOwner;
        [SerializeField] private EventChannelSO stageEventChannel;

        protected override void Awake()
        {
            base.Awake();

            _scriptableOwner = scriptableOwner;
            stageEventChannel.AddListener<StageClearEvent>(HandleStageClearEvent);
        }

        protected override void OnDestroy()
        {
            stageEventChannel.RemoveListener<StageClearEvent>(HandleStageClearEvent);

            base.OnDestroy();
        }

        private void HandleStageClearEvent(StageClearEvent @event)
        {
            Abort();
        }
    }
}
