using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    public class TilemapScript : MonoBehaviour
    {
        [SerializeField] private TilePath _tilePath;
        [SerializeField] private GameObject _tilePrefab;

        [SerializeField] private float HexOuterRadius = 1.0f;
        private float HexInnerRadius => HexOuterRadius * 0.866025404f; // * sqrRoot(3) / 2

        private Vector3[] HexCorners => new Vector3[] {
            new Vector3(0f, 0f, HexOuterRadius),
            new Vector3(HexInnerRadius, 0f, 0.5f * HexOuterRadius),
            new Vector3(HexInnerRadius, 0f, -0.5f * HexOuterRadius),
            new Vector3(0f, 0f, -HexOuterRadius),
            new Vector3(-HexInnerRadius, 0f, -0.5f * HexOuterRadius),
            new Vector3(-HexInnerRadius, 0f, 0.5f * HexOuterRadius),
            new Vector3(0f, 0f, HexOuterRadius) // duplicate first
        };

        void Start()
        {
            Setup(_tilePath);
        }

        void Update()
        {
        
        }

        private void Setup(TilePath tilePath)
        {
            var mesh = GetHexMesh();

            foreach (var tileEntry in tilePath.Tiles)
            {
                CreateTile(tileEntry.Position, mesh);
            }
        }

        void CreateTile(HexCoordinates coords, Mesh mesh)
        {
            Vector3 position = coords.ToWorldCoords(HexOuterRadius, HexInnerRadius);

            var tile = Instantiate(_tilePrefab, transform, false);
            tile.transform.localPosition = position;

            var meshFilter = tile.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        private Mesh GetHexMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                int vertexIndex = vertices.Count;
                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
            }

            var hexCorners = HexCorners;
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(Vector3.zero, hexCorners[i], hexCorners[i + 1]);
            }

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
