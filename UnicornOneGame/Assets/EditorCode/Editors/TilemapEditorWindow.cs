using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnicornOneEditorEditors
{
    internal class TilemapEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Tilemap Editor")]
        private static void OpenWindow()
        {
            EditorWindow window = GetWindow<TilemapEditorWindow>();
            window.titleContent = new GUIContent("Tilemap Editor");
        }

        private const float MinScale = 10.0f;
        private const float MaxScale = 120.0f;

        private Vector2 _offset = Vector2.zero;

        private float _scale = 100.0f;
        private float Scale
        {
            get => _scale;
            set 
            {
                _scale = Mathf.Clamp(value, MinScale, MaxScale);
            }
        }

        private Vector2 HalfScreenSize => position.size / 2;

        private void OnGUI()
        {
            DrawCenterCross();

            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawCenterCross()
        {
            Handles.color = Color.red;
            Handles.DrawLine(WorldToScreen(new Vector3(-1.0f, 0.0f)), WorldToScreen(new Vector3(1.0f, 0.0f)));
            Handles.DrawLine(WorldToScreen(new Vector3(0.0f, -1.0f)), WorldToScreen(new Vector3(0.0f, 1.0f)));
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        _offset += e.delta / Scale;
                    }
                    break;

                case EventType.ScrollWheel:
                    Vector2 mouseOldWorldPosition = ScreenToWorld(e.mousePosition);
                    Scale -= e.delta.y;
                    Vector2 mouseNewWorldPosition = ScreenToWorld(e.mousePosition);
                    _offset +=  mouseNewWorldPosition - mouseOldWorldPosition;
                    break;
            }

            GUI.changed = true;
        }

        private Vector3 WorldToScreen(Vector2 vector, float depth = 0.0f)
        {
            Vector2 screenVector = (vector + _offset) * Scale + HalfScreenSize;
            return new Vector3(screenVector.x, screenVector.y, depth);
        }

        private Vector2 ScreenToWorld(Vector2 vector)
        {
            return ((vector - HalfScreenSize) / Scale) - _offset;
        }
    }
}
