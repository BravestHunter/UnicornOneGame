using UnityEditor;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class GameSpeedDebugUIScript : BaseDebugUIScript
    {
        private float GameSpeed
        {
            get => Time.timeScale;
            set
            {
                if (value == Time.timeScale || value < 0)
                {
                    return;
                }

                Time.timeScale = value;
            }
        }

        protected override Vector2Int Size => new Vector2Int(200, 80);

        protected override string Title => "Game speed";

        protected override void BuildWindowLayout()
        {
            EditorGUILayout.LabelField($"Current value: {GameSpeed.ToString("0.00")}");

            GameSpeed = GUILayout.HorizontalSlider(GameSpeed, 0.0f, 8.0f);

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Stop"))
                {
                    GameSpeed = 0.0f;
                }
                if (GUILayout.Button("x1"))
                {
                    GameSpeed = 1.0f;
                }
                if (GUILayout.Button("x2"))
                {
                    GameSpeed = 2.0f;
                }
                if (GUILayout.Button("x4"))
                {
                    GameSpeed = 4.0f;
                }
                if (GUILayout.Button("x8"))
                {
                    GameSpeed = 8.0f;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
