using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Board
{
    public class LogicScript : MonoBehaviour
    {
        [SerializeField] private TilemapScript _tilemapScript;

        [SerializeField] private TilePath _tilePath;

        [SerializeField] private bool _randomGeneration = false;
        [SerializeField] private int _generatedPathLength = 10;
        [SerializeField] private Tile _startTile;
        [SerializeField] private Tile _finishTile;
        [SerializeField] private Tile _roadTile;

        void Start()
        {
            if (_randomGeneration || _tilePath == null)
            {
                _tilePath = TilePathGenerator.Generate(_startTile, _finishTile, _roadTile, _generatedPathLength);
            }
            _tilemapScript.Setup(_tilePath);
        }

        public void RegenerateTilePath()
        {
            _tilePath = TilePathGenerator.Generate(_startTile, _finishTile, _roadTile, _generatedPathLength);
            _tilemapScript.Setup(_tilePath);
        }
    }
}
