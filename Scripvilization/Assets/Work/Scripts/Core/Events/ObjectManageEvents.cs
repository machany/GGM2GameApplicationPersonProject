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
        public IScriptable scriptable;
        public string newName;
        public string beforeName;

        public bool remove;

        public ChangeObjectEvent Initialize(bool remove, IScriptable scriptable, string newName, string beforeName = null)
        {
            this.remove = remove;

            this.scriptable = scriptable;
            this.newName = newName;
            this.beforeName = beforeName;

            return this;
        }
    }
}
