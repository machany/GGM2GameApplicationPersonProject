using AgamaLibrary.Unity.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Work.Scripts.Core.Events
{
    public class StageEvents
    {
        public static NextStageEvent NextStageEvent = new NextStageEvent();
        public static StageClearEvent StageClearEvent = new StageClearEvent();
        public static ResetStageEvent ResetStageEvent = new ResetStageEvent();
        public static ArrivedGoalEvent ArrivedGoalEvent = new ArrivedGoalEvent();
        public static LeavedGoalEvent LeavedGoalEvent = new LeavedGoalEvent();
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
}
