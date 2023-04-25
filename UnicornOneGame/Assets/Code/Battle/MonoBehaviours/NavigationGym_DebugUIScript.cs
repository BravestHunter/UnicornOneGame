using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    public class NavigationGym_DebugUIScript : MonoBehaviour
    {
        [SerializeField] private NavigationGym_EcsWorldScript _ecsWorldScript;

        private int _navigationWindowId;
        private Rect _navigationWindowRect = new Rect(20, 120, 200, 60);

        private void Awake()
        {
            _navigationWindowId = GlobalDebug.NextDebugWindowId;
        }

        private void OnGUI()
        {
            _navigationWindowRect = GUILayout.Window(_navigationWindowId, _navigationWindowRect, DoNavigationGymWindow, "Navigation Gym Parameters");
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
