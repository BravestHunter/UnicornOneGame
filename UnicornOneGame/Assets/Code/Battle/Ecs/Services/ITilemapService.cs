using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Battle.Models;
using UnicornOne.Core.Utils;
using UnityEngine;

namespace UnicornOne.Battle.Ecs.Services
{
    internal interface ITilemapService
    {
        public GameObject TilePrefab { get; }
        public Tilemap Tilemap { get; }
        public HexParams HexParams { get; }
    }
}
