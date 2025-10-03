using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


namespace _Main.Scripts.DebugTools
{
    [RequireComponent(typeof(DebugUIAbility))]
    [RequireComponent(typeof(DebugUIMeteor))]
    [RequireComponent(typeof(DebugUIGameValues))]
    public class DebugUIView : MonoBehaviour
    {
        public DebugUIAbility Ability { get; private set; }
        public DebugUIMeteor Meteor { get; private set; }
        public DebugUIGameValues GameValues { get; private set; }

        public void Initialize()
        {
            Ability = GetComponent<DebugUIAbility>();
            Meteor = GetComponent<DebugUIMeteor>();
            GameValues = GetComponent<DebugUIGameValues>();
        }
    }
}