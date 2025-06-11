using Assets.Work.Scripts.Executors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Entities
{
    public enum EntityAnimation
    {
        Idle,
        Move,
    }

    public class EntityAnimator : MonoBehaviour, IEntityComponent, IRunner
    {
        [Serializable]
        public class AnimationInformation
        {
            public EntityAnimation animationType;
            public string parameter;
        }

        [SerializeField] private Animator animator;
        [SerializeField] private AnimationInformation[] animationInfoList;

        private Dictionary<EntityAnimation, int> _animationSet;
        private EntityAnimation _currentAnimation;

        public void Initialize(Entity entity)
        {
            _animationSet = new Dictionary<EntityAnimation, int>();

            foreach (AnimationInformation animInfo in animationInfoList)
                _animationSet[animInfo.animationType] = Animator.StringToHash(animInfo.parameter);
        }

        public void AnimationChange(EntityAnimation entityAnimation)
        {
            SetParameter(_currentAnimation, false);
            SetParameter(_currentAnimation = entityAnimation, true);
        }

        public void SetParameter(EntityAnimation entityAnimation)
            => animator.SetTrigger(_animationSet[entityAnimation]);

        public void SetParameter(EntityAnimation entityAnimation, bool value)
            => animator.SetBool(_animationSet[entityAnimation], value);

        public void SetParameter(EntityAnimation entityAnimation, int value)
            => animator.SetInteger(_animationSet[entityAnimation], value);

        public void SetParameter(EntityAnimation entityAnimation, float value)
            => animator.SetFloat(_animationSet[entityAnimation], value);

        public void Kill()
        {
            AnimationChange(EntityAnimation.Idle);
        }
    }
}
