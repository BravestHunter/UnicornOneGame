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
        public Tilemap Tilemap { get; }
        public HexParams HexParams { get; }
        public Dictionary<HexCoords, TileScript> TileScripts { get; } = new Dictionary<HexCoords, TileScript>();

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
            while (!tileEntry.Value.IsWalkable);

            return tileEntry.Key;
        }

        private void InitializeTilemap(GameObject tilePrefab, Material availableMaterial, Material unavailableMaterial)
        {
            var tileMesh = MeshGenerator.TileMesh(HexParams, 4.0f);
            var borderMesh = MeshGenerator.TileBorderMesh(HexParams, 0.95f);

            GameObject tilemapGameObject = new GameObject("Tilemap");
            foreach (var tileEntrance in Tilemap)
            {
                HexCoords position = tileEntrance.Key;

                Vector2 flatPosition = position.ToWorldCoords(HexParams);
                Vector3 worldPosition = new Vector3(flatPosition.x, 0.0f, flatPosition.y);

                var gameObject = GameObject.Instantiate(tilePrefab, worldPosition, Quaternion.identity, tilemapGameObject.transform);
                var tileMaterial = tileEntrance.Value.IsWalkable ? availableMaterial : unavailableMaterial;

                var tileScript = gameObject.GetComponent<TileScript>();
                tileScript.Setup(tileMesh, tileMaterial, borderMesh);

                TileScripts.Add(position, tileScript);
            }
        }
    }
}
