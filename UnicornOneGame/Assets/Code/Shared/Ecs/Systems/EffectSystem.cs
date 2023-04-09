using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class EffectSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<EffectFlag>()
                    .Inc<EffectLifeSpanComponent>()
                    .Exc<DestroyRequest>()
                    .End();
            }

            var effectLifeSpanPool = world.GetPool<EffectLifeSpanComponent>();
            var destroyRequestPool = world.GetPool<DestroyRequest>();

            foreach (var entity in _filter)
            {
                var effectLifeSpanComponent = effectLifeSpanPool.Get(entity);

                if (Time.timeSinceLevelLoad >= effectLifeSpanComponent.CreationTime + effectLifeSpanComponent.LifeSpan)
                {
                    destroyRequestPool.Add(entity);
                }
            }
        }
    }
}
