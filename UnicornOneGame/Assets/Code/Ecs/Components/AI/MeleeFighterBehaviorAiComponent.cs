using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOne.Ecs.Components.AI
{
    internal struct MeleeFighterBehaviorAiComponent
    {
        public enum State
        {
            SearchForTarget,
            MoveToTarget,
            AttackTarget
        }

        public State CurrentState;
    }
}
