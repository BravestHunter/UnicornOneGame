using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Core.Utils
{
    public static class MeshGenerator
    {
        public static Mesh TileMesh(in HexParams hexParams, float height)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                int vertexIndex = vertices.Count;
                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
            }

            var hexCorners = GetHexCorners(hexParams);

            // Top
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(Vector3.zero, hexCorners[i], hexCorners[i + 1]);
            }

            // Side
            Vector3 verticalOffset = -Vector3.up * height;
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(hexCorners[i], hexCorners[i] + verticalOffset, hexCorners[i + 1] + verticalOffset);
                AddTriangle(hexCorners[i], hexCorners[i + 1] + verticalOffset, hexCorners[i + 1]);
            }

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();

            return mesh;
        }

        public static Mesh TileBorderMesh(in HexParams hexParams, float borderOffset)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                int vertexIndex = vertices.Count;
                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
            }

            var hexCorners = GetHexCorners(hexParams);

            // Top border
            for (int i = 0; i < 6; i++)
            {
                Vector3 closePoint1 = hexCorners[i] * borderOffset;
                Vector3 closePoint2 = hexCorners[i + 1] * borderOffset;

                AddTriangle(hexCorners[i], hexCorners[i + 1], closePoint1);
                AddTriangle(closePoint1, hexCorners[i + 1], closePoint2);
            }

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();

            return mesh;
        }

        private static Vector3[] GetHexCorners(in HexParams hexParams) => new Vector3[] {
            new Vector3(0f, 0f, hexParams.OuterRadius),
            new Vector3(hexParams.InnerRadius, 0f, 0.5f * hexParams.OuterRadius),
            new Vector3(hexParams.InnerRadius, 0f, -0.5f * hexParams.OuterRadius),
            new Vector3(0f, 0f, -hexParams.OuterRadius),
            new Vector3(-hexParams.InnerRadius, 0f, -0.5f * hexParams.OuterRadius),
            new Vector3(-hexParams.InnerRadius, 0f, 0.5f * hexParams.OuterRadius),
            new Vector3(0f, 0f, hexParams.OuterRadius) // duplicate first
        };
    }
}
