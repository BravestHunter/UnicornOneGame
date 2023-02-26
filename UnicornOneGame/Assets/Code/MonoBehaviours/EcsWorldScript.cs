using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnicornOne.Assets.Code.Ecs.Systems;
using UnicornOne.Ecs;
using UnicornOne.Ecs.Services;
using UnicornOne.Ecs.Systems;
using UnicornOne.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace UnicornOne.MonoBehaviours
{
    public class EcsWorldScript : MonoBehaviour
    {
        public event Action GameFinished;

        [SerializeField] private List<Hero> Heroes;
        [SerializeField] private Camera Camera;
        [SerializeField] private GameObject Label3D;
        [SerializeField] private GameSettings GameSettings;

        private bool _isRunning = false;

        private EcsWorld _world;
        private IEcsSystems _systems;

#if UNITY_EDITOR
        private IEcsSystems _debugSystems;
#endif

        private GameControlService _gameControlService = null;

        private void Update()
        {
            if (!_isRunning)
            {
                return;
            }

            _systems.Run();

#if UNITY_EDITOR
            _debugSystems.Run();
#endif

            if (_gameControlService.GameFinished)
            {
                Destroy();
                GameFinished?.Invoke();
            }
        }

        private void OnDestroy()
        {
            Destroy();
        }

        public void Init(Level level)
        {
            _world = new EcsWorld();

            var uniqueHeroes = Heroes.Distinct();
            // Get all enemy types from level waves
            var uniqueEnemies = level.Script.Waves.Select(w => w.Enemy).Distinct();

            _gameControlService = new();
            // TODO: combine all these services with init data into one?
            var levelService = new LevelService(level);
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
            _systems.Inject(_gameControlService, levelService, heroService, cameraService, uiService, settingsService, abilityService);
            _systems.Init();

#if UNITY_EDITOR
            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _debugSystems.Add(new TargetDebugSystem());
            _debugSystems.Init();
#endif

            _isRunning = true;
        }

        private void Destroy()
        {
            _isRunning = false;

            _gameControlService = null;

            _systems?.Destroy();
            _systems = null;

#if UNITY_EDITOR
            _debugSystems?.Destroy();
            _debugSystems = null;
#endif

            _world?.Destroy();
            _world = null;
        }
    }
}
