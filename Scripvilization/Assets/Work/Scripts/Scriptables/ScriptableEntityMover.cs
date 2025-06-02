using Assets.Work.Scripts.Core._3DGrids;
using Assets.Work.Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace Assets.Work.Scripts.Scriptables
{
    public class ScriptableEntityMover : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private NodeType[] canMoveNodes;
        private int _canMoveNodeTypes;

        private ScriptableEntity _owner;

        public void Initialize(Entity entity)
        {
            _owner = entity as ScriptableEntity;
            Debug.Assert(_owner != null, "Can't cast to ScriptableEntity from entity.");

            SetNodeTypes(out _canMoveNodeTypes, canMoveNodes);
            Debug.Log(_canMoveNodeTypes);
        }

        public virtual void MoveTo(GridNode target, float duration)
        {
            Debug.Log((int)target.nodeType + " " + _canMoveNodeTypes);
            if (((int)target.nodeType & _canMoveNodeTypes) > 0)
            {
                _owner.Object.transform.DOMove(target.center, duration)
                            .SetEase(Ease.InOutBack)
                            .OnComplete(() => _owner.Complete());
            }
        }

        private void SetNodeTypes(out int nodeTypes, NodeType[] canMoveNodes)
        {
            nodeTypes = 0;
            foreach (NodeType nodeType in canMoveNodes)
            {
                nodeTypes |= (int)nodeType;
            }
        }
    }
}
