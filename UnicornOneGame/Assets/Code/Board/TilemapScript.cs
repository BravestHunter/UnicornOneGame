using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnicornOne.Board.TilePath;

namespace UnicornOne.Board
{
    public class TilemapScript : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab;

        [SerializeField] public float HexOuterRadius = 1.0f;
        public float HexInnerRadius => HexOuterRadius * 0.866025404f; // * sqrRoot(3) / 2

        [SerializeField] private float _tileHeight = 4.0f;

        [SerializeField] private float _tileBorderSize = 0.05f;

        [SerializeField] private bool _fill = true;
        [SerializeField] private Tile _fillTile;
        [SerializeField] private Vector2Int _fillCenter = Vector2Int.zero;
        [SerializeField] private int _fillRadius = 12;

        private Vector3[] HexCorners => new Vector3[] {
            new Vector3(0f, 0f, HexOuterRadius),
            new Vector3(HexInnerRadius, 0f, 0.5f * HexOuterRadius),
            new Vector3(HexInnerRadius, 0f, -0.5f * HexOuterRadius),
            new Vector3(0f, 0f, -HexOuterRadius),
            new Vector3(-HexInnerRadius, 0f, -0.5f * HexOuterRadius),
            new Vector3(-HexInnerRadius, 0f, 0.5f * HexOuterRadius),
            new Vector3(0f, 0f, HexOuterRadius) // duplicate first
        };

        private List<GameObject> _fillTiles = new List<GameObject>();

        public void Setup(TilePath tilePath)
        {
            _fillTiles.Clear();
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            var tileMesh = GetTileMesh();
            var borderMesh = GetTileBorderMesh();

            HashSet<HexCoordinates> existingTilesSet = new();
            foreach (var tileEntry in tilePath.Tiles)
            {
                CreateTile(tileEntry.Position, tileEntry.Tile, tileMesh, borderMesh);
                existingTilesSet.Add(tileEntry.Position);
            }

            if (_fill)
            {
                FillTiles(tileMesh, borderMesh, existingTilesSet);
            }
        }

        public void Shuffle(Vector2 heightRange)
        {
            Debug.Assert(heightRange.y >= heightRange.x);
            if (heightRange.y < heightRange.x)
            {
                return;
            }

            foreach (GameObject fillTile in _fillTiles)
            {
                Vector3 position = fillTile.transform.position;
                position.y = Random.Range(heightRange.x, heightRange.y);
                fillTile.transform.position = position;
            }
        }

        public void ShuffleNoise(Vector2 heightRange)
        {
            Debug.Assert(heightRange.y >= heightRange.x);
            if (heightRange.y < heightRange.x)
            {
                return;
            }

            float diff = heightRange.y - heightRange.x;
            float xOffset = Random.Range(0, 10_000);
            float yOffset = Random.Range(0, 10_000);
            float scale = 0.1f;

            foreach (GameObject fillTile in _fillTiles)
            {
                Vector3 position = fillTile.transform.position;
                position.y = heightRange.x + Mathf.PerlinNoise(position.x * scale + xOffset, position.z * scale + yOffset) * diff;
                fillTile.transform.position = position;
            }
        }

        private void FillTiles(Mesh tileMesh, Mesh borderMesh, HashSet<HexCoordinates> existingTilesSet)
        {
            HexCoordinates center = new HexCoordinates(_fillCenter);

            if (!existingTilesSet.Contains(center))
            {
                _fillTiles.Add(CreateTile(center, _fillTile, tileMesh, borderMesh));
            }

            for (int i = 1; i <= _fillRadius; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    HexCoordinates position = center + TilePathGenerator.Directions[(j + 4) % 6] * i;

                    if (!existingTilesSet.Contains(position))
                    {
                        _fillTiles.Add(CreateTile(position, _fillTile, tileMesh, borderMesh));
                    }

                    for (int k = 1; k < i; k++)
                    {
                        position += TilePathGenerator.Directions[j];

                        if (!existingTilesSet.Contains(position))
                        {
                            _fillTiles.Add(CreateTile(position, _fillTile, tileMesh, borderMesh));
                        }
                    }
                }
            }
        }


        private GameObject CreateTile(HexCoordinates coords, Tile tile, Mesh tileMesh, Mesh borderMesh)
        {
            Vector3 position = coords.ToWorldCoords(HexOuterRadius, HexInnerRadius);

            var tileObject = Instantiate(_tilePrefab, transform, false);
            tileObject.transform.localPosition = position;

            var tileScript = tileObject.GetComponent<TileScript>();
            tileScript.Init(tile, tileMesh, borderMesh);

            return tileObject;
        }

        private Mesh GetTileMesh()
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

        private Mesh GetTileBorderMesh()
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

            float borderOffset = 1.0f - _tileBorderSize;
            // Top border
            for (int i = 0; i < 6; i++)
            {
                Vector3 closePoint1 = hexCorners[i] * borderOffset;
                Vector3 closePoint2 = hexCorners[i + 1] * borderOffset;

                AddTriangle(hexCorners[i], hexCorners[i + 1], closePoint1);
                AddTriangle(closePoint1, hexCorners[i + 1], closePoint2);
            }

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
