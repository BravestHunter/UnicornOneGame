using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class SimulationRestartDebugUIScript : BaseDebugUIScript
    {
        private bool _regenerateTilemap = false;
        private bool _regenerateUnitPositions = false;

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
                // Restart
            }
        }
    }
}
