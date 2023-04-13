using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.MonoBehaviours
{
    public class SpawnPointScript : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 4.0f);
        }
    }
}
