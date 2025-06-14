using AgamaLibrary.Unity.EventSystem;
using UnityEngine;

namespace Assets.Work.Scripts.Core.Events
{
    public class StageEvents
    {
        public static NextStageEvent NextStageEvent = new NextStageEvent();
        public static StageClearEvent StageClearEvent = new StageClearEvent();
        public static ResetStageEvent ResetStageEvent = new ResetStageEvent();
        public static ArrivedGoalEvent ArrivedGoalEvent = new ArrivedGoalEvent();
        public static LeavedGoalEvent LeavedGoalEvent = new LeavedGoalEvent();
        public static SelectScriptableEvent SelectScriptable = new SelectScriptableEvent();
    }

    public class StageClearEvent : GameEvent
    {
    }

    public class NextStageEvent : GameEvent
    {
        public int stage;

        public NextStageEvent Init(int stage)
        {
            this.stage = stage;
            return this;
        }
    }

    public class ResetStageEvent : GameEvent
    {
    }

    public class ArrivedGoalEvent : GameEvent
    {
    }

    public class LeavedGoalEvent : GameEvent
    {
    }

    public class SelectScriptableEvent : GameEvent
    {
        public Vector3 position;

        public SelectScriptableEvent Init(Vector3 position)
        {
            this.position = position;
            return this;
        }
    }
}
