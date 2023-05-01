using System.Collections;
using System.Collections.Generic;
using UnicornOne.Battle.Utils;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Components
{
    internal struct UnitAiComponent
    {
        public UnitAiState State;
        public float TargetSetTime;

        public override string ToString()
        {
            return $"AI:{State}";
        }
    }
}
