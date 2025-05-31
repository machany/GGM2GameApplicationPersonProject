using UnityEngine;

namespace AgamaLibrary.Unity._3DGrids
{
    public class Grid
    {
        public readonly Vector3Int _gridSize;
        public readonly Vector3 _nodeSize;

        private readonly Vector3 _startPosition;

        private GridNode[,,] nodes;

        public Grid(Vector3Int gridSize, Vector3 nodeSize, Vector3 startPosition)
        {
            this._gridSize = gridSize;
            this._nodeSize = nodeSize;
            this._startPosition = startPosition;

            nodes = new GridNode[gridSize.x, gridSize.y, gridSize.z];

            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Vector3Int position = new Vector3Int(x, y, z);

                        Vector3 gridCenter = startPosition
                            + new Vector3(x * nodeSize.x, y * nodeSize.y, z * nodeSize.z)
                            + (nodeSize / 2);

                        nodes[x, y, z] = new GridNode()
                        {
                            index = position,
                            center = gridCenter,
                        };
                    }
        }

        public GridNode GetNode(Vector3Int position)
            => GetNode(position.x, position.y, position.z);
        public GridNode GetNode(int x, int y, int z)
        {
            if (x < 0 || x >= _gridSize.x ||
                y < 0 || y >= _gridSize.y ||
                z < 0 || z >= _gridSize.z)
                return null;

            return nodes[x, y, z];
        }

        public Vector3 GetNodeToWorld(GridNode node)
            => GetNodeToWorld(node.index.x, node.index.y, node.index.z);
        public Vector3 GetNodeToWorld(Vector3Int position)
            => GetNodeToWorld(position.x, position.y, position.z);
        public Vector3 GetNodeToWorld(int x, int y, int z)
            => _startPosition
             + new Vector3(x * _nodeSize.x, y * _nodeSize.y, z * _nodeSize.z)
             + (_nodeSize / 2);

        public Vector3Int GetWorldToNode(Vector3 worldPosition)
        {
            Vector3 temp = worldPosition - _startPosition;
            temp.x /= _nodeSize.x;
            temp.y /= _nodeSize.y;
            temp.z /= _nodeSize.z;

            return Vector3Int.FloorToInt(temp);
        }
    }
}
