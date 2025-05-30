using System.Collections.Generic;
using System.Linq;

namespace AgamaLibrary.Methods
{
    public static class LidraryMethod
    {
        #region ICollection

        /// <summary>
        /// IColliction Ÿ�Կ� value�� �ߺ��Ǵ��� Ȯ���ϰ� �߰��մϴ�.
        /// </summary>
        /// <param name="value">�߰��� ���Դϴ�.</param>
        /// <returns>�߰��� �����ϸ� true, �����ϸ� false�� ��ȯ�մϴ�.</returns>
        public static bool TryAdd<T>(this ICollection<T> range, T value)
        {
            if (range.Contains(value))
                return false;

            range.Add(value);
            return true;
        }

        public static bool MargeTo<T>(this ICollection<T> margeTarget, ICollection<T> margeValue)
        {
            bool success = false;

            foreach (T item in margeValue)
            {
                if (item != null && !margeTarget.Contains(item))
                {
                    success = true;
                    margeTarget.Add(item);
                }
            }

            return success;
        }

        #endregion

        #region Dictionary

        /// <summary>
        /// ��ųʸ��� �־��� Ű�� ���� �߰��ϰų�, Ű�� �̹� ������ ��� ���� ������Ʈ�մϴ�.
        /// </summary>
        /// <remarks>
        /// Ű�� �̹� �����ϸ� ���� ���� �� ������ ��ü�ϸ�, �������� ������ �� Ű�� ���� �߰��մϴ�.
        /// </remarks>
        public static void AddOrUpdate<K, V>(this Dictionary<K, V> dict, K key, V value)
            => dict[key] = value;

        /// <summary>
        /// �־��� ���� ���� ù ��° Ű�� ã���ϴ�.
        /// </summary>
        /// <param name="dict">���� �˻��� ��ųʸ�</param>
        /// <param name="val">ã���� �ϴ� ��</param>
        /// <returns>
        /// ã�� Ű�� ��ȯ�մϴ�. ���� �������� ���� ���, �⺻��(<typeparamref name="K"/>)�� ��ȯ�մϴ�.
        /// </returns>
        /// <remarks>
        /// �� �޼���� ���� �ߺ��� ��� ù ��°�� �߰ߵ� Ű�� ��ȯ�մϴ�.
        /// </remarks>
        public static K FindFirstKeyByValue<K, V>(this Dictionary<K, V> dict, V val)
            => dict.FirstOrDefault(entry => EqualityComparer<V>.Default.Equals(entry.Value, val)).Key;

        /// <summary>
        /// �־��� ��ųʸ��� ������ŵ�ϴ�. �� Ű�� ���� ��ȯ�Ͽ� ���ο� ��ųʸ��� �����մϴ�.
        /// </summary>
        /// <param name="dict">������ų ���� ��ųʸ�</param>
        /// <returns>Ű�� ���� ������ �� ��ųʸ�</returns>
        /// <remarks>
        /// ���� ��ųʸ����� ���� ������ ��쿡�� ��ȿ�ϰ� �۵��մϴ�.
        /// ���� �ߺ��Ǹ� ������ ���� Ű�� ����˴ϴ�.
        /// </remarks>
        public static Dictionary<V, K> Invert<K, V>(this Dictionary<K, V> dict)
            => dict.ToDictionary(valuePair => valuePair.Value, valuePair => valuePair.Key);

        /// <summary>
        /// �� ���� ��ųʸ��� �����մϴ�.
        /// </summary>
        /// <param name="target">���յ� ��� ��ųʸ�</param>
        /// <param name="adder">������ ��ųʸ�</param>
        /// <param name="firstTarget">
        /// true�� ���, Ű�� �ߺ��� �� ��� ��ųʸ��� ���� �����մϴ�.
        /// false�� ���, �߰��� ��ųʸ��� ������ ����ϴ�.
        /// </param>
        public static void MergeTo<K, V>(this Dictionary<K, V> target, Dictionary<K, V> adder, bool firstTarget = true)
        {
            foreach (K kValue in adder.Keys)
                target[kValue] = firstTarget && target.ContainsKey(kValue) ? target[kValue] : adder[kValue];

            target.Where(pair => EqualityComparer<V>.Default.Equals(pair.Value, pair.Value)).Select(pair => pair.Key).ToList();
        }

        /// <summary>
        /// ������ ���� ��ġ�ϴ� ���� �ִ� ��� Ű-���� ��ųʸ����� �����մϴ�.
        /// </summary>
        /// <param name="value">������ ���Դϴ�.</param>
        /// <remarks>
        /// �� �޼���� ������ ���� ��ġ�ϴ� Ű�� �ش� Ű�� ���� ��ųʸ����� ��� �����մϴ�.
        /// </remarks>
        public static void DeleteByValue<K, V>(this Dictionary<K, V> dict, V value)
            => dict.Where(pair => EqualityComparer<V>.Default.Equals(pair.Value, value)).Select(pair => pair.Key).ToList().ForEach(key => dict.Remove(key));

        /// <summary>
        /// ��ųʸ����� ������ ���� ��ġ�ϴ� ��� ���� �� ������ �����մϴ�.
        /// </summary>
        /// <param name="valueForChange">������ ���� ���Դϴ�.</param>
        /// <param name="value">���� ������ ���Դϴ�.</param>
        /// <remarks>
        /// �� �޼���� ��ųʸ����� <paramref name="valueForChange"/>�� ��ġ�ϴ� ���� ���� �׸��� ã�� �ش� ���� <paramref name="value"/>�� ������Ʈ�մϴ�.
        /// </remarks>
        public static void ChangeValueTo<K, V>(this Dictionary<K, V> dict, V valueForChage, V value)
            => dict.Where(pair => EqualityComparer<V>.Default.Equals(pair.Value, valueForChage)).Select(pair => pair.Key).ToList().ForEach(key => dict[key] = value);

        /// <summary>
        /// �־��� ���� ��ġ�ϴ� ��� Ű-�� ���� ã�� ���ο� ��ųʸ��� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="findValue">ã���� �ϴ� ��</param>
        /// <returns>���� ��ġ�ϴ� ��� Ű, ���� �����ϴ� ���ο� ��ųʸ�</returns>
        public static Dictionary<K, V> GetOverlapValue<K, V>(this Dictionary<K, V> dict, V findValue)
            => dict.Where(pair => EqualityComparer<V>.Default.Equals(pair.Value, findValue)).ToDictionary(pair => pair.Key, pair => pair.Value) ?? new Dictionary<K, V>();

        #endregion

        public static bool TryGetValue<T>(this HashSet<T> set, T equalValue, out T actualValue)
        {
            foreach (var item in set)
            {
                if (set.Comparer.Equals(item, equalValue))
                {
                    actualValue = item;
                    return true;
                }
            }
            actualValue = default!;
            return false;
        }
    }
}