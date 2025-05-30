using UnityEngine;

namespace AgamaLibrary.Unity.DataStructures
{
    public static class LidraryUnityMethod
    {
        /// <summary>
        /// Transform���� <typeparamref name="T"/> Ÿ���� ������Ʈ�� �������ų� ������ �߰��մϴ�.
        /// </summary>
        /// <typeparam name="T">�������ų� �߰��� ������Ʈ Ÿ���Դϴ�.</typeparam>
        /// <param name="transform">������Ʈ�� �������ų� �߰��� Transform�Դϴ�.</param>
        /// <returns>ã�� �Ǵ� ���� �߰��� <typeparamref name="T"/> Ÿ���� ������Ʈ�Դϴ�.</returns>
        public static T TryGetOrAddComponent<T>(this Transform transform) where T : Component
            => transform.TryGetComponent(out T comp) ? comp : transform.gameObject.AddComponent<T>();

        /// <summary>
        /// Transform���� <typeparamref name="T"/> Ÿ���� ������Ʈ�� �������ų� ������ <typeparamref name="A"/> Ÿ���� ������Ʈ�� �߰��մϴ�.
        /// </summary>
        /// <typeparam name="T">�������ų� �߰��� ������Ʈ Ÿ���Դϴ�.</typeparam>
        /// <typeparam name="A"><typeparamref name="T"/>�� ����Ŭ���� Ÿ���Դϴ�.</typeparam>
        /// <param name="transform">������Ʈ�� �������ų� �߰��� Transform�Դϴ�.</param>
        /// <returns>ã�� <typeparamref name="T"/> �Ǵ� ���� �߰��� <typeparamref name="A"/>�� <typeparamref name="T"/> Ÿ������ �����ɴϴ�.</returns>

        public static T TryGetOrAddComponent<T, A>(this Transform transform)
            where A : Component, T
            => transform.TryGetComponent(out T comp) ? comp : transform.gameObject.AddComponent<A>();
    }
}