using System;
using _Main.Scripts.ShieldRotation.Contracts;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Mediator.Test
{
    public class DummyTarget : MonoBehaviour, ITargetable
    {
        [SerializeField] private bool canBeTargeted;
        public Vector2 Position => transform.position;
        public bool CanBeTargeted => canBeTargeted;
        
        public event Action<ITargetable> OnTargetDeath;
        
        public void DisableTargetable()
        {
            canBeTargeted = false;
        }

        public void EnableTargetable()
        {
            canBeTargeted = true;
        }

        public void TriggerDeath()
        {
            OnTargetDeath?.Invoke(this);
            canBeTargeted = false;
        }

        public void SetTargetable(bool isTargetable) => canBeTargeted = isTargetable;
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(DummyTarget))]
    public class DummyTargetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            DummyTarget script = (DummyTarget)target;
            if (script.CanBeTargeted)
            {
                if (GUILayout.Button("Disable Targeted")) script.SetTargetable(false);
            }
            else
            {
                if (GUILayout.Button("Enable Targeted")) script.SetTargetable(true);
            }
            
            if (GUILayout.Button("Trigger Death")) script.TriggerDeath();

        }
    }

#endif
}