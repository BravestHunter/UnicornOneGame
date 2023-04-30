using Leopotam.EcsLite;
using UnicornOne.Battle.MonoBehaviours;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Components
{
    internal struct DebugStatusUIComponent : IEcsAutoReset<DebugStatusUIComponent>
    {
        public GameObject GameObject;
        public DebugStatusUIScript Script;

        public void AutoReset(ref DebugStatusUIComponent c)
        {
            if (c.GameObject != null)
            {
                GameObject.Destroy(c.GameObject);
                c.GameObject = null;
            }
        }
    }
}
