using UnityEditor.Compilation;

namespace EditorUtilities
{
    using UnityEditor;
    using UnityEngine;

    public class ForceRecompile
    {
        [MenuItem("Tools/Force Recompilation %e")]
        public static void Recompile()
        {
            AssetDatabase.Refresh();
            Debug.Log("Script recompilation has been forced.");
        }
        
        [MenuItem("Tools/Force Complete Recompilation %t")]
        public static void RecompileAll()
        {
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();
            Debug.Log("Full script recompilation has been requested.");
        }
    }
}