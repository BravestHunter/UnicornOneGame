using UnicornOne.Battle.Models;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal abstract class BaseEcsWorldScript : MonoBehaviour
    {
        public abstract EcsWorldSimulationParameters Parameters { get; }
        public abstract void InitSimulation(EcsWorldSimulationParameters parameters);
    }
}
