using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Core.Inputs;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;

namespace Assets.Work.Scripts.Levels
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private EventChannelSO stageEventChannel;
        [SerializeField] private int clearDelay;
        [SerializeField] private int[] needClearGoals;

        private int _currentLevel;
        private int _currentArrivedGoal;

        private StageObject[] _stageObjects;

        private void Awake()
        {
            stageEventChannel.AddListener<ArrivedGoalEvent>(HandleArrivedGoalEvent);
            stageEventChannel.AddListener<LeavedGoalEvent>(HandleLeavedGoalEvent);

            _stageObjects = Resources.FindObjectsOfTypeAll<StageObject>();
        }

        private void Start()
        {
            _currentLevel = 1;
            stageEventChannel.InvokeEvent(StageEvents.NextStageEvent.Init(_currentLevel));
        }

        private void OnDestroy()
        {
            stageEventChannel.RemoveListener<ArrivedGoalEvent>(HandleArrivedGoalEvent);
            stageEventChannel.RemoveListener<LeavedGoalEvent>(HandleLeavedGoalEvent);
        }

        private void HandleResetKeyPressed()
        {
            stageEventChannel.InvokeEvent(StageEvents.ResetStageEvent);
        }

        private void HandleLeavedGoalEvent(LeavedGoalEvent @event)
        {
            --_currentArrivedGoal;
        }

        private void HandleArrivedGoalEvent(ArrivedGoalEvent @event)
        {
            if (_currentLevel > needClearGoals.Length)
                return;

            ++_currentArrivedGoal;

            if (_currentArrivedGoal >= needClearGoals[_currentLevel - 1])
                CheackClear();
        }

        private async void CheackClear()
        {
            ++_currentLevel;
            stageEventChannel.InvokeEvent(StageEvents.StageClearEvent);
            await Task.Delay(clearDelay);
            stageEventChannel.InvokeEvent(StageEvents.NextStageEvent.Init(_currentLevel));
        }

        private void ApplyLevel(int level)
        {
            foreach (StageObject stageObject in _stageObjects)
                stageObject.ApplyLevel(false, level);
        }

#if UNITY_EDITOR

        [Header("Editor")]
        [SerializeField] private int levelTest;
        [SerializeField] private bool allActiveTest;

        [ContextMenu("Apply Level")]
        private void ApplyLevelTest()
        {
            StageObject[] stageObjects = Resources.FindObjectsOfTypeAll<StageObject>();

            foreach (StageObject stageObject in stageObjects)
                stageObject.ApplyLevel(allActiveTest, levelTest);
        }

#endif
    }
}
