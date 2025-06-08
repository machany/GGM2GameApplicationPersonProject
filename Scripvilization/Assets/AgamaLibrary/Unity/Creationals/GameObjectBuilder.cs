using AgamaLibrary.Unity.DataStructures;
using System;
using UnityEngine;

namespace AgamaLibrary.Unity.Creationals
{
    public class GameObjectBuilder
    {
        private GameObject targetGameObject;
        //private Dictionary<string, Action<GameObjectBuilder>> presets = new Dictionary<string, Action<GameObjectBuilder>>();
        //private readonly string presetDefaultKey;

        public GameObjectBuilder(GameObject obj = null)//, string presetDefult = "Defult")
        {
            // targetGameObject = obj ?? new GameObject();
            //presetDefaultKey = presetDefult;
        }

        public GameObjectBuilder StartBuild(GameObject obj = null)
        {
            targetGameObject = obj ?? new GameObject();
            return this;
        }

        public GameObjectBuilder SetName(string name)
        {
            UnityEngine.Debug.Assert(!string.IsNullOrEmpty(name), $"GameObject name can not be null or empty. name : {name}");
            targetGameObject.name = name;
            return this;
        }

        public GameObjectBuilder SetStatic(bool isStatic)
        {
            targetGameObject.isStatic = isStatic;
            return this;
        }

        public GameObjectBuilder SetTag(string tag)
        {
            UnityEngine.Debug.Assert(!string.IsNullOrEmpty(tag), $"GameObject tag can not be null or empty. tag : {tag}");
            targetGameObject.tag = tag;
            return this;
        }

        public GameObjectBuilder SetLayer(int layer)
        {
            targetGameObject.layer = layer;
            return this;
        }

        public GameObjectBuilder SetParent(Transform parent)
        {
            targetGameObject.transform.parent = parent;
            return this;
        }

        public GameObjectBuilder SetPosition(Vector3 position)
        {
            targetGameObject.transform.position = position;
            return this;
        }

        public GameObjectBuilder SetLocalPosition(Vector3 position)
        {
            targetGameObject.transform.localPosition = position;
            return this;
        }

        public GameObjectBuilder SetRotation(Vector3 rotation)
        {
            targetGameObject.transform.rotation = Quaternion.Euler(rotation);
            return this;
        }

        public GameObjectBuilder SetScale(float scale)
            => SetScale(Vector3.one * scale);

        public GameObjectBuilder SetScale(Vector3 scale)
        {
            targetGameObject.transform.localScale = scale;
            return this;
        }

        public GameObjectBuilder SetActive(bool active)
        {
            targetGameObject.SetActive(active);
            return this;
        }

        /// <summary>
        /// ���ǿ� ���� ���� �Ǵ� ��ü ������ �����մϴ�.
        /// </summary>
        /// <param name="condition">������ ������ �����Դϴ�.</param>
        /// <param name="setting">������ true�� ��� ������ �����Դϴ�.</param>
        /// <param name="elseSetting">������ false�� ��� ������ �����Դϴ�.</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder If(Func<bool> condition, Action<GameObjectBuilder> setting, Action<GameObjectBuilder> elseSetting = null)
        {
            bool shouldApplySetting = condition?.Invoke() ?? false;

            if (shouldApplySetting)
                setting?.Invoke(this);
            else
                elseSetting?.Invoke(this);

            return this;
        }

        /// <summary>
        /// ������ Ƚ����ŭ ������ �ݺ� �����մϴ�.
        /// </summary>
        /// <param name="loopCount">�ݺ��� Ƚ���Դϴ�.</param>
        /// <param name="setting">�ݺ����� ������ �����Դϴ�. ���� �ε����� �����մϴ�.</param>
        /// <param name="startNumber">�ݺ��� ������ �ε����Դϴ�. �⺻���� 0�Դϴ�.</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder For(int loopCount, Action<GameObjectBuilder, int> setting, int startNumber = 0)
        {
            for (int i = startNumber; i < startNumber + loopCount; ++i)
                setting?.Invoke(this, i);

            return this;
        }

