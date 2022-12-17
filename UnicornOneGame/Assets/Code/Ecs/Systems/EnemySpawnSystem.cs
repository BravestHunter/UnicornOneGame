using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class EnemySpawnSystem : IEcsRunSystem
    {
        private const int MaxEnemyCount = 10;

        private readonly EcsCustomInject<MobService> _mobService;
        private EcsFilter _enemyFilter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_enemyFilter == null)
            {
                _enemyFilter = world
                    .Filter<EnemyFlag>()
                    .End();
            }

            int enemiesToSpawn = MaxEnemyCount - _enemyFilter.GetEntitiesCount();
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy(world);
            }
        }

        private void SpawnEnemy(EcsWorld world)
        {
            var entity = world.NewEntity();

            EcsPool<EnemyFlag> enemyFlagPool = world.GetPool<EnemyFlag>();
            enemyFlagPool.Add(entity);

            var enemyGameObject = GameObject.Instantiate(_mobService.Value.EnemyPrefab);
            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle * 20.0f;
            float randomAngle = UnityEngine.Random.Range(0.0f, 360.0f);
            enemyGameObject.transform.position = new Vector3(randomPosition.x, 0.0f, randomPosition.y);
            enemyGameObject.transform.rotation = Quaternion.Euler(0.0f, randomAngle, 0.0f);
        }
    }
}
