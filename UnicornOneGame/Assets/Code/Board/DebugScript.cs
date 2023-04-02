using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace UnicornOne.Board
{
    public class DebugScript : MonoBehaviour
    {
        [SerializeField] private LogicScript _logicScript;

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        void OnGUI()
        {
            float aspect = (float)Screen.height / Screen.width;

            GUIStyle style = new GUIStyle();
            style.fontSize = (int)(24.0f * aspect);
            GUI.skin.button.fontSize = (int)(20.0f * aspect);

            Vector2 offset = new Vector2(30.0f * aspect, 50.0f * aspect);
            Vector2 size = new Vector2(300.0f * aspect, 300.0f * aspect);

            GUILayout.BeginArea(new Rect(offset, size));
            GUILayout.BeginVertical();

            if (GUILayout.Button("Regenerate", GUILayout.Height(50 * aspect)))
            {
                _logicScript.RegenerateTilePath();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
