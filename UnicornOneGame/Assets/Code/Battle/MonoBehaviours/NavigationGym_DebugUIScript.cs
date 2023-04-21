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

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Random Teleport"))
            {
                _ecsWorldScript.TeleportPlayerRandomly();
            }

            GUILayout.EndHorizontal();
        }
    }
}
