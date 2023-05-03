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
        [SerializeField] private TilemapVisualSettings VisualSettings;

        private readonly Dictionary<HexCoords, TileScript> TileScripts = new();

        public void SetupTilemap(Tilemap tilemap, TilemapSettings settings)
        {
            // Destroy previous tilemap
            {
                foreach (Transform child in transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                TileScripts.Clear();
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
                    VisualSettings.TilePrefab, worldPosition, Quaternion.identity, transform);
                var tileMaterial = tileEntrance.Value.IsWalkable ?
                    VisualSettings.TileWalkableMaterial : VisualSettings.TileUnwalkableMaterial;

                var tileScript = gameObject.GetComponent<TileScript>();
                tileScript.Setup(tileMesh, tileMaterial, borderMesh);

                TileScripts.Add(position, tileScript);
            }
        }
    }
}
