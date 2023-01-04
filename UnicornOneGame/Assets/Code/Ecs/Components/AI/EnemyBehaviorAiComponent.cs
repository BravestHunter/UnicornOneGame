using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOne.Ecs.Components
{
    internal struct EnemyBehaviorAiComponent
    {
        public enum State
        {
            SearchForTarget,
            MoveToTarget
        }

        public State CurrentState;
    }
}
