using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class UIService
    {
        public GameObject Label3DPrefab { get; }

        public UIService(GameObject label3DPrefab)
        {
            Label3DPrefab = label3DPrefab;
        }
    }
}
