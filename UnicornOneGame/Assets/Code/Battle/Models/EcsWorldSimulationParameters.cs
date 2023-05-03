using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.ScriptableObjects;
using UnicornOne.Core.Utils;
using UnityEngine;

namespace UnicornOne.Battle.Models
{
    internal struct EcsWorldSimulationParameters
    {
        public Tilemap Tilemap;
        public TilemapSettings TilemapSettings;
        public UnitInstance[] AllyTeam;
        public UnitInstance[] EnemyTeam;
    }
}
