using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Ecs.Services
{
    internal class ProjectileService
    {
        private readonly Projectile _projectile;

        public GameObject Prefab => _projectile.Prefab;
        public int Damage => _projectile.Damage;
        public float MoveSpeed => _projectile.MoveSpeed;

        public ProjectileService(Projectile projectile)
        {
            _projectile = projectile;
        }
    }
}
