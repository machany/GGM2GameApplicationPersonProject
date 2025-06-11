using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Core.Inputs;
using Assets.Work.Scripts.Executors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Work.Scripts.Core.Managers
{
    public class ExecutorManager : MonoBehaviour
    {
        [SerializeField] private InputSO inputSO;
        [SerializeField] private EventChannelSO stageEventChannel;
        [SerializeField] private float resetThreshold = 1;

        private List<IExecutor> _executors;
        private List<IRunner> _runners;

        private float _lastResetClickTime;

        private void Awake()
        {
            _executors = GetObjectsByTypeFromMonoBehaviour<IExecutor>()
                .ToList();

            _runners = GetObjectsByTypeFromMonoBehaviour<IRunner>()
                .ToList();

            inputSO.OnResetStatusEvent += HandleResetKeyPressed;
        }

        private void OnDestroy()
        {
            inputSO.OnResetStatusEvent -= HandleResetKeyPressed;
        }

        private void HandleResetKeyPressed(bool status)
        {
            if (status)
            {
                _lastResetClickTime = Time.time;
                return;
            }
            else
            {
                if (Time.time - _lastResetClickTime < resetThreshold)
                    return;
            }

            AbortAllExecutors();
            KillAllRunners();
            stageEventChannel.InvokeEvent(StageEvents.ResetStageEvent);
        }

        private void AbortAllExecutors()
        {
            foreach (IExecutor executor in _executors)
            {
                executor.Repeat = false;
                executor.Abort();
            }
        }

        private void KillAllRunners()
        {
            foreach (IRunner runner in _runners)
                if (runner != null && (runner as MonoBehaviour).gameObject.activeSelf)
                    runner.Kill();
        }

        private IEnumerable<T> GetObjectsByTypeFromMonoBehaviour<T>()
        {
            MonoBehaviour[] monoes = GameObject.FindObjectsOfType<MonoBehaviour>(true);
            List<T> result = new List<T>();
            foreach (MonoBehaviour mono in monoes)
                if (mono.TryGetComponent(out T comp))
                    result.Add(comp);
            return result;
        }
    }
}
