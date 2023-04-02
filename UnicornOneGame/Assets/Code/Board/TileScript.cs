using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    public class TileScript : MonoBehaviour
    {
        [SerializeField] private MeshFilter _tileMeshFilter;
        [SerializeField] private MeshRenderer _tileMeshRenderer;

        [SerializeField] private MeshFilter _borderMeshFilter;

        public void Init(Tile tile, Mesh tileMesh, Mesh borderMesh)
        {
            _tileMeshRenderer.material = tile.Material;
            _tileMeshFilter.mesh = tileMesh;

            _borderMeshFilter.mesh = borderMesh;
        }
    }
}
