using Assets.Work.Scripts.Core.Inputs;
using Assets.Work.Scripts.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Assets.Work.Scripts.Core.Managers
{
    public class ScriptInputUIManager : MonoBehaviour
    {
        [SerializeField] private InputSO inputSO;

        [Header("UI Setting")]
        [SerializeField] private RectTransform panel;
        [SerializeField] private ScriptInputBlock scriptInputBlock;

        [Header("Slide Setting")]
        [SerializeField] private Vector2 visiblePosition;
        [SerializeField] private Vector2 hidePosition;
        [SerializeField] private float slideTime;

        public Action<string[]> OnSaveCommand;

        private Vector2 _pannelStartPosition;

        private bool _opened = false;
        private bool _uiInput;

        private void Awake()
        {
            _pannelStartPosition = panel.position;

            scriptInputBlock.OnSelectedEvent += HandleSelectedEvent;
            inputSO.OnToggleScriptUIEvent += HandleToggleScriptUIEvent;

            SetUIInput(false);
        }

        private void OnDestroy()
        {
            scriptInputBlock.OnSelectedEvent -= HandleSelectedEvent;
            inputSO.OnToggleScriptUIEvent -= HandleToggleScriptUIEvent;

            SetUIInput(false);
        }

        private void HandleToggleScriptUIEvent()
        {
            if (_opened)
                Close();
            else
                Open();
        }

        private void HandleDeselectEvent(string scripts)
        {
            SaveScript(scripts);
        }

        private void HandleMouseClick()
        {
            if (!CheckUI())
                SetUIInput(false);
        }

        private void HandleSelectedEvent()
        {
            SetUIInput(true);
        }

        private void SetUIInput(bool on)
        {
            if (_uiInput == on)
                return;

            if (_uiInput = on)
            {
                scriptInputBlock.OnSaveTextEvent += HandleDeselectEvent;
                inputSO.OnMouseClickEvent += HandleMouseClick;
            }
            else
            {
                scriptInputBlock.OnSaveTextEvent -= HandleDeselectEvent;
                inputSO.OnMouseClickEvent -= HandleMouseClick;
            }

            inputSO.SetPlayerEnabled(!on);
            inputSO.SetScriptEnabled(on);
        }

        // 분리 필요
        [ContextMenu("Open")]
        public void Open()
        {
            if (_opened)
                return;

            _opened = true;
            Silde(!_opened);

            SetUIInput(_opened);
        }

        // 분리 필요
        [ContextMenu("Close")]
        public void Close()
        {
            if (!_opened)
                return;

            _opened = false;
            Silde(!_opened);

            SetUIInput(_opened);
        }

        // 분리 필요
        private void Silde(bool hide)
        {
            if (hide)
                panel.DOMove(_pannelStartPosition + hidePosition, slideTime).SetEase(Ease.Linear).OnComplete(() => panel.gameObject.SetActive(false));
            else
            {
                panel.gameObject.SetActive(true);
                panel.DOMove(_pannelStartPosition + visiblePosition, slideTime).SetEase(Ease.Linear);
            }
        }

        private bool CheckUI()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            bool checkOverlapUI = results.Count > 0;
            return checkOverlapUI;
        }

        private void SaveScript(string script)
        {
            scriptInputBlock.SetText(script.Split('\n'));

            script = Regex.Replace(script, "<.*?>", "");
            OnSaveCommand?.Invoke(script.Split('\n'));
        }

        public void SetText(string[] value)
        {
            scriptInputBlock.SetText(value);
        }
    }
}
