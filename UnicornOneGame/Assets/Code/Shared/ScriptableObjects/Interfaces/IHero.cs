using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;

namespace UnicornOne.ScriptableObjects.Interfaces
{
    public interface IHero
    {
        PrefabInfo PrefabInfo { get; }
        MoveInfo MoveInfo { get; }
        HealthInfo HealthInfo { get; }
        IEnumerable<IAbility> Abilities { get; }
    }
}
