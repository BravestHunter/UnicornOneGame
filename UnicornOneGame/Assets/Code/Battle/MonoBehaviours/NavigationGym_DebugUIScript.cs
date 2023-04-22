using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    public class NavigationGym_DebugUIScript : MonoBehaviour
    {
        [SerializeField] private NavigationGym_EcsWorldScript _ecsWorldScript;

        private Rect _navigationGymWindowRect = new Rect(20, 150, 200, 80);

        private void OnGUI()
        {
            _navigationGymWindowRect = GUILayout.Window(1, _navigationGymWindowRect, DoNavigationGymWindow, "NavigationGym");
        }

        void DoNavigationGymWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 30));

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
