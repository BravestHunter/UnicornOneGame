using Leopotam.EcsLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems.Movement;
using UnicornOne.Battle.Ecs.Systems;
using UnityEngine;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Core.Utils;
using Codice.CM.Client.Differences;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.Utils;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class NavigationGym_EcsWorldScript : MonoBehaviour
    {
        public const int MinRivalCount = 0;
        public const int MaxRivalCount = 40;

        private static readonly Plane _plane = new Plane(Vector3.up, 0.0f);

        [Range(1, 15)]
        [SerializeField] private int _tilemapRadius;

        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private Material _tileAvailableMaterial;
        [SerializeField] private Material _tileUnavailableMaterial;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _rivalPrefab;
        [Range(MinRivalCount, MaxRivalCount)]
        [SerializeField] private int _rivalCount;

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

        private TimeService _timeService;
        private TilemapService _tilemapService;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _debugSystems;

        private int _playerEntity;
        private readonly List<int> _rivalEntities = new();

        private void Start()
        {
            _timeService = new TimeService();

            var tilemap = TilemapGenerator.Generate(_tilemapRadius);
            _tilemapService = new TilemapService(tilemap, _tilePrefab, _tileAvailableMaterial, _tileUnavailableMaterial);

            _world = new EcsWorld();

            _systems = new EcsSystems(_world);
            _systems.Add(new RandomDestinationTileChooseSystem());
            _systems.Add(new NavigationSystem());
            _systems.Add(new TilepathMoveSystem());
            _systems.Add(new TileMoveSystem());
            _systems.Add(new MoveSystem());
            _systems.Inject(_timeService, _tilemapService);
            _systems.Init();

            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _debugSystems.Add(new DebugMoveSystem());
            _debugSystems.Inject(_timeService, _tilemapService);
            _debugSystems.Init();

            InitPlayer();
            UpdateRivals();
        }

        private void Update()
        {
            ProcessMouse();

            _timeService.Delta = Time.deltaTime;

            _systems.Run();
            _debugSystems.Run();
        }

        private void OnDestroy()
        {
            _debugSystems.Destroy();
            _systems.Destroy();
            _world.Destroy();
        }

        private void InitPlayer()
        {
            _playerEntity = InstantiateUnit(_world, _playerPrefab, HexCoords.Center, 5.0f, _tilemapService);
        }

        private void UpdateRivals()
        {
            while (_rivalEntities.Count < _rivalCount)
            {
                float movementSpeed = Random.Range(1.0f, 10.0f);
                int rivalEntity = InstantiateUnit(_world, _rivalPrefab, _tilemapService.GetRandomAvailablePosition(), movementSpeed, _tilemapService);

                var raangomDestinationTileChoseFlagPool = _world.GetPool<RangomDestinationTileChoseFlag>();
                raangomDestinationTileChoseFlagPool.Add(rivalEntity);

                _rivalEntities.Add(rivalEntity);
            }

            while (_rivalEntities.Count > _rivalCount)
            {
                DestroyUnit(_rivalEntities.Last(), _world, _tilemapService);
                _rivalEntities.RemoveAt(_rivalEntities.Count - 1);
            }
        }

        private void ProcessMouse()
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

                    var destinationTileComponentPool = _world.GetPool<DestinationTileComponent>();
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

            world.DelEntity(entity);
        }
    }
}
