using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.Battle.Models
{
    internal class Tile
    {
        public bool IsWalkable { get; set; }
        public bool IsReserved { get; set; }

        public bool IsAvailable => IsWalkable && !IsReserved;
    }
}
