using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace UnicornOne.Board
{
    public class LogicScript : MonoBehaviour
    {
        [SerializeField] private TilemapScript _tilemapScript;

        [SerializeField] private TilePath _tilePath;
        [SerializeField] private GameObject _playerChipPrefab;

        [SerializeField] private bool _randomGeneration = false;
        [SerializeField] private int _generatedPathLength = 10;
        [SerializeField] private Tile _startTile;
        [SerializeField] private Tile _finishTile;
        [SerializeField] private Tile _roadTile;

        private int _playerTileIndex = 0;
        private GameObject _playerGameObject = null;

        public bool IsMoving { get; private set; } = false;

        void Start()
        {
            if (_randomGeneration || _tilePath == null)
            {
                _tilePath = TilePathGenerator.Generate(_startTile, _finishTile, _roadTile, _generatedPathLength);
            }

            _playerGameObject = Instantiate(_playerChipPrefab);

            SetupBoard();
        }

        public void SetupBoard()
        {
            _playerTileIndex = 0;

            // Set chip to face next tile
            _playerGameObject.transform.position =
                _tilePath.Tiles[0].Position.ToWorldCoords(_tilemapScript.HexOuterRadius, _tilemapScript.HexInnerRadius);
            _playerGameObject.transform.LookAt(
                _tilePath.Tiles[1].Position.ToWorldCoords(_tilemapScript.HexOuterRadius, _tilemapScript.HexInnerRadius)
            );

            _tilemapScript.Setup(_tilePath);
        }

        public void RegenerateTilePath()
        {
            _tilePath = TilePathGenerator.Generate(_startTile, _finishTile, _roadTile, _generatedPathLength);

            SetupBoard();
        }

        public void MovePlayer(int tilesNumber)
        {
            int newTileIndex = _playerTileIndex + tilesNumber;
            if (newTileIndex < 0 || newTileIndex >= _tilePath.Tiles.Length)
            {
                return;
            }

            if (IsMoving)
            {
                return;
            }
            IsMoving = true;

            int increment = Math.Sign(tilesNumber);
            StartCoroutine(MovePlayerCoroutine(newTileIndex, increment));
        }

        private IEnumerator MovePlayerCoroutine(int newTileIndex, int increment)
        {
            while (_playerTileIndex != newTileIndex)
            {
                _playerTileIndex += increment;

                _playerGameObject.transform.position =
                    _tilePath.Tiles[_playerTileIndex].Position.ToWorldCoords(_tilemapScript.HexOuterRadius, _tilemapScript.HexInnerRadius);
                if (_playerTileIndex < _tilePath.Tiles.Length - 1)
                {
                    _playerGameObject.transform.LookAt(
                        _tilePath.Tiles[_playerTileIndex + 1].Position.ToWorldCoords(_tilemapScript.HexOuterRadius, _tilemapScript.HexInnerRadius)
                    );
                }

                yield return null;
            }

            IsMoving = false;
        }
    }
}
