using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Sriptable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Work.Scripts.Core.Events
{
    public class CommandExecueteManageEvents
    {
        public static ExecuteCommandEvent ExecuteCommandEvent = new ExecuteCommandEvent();
    }

    public class ExecuteCommandEvent : GameEvent
    {
        public string command;
        public IScriptable scriptable;

        public ExecuteCommandEvent Initialize(string command, IScriptable scriptable)
        {
            this.command = command;
            this.scriptable = scriptable;
            return this;
        }
    }
}
