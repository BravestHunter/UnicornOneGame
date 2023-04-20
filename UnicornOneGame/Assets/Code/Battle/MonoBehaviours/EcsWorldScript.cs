using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class EcsWorldScript : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private Unit[] _allyTeam;
        [SerializeField] private Unit[] _enemyTeam;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _debugStatusUIPrefab;

        private TimeService _timeService;
        private CameraService _cameraService;
        private TilemapService _tilemapService;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _debugSystems;

        private void Start()
        {
            _timeService = new TimeService();
            _cameraService = new CameraService(_camera);
            _tilemapService = new TilemapService(_tilePrefab, 10);

            _world = new EcsWorld();

            _systems = new EcsSystems(_world);
            _systems.Add(new UnitInitSystem(_allyTeam, _enemyTeam));
            _systems.Add(new TilemapInitSystem());
            _systems.Add(new MoveSystem());
            _systems.Add(new TilepathMoveSystem());
            _systems.Add(new RandomUnitMoveTargetSystem());
            _systems.Inject(_timeService, _cameraService, _tilemapService);
            _systems.Init();

            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _debugSystems.Add(new DebugMoveSystem());
            _debugSystems.Add(new DebugStatusUISystem(_debugStatusUIPrefab));
            _debugSystems.Inject(_timeService, _cameraService, _tilemapService);
            _debugSystems.Init();
        }

        private void Update()
        {
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
    }
}
