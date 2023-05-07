using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using System.Linq;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.Utils;
using UnicornOne.Core.Utils;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class UnitInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<ITimeService> _timeService;
        private readonly EcsCustomInject<ITilemapService> _tilemapService;
        private readonly EcsCustomInject<IAbilityService> _abilityService;

        private readonly UnitInstance[] _allyTeam;
        private readonly UnitInstance[] _enemyTeam;

        public UnitInitSystem(UnitInstance[] allyTeam, UnitInstance[] enemyTeam)
        {
            _allyTeam = allyTeam;
            _enemyTeam = enemyTeam;
        }

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var units = _allyTeam.Concat(_enemyTeam).Select(ui => ui.Unit).Distinct();
            List<Ability> abilities = new();
            foreach (var unit in units)
            {
                abilities.AddRange(unit.Abilities);
            }

            foreach (var heroInstance in _allyTeam)
            {
                SpawnUnit(world, heroInstance.Unit, heroInstance.Position, true, abilities);
            }
            foreach (var enemyInstance in _enemyTeam)
            {
                SpawnUnit(world, enemyInstance.Unit, enemyInstance.Position, false, abilities);
            }
        }

        private void SpawnUnit(EcsWorld world, Unit unit, HexCoords position, bool isAlly, List<Ability> abilities)
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

            var unitAiComponentPool = world.GetPool<UnitAiComponent>();
            ref var unitAiComponent = ref unitAiComponentPool.Add(entity);
            unitAiComponent.State = UnitAiState.SearchingTarget;

            var movementComponentPool = world.GetPool<MovementComponent>();
            ref var movementComponent = ref movementComponentPool.Add(entity);
            movementComponent.Speed = unit.Speed;

            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            ref var tilePositionComponent = ref tilePositionComponentPool.Add(entity);
            tilePositionComponent.Position = position;

            _tilemapService.Value.Tilemap.Tiles[position].IsReserved = true;

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(entity);
            gameObjectUnityRefComponent.GameObject =
                Object.Instantiate(unit.Prefab, position.ToWorldCoordsXZ(_tilemapService.Value.HexParams), Quaternion.identity);

            var animatorUnityRefComponentPool = world.GetPool<AnimatorUnityRefComponent>();
            ref var animatorUnityRefComponent = ref animatorUnityRefComponentPool.Add(entity);
            animatorUnityRefComponent.Animator = gameObjectUnityRefComponent.GameObject.GetComponentInChildren<Animator>();

            var healthComponentPool = world.GetPool<HealthComponent>();
            ref var healthComponent = ref healthComponentPool.Add(entity);
            healthComponent.Max = unit.Health;
            healthComponent.Current = unit.Health;

            var attackParamsComponentPool = world.GetPool<AttackParamsComponent>();
            ref var attackParamsComponent = ref attackParamsComponentPool.Add(entity);
            attackParamsComponent.Range = unit.AttackRange;

            var abilitySetComponentPool = world.GetPool<AbilitySetComponent>();
            ref var abilitySetComponent = ref abilitySetComponentPool.Add(entity);
            abilitySetComponent.Id = _abilityService.Value.GetAbilitySetIndex(unit);
            abilitySetComponent.TimeLastUsed =
                Enumerable.Repeat(_timeService.Value.TimeSinceStart, unit.Abilities.Length).ToArray();
        }
    }
}
