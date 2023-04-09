using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace UnicornOne.Board.MonoBehaviours
{
    public class DebugScript : MonoBehaviour
    {
        [SerializeField] private LogicScript _logicScript;
        [SerializeField] private TilemapScript _tilemapScript;

        [SerializeField] private Vector2 _fillTilesHeightRange;

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
                if (!_logicScript.IsMoving)
                    _logicScript.RegenerateTilePath();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Shuffle", GUILayout.Height(50 * aspect)))
            {
                _tilemapScript.Shuffle(_fillTilesHeightRange);
            }
            if (GUILayout.Button("Noise", GUILayout.Height(50 * aspect)))
            {
                _tilemapScript.ShuffleNoise(_fillTilesHeightRange);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Backward", GUILayout.Height(50 * aspect)))
            {
                if (!_logicScript.IsMoving)
                    _logicScript.MovePlayer(-1);
            }
            if (GUILayout.Button("Forward", GUILayout.Height(50 * aspect)))
            {
                if (!_logicScript.IsMoving)
                    _logicScript.MovePlayer(1);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
