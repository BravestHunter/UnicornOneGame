using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Battle.Utils;
using UnityEngine;
using UnicornOne.Core.Utils;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Models;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class UnitAiSystem : IEcsRunSystem
    {
        private struct UnitInfo
        {
            public int Entity;
            public HexCoords Position;
        }

        private const float RetargetTime = 1.0f;

        private readonly EcsCustomInject<ITimeService> _timeService;
        private readonly EcsCustomInject<ITilemapService> _tilemapService;
        private readonly EcsCustomInject<IAbilityService> _abilityService;

        private EcsFilter _aiFilter;
        private EcsFilter _allyFilter;
        private EcsFilter _enemyFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_aiFilter == null)
            {
                _aiFilter = world
                    .Filter<UnitAiComponent>()
                    .Inc<AbilitySetComponent>()
                    .End();
            }

            var unitAiComponentPool = world.GetPool<UnitAiComponent>();
            var allyFlagPool = world.GetPool<AllyFlag>();
            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();
            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();
            var attackParamsComponentPool = world.GetPool<AttackParamsComponent>();
            var abilitySetComponentPool = world.GetPool<AbilitySetComponent>();
            var abilityUseRequestComponentPool = world.GetPool<AbilityUseRequestComponent>();
            var abilityInUsageComponentPool = world.GetPool<AbilityInUsageComponent>();

            var allies = GetAllies(world);
            var enemies = GetEnemies(world);

            foreach (var entity in _aiFilter)
            {
                ref var unitAiComponent = ref unitAiComponentPool.Get(entity);

                // Skip if there is an ability in use
                if (abilityInUsageComponentPool.Has(entity))
                {
                    continue;
                }

                switch (unitAiComponent.State)
                {
                    case UnitAiState.SearchingTarget:
                        {
                            // Check if entity has target
                            if (targetEntityComponentPool.Has(entity))
                            {
                                unitAiComponent.State = UnitAiState.MovingToTarget;
                                continue;
                            }

                            bool isAlly = allyFlagPool.Has(entity);

                            List<UnitInfo> targets = isAlly ? enemies : allies;
                            if (targets.Count == 0)
                            {
                                continue;
                            }

                            // Select closest target
                            var tilePositionComponent = tilePositionComponentPool.Get(entity);
                            UnitInfo target = GetClosestTarget(targets, tilePositionComponent.Position);

                            ref var targetEntityComponent = ref targetEntityComponentPool.Add(entity);
                            targetEntityComponent.PackedEntity = world.PackEntity(target.Entity);

                            unitAiComponent.State = UnitAiState.MovingToTarget;
                            unitAiComponent.TargetSetTime = _timeService.Value.TimeSinceStart;

                            break;
                        }

                    case UnitAiState.MovingToTarget:
                        {
                            // Try to retarget
                            if (_timeService.Value.TimeSinceStart >= unitAiComponent.TargetSetTime + RetargetTime)
                            {
                                targetEntityComponentPool.Del(entity);
                                unitAiComponent.State = UnitAiState.SearchingTarget;
                                continue;
                            }

                            // Check if there is target
                            if (!targetEntityComponentPool.Has(entity))
                            {
                                unitAiComponent.State = UnitAiState.SearchingTarget;
                                continue;
                            }
                            var targetEntityComponent = targetEntityComponentPool.Get(entity);

                            // Check if target is still alive
                            int targetEntity;
                            if (!targetEntityComponent.PackedEntity.Unpack(world, out targetEntity))
                            {
                                targetEntityComponentPool.Del(entity);

                                unitAiComponent.State = UnitAiState.SearchingTarget;
                                continue;
                            }

                            // Check if target is close enough
                            HexCoords entityPosition;
                            if (targetTileMoveComponentPool.Has(entity))
                            {
                                entityPosition = targetTileMoveComponentPool.Get(entity).Position;
                            }
                            else
                            {
                                entityPosition = tilePositionComponentPool.Get(entity).Position;
                            }
                            HexCoords targetPosition;
                            if (targetTileMoveComponentPool.Has(targetEntity))
                            {
                                targetPosition = targetTileMoveComponentPool.Get(targetEntity).Position;
                            }
                            else
                            {
                                targetPosition = tilePositionComponentPool.Get(targetEntity).Position;
                            }
                            var attackParamsComponent = attackParamsComponentPool.Get(entity);
                            if (entityPosition.DistanceTo(targetPosition) <= attackParamsComponent.Range)
                            {
                                unitAiComponent.State = UnitAiState.AttacksTarget;
                                continue;
                            }

                            // Set destination tile
                            var possibleDestinations =
                                HexUtils.InRange(targetPosition, attackParamsComponent.Range)
                                .Where(t => {
                                    if (_tilemapService.Value.Tilemap.Tiles.TryGetValue(t, out Tile tile))
                                    {
                                        return tile.IsAvailable;
                                    }

                                    return false;
                                });
                            if (possibleDestinations.Count() == 0)
                            {
                                continue;
                            }

                            HexCoords destinationPosition = possibleDestinations.First();
                            int minDistance = int.MaxValue;
                            foreach (var possibleDestination in possibleDestinations)
                            {
                                int distance = possibleDestination.DistanceTo(entityPosition);
                                if (minDistance > distance)
                                {
                                    minDistance = distance;
                                    destinationPosition = possibleDestination;
                                }
                            }

                            if (!destinationTileComponentPool.Has(entity))
                            {
                                destinationTileComponentPool.Add(entity);
                            }
                            ref var destinationTileComponent = ref destinationTileComponentPool.Get(entity);
                            destinationTileComponent.Position = destinationPosition;

                            break;
                        }

                    case UnitAiState.AttacksTarget:
                        {
                            if (destinationTileComponentPool.Has(entity))
                            {
                                destinationTileComponentPool.Del(entity);
                            }

                            // Check if there is target
                            if (!targetEntityComponentPool.Has(entity))
                            {
                                unitAiComponent.State = UnitAiState.SearchingTarget;
                                continue;
                            }
                            var targetEntityComponent = targetEntityComponentPool.Get(entity);

                            // Check if target is still alive
                            int targetEntity;
                            if (!targetEntityComponent.PackedEntity.Unpack(world, out targetEntity))
                            {
                                targetEntityComponentPool.Del(entity);

                                unitAiComponent.State = UnitAiState.SearchingTarget;
                                continue;
                            }

                            // Check if target is close enough
                            HexCoords entityPosition;
                            if (targetTileMoveComponentPool.Has(entity))
                            {
                                entityPosition = targetTileMoveComponentPool.Get(entity).Position;
                            }
                            else
                            {
                                entityPosition = tilePositionComponentPool.Get(entity).Position;
                            }
                            HexCoords targetPosition;
                            if (targetTileMoveComponentPool.Has(targetEntity))
                            {
                                targetPosition = targetTileMoveComponentPool.Get(targetEntity).Position;
                            }
                            else
                            {
                                targetPosition = tilePositionComponentPool.Get(targetEntity).Position;
                            }
                            var attackParamsComponent = attackParamsComponentPool.Get(entity);
                            if (entityPosition.DistanceTo(targetPosition) > attackParamsComponent.Range)
                            {
                                unitAiComponent.State = UnitAiState.MovingToTarget;
                                continue;
                            }

                            // Check if we have to wait for target get into target cell
                            HexCoords realEntityPosition = tilePositionComponentPool.Get(entity).Position;
                            HexCoords realTargetEntityPosition = tilePositionComponentPool.Get(targetEntity).Position;
                            if (realEntityPosition.DistanceTo(realTargetEntityPosition) > attackParamsComponent.Range)
                            {
                                continue;
                            }

                            // Check if there is cooldown
                            var abilitySetComponent = abilitySetComponentPool.Get(entity);
                            int selectedAbilityIndex = -1;
                            for (int i = abilitySetComponent.AbilitySet.Length - 1; i >= 0; i--)
                            {
                                var abilityState = abilitySetComponent.AbilitySet[i];
                                var ability = _abilityService.Value.GetAbility(abilityState.AbilityId);
                                if (abilityState.TimeLastUsed + ability.Cooldown < _timeService.Value.TimeSinceStart)
                                {
                                    selectedAbilityIndex = i;
                                    break;
                                }
                            }

                            // All abilities in cooldown
                            if (selectedAbilityIndex < 0)
                            {
                                continue;
                            }

                            // Use ability
                            ref var abilityUseRequestComponent = ref abilityUseRequestComponentPool.Add(entity);
                            abilityUseRequestComponent.AbilityIndex = selectedAbilityIndex;

                            break;
                        }
                }
            }
        }

        private List<UnitInfo> GetAllies(EcsWorld world)
        {
            if (_allyFilter == null)
            {
                _allyFilter = world
                    .Filter<AllyFlag>()
                    .Inc<TilePositionComponent>()
                    .End();
            }

            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();

            List<UnitInfo> allies = new();
            foreach (int ally in _allyFilter)
            {
                allies.Add(new UnitInfo() { Entity = ally, Position = tilePositionComponentPool.Get(ally).Position });
            }

            return allies;
        }

        private List<UnitInfo> GetEnemies(EcsWorld world)
        {
            if (_enemyFilter == null)
            {
                _enemyFilter = world
                    .Filter<EnemyFlag>()
                    .Inc<TilePositionComponent>()
                    .End();
            }

            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();

            List<UnitInfo> enemies = new();
            foreach (int enemy in _enemyFilter)
            {
                enemies.Add(new UnitInfo() { Entity = enemy, Position = tilePositionComponentPool.Get(enemy).Position });
            }

            return enemies;
        }

        private UnitInfo GetClosestTarget(List<UnitInfo> targets, HexCoords position)
        {
            UnitInfo closestTarget = targets.First();
            int minDistance = int.MaxValue;

            foreach (var target in targets)
            {
                int distance = target.Position.DistanceTo(position);
                if (minDistance > distance)
                {
                    closestTarget = target;
                    minDistance = distance;
                }
            }

            return closestTarget;
        }
    }
}
