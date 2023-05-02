using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class NavigationGymDebugUIScript : BaseDebugUIScript
    {
        [SerializeField] private NavigationGym_EcsWorldScript _ecsWorldScript;

        protected override Vector2Int Size => new Vector2Int(200, 60);

        protected override string Title => "Navigation Gym Parameters";

        protected override void BuildWindowLayout()
        {
            EditorGUILayout.LabelField($"Rivals count: {_ecsWorldScript.RivalCount}");

            _ecsWorldScript.RivalCount = Mathf.RoundToInt(
                GUILayout.HorizontalSlider(
                    _ecsWorldScript.RivalCount,
                    NavigationGym_EcsWorldScript.MinRivalCount,
                    NavigationGym_EcsWorldScript.MaxRivalCount
                )
            );
        }
    }
}
