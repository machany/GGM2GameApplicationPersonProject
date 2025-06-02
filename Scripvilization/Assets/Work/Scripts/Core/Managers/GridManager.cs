using Assets.Work.Scripts.Core._3DGrids;
using System;
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

        private void Awake()
        {
            Grid = new Core._3DGrids.Grid(gridSize, nodeSize, transform.position);
            Initialize();
        }

        private void Initialize()
        {
            int sensedTargetLayer = 0;
            foreach (NodeTypeInfo layer in nodeInfoes)
                sensedTargetLayer |= layer.NodeLayer;

            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        GridNode node = Grid.GetNode(x, y, z);
                        Collider[] colliders = Physics.OverlapBox(node.center, nodeSize / 4, Quaternion.identity, sensedTargetLayer);
                        node.nodeType = NodeType.Air;

                        if (colliders == null || colliders.Length <= 0)
                            continue;

                        foreach (NodeTypeInfo nodeInfo in nodeInfoes)
                            if (((1 << colliders[0].gameObject.layer) & nodeInfo.NodeLayer) != 0)
                            {
                                node.nodeType = nodeInfo.NodeType;
                                break;
                            }
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
                            NodeType.Wall => new Color(0.5f, 1f, 0.5f, 1f),// darkgreen
                            NodeType.Entity => Color.white,
                            NodeType.Block => Color.red,
                            NodeType.Home => Color.yellow,
                            NodeType.Resource => Color.blue,
                            _ => Color.black,
                        };
                        Gizmos.DrawWireCube(Grid.GetNode(x, y, z).center, nodeSize);
                    }
        }

    }
}
