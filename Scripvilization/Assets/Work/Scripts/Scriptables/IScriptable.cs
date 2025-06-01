using AgamaLibrary.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Sriptable
{
    public interface IScriptable
    {
        public GameObject Object { get; }
        public string ObjectName { get; }

        public NotifyValue<bool> CanExecuteCommandState { get; }

        public void Execute();
        public void Complete();
    }
}