        /// <summary>
        /// ������ ���� ���� ������ �ݺ� �����մϴ�.
        /// </summary>
        /// <param name="condition">�ݺ��� ����� �����Դϴ�.</param>
        /// <param name="setting">�ݺ����� ������ �����Դϴ�.</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder While(Func<bool> condition, Action<GameObjectBuilder> setting)
        {
            while (condition?.Invoke() ?? false)
                setting?.Invoke(this);

            return this;
        }

        /// <summary>
        /// ���� ó���� ������ ������ �۾��� �����մϴ�.
        /// </summary>
        /// <param name="tryAction">�õ��� �۾��Դϴ�.</param>
        /// <param name="catchAction">���� �߻� �� ������ �۾��Դϴ�. ���� ��ü�� ���ڷ� ���޹޽��ϴ�.</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder TryCatch(Action<GameObjectBuilder> tryAction, Action<GameObjectBuilder, Exception> catchAction = null)
        {
            try
            {
                tryAction?.Invoke(this);
            }
            catch (Exception ex)
            {
                catchAction?.Invoke(this, ex);
            }

            return this;
        }

        /// <summary>
        /// ��� GameObject�� ������ �����մϴ�.
        /// </summary>
        /// <param name="action">GameObject�� ������ �����Դϴ�.</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder ActObject(Action<GameObject> action)
        {
            action?.Invoke(targetGameObject);

            return this;
        }

        /// <summary>
        /// GameObject�� ������Ʈ�� �߰��մϴ�.
        /// </summary>
        /// <typeparam name="T">�߰��� ������Ʈ�� Ÿ��</typeparam>
        /// <param name="force">�߰��� ������Ʈ�� �־ �߰������� ���� �����Դϴ�.</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder TryAddComponent<T>(bool force = false) where T : Component
        {
            if (force || !targetGameObject.TryGetComponent(out T component))
                targetGameObject.AddComponent<T>();
            return this;
        }

        /// <summary>
        /// GameObject�� ������Ʈ�� �߰��մϴ�.
        /// </summary>
        /// <typeparam name="T">�߰��� ������Ʈ�� Ÿ��</typeparam>
        /// <param name="force">�߰��� ������Ʈ�� �־ �߰������� ���� �����Դϴ�.</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder TryAddComponent<T, A>() where A : Component, T
        {
            targetGameObject.transform.TryGetOrAddComponent<T, A>();
            return this;
        }

        /// <summary>
        /// GameObject�� ������ ������Ʈ�� ��������, �ش� ������Ʈ�� �ʿ��� ������ �մϴ�.
        /// </summary>
        /// <remarks>
        /// ���� ������Ʈ�� �������� ���ߴٸ�, ������Ʈ�� ������Ʈ�� �߰��ϰ� �����մϴ�.
        /// </remarks>
        /// <typeparam name="T">������ ������Ʈ�� Ÿ��</typeparam>
        /// <param name="func">������ ������Ʈ�� ���� ������ �����ϴ� �Լ�</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder SetComponent<T>(Action<T> func) where T : Component
        {
            func?.Invoke(targetGameObject.transform.TryGetOrAddComponent<T>());
            return this;
        }

        /// <summary>
        /// GameObject�� ������ ������Ʈ�� ��������, �ش� ������Ʈ�� �ʿ��� �۾��� �մϴ�.
        /// </summary>
        /// <remarks>
        /// ���� ������Ʈ�� �������� ���ߴٸ�, �ƹ��� ������ ���� �ʽ��ϴ�.
        /// </remarks>
        /// <typeparam name="T"> ������ ������Ʈ�� Ÿ��</typeparam>
        /// <param name="func">������ ������Ʈ�� ���� ������ �����ϴ� �Լ�</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder SetToGetComponent<T>(Action<T> func) where T : Component
        {
            if (targetGameObject.transform.TryGetComponent(out T component))
                func?.Invoke(component);
            return this;
        }

