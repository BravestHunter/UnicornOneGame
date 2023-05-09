using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnicornOne.Utils;
using UnicornOneEitor;
using UnityEditor;
using UnityEngine;

namespace UnicornOneEditor
{
    public class TilemapGeneratorWindow : EditorWindow
    {
        private const string TilemapFolderName = "Tilemaps";

        [MenuItem("Tools/Tilemap Generator")]
        public static void ShowMyEditor()
        {
            EditorWindow window = GetWindow<TilemapGeneratorWindow>();
            window.titleContent = new GUIContent("Tilemap Generator");
        }

        private string _name = "Tilemap";
        private int _radius = 8;
        private float _unwalkableDensity = 0.15f;

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            _name = GUILayout.TextField(_name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Radius");
            _radius = EditorGUILayout.IntSlider(_radius, 0, 10);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Unwalkable tiles density");
            _unwalkableDensity = EditorGUILayout.Slider(_unwalkableDensity, 0.0f, 1.0f);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Generate"))
            {
                var tilemap = TilemapGenerator.Generate(_radius, _unwalkableDensity);
                SaveTilemap(tilemap, _name);
            }
        }

        private void SaveTilemap(Tilemap tilemap, string name)
        {
            AssetDatabase.CreateAsset(tilemap, $"Assets/{TilemapFolderName}/{name}.asset");

            AssetDatabase.SaveAssets();
        }
    }
}
