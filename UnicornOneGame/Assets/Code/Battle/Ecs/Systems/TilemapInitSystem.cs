using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Core.Utils;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class TilemapInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<ITilemapService> _tilemapService;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            HexParams hexParams = _tilemapService.Value.HexParams;
            var tileMesh = MeshGenerator.TileMesh(hexParams, 4.0f);
            var borderMesh = MeshGenerator.TileBorderMesh(hexParams, 0.95f);

            GameObject tilemapGameObject = new GameObject("Tilemap");
            foreach (var pair in _tilemapService.Value.Tilemap)
            {
                Vector2 flatPosition = pair.Key.ToWorldCoords(hexParams);
                Vector3 worldPosition = new Vector3(flatPosition.x, 0.0f, flatPosition.y);

                var gameObject = GameObject.Instantiate(_tilemapService.Value.TilePrefab, worldPosition, Quaternion.identity, tilemapGameObject.transform);
                gameObject.GetComponent<MeshFilter>().mesh = tileMesh;
                gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh = borderMesh;
            }
        }
    }
}
