using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class EcsWorldScript : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private GameObject _heroPrefab;
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private GameObject _debugStatusUIPrefab;

        private TimeService _timeService;
        private CameraService _cameraService;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _debugSystems;

        private void Start()
        {
            _timeService = new TimeService();
            _cameraService = new CameraService(_camera);

            _world = new EcsWorld();

            _systems = new EcsSystems(_world);
            _systems.Add(new HeroInitSystem(_heroPrefab));
            _systems.Add(new EnemyInitSystem(_enemyPrefab));
            _systems.Inject(_timeService, _cameraService);
            _systems.Init();

            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _debugSystems.Add(new DebugStatusUISystem(_debugStatusUIPrefab));
            _debugSystems.Inject(_timeService, _cameraService);
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
