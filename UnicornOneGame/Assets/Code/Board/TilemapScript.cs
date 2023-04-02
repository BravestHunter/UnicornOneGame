using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnicornOne.Board.TilePath;

namespace UnicornOne.Board
{
    public class TilemapScript : MonoBehaviour
    {
        [SerializeField] private TilePath _tilePath;
        [SerializeField] private GameObject _tilePrefab;

        [SerializeField] private float HexOuterRadius = 1.0f;
        private float HexInnerRadius => HexOuterRadius * 0.866025404f; // * sqrRoot(3) / 2

        [SerializeField] private float _tileHeight = 4.0f;

        [SerializeField] private Tile _fillTile;
        [SerializeField] private Vector2Int _fillFieldXRange;
        [SerializeField] private Vector2Int _fillFieldYRange;

        [SerializeField] private bool _randomGeneration = false;
        [SerializeField] private int _generatedPathLength = 10;
        [SerializeField] private Tile _startTile;
        [SerializeField] private Tile _finishTile;
        [SerializeField] private Tile _roadTile;

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
            if (_randomGeneration)
            {
                Setup(TilePathGenerator.Generate(_startTile, _finishTile, _roadTile, _generatedPathLength));
            }
            else
            {
                Setup(_tilePath);
            }
        }

        void Update()
        {
        
        }

        private void Setup(TilePath tilePath)
        {
            var mesh = GetHexMesh();

            HashSet<(int, int)> existingTilesSet = new();
            foreach (var tileEntry in tilePath.Tiles)
            {
                CreateTile(tileEntry.Position, mesh, tileEntry.Tile);
                existingTilesSet.Add((tileEntry.Position.X, tileEntry.Position.Y));
            }

            for (int i = _fillFieldXRange.x; i <= _fillFieldXRange.y; i++)
            {
                for (int j = _fillFieldYRange.x; j <= _fillFieldYRange.y; j++)
                {
                    if (!existingTilesSet.Contains((i, j)))
                    {
                        CreateTile(new HexCoordinates(i, j), mesh, _fillTile);
                    }
                }
            }
        }

        void CreateTile(HexCoordinates coords, Mesh mesh, Tile tile)
        {
            Vector3 position = coords.ToWorldCoords(HexOuterRadius, HexInnerRadius);

            var tileObject = Instantiate(_tilePrefab, transform, false);
            tileObject.transform.localPosition = position;

            var tileScript = tileObject.GetComponent<TileScript>();
            tileScript.Init(tile, mesh);
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

            // Top
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(Vector3.zero, hexCorners[i], hexCorners[i + 1]);
            }

            // Side
            Vector3 verticalOffset = -Vector3.up * _tileHeight;
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(hexCorners[i], hexCorners[i] + verticalOffset, hexCorners[i + 1] + verticalOffset);
                AddTriangle(hexCorners[i], hexCorners[i + 1] + verticalOffset, hexCorners[i + 1]);
            }

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
