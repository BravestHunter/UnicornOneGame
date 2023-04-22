using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    public class TileScript : MonoBehaviour
    {
        [SerializeField] private MeshFilter _tileMeshFilter;
        [SerializeField] private MeshRenderer _tileMeshRenderer;
        [SerializeField] private MeshFilter _borderMeshFilter;

        public void Setup(Mesh mesh, Material material, Mesh borderMesh)
        {
            _tileMeshFilter.mesh = mesh;
            _tileMeshRenderer.material = material;

            _borderMeshFilter.mesh = borderMesh;
        }
    }
}
