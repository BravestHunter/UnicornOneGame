using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Other.Ability;

namespace UnicornOne.Ecs.Components
{
    internal struct EnemyBehaviorAiComponent
    {
        public enum State
        {
            SearchingForTarget,
            SelectingAbility,
            MovingToTarget,
            AttackingWithAbility
        }

        public State CurrentState;
        public Ability SelectedAbility;
        public int SelectedAbilityIndex;
    }
}
