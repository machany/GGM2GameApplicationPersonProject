using Assets.Work.Scripts.Sriptable;
using System;
using UnityEngine;

namespace Assets.Work.Scripts.Scriptables
{
    public class FieldScriptable : MonoBehaviour, IScriptable
    {
        [field : SerializeField] public GameObject Object { get; set; }
        [field : SerializeField] public string ObjectName { get; set; }

        public void Complete()
        {
        }

        public void Execute()
        {
        }

        public void Selected()
        {
        }

        public void UnSelected()
        {
        }
    }
}
