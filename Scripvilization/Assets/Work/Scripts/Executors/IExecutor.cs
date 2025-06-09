using MethodArchiveSystem;

namespace Assets.Work.Scripts.Executors
{
    public interface IExecutor : IArchivedMethods 
    {
        public bool Repeat { get; }
        public string[] Commands { get; }

        public void ExecuteCommands();
        public void Abort(); // 실행 중인 현재 자신이 실행 중인 명령어 중단
    }
}
