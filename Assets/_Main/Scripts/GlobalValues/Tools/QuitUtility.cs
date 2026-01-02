using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MeteorMadness.GlobalValues.Tools
{
    public static class QuitUtility
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Sale del build
#endif
        }
    }
}