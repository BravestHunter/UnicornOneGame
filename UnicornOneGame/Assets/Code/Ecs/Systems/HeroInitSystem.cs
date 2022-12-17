using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Services;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Systems
{
    internal class HeroInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<HeroService> _heroService;

        public void Init(IEcsSystems systems)
        {
            GameObject heroGameObject = GameObject.Instantiate(_heroService.Value.Prefab);

            EcsWorld world = systems.GetWorld();

            NavMeshAgent agent = heroGameObject.GetComponent<NavMeshAgent>();
            //agent.destination = heroGameObject.transform.position + new Vector3(10.0f, 0.0f, 0.0f);
        }
    }
}
