using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.Board
{
    public abstract class BaseTileScript : ScriptableObject
    {
        public abstract void Activate();
    }
}
