using Leopotam.EcsLite;
using System;
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
using System.Diagnostics;
using Codice.CM.Client.Differences;
using UnicornOne.Battle.Ecs.Components.Flags;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class NavigationGym_EcsWorldScript : MonoBehaviour
    {
        private static readonly Plane _plane = new Plane(Vector3.up, 0.0f);

        [Range(1, 15)]
        [SerializeField] private int _tilemapRadius;

        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _unitPrefab;

        private TimeService _timeService;
        private TilemapService _tilemapService;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _debugSystems;
        private int _unitEntity;

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

        private void InitPlayer()
        {
            _unitEntity = _world.NewEntity();

            var unitFlagPool = _world.GetPool<UnitFlag>();
            unitFlagPool.Add(_unitEntity);

            var tilePositionComponentPool = _world.GetPool<TilePositionComponent>();
            ref var tilePositionComponent = ref tilePositionComponentPool.Add(_unitEntity);
            tilePositionComponent.Position = HexCoords.FromAxial(0, 0);

            var gameObjectUnityRefComponentPool = _world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(_unitEntity);
            gameObjectUnityRefComponent.GameObject = 
                GameObject.Instantiate(_unitPrefab, tilePositionComponent.Position.ToWorldCoordsXZ(_tilemapService.HexParams), Quaternion.identity);
        }

        private void ProcessMouse()
        {
            Mouse mouse = Mouse.current;
            if (mouse.rightButton.wasPressedThisFrame)
            {
                Vector3 mousePosition = mouse.position.ReadValue();
                Ray ray = _camera.ScreenPointToRay(mousePosition);

                float distande;
                if (_plane.Raycast(ray, out distande))
                {
                    Vector3 intersectionPoint = ray.GetPoint(distande);
                    HexCoords destinationTile = HexCoords.FromWorldCoords(new Vector2(intersectionPoint.x, intersectionPoint.z), _tilemapService.HexParams);

                    var destinationTileComponentPool = _world.GetPool<DestinationTileComponent>();
                    if (destinationTileComponentPool.Has(_unitEntity))
                    {
                        ref var destinationTileComponent = ref destinationTileComponentPool.Get(_unitEntity);
                        destinationTileComponent.Position = destinationTile;
                    }
                    else
                    {
                        ref var destinationTileComponent = ref destinationTileComponentPool.Add(_unitEntity);
                        destinationTileComponent.Position = destinationTile;
                    }
                }
            }
        }
    }
}
