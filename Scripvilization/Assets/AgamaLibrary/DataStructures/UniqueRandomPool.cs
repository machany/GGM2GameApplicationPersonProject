using System;
using System.Collections.Generic;
using System.Linq;

namespace AgamaLibrary.DataStructures
{
    /// <summary>
    /// ������ �����ϰ� �� ���� �ȿ��� ���� �ߺ����� �����ɴϴ�.
    /// </summary>
    public class UniqueRandomPool<T>
    {
        public event Action OnReset;

        private List<T> _range;
        private List<T> _values;

        private Random _random;

        private readonly bool _allowDuplicate;

        /// <summary>
        /// �ʱ�ȭ �� ������ �� ������ ��
        /// </summary>
        public int RangeCount
            => _range.Count;

        /// <summary>
        /// �ʱ�ȸ �Ǳ� ���� ���� ������ ��
        /// </summary>
        public int ValueCount
            => _values.Count;

        public IEnumerable<T> Values
            => _values;

        public IEnumerable<T> Range
            => _range;

        public UniqueRandomPool(IEnumerable<T> range = null, bool allowDuplicate = false, Random random = null)
        {
            _range = new List<T>();
            _values = new List<T>();

            _random = random ?? new Random();

            _allowDuplicate = allowDuplicate;

            if (range != null)
            {
                _range.AddRange(range);
                _values.AddRange(range);
            }
        }

        /// <summary>
        /// ������ �ʱ�ȭ�ϰ� ���� �����մϴ�.
        /// </summary>
        /// <param name="withValue">���� ���� �ʱ�ȭ ��ų�� �����Դϴ�. ��, Reset�� �����ʰ� �����մϴ�.</param>
        public void SetRange(T range, bool withValue = true)
        {
            ClearRange();
            _range.Add(range);

            if (withValue)
            {
                _values.Clear();
                _values.Add(range);
            }
        }

        /// <summary>
        /// ������ �ʱ�ȭ�ϰ� ���� �����մϴ�.
        /// </summary>
        /// <param name="withValue">���� ���� �ʱ�ȭ ��ų�� �����Դϴ�. ��, Reset�� �����ʰ� �����մϴ�.</param>
        public void SetRange(IEnumerable<T> range, bool withValue = true)
        {
            if (range == null)
                return;

            ClearRange();
            if (_allowDuplicate)
                _range.AddRange(range);
            else
            {
                _range.AddRange(range);
                _range = _range.Distinct().ToList();
            }

            if (withValue)
            {
                _values.Clear();
                _values.AddRange(_range);
            }
        }

        /// <summary>
        /// ��(<typeparamref name="T"/>)�� ������ �߰��մϴ�.
        /// </summary>
        /// <param name="value">�߰��� ���Դϴ�.</param>
        /// <param name="withValue">���� ���� �߰� ��ų�� �����Դϴ�.</param>
        /// <remarks>
        /// ���� �߰��� �� ���� ��쿡 ���� ���� ó���� ���ԵǾ� ���� �ʽ��ϴ�.
        /// </remarks>
        public bool AddRange(T range, bool withValue = true)
        {
            if (range == null)
                return false;

            if (_allowDuplicate || !_range.Contains(range))
            {
                _range.Add(range);

                if (withValue)
                    _values.Add(range);

                return true;
            }
            return false;
        }

        /// <summary>
        /// ������ ���� �÷����� ������ �߰��մϴ�.
        /// </summary>
        /// <param name="value">�߰��� ���� �÷����Դϴ�.</param>
        /// <param name="withValue">���� ���� �߰� ��ų�� �����Դϴ�.</param>
        /// <remarks>
        /// null ���� ���� ���� ó���� ���ԵǾ� ���� �ʽ��ϴ�.
        /// </remarks>
        public void AddRange(IEnumerable<T> range, bool withValue = true)
        {
            if (range == null)
                return;

            _range.AddRange(range);
            if (withValue)
                _values.AddRange(range);

            if (!_allowDuplicate)
            {
                _range = _range.Distinct().ToList();
                _values = _values.Distinct().ToList();
            }
        }

        /// <summary>
        /// ��(<typeparamref name="T"/>)�� �Ŀ� ���� ���� �߰��մϴ�.
        /// �߰��� ���� �ʱ�ȭ ���� �ʰ� ������ϴ�.
        /// </summary>
        /// <param name="value">�߰��� ���Դϴ�.</param>
        /// <remarks>
        /// ���� �߰��� �� ���� ��쿡 ���� ���� ó���� ���ԵǾ� ���� �ʽ��ϴ�.
        /// </remarks>
        public void AddValue(T value)
        {
            if (value == null)
                return;

            _values.Add(value);
        }

        /// <summary>
        /// ������ ���� �÷����� �Ŀ� ���� ���� �߰��մϴ�.
        /// �߰��� ���� �ʱ�ȭ ���� �ʰ� ������ϴ�.
        /// </summary>
        /// <param name="value">�߰��� ���� �÷����Դϴ�.</param>
        /// <remarks>
        /// null ���� ���� ���� ó���� ���ԵǾ� ���� �ʽ��ϴ�.
        /// </remarks>
        public void AddValue(IEnumerable<T> value)
        {
            if (value == null)
                return;

            _values.AddRange(value);
        }

        /// <summary>
        /// ������ ��(<typeparamref name="T"/>)�� �����մϴ�.
        /// </summary>
        /// <param name="withValue">���� ���� ���� ��ų�� �����Դϴ�.</param>
        public void RemoveRange(T value, bool withValue = true)
        {
            _range.Remove(value);

            if (withValue)
                _values.Remove(value);
        }

        /// <summary>
        /// ������ ��(<typeparamref name="T"/>)�� �����մϴ�.
        /// </summary>
        /// <param name="withValue">���� ���� ���� ��ų�� �����Դϴ�.</param>
        public void RemoveRange(IEnumerable<T> value, bool withValue = true)
        {
            if (value == null)
                return;

            foreach (T item in value)
            {
                _range.Remove(item);
                if (withValue)
                    _values.Remove(item);
            }
        }

        /// <summary>
        /// ������ �ʱ�ȭ�մϴ�.
        /// </summary>
        public void ClearRange()
            => _range.Clear();

        /// <summary>
        /// ���� ������ �ʱ�ȭ�մϴ�.
        /// </summary>
        public void Reset()
        {
            _values.Clear();
            _values.AddRange(_range);
            OnReset?.Invoke();
        }

        /// <summary>
        /// �������� ���� ���� T���� �����ɴϴ�.
        /// �� ������ �� ������ ������ ���� ���� �������� �ʽ��ϴ�.
        /// </summary>
        /// <returns>������ ��� ���� �� �⺻��(<typeparamref name="T"/>)�� ��ȯ�մϴ�.</returns>
        public T GetValue()
        {
            if (_values.Count <= 0)
                Reset();

            int randIndex = _random.Next(0, _values.Count);
            T value = _values[randIndex];
            _values.Remove(value);

            return value;
        }

        /// <summary>
        /// ���� ���� ��(<typeparamref name="T"/>)�� �������� �����ɴϴ�.
        /// </summary>
        /// <returns>��(<typeparamref name="T"/>)�� ��ȯ�մϴ�. ���� ������ �ƹ��͵� ���ٸ� default(<typeparamref name="T"/>)�� ��ȯ�մϴ�.</returns>
        public T GetRangeInValue()
        {
            if (_range.Count <= 0)
                return default;

            return _range[_random.Next(0, _range.Count)];
        }
    }
}
