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
    internal class AttackRechargeSystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<AttackRechargeComponent>()
                    .End();
            }

            var attackRechargePool = world.GetPool<AttackRechargeComponent>();
            var atackParametersPool = world.GetPool<AtackParametersComponent>();

            foreach (var entity in _filter)
            {
                var attackRechargeComponent = attackRechargePool.Get(entity);

                float cooldown = 0.0f;
                if (atackParametersPool.Has(entity))
                {
                    var atackParametersComponent = atackParametersPool.Get(entity);
                    cooldown = atackParametersComponent.AttackRechargeTime;
                }
                else
                {
                    cooldown = 2.0f;
                }

                if (Time.timeSinceLevelLoad - attackRechargeComponent.LastAttackTime >= cooldown)
                {
                    attackRechargePool.Del(entity);
                }
            }
        }
    }
}
