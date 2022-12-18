using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOne.Ecs.Components.AI
{
    internal struct AiBehaviorComponent
    {
        public enum HeroAiState
        {
            SearchingForTarget,
            WalkingToTarget
        }

        public HeroAiState State;
    }
}
