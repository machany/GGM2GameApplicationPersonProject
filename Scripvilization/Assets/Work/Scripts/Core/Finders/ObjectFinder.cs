using UnityEngine;

namespace Assets.Work.Scripts.Core.Finders
{
    [CreateAssetMenu(fileName = "ObjectFinder", menuName = "SO/ObjecFinder", order = 0)]
    public class ObjectFinder : ScriptableObject
    {
        public GameObject Object { get; set; }

        public T GetObject<T>() where T : Component
        {
            if (Object.TryGetComponent(out T component))
                return component;

            return null;
        }
    }
}