        /// <summary>
        /// �ڽ��� �߰��մϴ�.
        /// </summary>
        /// <param name="target">�߰��� �ڽ�</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder SetToChild(Transform target)
        {
            target.parent = targetGameObject.transform;
            return this;
        }

        /// <summary>
        /// �ڽ��� �߰��մϴ�.
        /// </summary>
        /// <param name="target">�߰��� �ڽ�</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder SetToChild(GameObject target)
        {
            return SetToChild(target.transform);
        }

        /*#region Presets

        /// <summary>
        /// ����� �������� Ű ����� ��ȯ�մϴ�.
        /// </summary>
        /// <returns>������ Ű ���</returns>
        public IEnumerable<string> GetPresetKeys()
        {
            return presets.Keys;
        }

        /// <summary>
        /// �⺻ Ű�� ����Ͽ� �������� �����մϴ�.
        /// </summary>
        /// <param name="preset">������ ������</param>
        public void SavePreset(Action<GameObjectBuilder> preset)
        {
            presets[presetDefaultKey] = preset;
        }

        /// <summary>
        /// ������ Ű�� ����Ͽ� �������� �����մϴ�.
        /// </summary>
        /// <param name="key">�������� ������ Ű</param>
        /// <param name="preset">������ ������</param>
        public void SavePreset(string key, Action<GameObjectBuilder> preset)
        {
            presets[key] = preset;
        }

        /// <summary>
        /// ������ Ű�� �������� �����մϴ�.
        /// </summary>
        /// <param name="key">������ �������� Ű</param>
        /// <exception cref="KeyNotFoundException">Ű�� �������� ���� ��� �߻�</exception>
        public void RemovePreset(string key)
        {
            if (!presets.Remove(key))
                throw new KeyNotFoundException($"Preset can't found key '{key}'.");
        }

        /// <summary>
        /// ������ Ű�� �ش��ϴ� �������� �����ϴ��� Ȯ���մϴ�.
        /// </summary>
        /// <param name="key">Ȯ���� �������� Ű</param>
        /// <returns>������ ���� ����</returns>
        public bool HasPreset(string key)
        {
            return presets.ContainsKey(key);
        }

        /// <summary>
        /// ������ Ű�� �������� �����մϴ�.
        /// </summary>
        /// <param name="key">������ �������� Ű (�ɼ�)</param>
        /// <returns>���� �ν��Ͻ�</returns>
        /// <exception cref="KeyNotFoundException">�������� �������� ������ �߻�</exception>
        public GameObjectBuilder ApplyPreset(string key = null)
        {
            key = key ?? presetDefaultKey;

            if (!presets.ContainsKey(key))
                throw new KeyNotFoundException($"Preset can't found key '{key}'.");
            presets[key](this);
            return this;
        }

#if UNITY_EDITOR
        public GameObjectBuilder DebugPresets()
        {
            foreach (var key in presets.Keys)
            {
                UnityEngine.Debug.Log($"Preset: {key}");
            }
            return this;
        }
#endif

        #endregion*/

#if UNITY_EDITOR
        public GameObjectBuilder Debug(object obj)
        {
            UnityEngine.Debug.Log(obj);
            return this;
        }

        /// <summary>
        /// ������ ����� ������ �����ϰ� Unity �ֿܼ� ����մϴ�.
        /// </summary>
        /// <param name="func">������ ����� ����</param>
        /// <returns>���� �ν��Ͻ�</returns>
        public GameObjectBuilder Debug(Action func)
        {
            func();
            return this;
        }
#endif


        /// <summary>
        /// GameObject�� �����ϰ� ���������� ���� ���¸� �����մϴ�.
        /// </summary>
        /// <param name="reset">true�� ���, ���带 �Ϸ��� �� ���� ���¸� ���ο� GameObject�� �����մϴ�.</param>
        /// <returns>������ GameObject.</returns>
        public GameObject Build(bool reset = false)
        {
            GameObject result = targetGameObject;

            if (reset)
                targetGameObject = new GameObject();

            return result;
        }
    }
}