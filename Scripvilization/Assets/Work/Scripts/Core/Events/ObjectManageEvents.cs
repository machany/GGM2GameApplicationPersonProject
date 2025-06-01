using AgamaLibrary.Unity.EventSystem;
using Assets.Work.Scripts.Sriptable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Work.Scripts.Core.Events
{
    public class ObjectManageEvents
    {
        public static ChangeObjectEvent ChangeObjectEvent = new ChangeObjectEvent();
    }

    public class ChangeObjectEvent : GameEvent
    {
        public bool isUpLoad;
        public string name;
        public IScriptable scriptable;

        public ChangeObjectEvent Initialize(bool isUpLoad, string name, IScriptable scriptable)
        {
            this.isUpLoad = isUpLoad;
            this.name = name;
            this.scriptable = scriptable;

            return this;
        }
    }
}
