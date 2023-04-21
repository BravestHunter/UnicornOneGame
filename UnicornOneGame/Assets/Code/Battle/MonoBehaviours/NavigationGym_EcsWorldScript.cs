using Leopotam.EcsLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems.Movement;
using UnicornOne.Battle.Ecs.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;
using UnicornOne.Core.Utils;
using Codice.CM.Client.Differences;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class NavigationGym_EcsWorldScript : MonoBehaviour
    {
        private static readonly Plane _plane = new Plane(Vector3.up, 0.0f);

        [Range(1, 15)]
        [SerializeField] private int _tilemapRadius;

        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _playerPrefab;

        private TimeService _timeService;
        private TilemapService _tilemapService;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _debugSystems;
        private int _playerEntity;

        private void Start()
        {
            _timeService = new TimeService();
            _tilemapService = new TilemapService(_tilePrefab, _tilemapRadius);

            _world = new EcsWorld();

            _systems = new EcsSystems(_world);
            _systems.Add(new TilemapInitSystem());
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

        public void TeleportPlayerRandomly()
        {
            int q = Random.Range(-6, 7);
            int r = Random.Range(-6, 7);
            var newHexPosition = HexCoords.FromCube(q, r);

            var tilePositionComponentPool = _world.GetPool<TilePositionComponent>();
            ref var tilePositionComponent = ref tilePositionComponentPool.Get(_playerEntity);
            tilePositionComponent.Position = newHexPosition;

            var gameObjectUnityRefComponentPool = _world.GetPool<GameObjectUnityRefComponent>();
            var gameObjectUnityRefComponent = gameObjectUnityRefComponentPool.Get(_playerEntity);
            gameObjectUnityRefComponent.GameObject.transform.position = newHexPosition.ToWorldCoordsXZ(_tilemapService.HexParams);
        }

        private void InitPlayer()
        {
            _playerEntity = _world.NewEntity();

            var unitFlagPool = _world.GetPool<UnitFlag>();
            unitFlagPool.Add(_playerEntity);

            var tilePositionComponentPool = _world.GetPool<TilePositionComponent>();
            ref var tilePositionComponent = ref tilePositionComponentPool.Add(_playerEntity);
            tilePositionComponent.Position = HexCoords.FromAxial(0, 0);

            var gameObjectUnityRefComponentPool = _world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(_playerEntity);
            gameObjectUnityRefComponent.GameObject = 
                GameObject.Instantiate(_playerPrefab, tilePositionComponent.Position.ToWorldCoordsXZ(_tilemapService.HexParams), Quaternion.identity);
        }

        private void ProcessMouse()
        {
            var mouse = Mouse.current;
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Vector3 mousePosition = mouse.position.ReadValue();
                Ray ray = _camera.ScreenPointToRay(mousePosition);

                float distande;
                if (_plane.Raycast(ray, out distande))
                {
                    Vector3 intersectionPoint = ray.GetPoint(distande);
                    HexCoords destinationTile = HexCoords.FromWorldCoords(new Vector2(intersectionPoint.x, intersectionPoint.z), _tilemapService.HexParams);

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
    }
}
