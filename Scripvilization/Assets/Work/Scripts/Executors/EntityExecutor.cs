using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Core.Managers;
using Assets.Work.Scripts.Sriptable;
using MethodArchiveSystem;
using UnityEngine;

namespace Assets.Work.Scripts.Executors
{
    public class EntityExecutor : MonoBehaviour, IExecutor
    {
        [SerializeField] private ObjectFinder gridManagerFinder;

        private static GridManager _gridManager;

        private void Awake()
        {
            _gridManager = gridManagerFinder.GetObject<GridManager>();
        }

        [ArchiveMethod("이동")]
        public static void Move(IScriptable target, string[] direction)
        {
            _gridManager.Grid.GetWorldToNodePoint(target.Object.transform.position);
            target.Object.transform.position = Vector3.zero;
        }
    }
}
