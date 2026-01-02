using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.GameplayComponents.Movement.Test
{
    public class MovementTesterUI : MonoBehaviour
    {
        [SerializeField] private Text speedText;
        [SerializeField] private MovementTester movementTester;

        private IMovementTester MovementTester => movementTester;
        
        private void Update()
        {
            speedText.text = $"Speed: {MovementTester.AngularSpeed} \n" +
                             $"Direction: {MovementTester.Direction} \n" +
                             $"Slot: {MovementTester.CurrentSlot} \n" + 
                             $"Current State: {MovementTester.CurrentState}";
        }
    }
}