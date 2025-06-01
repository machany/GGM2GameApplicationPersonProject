using AgamaLibrary.DataStructures;
using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Entities;
using Assets.Work.Scripts.Sriptable;
using UnityEngine;

namespace Assets.Work.Scripts.Scriptables
{
    public class ScriptableEntity : Entity, IScriptable
    {
        [SerializeField] protected EventChannelSO objectMangeEventChannel;

        public GameObject Object => gameObject;
        [field : SerializeField] // test
        public string ObjectName {  get; protected set; }

        public NotifyValue<bool> CanExecuteCommandState { get; protected set; }

        protected override void Awake()
        {
            base.Awake();

            CanExecuteCommandState = new NotifyValue<bool>();
        }

        protected virtual void OnEnable()
        {
            objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(true, ObjectName, this));
        }

        protected virtual void OnDisable()
        {
            objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(false, ObjectName, this));
        }

        public void Execute()
        {
        }

        public void Complete()
        {
        }
    }
}
