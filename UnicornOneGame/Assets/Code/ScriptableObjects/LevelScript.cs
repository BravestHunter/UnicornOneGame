using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelScript", menuName = "Custom/LevelScript")]
    public class LevelScript : ScriptableObject, ILevelScript
    {
        public WaveInfo[] Waves;

        WaveInfo[] ILevelScript.Waves => Waves;
    }
}
