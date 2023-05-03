using System.Collections;
using System.Collections.Generic;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.ScriptableObjects;
using UnicornOne.Core.Utils;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class TilemapScript : MonoBehaviour
    {
        [SerializeField] private TilemapVisualSettings _visualSettings;

        private readonly Dictionary<HexCoords, (Tile, TileScript)> _tiles = new();

        public void SetupTilemap(Tilemap tilemap, TilemapSettings settings)
        {
            // Destroy previous tilemap
            {
                foreach (Transform child in transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                _tiles.Clear();
            }

            var hexParams = settings.HexParams;

            var tileMesh = MeshGenerator.TileMesh(hexParams, 4.0f);
            var borderMesh = MeshGenerator.TileBorderMesh(hexParams, 0.95f);

            foreach (var tileEntrance in tilemap)
            {
                HexCoords position = tileEntrance.Key;

                Vector2 flatPosition = position.ToWorldCoords(hexParams);
                Vector3 worldPosition = new Vector3(flatPosition.x, 0.0f, flatPosition.y);

                var gameObject = GameObject.Instantiate(
                    _visualSettings.TilePrefab, worldPosition, Quaternion.identity, transform);
                var tileMaterial = tileEntrance.Value.IsWalkable ?
                    _visualSettings.TileWalkableMaterial : _visualSettings.TileUnwalkableMaterial;

                var tileScript = gameObject.GetComponent<TileScript>();
                tileScript.Setup(tileMesh, tileMaterial, borderMesh);

                _tiles.Add(position, (tileEntrance.Value, tileScript));
            }
        }
    }
}
