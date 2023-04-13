using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class UnitInitSystem : IEcsInitSystem
    {
        private readonly Unit[] _heroTeam;
        private readonly Unit[] _enemyTeam;

        private static Vector3 RandomPosition
        {
            get
            {
                Vector2 random = Random.insideUnitCircle * 5.0f;
                return new Vector3(random.x, 0.0f, random.y);
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
                SpawnUnit(world, hero, RandomPosition, true);
            }
            foreach (var enemy in _enemyTeam)
            {
                SpawnUnit(world, enemy, RandomPosition, false);
            }
        }

        private void SpawnUnit(EcsWorld world, Unit unit, Vector3 position, bool isAlly)
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

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(entity);
            gameObjectUnityRefComponent.GameObject = Object.Instantiate(unit.Prefab, position, Quaternion.identity);

            foreach (var component in unit.Components)
            {
                ProcessComponent(world, entity, component);
            }
        }

        private void ProcessComponent(EcsWorld world, int entity, UnitComponent component)
        {
            // TODO: refactor this sh*t
            switch (component)
            {
                case ScriptableObjects.HealthComponent health:
                    {
                        var healthComponentPool = world.GetPool<Components.HealthComponent>();
                        ref var healthComponent = ref healthComponentPool.Add(entity);
                        healthComponent.MaxHealth = health.Health;
                        healthComponent.CurrentHealth = health.Health;
                    }
                    break;
            }
        }
    }
}
