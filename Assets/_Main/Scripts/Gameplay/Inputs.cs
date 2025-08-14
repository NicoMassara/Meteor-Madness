using System;
using _Main.Scripts.Gameplay.Meteor;
using UnityEngine;
using _Main.Scripts.Gameplay.Shield;
using UnityEngine.Serialization;

namespace _Main.Scripts.Gameplay
{
    public class Inputs : MonoBehaviour
    {
        [SerializeField] private ShieldController shieldController;
        private bool _isPaused;

        private void Start()
        {
            GameManager.Instance.OnPaused += GM_OnPausedHandler;
        }

        private void Update()
        {
            if(_isPaused) return;
            
            if (GameManager.Instance.CanPlay == true)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    shieldController.Rotate(1);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    shieldController.Rotate(-1);
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    GameManager.Instance.SetPaused(true);
                }
            }
        }
        
        private void GM_OnPausedHandler(bool isPaused)
        {
            _isPaused = isPaused;
        }
    }
}