using MethodArchiveSystem;
using System;

namespace Assets.Work.Scripts.Executors
{
    public interface IExecutor : IArchivedMethods 
    {
        public bool Repeat { get; set; }
        public string[] Commands { get; set; }

        public bool CommandLock { get; set; }

        public event Action<int> OnCommandExecuted;
        public event Action OnCommandEndOrAbort;

        public void ExecuteCommands();
        public void Abort(); // 실행 중인 현재 자신이 실행 중인 명령어 중단
    }
}
