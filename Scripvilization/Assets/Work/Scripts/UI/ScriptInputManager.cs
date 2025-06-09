using Assets.Work.Scripts.Core.Inputs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Work.Scripts.UI
{
    public class ScriptInputManager : MonoBehaviour
    {
        [SerializeField] private InputSO inputSO;
        [SerializeField] private ScriptInputBlock scriptInputBlock;

        [Header("Slide Setting")]
        [SerializeField] private RectTransform panel;
        [SerializeField] private Vector2 visiblePosition;
        [SerializeField] private Vector2 hidePosition;
        [SerializeField] private float slideTime;

        private Vector2 _pannelStartPosition;

        private bool _opened = false;

        private void Awake()
        {
            _pannelStartPosition = panel.position;
        }

        private void OnDestroy()
        {
            SetInput(false);
        }

        public void SetText(string[] value)
        {
            scriptInputBlock.SetText(value);
        }

        // 분리 필요
        [ContextMenu("Open")]
        public void Open()
        {
            if (_opened)
                return;

            _opened = true;
            Silde(!_opened);

            SetInput(_opened);
        }

        // 분리 필요
        [ContextMenu("Close")]
        public void Close()
        {
            if (!_opened)
                return;

            _opened = false;
            Silde(_opened);

            SetInput(!_opened);
        }

        private void SetInput(bool on)
        {
            if (on)
            {
                scriptInputBlock.OnDeselectEvent += HandleDeselectEvent;
                inputSO.OnMouseClickEvent += HandleMouseClick;
            }
            else
            {
                scriptInputBlock.OnDeselectEvent -= HandleDeselectEvent;
                inputSO.OnMouseClickEvent -= HandleMouseClick;
            }

            inputSO.SetPlayerEnabled(!on);
            inputSO.SetScriptEnabled(on);
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

        private void HandleDeselectEvent(string[] scripts)
        {
            SaveScript(scripts);
        }

        private void HandleMouseClick()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                Close();
        }

        private void SaveScript(string[] scripts)
        {
            Debug.Log("as");
        }
    }
}
