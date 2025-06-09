using Assets.Work.Scripts.Core._3DGrids;
using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Core.Managers;
using Assets.Work.Scripts.Scriptables;
using Assets.Work.Scripts.Sriptable;
using MethodArchiveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.Scripts.Registers
{
    public class EntityRegister : MonoBehaviour, IRegister
    {
        [Serializable]
        public class DirectionInfo
        {
            public string direction;
            public Vector3Int directionValue;
        }

        [Header("Default Set")]
        [SerializeField] private ObjectFinder gridManagerFinder;

        [Header("Move")]
        [SerializeField] private float moveDuration;
        [SerializeField] private DirectionInfo[] directionInfoes;

        private static GridManager _gridManager;

        private static Dictionary<string, Vector3Int> _directionInfoDict;
        private static float _moveDuration;

        private void Awake()
        {
            _gridManager = gridManagerFinder.GetObject<GridManager>();
            _moveDuration = moveDuration;

            InitializeDirectionDict();
        }

        private void InitializeDirectionDict()
        {
            _directionInfoDict = new Dictionary<string, Vector3Int>();

            if (directionInfoes == null || directionInfoes.Length <= 0)
                return;

            foreach (DirectionInfo directionInfo in directionInfoes)
            {
                _directionInfoDict.Add(directionInfo.direction, directionInfo.directionValue);
            }
        }

        [ArchiveMethod("이동")]
        public static void Move(IScriptable target, string direction)
        {
            Vector3Int nodePosition = _gridManager.Grid.GetWorldToNodePosition(target.Object.transform.position);

            if (_directionInfoDict.TryGetValue(direction, out Vector3Int dir))
            {
                nodePosition += dir;
                GridNode node = _gridManager.Grid.GetNode(nodePosition);

                if (node == null)
                    return;

                target.Execute();
                if (target is ScriptableEntity entity)
                {
                    try
                    {
                        entity.MoveTo(node, _moveDuration);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                else
                {
                    target.Object.transform.position = node.center;
                    target.Complete();
                }
            }
        }
    }
}
