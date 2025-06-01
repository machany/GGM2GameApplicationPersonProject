using Assets.Work.Scripts.Entities;
using DG.Tweening;
using UnityEngine;

namespace Assets.Work.Scripts.Scriptables
{
    public class ScriptableEntityMover : MonoBehaviour, IEntityComponent
    {
        private ScriptableEntity _owner;

        public void Initialize(Entity entity)
        {
            _owner = entity as ScriptableEntity;
            Debug.Assert(_owner != null, "Can't cast to ScriptableEntity from entity.");
        }

        public virtual void MoveTo(Vector3 targetPosition, float duration)
        {
            _owner.Object.transform.DOMove(targetPosition, duration)
                        .SetEase(Ease.InOutBack)
                        .OnComplete(() => _owner.Complete());
        }
    }
}
