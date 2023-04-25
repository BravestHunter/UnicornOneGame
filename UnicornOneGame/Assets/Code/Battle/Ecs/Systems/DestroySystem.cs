using Leopotam.EcsLite;
using UnicornOne.Battle.Ecs.Components;

namespace UnicornOne.Battle.Ecs.Systems
{
    internal class DestroySystem : IEcsRunSystem
    {
        private EcsFilter _filter;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_filter == null)
            {
                _filter = world
                    .Filter<DestroyFlag>()
                    .End();
            }

            foreach (var entity in _filter)
            {
                world.DelEntity(entity);
            }
        }
    }
}
