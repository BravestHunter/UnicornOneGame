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

        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private Material _tileWalkableMaterial;
        [SerializeField] private Material _tileUnwalkableMaterial;

        [SerializeField] private DebugStatusUISettings _debugStatusUISettings;

        [SerializeField] private UnitInstance[] _allyTeam;
        [SerializeField] private UnitInstance[] _enemyTeam;

        private EcsWorldSimulation _simulation = new EcsWorldSimulation();
        private Tilemap _tilemap = null;

        private void Start()
        {
            _tilemap = TilemapGenerator.Generate(10);

            SetRandomCellsForUnits(_allyTeam, _tilemap);
            SetRandomCellsForUnits(_enemyTeam, _tilemap);

            InitSimulation();
        }

        private void Update()
        {
            _simulation.Update(Time.deltaTime, Time.timeSinceLevelLoad);
        }

        private void OnDestroy()
        {
            _simulation.Dispose();
        }

        private void InitSimulation()
        {
            EcsWorldSimulationParameters parameters = new()
            {
                Camera = _camera,
                TilePrefab = _tilePrefab,
                TileWalkableMaterial = _tileWalkableMaterial,
                TileUnwalkableMaterial = _tileUnwalkableMaterial,
                DebugStatusUISettings = _debugStatusUISettings,
                Tilemap = _tilemap,
                AllyTeam = _allyTeam,
                EnemyTeam = _enemyTeam
            };

            _simulation.Init(parameters);
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
