using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        public bool CanPlay { get; set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}