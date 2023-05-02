using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Core.Utils;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.Battle.Models
{
    [Serializable]
    internal struct UnitInstance
    {
        public Unit Unit;
        public HexCoords Position;
    }
}
