using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core._3DGrids;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Entities;
using Assets.Work.Scripts.Sriptable;
using EPOOutline;
using System;
using UnityEngine;

namespace Assets.Work.Scripts.Scriptables
{
    public class ScriptableEntity : Entity, IScriptable
    {
        [Header("Default Setting")]
        [SerializeField] protected EventChannelSO objectMangeEventChannel;
        [SerializeField] protected Outlinable outLine;
        [ColorUsage(true, true)]
        [SerializeField] private Color selectedColor;
        [ColorUsage(true, true)]
        [SerializeField] private Color notSelectedColor;

        public GameObject Object => gameObject; // 자기자신
        [SerializeField] private string _objectName;
        public string ObjectName
        {
            get => _objectName;
            set
            {
                string before = _objectName;
                _objectName = value.Replace(" ", "");
                objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(false, this, _objectName, before)); // 기존 등록 변경
            }
        }

        public Func<GridNode, float, bool> OnMove;

        protected virtual void OnEnable()
        {
            objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(false, this, ObjectName));
        }

        protected virtual void OnDisable()
        {
            objectMangeEventChannel.InvokeEvent(ObjectManageEvents.ChangeObjectEvent.Initialize(true, this, ObjectName));
        }

        public bool? MoveTo(GridNode node, float moveDuration)
        {
            return OnMove?.Invoke(node, moveDuration);
        }

        public virtual void Execute()
        {

        }

        public virtual void Complete()
        {

        }

        public void Selected()
        {
            outLine.OutlineParameters.Color = selectedColor;
        }

        public void UnSelected()
        {
            outLine.OutlineParameters.Color = notSelectedColor;
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
