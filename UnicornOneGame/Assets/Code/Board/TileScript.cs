using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    public class TileScript : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        public void Init(Tile tile, Mesh mesh)
        {
            _meshFilter.mesh = mesh;
            _meshRenderer.material = tile.Material;
        }
    }
}
