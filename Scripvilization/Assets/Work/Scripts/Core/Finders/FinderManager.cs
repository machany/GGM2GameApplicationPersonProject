using UnityEngine;

namespace Assets.Work.Scripts.Core.Finders
{
    [DefaultExecutionOrder(-5)]
    public class FinderManager : MonoBehaviour
    {
        [SerializeField] protected ObjectFinder finder;
        [SerializeField] protected GameObject findedTarget;

        protected virtual void Awake()
        {
            finder.Object = findedTarget;
        }
    }
}
