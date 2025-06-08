using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Core.Events;
using Assets.Work.Scripts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Levels
{
    public class GoalPoint : MonoBehaviour
    {
        [Header("Clear")]
        [SerializeField] private LayerMask whatIsEntity;
        [SerializeField] private EventChannelSO stageEventChannelSO;

        [Header("Wave Moing")]
        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private float moveTime;

        [SerializeField] private WaveMover waveMover;

        private void Awake()
        {
            stageEventChannelSO.AddListener<StageClearEvent>(HandleStageClear);
        }

        private void HandleStageClear(StageClearEvent @event)
        {
            waveMover.Move(moveDirection, moveTime);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (((1 << collision.gameObject.layer) & whatIsEntity) != 0)
            {
                stageEventChannelSO.InvokeEvent(StageEvents.ArrivedGoalEvent);
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (((1 << collision.gameObject.layer) & whatIsEntity) != 0)
            {
                stageEventChannelSO.InvokeEvent(StageEvents.LeavedGoalEvent);
            }
        }
    }
}
