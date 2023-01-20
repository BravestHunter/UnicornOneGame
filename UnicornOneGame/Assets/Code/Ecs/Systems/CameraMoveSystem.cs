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
    internal class CameraMoveSystem : IEcsRunSystem
    {
        private const float SqrRatationDistance = 400.0f;

        private readonly EcsCustomInject<CameraService> _cameraService;

        private EcsFilter _heroFilter;
        private EcsFilter _enemyFilter;

        private Vector3 _desiredCameraPlaneDirection;

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

            if (heroPositions.Count > 0)
            {
                var bounds = GetBounds(heroPositions);
                _cameraService.Value.MoveToFitBounds(bounds);
            }

            Vector3 heroAvarage = heroPositions.Aggregate(Vector3.zero, (sum, v) => sum + v) / heroPositions.Count;

            if (heroPositions.Count > 0 && enemyPositions.Count > 0)
            {
                Vector3 enemyAvarage = enemyPositions.Aggregate(Vector3.zero, (sum, v) => sum + v) / enemyPositions.Count;

                if ((heroAvarage - enemyAvarage).sqrMagnitude < SqrRatationDistance)
                {
                    Vector3 desiredPlaneDirection = enemyAvarage - heroAvarage;
                    desiredPlaneDirection.y = 0;
                    desiredPlaneDirection.Normalize();

                    _desiredCameraPlaneDirection = desiredPlaneDirection;
                }
            }

            Vector3 cameraPlaneDirection = _cameraService.Value.Camera.transform.forward;
            cameraPlaneDirection.y = 0;
            cameraPlaneDirection.Normalize();
            float signedAngle = Vector3.SignedAngle(cameraPlaneDirection, _desiredCameraPlaneDirection, Vector3.up);
            // TODO: move to fixed update
            if (MathF.Abs(signedAngle) > 0.5f)
            {
                float angleToRotate = MathF.Min(MathF.Abs(signedAngle), 0.5f) * MathF.Sign(signedAngle);

                _cameraService.Value.Camera.transform.RotateAround(heroAvarage, Vector3.up, angleToRotate);
            }
        }

        private Bounds GetBounds(IEnumerable<Vector3> positions)
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
