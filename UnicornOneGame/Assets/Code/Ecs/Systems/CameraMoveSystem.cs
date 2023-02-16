using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.Ecs.Components;
using UnicornOne.Ecs.Services;
using UnityEngine;

namespace UnicornOne.Ecs.Systems
{
    internal class CameraMoveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float AngleThreshold = 0.5f;

        private readonly EcsCustomInject<CameraService> _cameraService;
        private readonly EcsCustomInject<SettingsService> _settingsService;

        private float _sqrRatationDistance;

        private EcsFilter _heroFilter;
        private EcsFilter _enemyFilter;

        private Vector3 _desiredCameraPlaneDirection;

        public void Init(IEcsSystems systems)
        {
            _sqrRatationDistance = _settingsService.Value.Camera.RotationStartDistance * _settingsService.Value.Camera.RotationStartDistance;
        }

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            if (_heroFilter == null)
            {
                _heroFilter = world
                    .Filter<HeroFlag>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            if (_enemyFilter == null)
            {
                _enemyFilter = world
                    .Filter<EnemyFlag>()
                    .Inc<GameObjectUnityRefComponent>()
                    .End();
            }

            var gameObjectRefPool = world.GetPool<GameObjectUnityRefComponent>();

            List<Vector3> heroPositions = new List<Vector3>();
            foreach (var entity in _heroFilter)
            {
                var gameObjectRefComponent = gameObjectRefPool.Get(entity);
                heroPositions.Add(gameObjectRefComponent.GameObject.transform.position);
            }

            List<Vector3> enemyPositions = new List<Vector3>();
            foreach (var entity in _enemyFilter)
            {
                var gameObjectRefComponent = gameObjectRefPool.Get(entity);
                enemyPositions.Add(gameObjectRefComponent.GameObject.transform.position);
            }

            Vector2 planeOffset = _settingsService.Value.Camera.TargetPointOffset;
            Vector3 offset = new Vector3(planeOffset.x, 0.0f, planeOffset.y);

            if (heroPositions.Count > 0)
            {
                var bounds = GetBounds(heroPositions);
                _cameraService.Value.MoveToFitBounds(bounds, _settingsService.Value.Camera.CameraDistanceScale, offset);
            }

            Vector3 heroAvarage = heroPositions.Aggregate(Vector3.zero, (sum, v) => sum + v) / heroPositions.Count;

            if (heroPositions.Count > 0 && enemyPositions.Count > 0)
            {
                Vector3 enemyAvarage = enemyPositions.Aggregate(Vector3.zero, (sum, v) => sum + v) / enemyPositions.Count;

                if ((heroAvarage - enemyAvarage).sqrMagnitude < _sqrRatationDistance)
                {
                    Vector3 desiredPlaneDirection = enemyAvarage - heroAvarage;
                    desiredPlaneDirection.y = 0;
                    desiredPlaneDirection.Normalize();

                    _desiredCameraPlaneDirection = desiredPlaneDirection;
                }
            }

            RotateCamera(heroAvarage);
        }

        private void RotateCamera(Vector3 rotationPoint)
        {
            Vector3 cameraPlaneDirection = _cameraService.Value.Camera.transform.forward;
            cameraPlaneDirection.y = 0;
            cameraPlaneDirection.Normalize();
            float signedAngle = Vector3.SignedAngle(cameraPlaneDirection, _desiredCameraPlaneDirection, Vector3.up);

            float angleDelta = _settingsService.Value.Camera.RotationSpeed * Time.deltaTime;
            float angleToRotate = MathF.Min(angleDelta, MathF.Abs(signedAngle)) * MathF.Sign(signedAngle);

            if (MathF.Abs(signedAngle) > AngleThreshold)
            {
                _cameraService.Value.Camera.transform.RotateAround(rotationPoint, Vector3.up, angleToRotate);
            }
        }

        private static Bounds GetBounds(IEnumerable<Vector3> positions)
        {
            Bounds bounds = new Bounds();

            if (positions.Count() == 0)
            {
                return bounds;
            }

            bounds.SetMinMax(positions.First(), positions.First());

            foreach (Vector3 position in positions)
            {
                bounds.Encapsulate(position);
            }

            return bounds;
        }
    }
}
