using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Core.Utils;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class UnitInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<ITilemapService> _tilemapService;

        private readonly Unit[] _heroTeam;
        private readonly Unit[] _enemyTeam;

        private HexCoords RandomTilePosition
        {
            get
            {
                HexCoords coords;
                do
                {
                    int q = Random.Range(-5, 6);
                    int r = Random.Range(-5, 6);
                    coords = HexCoords.FromCube(q, r);
                }
                while (!_tilemapService.Value.Tilemap[coords].HasValue || _tilemapService.Value.Tilemap[coords].Value.IsReserved);

                return coords;
            }
        }

        public UnitInitSystem(Unit[] heroTeam, Unit[] enemyTeam)
        {
            _heroTeam = heroTeam;
            _enemyTeam = enemyTeam;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            foreach (var hero in _heroTeam)
            {
                SpawnUnit(world, hero, RandomTilePosition, true);
            }
            foreach (var enemy in _enemyTeam)
            {
                SpawnUnit(world, enemy, RandomTilePosition, false);
            }
        }

        private void SpawnUnit(EcsWorld world, Unit unit, HexCoords tilePosition, bool isAlly)
        {
            var entity = world.NewEntity();

            var unitFlagPool = world.GetPool<UnitFlag>();
            unitFlagPool.Add(entity);

            if (isAlly)
            {
                var allyFlagPool = world.GetPool<AllyFlag>();
                allyFlagPool.Add(entity);
            }
            else
            {
                var enemyFlagPool = world.GetPool<EnemyFlag>();
                enemyFlagPool.Add(entity);
            }

            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            ref var tilePositionComponent = ref tilePositionComponentPool.Add(entity);
            tilePositionComponent.Position = tilePosition;

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(entity);
            gameObjectUnityRefComponent.GameObject = Object.Instantiate(unit.Prefab, tilePosition.ToWorldCoordsXZ(_tilemapService.Value.HexParams), Quaternion.identity);

            var healthComponentPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthComponentPool.Add(entity);
            healthComponent.MaxHealth = unit.Health;
            healthComponent.CurrentHealth = unit.Health;
        }
    }
}
