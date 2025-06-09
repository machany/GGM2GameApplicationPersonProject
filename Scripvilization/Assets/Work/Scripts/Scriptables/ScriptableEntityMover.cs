using Assets.Work.Scripts.Core._3DGrids;
using Assets.Work.Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace Assets.Work.Scripts.Scriptables
{
    public class ScriptableEntityMover : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private NodeType[] canMoveNodes;

        private ScriptableEntity _owner;
        private EntityAnimator _animator;

        private int _canMoveNodeTypes;

        public void Initialize(Entity entity)
        {
            _owner = entity as ScriptableEntity;
            Debug.Assert(_owner != null, "Can't cast to ScriptableEntity from entity.");

            SetNodeTypes(out _canMoveNodeTypes, canMoveNodes);

            _animator = entity.GetCompo<EntityAnimator>();
            _owner.OnMove += MoveTo;
        }

        private void OnDestroy()
        {
            _owner.OnMove -= MoveTo;
        }

        public virtual bool MoveTo(GridNode target, float duration)
        {
            if (((int)target.nodeType & _canMoveNodeTypes) > 0)
            {
                Vector3 direction = target.center - _owner.Object.transform.position;
                if (direction.sqrMagnitude < 0.001f)
                    return false;

                Quaternion rotation = _owner.Object.transform.rotation;
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                // 좀 더 깔끔하게 하고싶은데..
                // DOTween에서 벗어나서 목적지가 정해지면 그 쪽으로 움직이게
                // 회전도 마찬가지로
                // move, turn 함수에서 연산하고 update하게
                // 시간이 오래걸릴듯
                float rotateDuration = duration / 3;

                _owner.Object.transform.DORotateQuaternion(lookRotation, rotateDuration)
                    .OnComplete(() => Move(target.center, rotateDuration * 2));

                return true;
            }
            return false;
        }

        private void Move(Vector3 target, float duration)
        {
            _animator.AnimationChange(EntityAnimation.Move);
            _owner.Object.transform.DOMove(target, duration)
                        .SetEase(Ease.InSine)
                        .OnComplete(() =>
                        {
                            _animator.AnimationChange(EntityAnimation.Idle);
                            _owner.Complete();
                        });
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
