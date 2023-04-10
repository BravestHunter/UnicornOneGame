using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class EcsWorldScript : MonoBehaviour
    {
        [SerializeField] private GameObject _heroPrefab;

        private TimeService _timeService;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private IEcsSystems _debugSystems;

        private void Start()
        {
            _timeService = new TimeService();

            _world = new EcsWorld();

            _systems = new EcsSystems(_world);
            _systems.Add(new HeroInitSystem(_heroPrefab));
            _systems.Inject(_timeService);
            _systems.Init();

            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
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
