using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems.Movement;
using UnicornOne.Battle.Ecs.Systems;
using UnicornOne.Battle.Utils;
using UnityEngine;

namespace UnicornOne.Battle.Models
{
    internal class EcsWorldSimulation : IDisposable
    {
        public bool _isInitialized = false;

        private TimeService _timeService;
        private CameraService _cameraService;
        private TilemapService _tilemapService;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _debugSystems;

        public void Init(in EcsWorldSimulationParameters parameters)
        {
            if (_isInitialized)
            {
                Dispose();
            }

            _timeService = new TimeService(Time.timeSinceLevelLoad);
            _cameraService = new CameraService(parameters.Camera);
            _tilemapService = new TilemapService(parameters.Tilemap, parameters.TilemapSettings);

            _world = new EcsWorld();

            _systems = new EcsSystems(_world);

            // Init
            _systems.Add(new UnitInitSystem(parameters.AllyTeam, parameters.EnemyTeam));

            // AI
            _systems.Add(new UnitAiSystem());
            _systems.Add(new AttackSystem());

            // Damage and health
            _systems.Add(new DamageSystem());
            _systems.Add(new HealthCheckSystem());

            // Navigation and movement
            _systems.Add(new NavigationSystem());
            _systems.Add(new TileMoveSystem());
            _systems.Add(new MoveSystem());
            _systems.Add(new UnitRotationSystem());

            // Animation
            _systems.Add(new AnimationSystem());

            // Final systems
            _systems.Add(new DestroySystem());

            _systems.Inject(_timeService, _cameraService, _tilemapService);
            _systems.Init();

            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _debugSystems.Add(new DebugMoveSystem());
            _debugSystems.Add(new DebugTargetSystem());
            _debugSystems.Add(new DebugStatusUISystem(parameters.DebugStatusUISettings));
            _debugSystems.Inject(_timeService, _cameraService, _tilemapService);
            _debugSystems.Init();

            _isInitialized = true;
        }

        public void Update(float delta, float time)
        {
            if (!_isInitialized)
            {
                return;
            }

            _timeService.Delta = delta;
            _timeService.CurrentTime = time;

            _systems.Run();
            _debugSystems.Run();
        }

        public void Dispose()
        {
            if (!_isInitialized)
            {
                return;
            }

            _timeService = null;
            _cameraService = null;
            _tilemapService = null;

            _debugSystems.Destroy();
            _debugSystems = null;
            _systems.Destroy();
            _systems = null;
            _world.Destroy();
            _world = null;
        }
    }
}
