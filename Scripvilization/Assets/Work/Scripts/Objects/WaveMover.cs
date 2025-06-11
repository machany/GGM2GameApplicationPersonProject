using AgamaLibrary.Methods;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.Scripts.Objects
{
    public class WaveMover : MonoBehaviour
    {
        [SerializeField] private List<WaveMover> chaningObjectList;
        [Range(0, 2f)]
        [SerializeField] private float chainMovePowerMultiply;
        [Range(0, 1f)]
        [SerializeField] private float moveToChainRatio = 0.3f;

        //[SerializeField] private string waveMoverTag;
        [SerializeField] private float sencingRange;

        private bool _move;
        private bool _chaining;

        private float _moveTime;
        private float _lifeTime;

        private Vector3 _movement;
        private Vector3 _originPosition;

        private void Awake()
        {
            _move = _chaining = false;
        }

        private void Start()
        {
            SetChaningObjects();
        }

        private void Update()
        {
            if (_move)
            {
                if ((_lifeTime += Time.deltaTime) <= _moveTime)
                    transform.position = _originPosition + ClacMove(_lifeTime, _moveTime, _movement);
                else
                {
                    _move = false;
                    transform.position = _originPosition;
                }

                if (!_chaining && _lifeTime > moveToChainRatio * _moveTime)
                {
                    MoveToChaining(_movement, _moveTime);
                }
            }
        }

        private Vector3 ClacMove(float t, float lifeTime, Vector3 movePower)
            => Mathf.Sin(t * (Mathf.PI / lifeTime)) * movePower;

        public void Move(Vector3 movePower, float moveTime)
        {
            if (_move)
                return;

            _move = true;
            _chaining = false;

            _originPosition = transform.position;
            _movement = movePower;
            _moveTime = moveTime;

            _lifeTime = 0;
        }

        public void MoveToChaining(Vector3 movePower, float moveTime)
        {
            _chaining = true;
            foreach (WaveMover chainedMover in chaningObjectList)
                if (chainedMover != null && chainedMover.gameObject.activeSelf)
                    chainedMover.Move(movePower * chainMovePowerMultiply, moveTime * chainMovePowerMultiply);
        }

        [ContextMenu("Set Chaning Object")]
        private void SetChaningObjects()
        {
            WaveMover[] wavemovers = GameObject.FindObjectsOfType<WaveMover>(true);//.FindGameObjectsWithTag(waveMoverTag);

            chaningObjectList.Clear();

            foreach (WaveMover mover in wavemovers)
                if (Vector3.Distance(mover.transform.position, transform.position) <= sencingRange)
                    chaningObjectList.Add(mover);

            chaningObjectList.TryRemove(this);
        }

#if UNITY_EDITOR

        [Header("Editor")]
        [SerializeField] private Vector3 moveDirectionTest;
        [SerializeField] private float moveTimeTest;

        [ContextMenu("Move")]
        private void MoveTest()
        {
            Move(moveDirectionTest, moveTimeTest);
        }

#endif
    }
}
