using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnicornOne.Battle.Ecs.Services;
using UnicornOne.Battle.Ecs.Systems;
using UnicornOne.Battle.Ecs.Systems.Movement;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.ScriptableObjects;
using UnicornOne.Battle.Utils;
using UnicornOne.Core.Utils;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class EcsWorldScript : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private TilemapScript _tilemapScript;

        [SerializeField] private TilemapSettings _tilemapSettings;
        [SerializeField] private DebugStatusUISettings _debugStatusUISettings;

        [SerializeField] private UnitInstance[] _allyTeam;
        [SerializeField] private UnitInstance[] _enemyTeam;

        private EcsWorldSimulation _simulation = new EcsWorldSimulation();
        private Tilemap _tilemap = null;

        private void Start()
        {
            InitSimulation(true, true);
        }

        private void Update()
        {
            _simulation.Update(Time.deltaTime, Time.timeSinceLevelLoad);
        }

        private void OnDestroy()
        {
            _simulation.Dispose();
        }

        public void InitSimulation(bool generateTilemap, bool generateUnitPositions)
        {
            if (generateTilemap)
            {
                _tilemap = TilemapGenerator.Generate(10);
            }
            else
            {
                CleanReservedTiles(_tilemap);
            }

            if (generateTilemap || generateUnitPositions)
            {
                SetRandomCellsForUnits(_allyTeam, _tilemap);
                SetRandomCellsForUnits(_enemyTeam, _tilemap);
            }

            EcsWorldSimulationParameters parameters = new()
            {
                Camera = _camera,
                Tilemap = _tilemap,
                TilemapSettings = _tilemapSettings,
                AllyTeam = _allyTeam,
                EnemyTeam = _enemyTeam,
                DebugStatusUISettings = _debugStatusUISettings
            };

            _simulation.Init(parameters);

            _tilemapScript.SetupTilemap(_tilemap, _tilemapSettings);
        }

        private static void CleanReservedTiles(Tilemap tilemap)
        {
            foreach (var tileEntry in tilemap)
            {
                tileEntry.Value.IsReserved = false;
            }
        }

        private static void SetRandomCellsForUnits(UnitInstance[] team, Tilemap tilemap)
        {
            // TODO: REMOVE THIS TRASH

            HashSet<HexCoords> reserved = new();

            HexCoords GetRandomFreePosition()
            {
                var availableTiles = tilemap.Where(p => p.Value.IsWalkable && !reserved.Contains(p.Key));
                int availableTilesCount = availableTiles.Count();

                if (availableTilesCount == 0)
                {
                    throw new System.Exception("There is no available cells for unit");
                }

                HexCoords position = availableTiles.ElementAt(Random.Range(0, availableTilesCount)).Key;

                reserved.Add(position);

                return position;
            }

            for (int i = 0; i < team.Length; i++)
            {
                team[i].Position = GetRandomFreePosition();
            }
        }
    }
}
