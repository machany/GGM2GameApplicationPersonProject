using Assets.Work.Scripts.Core._3DGrids;
using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Core.Managers;
using Assets.Work.Scripts.Sriptable;
using MethodArchiveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Work.Scripts.Registers.EntityRegister;

namespace Assets.Work.Scripts.Registers
{
    public class ConditionRegister : MonoBehaviour, IRegister
    {
        [Serializable]
        private class SencingTargetInfo
        {
            public string targetName;
            public NodeType tatgetNodeType;
        }

        [Header("Default Setting")]
        [SerializeField] private ObjectFinderSO gridManagerFinderSO;

        [Header("Sencing Setting")]
        [SerializeField] private SencingTargetInfo[] sencingTargetInfoes;
        [SerializeField] private DirectionInfo[] directionInfoes;

        private static GridManager _gridManager;

        private static Dictionary<string, Vector3Int> _directionInfoDict;
        private static Dictionary<string, NodeType> _targetInfoDict;

        private void Awake()
        {
            InitializeDirectionDict();
            InitializeTargetnDict();
        }

        private void Start()
        {
            _gridManager = gridManagerFinderSO.GetObject<GridManager>();
        }

        private void InitializeTargetnDict()
        {
            _targetInfoDict = new Dictionary<string, NodeType>();

            if (sencingTargetInfoes == null || sencingTargetInfoes.Length <= 0)
                return;

            foreach (SencingTargetInfo sencingTargetInfo in sencingTargetInfoes)
                _targetInfoDict.Add(sencingTargetInfo.targetName, sencingTargetInfo.tatgetNodeType);
        }

        private void InitializeDirectionDict()
        {
            _directionInfoDict = new Dictionary<string, Vector3Int>();

            if (directionInfoes == null || directionInfoes.Length <= 0)
                return;

            foreach (DirectionInfo directionInfo in directionInfoes)
                _directionInfoDict.Add(directionInfo.direction, directionInfo.directionValue);
        }

        [ArchiveMethod("감지")]
        public static bool? Senece(IScriptable target, string sencingTarget, string directionName)
        {
            if (!_targetInfoDict.TryGetValue(sencingTarget, out NodeType targetNodeType))
                return null;

            if (!_directionInfoDict.TryGetValue(directionName, out Vector3Int direction))
                return null;

            GridNode currentNode = _gridManager.Grid.GetWorldPositionToNode(target.Object.transform.position);
            GridNode targetNode = _gridManager.Grid.GetNode(currentNode.index + direction);

            if (targetNode.nodeType == targetNodeType)
                return true;
            else
                return false;
        }
    }
}
