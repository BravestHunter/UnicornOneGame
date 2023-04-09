using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.ScriptableObjects.Data
{
    [Serializable]
    public struct CameraSettings
    {
        public float CameraDistanceScale;

        [Tooltip("Degrees per second")]
        public float RotationSpeed;

        [Tooltip("Distance between hero average and enemy average from which rotation starts")]
        public float RotationStartDistance;

        public Vector2 TargetPointOffset;
    }
}
