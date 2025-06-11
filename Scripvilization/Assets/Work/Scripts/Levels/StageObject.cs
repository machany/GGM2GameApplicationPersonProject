using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Core.Managers;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.Scripts.Levels
{
    [DefaultExecutionOrder(10)]
    public class StageObject : MonoBehaviour
    {
        [SerializeField] private EventChannelSO stageEventChannel;
        [SerializeField] private ObjectFinder gridManagerFinder;
        [SerializeField] private bool needResetPos;
        [SerializeField] private float fadeTime;
        [SerializeField] private int[] activeStages;

        private Vector3 _resetPos;
        private Quaternion _resetRotation;

        private HashSet<int> _activeStageSet;

        private bool _active;
        private Vector3 _originScale;
        private GridManager _gridManager;

        private void Awake()
        {
            _activeStageSet = new HashSet<int>();

            foreach (int stage in activeStages)
            {
                _activeStageSet.Add(stage);
            }

            _gridManager = gridManagerFinder.GetObject<GridManager>();

            _originScale = transform.localScale;

            if (needResetPos)
            {
                _resetPos = transform.position;
                _resetRotation = transform.rotation;
            }

            transform.localScale = Vector3.zero;

            stageEventChannel.AddListener<NextStageEvent>(HandleStageClear);
            if (needResetPos)
                stageEventChannel.AddListener<ResetStageEvent>(HandleResetEvent);

            _active = false;
            gameObject.SetActive(false);
            _gridManager.BakeNode(_gridManager.Grid.GetWorldPositionToNode(transform.position));
        }

        private void OnDestroy()
        {
            stageEventChannel.RemoveListener<NextStageEvent>(HandleStageClear);
            if (needResetPos)
                stageEventChannel.RemoveListener<ResetStageEvent>(HandleResetEvent);
        }

        private void HandleResetEvent(ResetStageEvent @event)
        {
            transform.position = _resetPos;
            transform.rotation = _resetRotation;
        }

        private void HandleStageClear(NextStageEvent @event)
        {
            if (needResetPos)
            {
                _resetPos = transform.position;
                _resetRotation = transform.rotation;
            }

            if (_activeStageSet.Contains(@event.stage))
                Active();
            else
                Deactive();
        }

        private void Active()
        {
            if (_active)
                return;

            _active = true;
            gameObject.SetActive(true);
            transform.DOScale(_originScale, fadeTime)
                .OnComplete(() =>
                    _gridManager.BakeNode(_gridManager.Grid.GetWorldPositionToNode(transform.position)));
        }

        private void Deactive()
        {
            if (!_active)
                return;

            _active = false;
            transform.DOScale(Vector3.zero, fadeTime)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    _gridManager.BakeNode(_gridManager.Grid.GetWorldPositionToNode(transform.position));
                });
        }

        // 없는게 더 이쁜듯
        private float EaseOutIn(float time, float duration, float overshootOrAmplitude, float period)
        {
            float t = time / duration;

            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;

            if (t < 0.5f)
                return 0.5f * (1 - Mathf.Pow(2f, -5 * t));
            else
                return 0.5f + 0.5f * Mathf.Pow(2f, 5 * (t - 1));
        }

#if UNITY_EDITOR

        public void ApplyLevel(bool on, int level)
        {
            bool contains = on;
            foreach (int i in activeStages)
                if (i == level)
                {
                    contains = true;
                    break;
                }

            gameObject.SetActive(contains);
        }

#endif
    }
}
