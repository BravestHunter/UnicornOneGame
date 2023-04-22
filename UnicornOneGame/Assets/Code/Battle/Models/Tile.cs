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

        private bool _isReserved = false;
        public bool IsReserved
        {
            get
            {
                return _isReserved;
            }
            set
            {
                if (_isReserved == value)
                {
                    return;
                }

                _isReserved = value;
                ReservationChanged?.Invoke(_isReserved);
            }
        }

        public bool IsAvailable => IsWalkable && !IsReserved;

        public event Action<bool> ReservationChanged;

        public Tile(bool isWalkable)
        {
            IsWalkable = isWalkable;
        }
    }
}
