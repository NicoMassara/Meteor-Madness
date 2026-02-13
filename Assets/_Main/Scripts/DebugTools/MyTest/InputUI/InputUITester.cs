using _Main.Scripts.EventBus;
using UnityEditor;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.Debug.MyTest.InputUI
{
    public class InputUITester : MonoBehaviour
    {
        public void EnableUI()
        {
            InputsEventCaller.SetEnable(true);
            InputsEventCaller.SetUIEnable(true);
        }

        public void DisableUI()
        {
            InputsEventCaller.SetEnable(false);
            InputsEventCaller.SetUIEnable(false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InputUITester))]
    internal class InputUITesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            InputUITester script = (InputUITester)target;
            if (GUILayout.Button("Enable UI")) script.EnableUI();
            if (GUILayout.Button("Disable UI")) script.DisableUI();

        }
    }
#endif
}