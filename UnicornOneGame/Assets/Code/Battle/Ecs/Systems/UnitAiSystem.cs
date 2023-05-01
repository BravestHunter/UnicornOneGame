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

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class UnitAiSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<ITilemapService> _tilemapService;

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
                    .End();
            }

            var unitAiComponentPool = world.GetPool<UnitAiComponent>();
            var allyFlagPool = world.GetPool<AllyFlag>();
            var targetEntityComponentPool = world.GetPool<TargetEntityComponent>();
            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();
            var destinationTileComponentPool = world.GetPool<DestinationTileComponent>();
            var attackInCooldownComponentPool = world.GetPool<AttackInCooldownComponent>();
            var attackParamsComponentPool = world.GetPool<AttackParamsComponent>();
            var attackComponentPool = world.GetPool<AttackComponent>();

            var allies = GetAllies(world);
            var enemies = GetEnemies(world);

            foreach (var entity in _aiFilter)
            {
                ref var unitAiComponent = ref unitAiComponentPool.Get(entity);

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

                            List<int> targets = isAlly ? enemies : allies;
                            if (targets.Count == 0)
                            {
                                continue;
                            }

                            // Select random target
                            int target = targets[Random.Range(0, targets.Count)];
                            ref var targetEntityComponent = ref targetEntityComponentPool.Add(entity);
                            targetEntityComponent.PackedEntity = world.PackEntity(target);

                            unitAiComponent.State = UnitAiState.MovingToTarget;

                            break;
                        }

                    case UnitAiState.MovingToTarget:
                        {
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
                            if (attackInCooldownComponentPool.Has(entity))
                            {
                                continue;
                            }

                            // Make attack
                            ref var attackComponent = ref attackComponentPool.Add(entity);
                            attackComponent.Damage = attackParamsComponent.Damage;
                            attackComponent.Cooldown = attackParamsComponent.Cooldown;

                            break;
                        }
                }
            }
        }

        private List<int> GetAllies(EcsWorld world)
        {
            if (_allyFilter == null)
            {
                _allyFilter = world.Filter<AllyFlag>().End();
            }

            List<int> allies = new();
            foreach (int ally in _allyFilter)
            {
                allies.Add(ally);
            }

            return allies;
        }

        private List<int> GetEnemies(EcsWorld world)
        {
            if (_enemyFilter == null)
            {
                _enemyFilter = world.Filter<EnemyFlag>().End();
            }

            List<int> enemies = new();
            foreach (int enemy in _enemyFilter)
            {
                enemies.Add(enemy);
            }

            return enemies;
        }
    }
}
