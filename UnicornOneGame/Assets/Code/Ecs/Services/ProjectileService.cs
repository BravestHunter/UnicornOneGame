using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class ProjectileService
    {
        public IProjectile Projectile { get; }

        public ProjectileService(Projectile projectile)
        {
            Projectile = projectile;
        }
    }
}
