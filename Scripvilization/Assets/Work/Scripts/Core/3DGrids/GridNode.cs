using UnityEngine;

namespace Assets.Work.Scripts.Core._3DGrids
{
    public enum NodeType
    {
        Air = 1,
        Ground = 2,
        Wall = 4, // 벽
        Entity = 8,
        Block = 16, // 어떤 방식으로도 못 지나감
        Goal = 32,
    }

    public class GridNode
    {
        public Vector3Int index;
        public Vector3 center;

        public NodeType nodeType;
    }
}
