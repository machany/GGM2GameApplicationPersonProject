using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Sriptable;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.Scripts.Core.Managers
{
    [DefaultExecutionOrder(-1)]
    public class ObjectManager : MonoBehaviour
    {
        [SerializeField] protected EventChannelSO objectMangeEventChannel;

        private Dictionary<string, IScriptable> _objectDict;

        private void Awake()
        {
            _objectDict = new Dictionary<string, IScriptable>();
            objectMangeEventChannel.AddListener<ChangeObjectEvent>(HandleChangeObjectEvent);
        }

        private void OnDestroy()
        {
            objectMangeEventChannel.RemoveListener<ChangeObjectEvent>(HandleChangeObjectEvent);
        }

        private void HandleChangeObjectEvent(ChangeObjectEvent @event)
        {
            if (@event.isUpLoad)
                TryAddObject(@event.name, @event.scriptable);
            else
                TryRemoveObject(@event.name);
            Debug.Log(@event.name);
        }

        public bool TryAddObject(string name, IScriptable obj)
        {
            return _objectDict.TryAdd(name, obj);
        }

        public bool TryRemoveObject(string name)
        {
            bool success = _objectDict.ContainsKey(name);

            if (success)
                _objectDict.Remove(name);

            return success;
        }

        public bool TryGetObject(string name, out IScriptable scriptable)
        {
            bool success = _objectDict.TryGetValue(name, out scriptable);
            return success;
        }
    }
}