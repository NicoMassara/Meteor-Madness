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

        private void Update()
        {
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
                
                }
            }
        }
    }
}