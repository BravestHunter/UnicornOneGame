using Leopotam.EcsLite;
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
using Leopotam.EcsLite.Di;
using UnicornOne.Battle.Ecs.Components;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class DamageGym_EcsWorldScript : MonoBehaviour
    {
        [SerializeField] private GameObject _unitPrefab;

        private TimeService _timeService;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _debugSystems;

        private int _playerEntity = 0;

        private void Start()
        {
            _timeService = new TimeService();

            _world = new EcsWorld();

            _systems = new EcsSystems(_world);


            _systems.Inject(_timeService);
            _systems.Init();

            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _debugSystems.Inject(_timeService);
            _debugSystems.Init();

            InitPlayer();
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

        private void InitPlayer()
        {
            _playerEntity = _world.NewEntity();

            var gameObjectUnityRefComponentPool = _world.GetPool<GameObjectUnityRefComponent>();
            ref var gameObjectUnityRefComponent = ref gameObjectUnityRefComponentPool.Add(_playerEntity);
            gameObjectUnityRefComponent.GameObject = GameObject.Instantiate(_unitPrefab);
        }
    }
}
