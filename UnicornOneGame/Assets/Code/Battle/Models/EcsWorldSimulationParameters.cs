using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.Models
{
    internal struct EcsWorldSimulationParameters
    {
        public Camera Camera;
        public GameObject TilePrefab;
        public Material TileWalkableMaterial;
        public Material TileUnwalkableMaterial;
        public DebugStatusUISettings DebugStatusUISettings;

        public Tilemap Tilemap;
        public UnitInstance[] AllyTeam;
        public UnitInstance[] EnemyTeam;
    }
}
