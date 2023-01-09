using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace UnicornOne.Ecs.Services
{
    internal class LevelService
    {
        public ILevel Level;

        public LevelService(Level level)
        {
            Level = level;
        }
    }
}
