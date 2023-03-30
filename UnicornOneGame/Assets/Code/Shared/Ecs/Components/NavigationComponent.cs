using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.Ecs.Components
{
    internal struct NavigationComponent
    {
        public Vector3 DestionationPosition;
        public float MovementSpeed;
    }
}
