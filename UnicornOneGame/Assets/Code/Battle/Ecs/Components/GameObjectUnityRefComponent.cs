using Leopotam.EcsLite;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Components
{
    internal struct GameObjectUnityRefComponent : IEcsAutoReset<GameObjectUnityRefComponent>
    {
        public GameObject GameObject;

        public void AutoReset(ref GameObjectUnityRefComponent c)
        {
            if (c.GameObject != null)
            {
                GameObject.Destroy(c.GameObject);
                c.GameObject = null;
            }
        }
    }
}
