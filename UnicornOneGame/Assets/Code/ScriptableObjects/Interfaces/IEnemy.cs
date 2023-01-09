﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Assets.Code.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Data;

namespace UnicornOne.ScriptableObjects.Interfaces
{
    internal interface IEnemy
    {
        PrefabInfo PrefabInfo { get; }
        MoveInfo MoveInfo { get; }
        HealthInfo HealthInfo { get; }
        AttackInfo AttackInfo { get; }
    }
}
