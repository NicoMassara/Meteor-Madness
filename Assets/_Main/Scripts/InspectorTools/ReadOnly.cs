using UnityEngine;
using UnityEditor;

namespace _Main.Scripts.InspectorTools
{
    public class ReadOnlyAttribute : PropertyAttribute { }
}

#if UNITY_EDITOR

namespace _Main.Scripts.InspectorTools
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnly : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // disable editing
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
#endif
