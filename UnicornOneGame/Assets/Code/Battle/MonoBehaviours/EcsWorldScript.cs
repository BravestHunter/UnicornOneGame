using Leopotam.EcsLite;
using System.Collections.Generic;
using System.Linq;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems;
using UnicornOne.Battle.Ecs.Systems.Movement;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.ScriptableObjects;
using UnicornOne.Battle.Utils;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class EcsWorldScript : BaseEcsWorldScript
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private TilemapScript _tilemapScript;

        private Tilemap _tilemap;
        [SerializeField] private TilemapSettings _tilemapSettings;
        [SerializeField] private DebugStatusUISettings _debugStatusUISettings;
        [SerializeField] private UnitInstance[] _allyTeam;
        [SerializeField] private UnitInstance[] _enemyTeam;

        public override EcsWorldSimulationParameters Parameters
        {
            get
            {
                return new EcsWorldSimulationParameters()
                {
                    Tilemap = _tilemap,
                    TilemapSettings = _tilemapSettings,
                    AllyTeam = _allyTeam,
                    EnemyTeam = _enemyTeam
                };
            }
        }

        private EcsWorldSimulation _simulation = new EcsWorldSimulation();

        private TimeService _timeService = null;
        private CameraService _cameraService = null;
        private TilemapService _tilemapService = null;
        private AbilityService _abilityService = null;

        private void Awake()
        {
            _tilemap = TilemapGenerator.Generate(8);
        }

        private void Start()
        {
            InitSimulation(Parameters);
        }

        private void Update()
        {
            _timeService.Delta = Time.deltaTime;
            _timeService.CurrentTime = Time.timeSinceLevelLoad;

            _simulation.Update();
        }

        private void OnDestroy()
        {
            _simulation.Dispose();
        }

        public override void InitSimulation(EcsWorldSimulationParameters parameters)
        {
            UpdateParameters(parameters);

            CleanReservedTiles();

            var systems = GetEcsSystems();
            var debugSystems = GetEcsDebugSystems();
            var services = GetServices();
            _simulation.Init(systems, debugSystems, services);

            _tilemapScript.SetupTilemap(parameters.Tilemap, parameters.TilemapSettings);
        }

        private void UpdateParameters(EcsWorldSimulationParameters parameters)
        {
            _tilemap = parameters.Tilemap;
            _tilemapSettings = parameters.TilemapSettings;
            _allyTeam = parameters.AllyTeam;
            _enemyTeam = parameters.EnemyTeam;

            _tilemapService = new TilemapService(_tilemap, _tilemapSettings);
            _timeService = new TimeService(Time.timeSinceLevelLoad);
            _cameraService = new CameraService(_camera);

            var units = _allyTeam.Concat(_enemyTeam).Select(ui => ui.Unit).Distinct();
            List<Ability> abilities = new();
            foreach (var unit in units)
            {
                abilities.AddRange(unit.Abilities);
            }
            _abilityService = new AbilityService(abilities.ToArray());
        }

        private void CleanReservedTiles()
        {
            foreach (var tileEntry in _tilemap)
            {
                tileEntry.Value.IsReserved = false;
            }
        }

        private List<IEcsSystem> GetEcsSystems()
        {
            List<IEcsSystem> systems = new()
            {
                // Init
                new UnitInitSystem(_allyTeam, _enemyTeam),

                // AI
                new UnitAiSystem(),

                // Ability
                new AbilitySystem(),

                // Projectile
                new ProjectileSystem(),

                // Damage and health
                new DamageSystem(),
                new HealthCheckSystem(),

                // Navigation and movement
                new NavigationSystem(),
                new TileMoveSystem(),
                new MoveSystem(),
                new UnitRotationSystem(),

                // Animation
                new AnimationSystem(),

                // Final systems
                new DestroySystem()
            };

            return systems;
        }

        private List<IEcsSystem> GetEcsDebugSystems()
        {
            List<IEcsSystem> systems = new()
            {
                new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(),
                new DebugMoveSystem(),
                new DebugTargetSystem(),
                new DebugStatusUISystem(_debugStatusUISettings)
            };

            return systems;
        }

        private List<IService> GetServices()
        {
            List<IService> services = new()
            {
                _timeService,
                _cameraService,
                _tilemapService,
                _abilityService
            };

            return services;
        }
    }
}
