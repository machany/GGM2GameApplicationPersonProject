using Assets.Work.Scripts.Core.Inputs;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Work.Scripts.UI
{
    public class ScriptInputManager : MonoBehaviour
    {
        [SerializeField] private InputSO inputSO;

        [Header("Script Setting")]
        [SerializeField] private GameObject scriptInputBlockPrefab;
        [SerializeField] private ushort minScriptBlock;
        [SerializeField] private ushort maxScriptBlock;

        [Header("Pannel Setting")]
        [SerializeField] private RectTransform pannel;
        [SerializeField] private Vector2 visiblePosition;
        [SerializeField] private Vector2 hidePosition;
        [SerializeField] private float sildeTime;

        private Vector2 _pannelStartPosition;

        private List<ScriptInputBlock> _scriptInputBlockList;

        private ushort _scriptIndex;

        private void Awake()
        {
            _scriptInputBlockList = new List<ScriptInputBlock>();
            _pannelStartPosition = pannel.position;

            for (int i = 0; i < minScriptBlock; ++i)
                CreateScriptInputBlock();

            inputSO.OnSubmitPressed += HandleSubmintEvent;
        }

        private void OnDestroy()
        {
            DesubscribeScriptBlock(_scriptIndex);

            inputSO.OnSubmitPressed -= HandleSubmintEvent;
        }

        public void SetText(string[] value)
        {
            for (int i = 0; i < value.Length; ++i)
            {
                if (_scriptInputBlockList.Count >= maxScriptBlock)
                    return;

                if (i + 1 >= _scriptInputBlockList.Count)
                    CreateScriptInputBlock();

                _scriptInputBlockList[i].SetText(value[i]);
            }
        }

        [ContextMenu("Open")]
        public void Open()
        {
            Silde(false);

            SetInput(true);
            _scriptIndex = 0;
            SubscribeScriptBlock(_scriptIndex);
            _scriptInputBlockList[_scriptIndex].Select();
        }

        [ContextMenu("Close")]
        public void Close()
        {
            Silde(true);

            SetInput(false);
            DesubscribeScriptBlock(_scriptIndex);
        }

        // 분리 필요?
        private void SetInput(bool on)
        {
            inputSO.SetPlayerEnabled(!on);
            inputSO.SetScriptEnabled(on);
        }

        // 분리 필요
        private void Silde(bool hide)
        {
            if (hide)
                pannel.DOMove(_pannelStartPosition + hidePosition, sildeTime).SetEase(Ease.Linear).OnComplete(() => pannel.gameObject.SetActive(false));
            else
            {
                pannel.gameObject.SetActive(true);
                pannel.DOMove(_pannelStartPosition + visiblePosition, sildeTime).SetEase(Ease.Linear);
            }
        }

        private void CreateScriptInputBlock()
        {
            ScriptInputBlock scriptInputBlock = Instantiate(scriptInputBlockPrefab).GetComponent<ScriptInputBlock>();

            scriptInputBlock.transform.SetParent(transform);
            scriptInputBlock.transform.SetSiblingIndex(0);

            _scriptInputBlockList.Add(scriptInputBlock);
        }

        private void SubscribeScriptBlock(int index)
        {
            _scriptInputBlockList[index].OnDeselectEvent += HandleDeselectEvent;
        }

        private void DesubscribeScriptBlock(int index)
        {
            _scriptInputBlockList[index].OnDeselectEvent -= HandleDeselectEvent;
        }

        private void HandleSubmintEvent()
        {
            DesubscribeScriptBlock(_scriptIndex);

            if (++_scriptIndex >= _scriptInputBlockList.Count)
            {
                if (_scriptInputBlockList.Count >= maxScriptBlock)
                    return;

                CreateScriptInputBlock();
            }

            SubscribeScriptBlock(_scriptIndex);
            _scriptInputBlockList[_scriptIndex].Select();
        }

        private void HandleDeselectEvent(string script)
        {
                Close();
        }
    }
}
