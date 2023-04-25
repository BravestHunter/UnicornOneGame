using CodiceApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class DebugUIScript : MonoBehaviour
    {
        private int _gameSpeedWindowId;
        private Rect _gameSpeedWindowRect = new Rect(20, 20, 200, 80);

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

        private void Awake()
        {
            _gameSpeedWindowId = GlobalDebug.NextDebugWindowId;
        }

        private void OnGUI()
        {
            _gameSpeedWindowRect = GUILayout.Window(_gameSpeedWindowId, _gameSpeedWindowRect, DoGameSpeedWindow, "Game Speed");
        }

        void DoGameSpeedWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 30));

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
