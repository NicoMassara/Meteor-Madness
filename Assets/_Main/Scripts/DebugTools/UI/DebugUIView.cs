using UnityEngine;

namespace _Main.Scripts.DebugTools
{
    [RequireComponent(typeof(DebugUIAbility))]
    [RequireComponent(typeof(DebugUIMeteor))]
    [RequireComponent(typeof(DebugUIGameValues))]
    [RequireComponent(typeof(DebugUICamera))]
    [RequireComponent(typeof(DebugUITimeScale))]
    public class DebugUIView : MonoBehaviour
    {
        public DebugUIAbility Ability { get; private set; }
        public DebugUIMeteor Meteor { get; private set; }
        public DebugUIGameValues GameValues { get; private set; }
        public DebugUICamera Camera { get; private set; }
        public DebugUITimeScale TimeScale { get; private set; }

        public void Initialize()
        {
            Ability = GetComponent<DebugUIAbility>();
            Meteor = GetComponent<DebugUIMeteor>();
            GameValues = GetComponent<DebugUIGameValues>();
            Camera = GetComponent<DebugUICamera>();
            TimeScale = GetComponent<DebugUITimeScale>();
        }
    }
}