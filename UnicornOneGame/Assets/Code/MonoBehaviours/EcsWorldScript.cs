using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnicornOne.Ecs;
using UnicornOne.Ecs.Services;
using UnicornOne.Ecs.Systems;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.MonoBehaviours
{
    public class EcsWorldScript : MonoBehaviour
    {
        [SerializeField] private Level Level;
        [SerializeField] private List<Hero> Heroes;
        [SerializeField] private Mob Enemy;

        [SerializeField] private Camera Camera;

        private EcsWorld _world;
        private IEcsSystems _systems;

        private void Start()
        {
            _world = new EcsWorld();

            // TODO: combine all these services with init data into one?
            LevelService levelService = new LevelService(Level);
            HeroService heroService = new HeroService(Heroes);
            MobService mobService = new MobService(Enemy);

            var cameraService = new CameraService(Camera);

            _systems = new EcsSystems(_world);
            _systems.Add(new LevelInitSystem());
            _systems.Add(new HeroInitSystem());
            _systems.Add(new EnemySpawnSystem());
            _systems.Add(new AiSystem());
            _systems.Add(new AttackSystem());
            _systems.Add(new NavigationSystem());
            _systems.Add(new AnimationSystem());
            _systems.Add(new DamageSystem());
            _systems.Add(new CameraMoveSystem());
            _systems.Add(new DeathSystem());
            _systems.Add(new DestroySystem());
            _systems.Inject(levelService, heroService, mobService, cameraService);
            _systems.Init();
        }

        private void Update()
        {
            _systems.Run();
        }

        private void OnDestroy()
        {
            _systems?.Destroy();
            _world?.Destroy();
        }
    }
}
