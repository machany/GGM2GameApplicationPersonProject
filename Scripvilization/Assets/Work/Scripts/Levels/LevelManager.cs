using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Levels
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private EventChannelSO stageEventChannel;
        [SerializeField] private int clearDelay;
        [SerializeField] private int[] needClearGoals;

        private int _currentLevel;
        private int _currentArrivedGoal;

        private void Awake()
        {
            stageEventChannel.AddListener<ArrivedGoalEvent>(HandleArrivedGoalEvent);
            stageEventChannel.AddListener<LeavedGoalEvent>(HandleLeavedGoalEvent);
        }

        private void Start()
        {
            _currentLevel = 1;
            stageEventChannel.InvokeEvent(StageEvents.StageClearEvent.Init(_currentLevel));
        }

        private void OnDestroy()
        {
            stageEventChannel.RemoveListener<ArrivedGoalEvent>(HandleArrivedGoalEvent);
            stageEventChannel.RemoveListener<LeavedGoalEvent>(HandleLeavedGoalEvent);
        }

        private void HandleLeavedGoalEvent(LeavedGoalEvent @event)
        {
            --_currentArrivedGoal;
        }

        private void HandleArrivedGoalEvent(ArrivedGoalEvent @event)
        {
            ++_currentArrivedGoal;

            if (_currentArrivedGoal >= needClearGoals[_currentLevel - 1])
                CheackClear();
        }

        private async void CheackClear()
        {
            await Task.Delay(clearDelay);
            if (_currentArrivedGoal >= needClearGoals[_currentLevel - 1])
            {
                ++_currentLevel;
                stageEventChannel.InvokeEvent(StageEvents.StageClearEvent.Init(_currentLevel));
            }
        }
    }
}
