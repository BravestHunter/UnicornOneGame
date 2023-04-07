using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnicornOne.Board.MonoBehaviours;
using UnityEngine;

namespace UnicornOne.Board.MonoBehaviours
{
    public class LogicScript : MonoBehaviour
    {
        [SerializeField] private TilemapScript _tilemapScript;
        [SerializeField] private TilepathGeneratorParameters _tilepathGeneratorParameters;

        [SerializeField] private GameObject _playerChipPrefab;

        [SerializeField] private float _movementDuration = 0.3f;
        [SerializeField] private AnimationCurve _movementAnimationCurve;
        [SerializeField] private float _rotationDuration = 0.1f;

        private TilePath _tilePath;
        private int _playerTileIndex = 0;
        private GameObject _playerGameObject = null;

        public bool IsMoving { get; private set; } = false;

        void Start()
        {
            _playerGameObject = Instantiate(_playerChipPrefab);

            RegenerateTilePath();
        }

        public void RegenerateTilePath()
        {
            _tilePath = TilePathGenerator.Generate(_tilepathGeneratorParameters);

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

                float expiredSeconds = 0.0f;
                float progress = 0.0f;
                Vector3 startPosition = _playerGameObject.transform.position;
                Vector3 targetPosition =
                    _tilePath.Tiles[_playerTileIndex].Position.ToWorldCoords(_tilemapScript.HexOuterRadius, _tilemapScript.HexInnerRadius);
                Vector3 diff = targetPosition - startPosition;
                
                while (progress < 1.0f)
                {
                    expiredSeconds += Time.deltaTime;
                    progress = Math.Min(expiredSeconds / _movementDuration, 1.0f);

                    _playerGameObject.transform.position =
                        startPosition + diff * _movementAnimationCurve.Evaluate(progress);

                    yield return null;
                }

                if (_playerTileIndex < _tilePath.Tiles.Length - 1)
                {
                    expiredSeconds = 0.0f;
                    progress = 0.0f;
                    Quaternion startRotation = _playerGameObject.transform.rotation;
                    Vector3 startDirection = _playerGameObject.transform.forward;
                    Vector3 lookAtPosition =
                        _tilePath.Tiles[_playerTileIndex + 1].Position.ToWorldCoords(_tilemapScript.HexOuterRadius, _tilemapScript.HexInnerRadius);
                    Vector3 targetDirection = (lookAtPosition - _playerGameObject.transform.position).normalized;
                    float angle = Vector3.SignedAngle(startDirection, targetDirection, Vector3.up);

                    while (progress < 1.0f)
                    {
                        expiredSeconds += Time.deltaTime;
                        progress = Math.Min(expiredSeconds / _rotationDuration, 1.0f);

                        _playerGameObject.transform.rotation = startRotation;
                        _playerGameObject.transform.Rotate(Vector3.up, angle * progress);

                        yield return null;
                    }
                }

                yield return null;
            }

            _tilePath.Tiles[_playerTileIndex].Tile.Script.Activate();

            IsMoving = false;
        }
    }
}
