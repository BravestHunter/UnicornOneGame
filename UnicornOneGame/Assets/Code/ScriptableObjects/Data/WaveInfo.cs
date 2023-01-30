using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.MonoBehaviours;

namespace UnicornOne.ScriptableObjects.Data
{
    [Serializable]
    public struct WaveInfo
    {
        public Enemy Enemy;
        public int Count;
    }
}
