using System.Collections.Generic;
using System.Linq;
using UnicornOne.Battle.Models;
using UnicornOne.Battle.Utils;
using UnicornOne.Core.Utils;
using UnityEditor;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class SimulationRestartDebugUIScript : BaseDebugUIScript
    {
        private bool _regenerateTilemap = false;
        private bool _regenerateUnitPositions = false;

        [SerializeField] private BaseEcsWorldScript _ecsWorldScript;

        protected override Vector2Int Size => new Vector2Int(200, 80);

        protected override string Title => "Restart simulation";

        protected override void BuildWindowLayout()
        {
            _regenerateTilemap = GUILayout.Toggle(_regenerateTilemap, "Regenerate tilemap");

            _regenerateUnitPositions |= _regenerateTilemap;
            EditorGUI.BeginDisabledGroup(_regenerateTilemap);
            _regenerateUnitPositions = GUILayout.Toggle(_regenerateUnitPositions, "Regenerate unit positions");
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Restart"))
            {
                var parameters = _ecsWorldScript.Parameters;

                if (_regenerateTilemap)
                {
                    parameters.Tilemap = GenerateTilemap();
                }

                if (_regenerateUnitPositions)
                {
                    HashSet<HexCoords> reserved = new();
                    ShuffleUnitPositions(parameters.Tilemap, parameters.AllyTeam, reserved);
                    ShuffleUnitPositions(parameters.Tilemap, parameters.EnemyTeam, reserved);
                }

                _ecsWorldScript?.InitSimulation(parameters);
            }
        }

        private static Tilemap GenerateTilemap()
        {
            return TilemapGenerator.Generate(8);
        }

        private static void ShuffleUnitPositions(Tilemap tilemap, UnitInstance[] units, HashSet<HexCoords> reserved)
        {
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

            for (int i = 0; i < units.Length; i++)
            {
                units[i].Position = GetRandomFreePosition();
            }
        }
    }
}
