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
        public bool IsWalkable { get; private set; }

        public bool IsReserved { get; set; } = false;

        public bool IsAvailable => IsWalkable && !IsReserved;

        public Tile(bool isWalkable)
        {
            IsWalkable = isWalkable;
        }
    }
}
