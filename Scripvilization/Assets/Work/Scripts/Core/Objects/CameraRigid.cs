using Assets.Work.Scripts.Core.Inputs;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Work.Scripts.Core.Objects
{
    public class CameraRigid : MonoBehaviour
    {
        [Header("Default Setting")]
        [SerializeField] private InputSO inputSO;
        [SerializeField] private CinemachineCamera cinemachineCam;
        [SerializeField] private float changeToOriginDuration;

        [Header("Move Setting")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private Vector2 startPosition;
        [SerializeField] private Vector2 movementBounds;
        [SerializeField] private bool moveSpeedApplyZoom;

        [Header("Zoom Setting")]
        [SerializeField] private float minZoomValue;
        [SerializeField] private float maxZoomValue;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float zoomCameraOffset;

        [Header("Turn Setting")]
        [SerializeField] private Transform cinemachineCamParant;
        [SerializeField] private float minXTurnRangeValue;
        [SerializeField] private float maxXTurnRangeValue;
        [SerializeField] private float yTurnRangeValue;
        [SerializeField] private float turnSpeed;

        private bool _mouseClicked;
        private bool _mouseOptionClicked;

        private Vector2 _movementBoundValue;
        private Vector3 _followCamRotationValue;

        private Vector3 _originCameraRotation;
        private float _originCameraZoom;
        private bool _changeToOrigin;

        private float _lastResetTime;

        private void Awake()
        {
            inputSO.OnMouseSelectedStatusEvent += HandleMouseSelectedStatusEvent;
            inputSO.OnMouseOptionClickStatusEvent += HandleMouseOptionClickStatusEvent;

            inputSO.OnMouseMoveEvent += HandleMouseMoveEvent;
            inputSO.OnZoomDeltaValueChangeEvent += HandleZoomDeltaValueChangeEvent;
            inputSO.OnResetStatusEvent += HandleResetKeyPressedEvent;

            _movementBoundValue = startPosition + movementBounds / 2;
            _followCamRotationValue = cinemachineCamParant.transform.eulerAngles;

            _originCameraRotation = _followCamRotationValue;
            _originCameraZoom = cinemachineCam.Lens.OrthographicSize;

            _mouseClicked = false;
            _changeToOrigin = false;
        }

        private void OnDestroy()
        {
            inputSO.OnMouseSelectedStatusEvent -= HandleMouseSelectedStatusEvent;
            inputSO.OnMouseOptionClickStatusEvent -= HandleMouseOptionClickStatusEvent;

            inputSO.OnMouseMoveEvent -= HandleMouseMoveEvent;
            inputSO.OnZoomDeltaValueChangeEvent -= HandleZoomDeltaValueChangeEvent;
            inputSO.OnResetStatusEvent -= HandleResetKeyPressedEvent;
        }

        private void Update()
        {
            if (!(_mouseClicked || _mouseOptionClicked))
            {
                Move(inputSO.MoveDirection);
                Turn(ref _followCamRotationValue.y, -inputSO.TurnValue, -yTurnRangeValue, yTurnRangeValue);
            }
            // + => -, - => + 돼야 의도한 방향대로 돌음
        }

        private void Move(Vector2 movementValue)
        {
            if (_changeToOrigin)
                return;

            Vector3 position = transform.position;
            float moveSpeed = movementSpeed * Time.deltaTime;

            Quaternion yRotation = Quaternion.Euler(0f, cinemachineCam.transform.eulerAngles.y, 0f);
            Vector3 inputDir = new Vector3(movementValue.x, 0f, movementValue.y);
            Vector3 moveValue = yRotation * inputDir;
            moveValue.Normalize();
            moveValue *= moveSpeed;

            if (moveSpeedApplyZoom)
            {
                // 줌에 따라 이속 변화
                moveValue *= movementSpeed * cinemachineCam.Lens.OrthographicSize / maxZoomValue;
            }

            position = new Vector3(Mathf.Clamp(position.x + moveValue.x, -_movementBoundValue.x, _movementBoundValue.x),
                position.y,
                Mathf.Clamp(position.z + moveValue.z, -_movementBoundValue.x, _movementBoundValue.x));

            transform.position = position;
        }

        private void Turn(ref float rotation, float value, float min, float max)
        {
            if (_changeToOrigin)
                return;

            float turnSpeed = this.turnSpeed * Time.deltaTime;

            rotation = Mathf.Clamp(rotation + value * turnSpeed, min, max);

            cinemachineCamParant.rotation = Quaternion.Euler(_followCamRotationValue);
        }

        private void HandleMouseSelectedStatusEvent(bool clicked)
        {
            _mouseClicked = clicked;
        }

        private void HandleMouseOptionClickStatusEvent(bool clicked)
        {
            _mouseOptionClicked = clicked;
        }

        private void HandleMouseMoveEvent(Vector2 deltaValue)
        {
            if (_mouseClicked)
                Move(deltaValue * -1);
            else if (_mouseOptionClicked)
            {
                Turn(ref _followCamRotationValue.x, -deltaValue.y, minXTurnRangeValue, maxXTurnRangeValue);
                Turn(ref _followCamRotationValue.y, deltaValue.x, -yTurnRangeValue, yTurnRangeValue);
            }
        }

        private void HandleZoomDeltaValueChangeEvent(Vector2 deltaValue)
        {
            if (_changeToOrigin)
                return;

            float size = cinemachineCam.Lens.OrthographicSize;

            size = Mathf.Clamp(size - deltaValue.y * zoomSpeed, minZoomValue, maxZoomValue);

            cinemachineCam.Lens.OrthographicSize = size;
        }

        private void HandleResetKeyPressedEvent(bool status)
        {
            if (!status)
                return;

            bool applyMove = false;
            if ((Time.time - _lastResetTime) < changeToOriginDuration)
                applyMove = true;
            else if (_changeToOrigin)
                return;

            _changeToOrigin = true;
            _lastResetTime = Time.time;

            if (applyMove)
            {
                // move
                Vector3 position = transform.position;
                DOTween.To(() => position, value => position = value, new Vector3(startPosition.x, transform.position.y, startPosition.y), changeToOriginDuration)
                    .OnUpdate(() => transform.position = position);
            }

            // rotation
            DOTween.To(() => _followCamRotationValue, value => _followCamRotationValue = value, _originCameraRotation, changeToOriginDuration)
                .OnUpdate(() => cinemachineCamParant.transform.rotation = Quaternion.Euler(_followCamRotationValue));

            // zoom
            float zoomOffset = cinemachineCam.Lens.OrthographicSize;
            DOTween.To(() => zoomOffset, value => zoomOffset = value, _originCameraZoom, changeToOriginDuration)
                .OnUpdate(() => cinemachineCam.Lens.OrthographicSize = zoomOffset)
                .OnComplete(() => _changeToOrigin = false);

            /*
            // move
            Vector3 position = transform.position;
            DOTween.To(() => position, value => position = value, new Vector3(startPosition.x, transform.position.y, startPosition.y), changeToOriginDuration)
                .OnUpdate(() => transform.position = position)
                .OnComplete(() =>
                {
                    // rotation
                    DOTween.To(() => _followCamRotationValue, value => _followCamRotationValue = value, _originCameraRotation, changeToOriginDuration)
                    .OnUpdate(() => followCam.transform.rotation = Quaternion.Euler(_followCamRotationValue))
                    .OnComplete(() =>
                    {
                        // zoom
                        float zoomOffset = followCam.FollowOffset.y;
                        DOTween.To(() => zoomOffset, value => zoomOffset = value, _originCameraZoom, changeToOriginDuration)
                            .OnUpdate(() => followCam.FollowOffset.y = zoomOffset)
                            .OnComplete(() => _changeToOrigin = false);
                    });
                });
            */
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector3 startPos = new Vector3(startPosition.x, transform.position.y, startPosition.y);
            Vector3 bounds = new Vector3(movementBounds.x, transform.position.y, movementBounds.y);

            Gizmos.DrawWireCube(startPos, bounds);
        }

#endif
    }
}
