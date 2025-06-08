using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
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
        [SerializeField] private float fadeTime;
        [SerializeField] private int[] activeStages;

        private HashSet<int> _activeStageSet;

        private bool _active;
        private Vector3 _originScale;

        private void Awake()
        {
            _activeStageSet = new HashSet<int>();

            foreach (int stage in activeStages)
            {
                _activeStageSet.Add(stage);
            }

            _originScale = transform.localScale;
            stageEventChannel.AddListener<StageClearEvent>(HandleStageClear);
        }

        private void OnDestroy()
        {
            stageEventChannel.RemoveListener<StageClearEvent>(HandleStageClear);
        }

        private void HandleStageClear(StageClearEvent @event)
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

            transform.DOScale(_originScale, fadeTime);
        }

        private void Deactive()
        {
            if (_active)
                return;

            transform.DOScale(Vector3.zero, fadeTime);
        }
    }
}
