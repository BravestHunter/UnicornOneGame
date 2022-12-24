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
        private readonly Camera _camera;
        private readonly Vector3 _cameraOffset;

        public CameraService(Camera camera)
        {
            _camera = camera;
            _cameraOffset = _camera.transform.localPosition;
        }

        public void SetCameraPosition(Vector3 position)
        {
            _camera.transform.localPosition = position + _cameraOffset;
        }
    }
}
