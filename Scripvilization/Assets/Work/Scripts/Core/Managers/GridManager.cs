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
                        Collider[] colliders = Physics.OverlapBox(node.center, nodeSize / 2, Quaternion.identity, sensedTargetLayer);

                        if (colliders == null || colliders.Length <= 0)
                            continue;

                        node.nodeType = NodeType.Air;
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
            Gizmos.color = Color.green;

            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Vector3 center = transform.position
                                       + new Vector3(x * nodeSize.x, y * nodeSize.y, z * nodeSize.z)
                                       + (nodeSize / 2);

                        if (Grid != null)
                            switch (Grid.GetNode(x, y, z).nodeType)
                            {
                                case NodeType.Air:
                                    Gizmos.color = Color.cyan;
                                    break;
                                case NodeType.Ground:
                                    Gizmos.color = Color.green;
                                    break;
                                case NodeType.Wall:
                                    // darkgreen
                                    Gizmos.color = new Color(0.7f, 1f, 0.7f, 1f);
                                    break;
                                case NodeType.Block:
                                    Gizmos.color = Color.red;
                                    break;
                                case NodeType.Home:
                                    Gizmos.color = Color.yellow;
                                    break;
                                case NodeType.Resource:
                                    Gizmos.color = Color.blue;
                                    break;
                            }

                        Gizmos.DrawWireCube(center, nodeSize);
                    }
        }

    }
}
