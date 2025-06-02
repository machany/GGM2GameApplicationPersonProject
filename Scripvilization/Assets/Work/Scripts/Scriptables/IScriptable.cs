using UnityEngine;

namespace Assets.Work.Scripts.Sriptable
{
    public interface IScriptable
    {
        public GameObject Object { get; }
        public string ObjectName { get; }

        

        public void Execute(); // 명령어의 타겟이 됨
        public void Complete(); // 해당 명령어에서 자유로워짐
    }
}
