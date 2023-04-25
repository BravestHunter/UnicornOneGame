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
                SpawnUnit(world, hero, _tilemapService.Value.GetRandomAvailablePosition(), true);
            }
            foreach (var enemy in _enemyTeam)
            {
                SpawnUnit(world, enemy, _tilemapService.Value.GetRandomAvailablePosition(), false);
            }
        }

        private void SpawnUnit(EcsWorld world, Unit unit, HexCoords position, bool isAlly)
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

            var movementComponentPool = world.GetPool<MovementComponent>();
            ref var movementComponent = ref movementComponentPool.Add(entity);
            movementComponent.Speed = Random.Range(1.0f, 10.0f);

            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            ref var tilePositionComponent = ref tilePositionComponentPool.Add(entity);
            tilePositionComponent.Position = position;

            _tilemapService.Value.Tilemap.Tiles[position].IsReserved = true;

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(entity);
            gameObjectUnityRefComponent.GameObject =
                Object.Instantiate(unit.Prefab, position.ToWorldCoordsXZ(_tilemapService.Value.HexParams), Quaternion.identity);

            var healthComponentPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthComponentPool.Add(entity);
            healthComponent.Max = unit.Health;
            healthComponent.Current = unit.Health;
        }
    }
}
