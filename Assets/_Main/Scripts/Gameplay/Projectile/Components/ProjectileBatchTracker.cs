using System;
using System.Collections.Generic;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileBatchTracker
    {
        private const float MinDeflectRatio = 0.5f;
        private readonly Queue<BatchTrackerData> _batchQueue;
        private BatchTrackerData _currentBatch;
        private bool _hasActiveBatch;

        public event Action<BatchType> OnBatchFinished;
        public event Action OnBatchDeflected;

        public ProjectileBatchTracker()
        {
            _batchQueue = new Queue<BatchTrackerData>();
        }

        public void RestartData()
        {
            ClearData();
        }

        public void ClearData()
        {
            _hasActiveBatch = false;
            _currentBatch = new BatchTrackerData();
            _batchQueue.Clear();
        }

        public void CreateBatchData(int amount, BatchType batchType)
        {
            if (amount == 0)
            {
                Debug.LogWarning($"Batch Amount is Zero");
                return;
            }

            var newBatch = new BatchTrackerData
            {
                BathAmount = amount,
                ActiveAmount = amount,
                BatchType = batchType
            };

            if (_batchQueue.Count == 0 && _hasActiveBatch == false)
            {
                _currentBatch = newBatch;
                _hasActiveBatch = true;
            }
            else
            {
                _batchQueue.Enqueue(newBatch);
            }
        }

        private void PrepareNewBatch()
        {
            _currentBatch = _batchQueue.Dequeue();
            _hasActiveBatch = true;
        }

        public void NotifyProjectileDeflected()
        {
            CheckForProjectile(true);
        }
        
        public void NotifyProjectileDestroyed()
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
                
                OnBatchFinished?.Invoke(_currentBatch.BatchType);
                
                _hasActiveBatch = false;
                
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
        public BatchType BatchType;

        public float GetDeflectedRatio() => (float)DeflectedAmount / BathAmount;
    }
}