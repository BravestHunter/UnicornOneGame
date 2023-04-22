using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.MonoBehaviours;
using UnicornOne.Core.Utils;
using UnityEngine;
using UnityEngine.XR;

namespace UnicornOne.Battle.Ecs.Services
{
    internal class TilemapService : ITilemapService
    {
        public Tilemap Tilemap { get; private set; }
        public HexParams HexParams { get; private set; }

        public TilemapService(Tilemap tilemap, GameObject tilePrefab, Material availableMaterial, Material unavailableMaterial) 
        {
            Tilemap = tilemap;
            HexParams = HexParams.FromInnerRadius(1.0f);

            InitializeTilemap(tilePrefab, availableMaterial, unavailableMaterial);
        }

        public HexCoords GetRandomAvailablePosition()
        {
            KeyValuePair<HexCoords, Tile> tileEntry;
            do
            {
                tileEntry = Tilemap.Tiles.ElementAt(Random.Range(0, Tilemap.Tiles.Count));
            }
            while (!tileEntry.Value.IsAvailable);

            return tileEntry.Key;
        }

        private void InitializeTilemap(GameObject tilePrefab, Material availableMaterial, Material unavailableMaterial)
        {
            var tileMesh = MeshGenerator.TileMesh(HexParams, 4.0f);
            var borderMesh = MeshGenerator.TileBorderMesh(HexParams, 0.95f);

            GameObject tilemapGameObject = new GameObject("Tilemap");
            foreach (var pair in Tilemap)
            {
                Vector2 flatPosition = pair.Key.ToWorldCoords(HexParams);
                Vector3 worldPosition = new Vector3(flatPosition.x, 0.0f, flatPosition.y);

                var gameObject = GameObject.Instantiate(tilePrefab, worldPosition, Quaternion.identity, tilemapGameObject.transform);

                var tileMaterial = pair.Value.IsAvailable ? availableMaterial : unavailableMaterial;

                var tileScript = gameObject.GetComponent<TileScript>();
                tileScript.Setup(tileMesh, tileMaterial, borderMesh);
            }
        }
    }
}
