using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace UnicornOne.Battle.MonoBehaviours
{
    public class EcsWorldScript : MonoBehaviour
    {
        private EcsWorld _world;
        private IEcsSystems _systems;
#if UNITY_EDITOR
        private IEcsSystems _debugSystems;
#endif

        private void Start()
        {
            _world = new EcsWorld();

            _systems = new EcsSystems(_world);
            _systems.Init();

#if UNITY_EDITOR
            _debugSystems = new EcsSystems(_world);
            _debugSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
            _debugSystems.Init();
#endif
        }

        private void Update()
        {
            _systems.Run();

#if UNITY_EDITOR
            _debugSystems.Run();
#endif
        }

        private void OnDestroy()
        {
#if UNITY_EDITOR
            _debugSystems.Destroy();
#endif
            _systems.Destroy();
            _world.Destroy();
        }
    }
}
