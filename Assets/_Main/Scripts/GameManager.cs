using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public UnityAction<bool> OnPaused;
        
        public bool CanPlay { get; set; }
        private int _currentPoints;

        private void Awake()
        {
            Instance = this;
        }

        public void IncreasePoints(int multiplier = 1)
        {
            _currentPoints += 1 * multiplier;
        }

        public void ClearCurrentPoints()
        {
            _currentPoints = 0;
        }

        public int GetCurrentPoints()
        {
            return _currentPoints * GameValues.VisualMultiplier;
        }

        public void SetPaused(bool paused)
        {
            OnPaused?.Invoke(paused);
        }
    }
}