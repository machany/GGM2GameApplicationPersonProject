﻿using UnityEngine;

namespace Assets.Work.Scripts.Core._3DGrids
{
    public enum NodeType
    {
        Air,
        Ground,
        Wall, // 벽
        Block, // 어떤 방식으로도 못 지나감
        Home,
        Resource, // 추후 자원 타입 enum만들어서 관리
    }

    public class GridNode
    {
        public Vector3Int index;
        public Vector3 center;

        public NodeType nodeType;
    }
}
