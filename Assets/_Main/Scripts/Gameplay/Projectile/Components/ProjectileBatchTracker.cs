using System;
using System.Collections.Generic;
using MeteorMadness.GlobalValues.Tools;
using NUnit.Framework;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileBatchTracker
    {
        private const float MinDeflectRatio = 0.5f;
        private readonly Queue<BatchData> _batchQueue;
        private BatchData _currentBatch;

        public event Action OnBatchFinished;
        public event Action OnBatchDeflected;

        public ProjectileBatchTracker()
        {
            _batchQueue = new Queue<BatchData>();
        }

        public void RestartData()
        {
            _currentBatch =  new BatchData();
            _batchQueue.Clear();
        }

        public void CreateBatchData(int amount)
        {
            var newBatch = new BatchData
            {
                BathAmount = amount,
            };
            
            _batchQueue.Enqueue(newBatch);
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

            if (_currentBatch.ActiveAmount == 0)
            {
                if (_currentBatch.GetDeflectedRatio() >= MinDeflectRatio)
                {
                    OnBatchDeflected?.Invoke();
                }

                OnBatchFinished?.Invoke();
                PrepareNewBatch();
            }
        }
    }

    internal struct BatchData
    {
        public int BathAmount;
        public int ActiveAmount;
        public int DeflectedAmount;

        public float GetDeflectedRatio() => (float)DeflectedAmount / BathAmount;
    }
}