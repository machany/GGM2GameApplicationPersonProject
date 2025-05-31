using Assets.Work.Scripts.Core._3DGrids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Work.Scripts.Core
{
    [DefaultExecutionOrder(-1)]
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Vector3Int gridSize;
        [SerializeField] private Vector3 nodeSize;

        public _3DGrids.Grid _grid;

        private void Awake()
        {
            _grid = new _3DGrids.Grid(gridSize, nodeSize, transform.position);
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

                        Gizmos.DrawWireCube(center, nodeSize);
                    }
        }

    }
}
