using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.GameplayComponents.MovementCorrection.Test
{
    internal class MovementCorrectionTesterUI : MonoBehaviour
    {
        [SerializeField] private Text debugText;
        [SerializeField] private Text speedText;
        [SerializeField] private MovementCorrectionTester view;

        private void Update()
        {
            debugText.text = $"Target Slot: {view.TargetSlot}\n" +
                             $"Is In Range: {view.IsInRange}\n" +
                             $"Is In Front: {view.IsInFrontOfTarget}\n" +
                             $"Distance: {view.Distance}\n" +
                             $"Direction: {view.DirectionToTarget}\n" +
                             $"Slot Distance: {view.SlotDistance}";

            var movement = view.Movement;
            
            speedText.text = $"Speed: {movement.AngularSpeed}\n" +
                             $"Direction: {movement.Direction}\n" +
                             $"Slot: {movement.CurrentSlot}\n" + 
                             $"Current State: {movement.CurrentState}\n" +
                             $"Has to Correct: {movement.HasToCorrect}";
        }
    }
}