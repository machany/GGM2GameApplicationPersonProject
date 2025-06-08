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
        public static StageClearEvent StageClearEvent = new StageClearEvent();
        public static ArrivedGoalEvent ArrivedGoalEvent = new ArrivedGoalEvent();
        public static LeavedGoalEvent LeavedGoalEvent = new LeavedGoalEvent();
    }

    public class StageClearEvent : GameEvent
    {
        public int stage;

        public StageClearEvent Init(int stage)
        {
            this.stage = stage;
            return this;
        }
    }

    public class ArrivedGoalEvent : GameEvent
    {
    }

    public class LeavedGoalEvent : GameEvent
    {

    }
}
