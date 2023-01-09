using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;

namespace UnicornOne.ScriptableObjects.Interfaces
{
    internal interface IProjectile
    {
        PrefabInfo PrefabInfo { get; }
        MoveInfo MoveInfo { get; }
    }
}
