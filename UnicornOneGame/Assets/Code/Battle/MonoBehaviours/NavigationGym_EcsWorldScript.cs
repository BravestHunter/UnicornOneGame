using Leopotam.EcsLite;
using System.Collections.Generic;
using System.Linq;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems.Movement;
using UnicornOne.Battle.Ecs.Systems;
using UnityEngine;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Core.Utils;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.Utils;
using UnicornOne.Battle.ScriptableObjects;
using UnicornOne.Utils;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class NavigationGym_EcsWorldScript : BaseEcsWorldScript
    {
        public const int MinRivalCount = 0;
        public const int MaxRivalCount = 40;

        private static readonly Plane _plane = new Plane(Vector3.up, 0.0f);

        [SerializeField] private Camera _camera;
        [SerializeField] private TilemapScript _tilemapScript;

        [SerializeField] private UnicornOne.ScriptableObjects.Tilemap _initialTilemap;
        [SerializeField] private TilemapSettings _tilemapSettings;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _rivalPrefab;
        
        private Tilemap _tilemap;

        public override EcsWorldSimulationParameters Parameters
        {
            get
            {
                return new EcsWorldSimulationParameters()
                {
                    Tilemap = _tilemap,
                    TilemapSettings = _tilemapSettings,
                    AllyTeam = new UnitInstance[0],
                    EnemyTeam = new UnitInstance[0]
                };
            }
        }

        private int _rivalCount;
        public int RivalCount
        {
            get { return _rivalCount; }
            set
            {
                if (value < MinRivalCount || value > MaxRivalCount)
                {
                    return;
                }

                _rivalCount = value;
                UpdateRivals();
            }
        }

        private EcsWorldSimulation _simulation = new EcsWorldSimulation();

        private TimeService _timeService = null;
        private TilemapService _tilemapService = null;

        private int _playerEntity = -1;
        private readonly List<int> _rivalEntities = new();

        private void Awake()
        {
            _tilemap = new Tilemap(TilemapGenerator.Generate(8, 0.15f));
        }

        private void Start()
        {
            InitSimulation(Parameters);
        }

        private void Update()
        {
            ProcessMouseClick();

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

            InitPlayer();

            _rivalEntities.Clear();
            UpdateRivals();

            _tilemapScript.SetupTilemap(parameters.Tilemap, parameters.TilemapSettings);
        }

        private void UpdateParameters(EcsWorldSimulationParameters parameters)
        {
            _tilemap = parameters.Tilemap;
            _tilemapSettings = parameters.TilemapSettings;

            _tilemapService = new TilemapService(_tilemap, _tilemapSettings);
            _timeService = new TimeService(Time.timeSinceLevelLoad);
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
                new RandomDestinationTileChooseSystem(),

                // Navigation and movement
                new NavigationSystem(),
                new TileMoveSystem(),
                new MoveSystem(),
                new UnitRotationSystem(),
            };

            return systems;
        }

        private List<IEcsSystem> GetEcsDebugSystems()
        {
            List<IEcsSystem> systems = new()
            {
                new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(),
                new DebugMoveSystem(),
                new DebugTargetSystem()
            };

            return systems;
        }

        private List<IService> GetServices()
        {
            List<IService> services = new()
            {
                _timeService,
                _tilemapService
            };

            return services;
        }

        private void InitPlayer()
        {
            HexCoords position = _tilemapService.GetRandomAvailablePosition();
            _playerEntity = InstantiateUnit(_simulation.World, _playerPrefab, position, 5.0f, _tilemapService);
        }

        private void UpdateRivals()
        {
            while (_rivalEntities.Count < _rivalCount)
            {
                float movementSpeed = Random.Range(1.0f, 10.0f);
                int rivalEntity = InstantiateUnit(_simulation.World, _rivalPrefab, _tilemapService.GetRandomAvailablePosition(), movementSpeed, _tilemapService);

                var rangomDestinationTileChoseFlagPool = _simulation.World.GetPool<RangomDestinationTileChoseFlag>();
                rangomDestinationTileChoseFlagPool.Add(rivalEntity);

                _rivalEntities.Add(rivalEntity);
            }

            while (_rivalEntities.Count > _rivalCount)
            {
                DestroyUnit(_rivalEntities.Last(), _simulation.World, _tilemapService);
                _rivalEntities.RemoveAt(_rivalEntities.Count - 1);
            }
        }

        private void ProcessMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = _camera.ScreenPointToRay(mousePosition);

                float distande;
                if (_plane.Raycast(ray, out distande))
                {
                    Vector3 intersectionPoint = ray.GetPoint(distande);
                    HexCoords destinationTile = HexCoords.FromWorldCoords(new Vector2(intersectionPoint.x, intersectionPoint.z), _tilemapService.HexParams);

                    if (!_tilemapService.Tilemap.Tiles.TryGetValue(destinationTile, out Tile tile) || !tile.IsWalkable)
                    {
                        return;
                    }

                    var destinationTileComponentPool = _simulation.World.GetPool<DestinationTileComponent>();
                    if (destinationTileComponentPool.Has(_playerEntity))
                    {
                        destinationTileComponentPool.Del(_playerEntity);
                    }

                    ref var destinationTileComponent = ref destinationTileComponentPool.Add(_playerEntity);
                    destinationTileComponent.Position = destinationTile;
                }
            }
        }

        private static int InstantiateUnit(EcsWorld world, GameObject prefab, in HexCoords position, float movementSpeed, ITilemapService tilemapService)
        {
            int entity = world.NewEntity();

            var unitFlagPool = world.GetPool<UnitFlag>();
            unitFlagPool.Add(entity);

            var movementComponentPool = world.GetPool<MovementComponent>();
            ref var movementComponent = ref movementComponentPool.Add(entity);
            movementComponent.Speed = movementSpeed;

            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            ref var tilePositionComponent = ref tilePositionComponentPool.Add(entity);
            tilePositionComponent.Position = position;
            tilemapService.Tilemap.Tiles[position].IsReserved = true;

            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(entity);
            gameObjectUnityRefComponent.GameObject = GameObject.Instantiate(
                prefab, tilePositionComponent.Position.ToWorldCoordsXZ(tilemapService.HexParams), Quaternion.identity
            );

            return entity;
        }

        private static void DestroyUnit(int entity, EcsWorld world, ITilemapService tilemapService)
        {
            var gameObjectUnityRefComponentPool = world.GetPool<GameObjectUnityRefComponent>();
            var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(entity);
            GameObject.Destroy(gameObjectUnityRefComponent.GameObject);

            var tilePositionComponentPool = world.GetPool<TilePositionComponent>();
            var tilePositionComponent = tilePositionComponentPool.Get(entity);
            tilemapService.Tilemap.Tiles[tilePositionComponent.Position].IsReserved = false;

            var targetTileMoveComponentPool = world.GetPool<TargetTileMoveComponent>();
            if (targetTileMoveComponentPool.Has(entity))
            {
                var targetTileMoveComponent = targetTileMoveComponentPool.Get(entity);
                tilemapService.Tilemap.Tiles[targetTileMoveComponent.Position].IsReserved = false;
            }

            world.DelEntity(entity);
        }
    }
}
