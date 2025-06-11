using Assets.Work.Scripts.Core._3DGrids;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Work.Scripts.Core.Managers
{
    [DefaultExecutionOrder(-1)]
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Info")]
        [SerializeField] private Vector3Int gridSize;
        [SerializeField] private Vector3 nodeSize;

        [Serializable]
        public class NodeTypeInfo
        {
            [field: SerializeField] public NodeType NodeType { get; private set; }
            [field: SerializeField] public LayerMask NodeLayer { get; private set; }
        }
        [Header("Node Layer Info")]
        [SerializeField] private NodeTypeInfo[] nodeInfoes;

        public Core._3DGrids.Grid Grid { get; private set; }
        private int _sensedTargetLayer;

        private void Awake()
        {
            Grid = new Core._3DGrids.Grid(gridSize, nodeSize, transform.position);
            Initialize();
        }

        private void Initialize()
        {
            foreach (NodeTypeInfo layer in nodeInfoes)
                _sensedTargetLayer |= layer.NodeLayer;

            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        GridNode node = Grid.GetNode(x, y, z);
                        BakeNode(node);
                    }
        }

        public void BakeNode(GridNode node)
        {
            Collider[] colliders = Physics.OverlapBox(node.center, nodeSize / 4, Quaternion.identity, _sensedTargetLayer);
            node.nodeType = NodeType.Air;

            if (colliders == null || colliders.Length <= 0)
                return;

            // 콜라이더들의 레이어들 저장해서
            int colliderLayers = 0;

            foreach (Collider col in colliders)
                colliderLayers |= 1 << col.gameObject.layer;

            // 뒤에서 부터 저장된 레이어 비교 후 뭔가 걸리면 그 타입으로 지정함.
            for (int i = nodeInfoes.Length - 1; i >= 0; --i)
                if ((colliderLayers & nodeInfoes[i].NodeLayer) != 0)
                {
                    node.nodeType = nodeInfoes[i].NodeType;
                    break;
                }
        }

        private void OnDrawGizmosSelected()
        {
            if (Grid == null)
                return;

            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Gizmos.color = Grid.GetNode(x, y, z).nodeType switch
                        {
                            NodeType.Air => Color.cyan,
                            NodeType.Ground => Color.green,
                            NodeType.Wall => new Color(0.3f, 1f, 0.3f, 1f),// darkgreen
                            NodeType.Entity => Color.yellow,
                            NodeType.Block => Color.black,
                            NodeType.Goal => Color.red,
                            _ => Color.black,
                        };
                        Gizmos.DrawWireCube(Grid.GetNode(x, y, z).center, nodeSize);
                    }
        }

    }
}
