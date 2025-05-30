using System;
using System.Collections.Generic;

namespace AgamaLibrary.Comperers
{
    // old
    /*public class GreaterComparer<T> : IComparer<T>
    {
        private readonly Func<T, int> _keySelector; // ���� �� ����
        
        public GreaterComparer(Func<T, int> keySelector) // �Ű����� T�� int������
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector)); // null�̸� �������, �ƴϸ� ����
        }

        public int Compare(T x, T y)
        {
            if (x is null || y is null)
                throw new ArgumentNullException("�Էµ� ������ �ϳ��� null�Դϴ�."); // ��������

            return _keySelector(x).CompareTo(_keySelector(y));
        }
    }*/

    public class GreaterComparer<T> : IComparer<T> where T : notnull, IComparable<T>
    {
        public static GreaterComparer<T> Default;

        public int Compare(T x, T y)
        {
            return x.CompareTo(y);
        }
    }
}
