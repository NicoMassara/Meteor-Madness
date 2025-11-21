using System;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Interfaces
{
    public interface IPausePanel
    {
        public GameObject Panel { get; }
        public void SetScoreText(int points);
        public event Action OnResumeButtonPressed;
        public event Action OnOptionsButtonPressed;
        public event Action OnMainMenuButtonPressed;
        
    }
}