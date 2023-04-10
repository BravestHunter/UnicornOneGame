using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Services
{
    internal class CameraService : ICameraService
    {
        public Camera Camera { get; private set; }

        public CameraService(Camera camera)
        {
            Camera = camera;
        }

    }
}
