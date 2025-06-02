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
        [SerializeField] private float minTurnValue;
        [SerializeField] private float maxTurnValue;
        [SerializeField] private float turnSpeed;

        private Vector2 _movementBoundValue;
        private Vector3 _followCamRotationValue;

        private Vector3 _originCameraRotation;
        private float _originCameraZoom;
        private bool _changeToOrigin;

        private float _lastResetTime;

        private void Awake()
        {
            inputSO.OnZoomDeltaValueChangeEvent += HandleZoomDeltaValueChangeEvent;
            inputSO.OnResetKeyPressedEvent += HandleResetKeyPressedEvent;

            _movementBoundValue = startPosition + movementBounds / 2;
            _followCamRotationValue = cinemachineCamParant.transform.eulerAngles;

            _originCameraRotation = _followCamRotationValue;
            _originCameraZoom = cinemachineCam.Lens.OrthographicSize;

            _changeToOrigin = false;
        }

        private void OnDestroy()
        {
            inputSO.OnZoomDeltaValueChangeEvent -= HandleZoomDeltaValueChangeEvent;
            inputSO.OnResetKeyPressedEvent -= HandleResetKeyPressedEvent;
        }

        private void Update()
        {
            Move(inputSO.MoveDirection);
            // + => -, - => + 돼야 의도한 방향대로 돌음
            Turn(-inputSO.TurnValue);
        }

        private void Move(Vector2 movementValue)
        {
            if (_changeToOrigin)
                return;

            Vector3 position = transform.position;
            float moveSpeed = movementSpeed * Time.deltaTime;

            Vector3 moveValue = cinemachineCam.transform.right * movementValue.x + cinemachineCam.transform.forward * movementValue.y;
            moveValue.y = 0;
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

        private void Turn(float value)
        {
            if (_changeToOrigin)
                return;

            float turnSpeed = this.turnSpeed * Time.deltaTime;

            _followCamRotationValue.y = Mathf.Clamp(_followCamRotationValue.y + value * turnSpeed, minTurnValue, maxTurnValue);

            cinemachineCamParant.rotation = Quaternion.Euler(_followCamRotationValue);
        }

        private void HandleZoomDeltaValueChangeEvent(Vector2 deltaValue)
        {
            if (_changeToOrigin)
                return;

            float size = cinemachineCam.Lens.OrthographicSize;

            size = Mathf.Clamp(size - deltaValue.y * zoomSpeed, minZoomValue, maxZoomValue);

            cinemachineCam.Lens.OrthographicSize = size;
        }

        private void HandleResetKeyPressedEvent()
        {
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
