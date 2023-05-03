using UnityEngine;

namespace UnicornOne.Battle.Ecs.Services
{
    internal interface ICameraService : IService
    {
        public Camera Camera { get; }
    }
}
