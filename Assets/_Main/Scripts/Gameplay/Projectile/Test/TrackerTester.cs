using System;
using _Main.Scripts.Gameplay.Projectile.Components;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Meteor;
using MeteorMadness.GlobalValues.Utilities;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Test
{
    public class TrackerTester : MonoBehaviour
    {
        [Header("Spawn Data")]
        [SerializeField] private Transform centerOfGravity;
        [SerializeField] private float spawnRadius;
        [SerializeField] private bool doesDebug;
        [Header("Meteor Factory")]
        [SerializeField] private MeteorView meteorPrefab;
        [Header("Distance Tracker")]
        [SerializeField] private float centerOfGravityOffset;
        
        private ProjectileDistanceTracker _distanceTracker;
        private MeteorFactory _meteorFactory;
        private int _currentAngle = 0;

        private void Start()
        {
            _meteorFactory = new MeteorFactory(meteorPrefab, ()=> doesDebug);
            _distanceTracker = new ProjectileDistanceTracker(centerOfGravity, centerOfGravityOffset);
            
            _distanceTracker.OnTargetDistanceReached += DistanceTracker_OnTargetDistanceReachedHandler;
        }

        private void Update()
        {
            _distanceTracker.Execute();
        }

        private void DistanceTracker_OnTargetDistanceReachedHandler(bool isLast)
        {
            SpawnMeteor();
        }
        
        private Vector2 GetSpawnPosition(int selectedAngle)
        {
            var slotAmount = GameParameters.GameplayValues.AngleSlots;

            var angle = AngleCalculations.GetAngleFromSlot(selectedAngle, slotAmount);
            var position = AngleCalculations.GetPositionByAngle(angle, spawnRadius);

            return position;
        }

        public void SpawnMeteor()
        {
            const float ratio = 0.5f;
            var spawnPosition = GetSpawnPosition(_currentAngle);
            var direction = (Vector2)centerOfGravity.position - spawnPosition;
            var finalSpeed = 30;
            
            var projectile = _meteorFactory.SpawnMeteor(new ProjectileSpawnValues
            {
                Position = spawnPosition,
                Direction = direction,
                MovementSpeed = finalSpeed
            });
            
            if(projectile == null) return;
            
            //_distanceTracker.SetProjectile((IProjectile)projectile,ratio, false);

            _currentAngle++;
            _currentAngle = (int)Mathf.Repeat(_currentAngle, 32);
        }

        private void OnDrawGizmos()
        {
            if (centerOfGravity != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius);
                Gizmos.DrawWireSphere(centerOfGravity.position, centerOfGravityOffset);
                
                if (_distanceTracker != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(centerOfGravity.position, _distanceTracker.GetTargetRadius());
                }
            }
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(TrackerTester))]
    public class TrackerTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            TrackerTester script = (TrackerTester)target;
            if (GUILayout.Button("Spawn")) script.SpawnMeteor();
        } 
    }
#endif
}