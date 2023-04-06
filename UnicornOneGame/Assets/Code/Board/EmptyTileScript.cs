using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnicornOne.Board.Assets.Code.Board
{
    [CreateAssetMenu(fileName = "EmptyTileScript", menuName = "Custom/Board/TileScripts/EmptyTileScript")]
    public class EmptyTileScript : BaseTileScript
    {
        public override void Activate()
        {
            Debug.Log("Empty tile!");
        }
    }
}
