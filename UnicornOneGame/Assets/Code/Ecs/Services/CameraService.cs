using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class CameraService
    {
        private const float MinCameraDistance = 24.0f;

        private readonly Camera _camera;

        public Camera Camera => _camera;

        public CameraService(Camera camera)
        {
            _camera = camera;
        }

        public void MoveToFitBounds(Bounds bounds)
        {
            float objectSize = bounds.size.magnitude;
            //float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z); - alternative

            float cameraDistance = 1.2f; // Constant factor
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * _camera.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object

            //distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object (no need for now)

            distance = Math.Max(MinCameraDistance, distance); // Min distance

            _camera.transform.position = bounds.center - distance * _camera.transform.forward;
        }
    }
}
