using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Entities;
using Assets.Work.Scripts.Sriptable;
using UnityEngine;

namespace Assets.Work.Scripts.Scriptables
{
    public class ScriptableEntity : Entity, IScriptable
    {
        [Header("Default Setting")]
        [SerializeField] protected EventChannelSO objectMangeEventChannel;

        public GameObject Object => gameObject; // 자기자신
        [SerializeField] private string _objectName;
        public string ObjectName
        {
            get => _objectName;
            set
            {
                // 이름 검사 필요할 듯
                string before = _objectName;
                _objectName = value.Replace(" ", "");
                objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(false, this, _objectName, before)); // 기존 등록 변경
            }
        }

        protected virtual void OnEnable()
        {
            objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(false, this, ObjectName));
        }

        protected virtual void OnDisable()
        {
            objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(true, this, ObjectName));
        }


        public virtual void Execute()
        {

        }

        public virtual void Complete()
        {

        }

#if UNITY_EDITOR
        [Header("Test")]
        [SerializeField] private string objectNameTest;
        [ContextMenu("SetOptions Option")]
        private void SetOptions()
        {
            ObjectName = objectNameTest;
        }
#endif
    }
}
