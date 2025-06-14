using Assets.Work.Scripts.Sriptable;

namespace Assets.Work.Scripts.Executors
{
    // 후에 뭐가 추가 될 가능성 높음.
    public class FieldExecutor : Executor
    {
        protected override void Awake()
        {
            base.Awake();

            _scriptableOwner = transform.GetComponent<IScriptable>(); ;
        }
    }
}
