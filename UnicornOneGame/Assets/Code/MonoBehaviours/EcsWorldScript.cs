using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnicornOne.Assets.Code.Ecs.Systems;
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
        [SerializeField] private Camera Camera;
        [SerializeField] private GameObject Label3D;
        [SerializeField] private GameSettings GameSettings;

        private EcsWorld _world;
        private IEcsSystems _systems;

#if UNITY_EDITOR
        private IEcsSystems _debugSystems;
#endif

        private void Start()
        {
            _world = new EcsWorld();

            var uniqueHeroes = Heroes.Distinct();
            // Get all enemy types from level waves
            var uniqueEnemies = Level.Script.Waves.Select(w => w.Enemy).Distinct();

            // TODO: combine all these services with init data into one?
            var levelService = new LevelService(Level);
            var heroService = new HeroService(Heroes);
            var cameraService = new CameraService(Camera);
            var uiService = new UIService(Label3D);
            var settingsService = new SettingsService(GameSettings);
            var abilityService = new AbilityService(uniqueHeroes, uniqueEnemies);

            _systems = new EcsSystems(_world);
            _systems.Add(new LevelInitSystem());
            _systems.Add(new HeroInitSystem());
            _systems.Add(new EnemySpawnSystem());
            _systems.Add(new AttackRechargeSystem());
            _systems.Add(new SquadAiSystem());
            _systems.Add(new HeroAiSystem());
            _systems.Add(new EnemyAiSystem());
            _systems.Add(new AbilitySystem());
            _systems.Add(new AttackSystem());
            _systems.Add(new EffectSystem());
            _systems.Add(new ProjectileMoveSystem());
            _systems.Add(new ProjectileHitSystem());
            _systems.Add(new NavigationSystem());
            _systems.Add(new AnimationSystem());
            _systems.Add(new DamageSystem());
            _systems.Add(new CameraMoveSystem());
            _systems.Add(new LifetimeSystem());
            _systems.Add(new DeathSystem());
            _systems.Add(new DestroySystem());
            _systems.Inject(levelService, heroService, cameraService, uiService, settingsService, abilityService);
            _systems.Init();

#if UNITY_EDITOR
            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _debugSystems.Add(new TargetDebugSystem());
            _debugSystems.Init();
#endif
        }

        private void Update()
        {
            _systems.Run();

#if UNITY_EDITOR
            _debugSystems.Run();
#endif
        }

        private void OnDestroy()
        {
            _systems?.Destroy();

#if UNITY_EDITOR
            _debugSystems?.Destroy();
#endif
            
            _world?.Destroy();
        }
    }
}
