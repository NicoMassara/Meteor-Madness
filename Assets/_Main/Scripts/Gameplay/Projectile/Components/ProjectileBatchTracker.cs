using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileBatchTracker
    {
        private const float MinDeflectRatio = 0.5f;
        private readonly Queue<BatchTrackerData> _batchQueue;
        private BatchTrackerData _currentBatch;

        public event Action OnBatchFinished;
        public event Action OnBatchDeflected;

        public ProjectileBatchTracker()
        {
            _batchQueue = new Queue<BatchTrackerData>();
        }

        public void RestartData()
        {
            _currentBatch = new BatchTrackerData();
            _batchQueue.Clear();
        }

        public void CreateBatchData(int amount)
        {
            var newBatch = new BatchTrackerData
            {
                BathAmount = amount,
                ActiveAmount = amount,
            };
            
            _batchQueue.Enqueue(newBatch);

            if (_batchQueue.Count == 0)
            {
                PrepareNewBatch();
            }
        }

        private void PrepareNewBatch()
        {
            _currentBatch = _batchQueue.Dequeue();
        }

        public void CheckForDeflectedProjectile()
        {
            CheckForProjectile(true);
        }
        
        public void CheckForCollisionProjectile()
        {
            CheckForProjectile(false);
        }

        private void CheckForProjectile(bool isDeflected)
        {
            if (isDeflected)
            {
                _currentBatch.DeflectedAmount++;
            }

            _currentBatch.ActiveAmount--;

            if (_currentBatch.ActiveAmount <= 0)
            {
                if (_currentBatch.GetDeflectedRatio() >= MinDeflectRatio)
                {
                    OnBatchDeflected?.Invoke();
                }
                else
                {
                    //Debug.Log("Not enough Deflected");
                }

                OnBatchFinished?.Invoke();
                
                if (_batchQueue.Count > 0)
                {
                    PrepareNewBatch();
                }
            }
        }
    }

    internal struct BatchTrackerData
    {
        public int BathAmount;
        public int ActiveAmount;
        public int DeflectedAmount;

        public float GetDeflectedRatio() => (float)DeflectedAmount / BathAmount;
    }
}