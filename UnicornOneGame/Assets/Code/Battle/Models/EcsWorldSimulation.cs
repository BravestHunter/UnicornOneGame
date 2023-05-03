using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using UnicornOne.Battle.Ecs.Services;

namespace UnicornOne.Battle.Models
{
    internal class EcsWorldSimulation : IDisposable
    {
        private bool _isInitialized = false;

        private EcsWorld _world = null;
        private IEcsSystems _systems = null;
        private IEcsSystems _debugSystems = null;

        public EcsWorld World => _world;

        public void Init(IEnumerable<IEcsSystem> systems, IEnumerable<IEcsSystem> debugSystems, IEnumerable<IService> services)
        {
            if (_isInitialized)
            {
                Dispose();
            }

            _world = new EcsWorld();

            _systems = new EcsSystems(_world);
            InitSystems(_systems, systems, services);

            _debugSystems = new EcsSystems(_world);
            InitSystems(_debugSystems, debugSystems, services);

            _isInitialized = true;
        }

        public void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            _systems.Run();
            _debugSystems.Run();
        }

        public void Dispose()
        {
            if (!_isInitialized)
            {
                return;
            }

            _debugSystems.Destroy();
            _debugSystems = null;
            _systems.Destroy();
            _systems = null;
            _world.Destroy();
            _world = null;
        }

        private static void InitSystems(IEcsSystems systems, IEnumerable<IEcsSystem> systemCollection, IEnumerable<IService> services)
        {
            foreach (var system in systemCollection)
            {
                systems.Add(system);
            }
            foreach (var service in services)
            {
                systems.Inject(service);
            }
            systems.Init();
        }
    }
}
