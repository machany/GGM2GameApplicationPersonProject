using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Core.Managers;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Levels
{
    public class StageObject : MonoBehaviour
    {
        [SerializeField] private EventChannelSO stageEventChannel;
        [SerializeField] private ObjectFinder gridManagerFinder;
        [SerializeField] private float fadeTime;
        [SerializeField] private int[] activeStages;
        [SerializeField] private bool startActive;

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
            stageEventChannel.AddListener<NextStageEvent>(HandleStageClear);
            gameObject.SetActive(startActive);
        }

        private void OnDestroy()
        {
            stageEventChannel.RemoveListener<NextStageEvent>(HandleStageClear);
        }

        private void HandleStageClear(NextStageEvent @event)
        {
            if (_activeStageSet.Contains(@event.stage))
                Active();
            else
                Deactive();
        }

        private void Active()
        {
            if (_active)
                return;

            gameObject.SetActive(true);
            transform.DOScale(_originScale, fadeTime)
                .OnComplete(() => _gridManager.BakeNode(_gridManager.Grid.GetWorldPositionToNode(transform.position)));
        }

        private void Deactive()
        {
            if (_active)
                return;

            transform.DOScale(Vector3.zero, fadeTime)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    _gridManager.BakeNode(_gridManager.Grid.GetWorldPositionToNode(transform.position));
                });
        }

#if UNITY_EDITOR

        public void TestApplyLevel(bool on, int level)
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
