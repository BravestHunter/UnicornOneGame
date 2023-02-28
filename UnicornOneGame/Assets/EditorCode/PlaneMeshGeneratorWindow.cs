using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace UnicornOneEitor
{
    public class PlaneMeshGeneratorWindow : EditorWindow
    {
        private int _cellsNumber;
        private float _cellSize;

        [MenuItem("Window/PlaneMeshGenerator")]
        public static void ShowMyEditor()
        {
            EditorWindow window = GetWindow<PlaneMeshGeneratorWindow>();
            window.titleContent = new GUIContent("PlaneMeshGenerator");
        }

        void OnGUI()
        {
            _cellsNumber = EditorGUILayout.IntField("Cells Number", _cellsNumber);
            _cellSize = EditorGUILayout.FloatField("Cells size", _cellSize);

            if (GUILayout.Button("Generate"))
            {
                var mesh = GenerateMesh();
                SaveMesh(mesh, "NewPlaneMesh");
            }
        }

        private Mesh GenerateMesh()
        {
            Vector3[] vertices = new Vector3[(_cellsNumber + 1) * (_cellsNumber + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            Vector4[] tangents = new Vector4[vertices.Length];
            Vector4 tangent = new Vector4(1.0f, 0.0f, 0f, -1.0f);
            int index = 0;
            for (int i = 0; i <= _cellsNumber; i++)
            {
                for (int j = 0; j <= _cellsNumber; j++, index++)
                {
                    vertices[index] = new Vector3(i * _cellSize, 0, j * _cellSize);
                    uv[index] = new Vector2(i, j);
                    tangents[index] = tangent;
                }
            }

            int[] triangles = new int[_cellsNumber * _cellsNumber * 6];
            index = 0;
            for (int vi = 0, y = 0; y < _cellsNumber; y++, vi++)
            {
                for (int x = 0; x < _cellsNumber; x++, index += 6, vi++)
                {
                    triangles[index] = vi + _cellsNumber + 2;
                    triangles[index + 3] = triangles[index + 2] = vi + 1;
                    triangles[index + 4] = triangles[index + 1] = vi + _cellsNumber + 1;
                    triangles[index + 5] = vi;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.tangents = tangents;
            mesh.triangles = triangles;

            return mesh;
        }

        private void SaveMesh(Mesh mesh, string name)
        {
            AssetDatabase.CreateAsset(mesh, $"Assets/{name}.mesh");
            AssetDatabase.SaveAssets();
        }
    }
}
