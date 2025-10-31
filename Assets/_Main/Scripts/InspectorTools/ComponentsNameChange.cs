using _Main.Scripts.MyTools;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.InspectorTools
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class ComponentsNameChange : MonoBehaviour
    {
        [Header("Remove Values")]
        [SerializeField] private int removeCount = 0;
        [Header("Add Values")]
        [SerializeField] private string prefixToAdd;
        [Header("Parent")]
        [SerializeField] private GameObject componentParent;
        
        public void AddStringToComponents()
        {
            if(componentParent == null ||
               string.IsNullOrEmpty(prefixToAdd)) return;
            
            int count = 0;
            RenameRecursive(componentParent.transform, ref count);
        }

        public void RemoveCharactersFromComponents()
        {
            if(removeCount == 0) return;
            
            int count = 0;
            TrimRecursive(componentParent.transform, ref count);
        }

        private void RenameRecursive(Transform parent, ref int count)
        {
            foreach (Transform child in parent)
            {
                child.name = prefixToAdd+child.name;
                count++;
                RenameRecursive(child, ref count);
            }
        }
        
        private void TrimRecursive(Transform parent, ref int count)
        {
            foreach (Transform child in parent)
            {
                string oldName = child.name;
                if (oldName.Length > removeCount)
                {
                    child.name = oldName.Substring(removeCount);
                }
                else
                {
                    child.name = "";
                }

                count++;
                TrimRecursive(child, ref count);
            }
        }
    }
#endif
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ComponentsNameChange))]
    public class ComponentsNameChangeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            ComponentsNameChange script = (ComponentsNameChange)target;
            if (GUILayout.Button("Add Prefix"))
            {
                // Llama al método normalmente
                script.AddStringToComponents();
            }
            
            if (GUILayout.Button("Remove Characters"))
            {
                // Llama al método normalmente
                script.RemoveCharactersFromComponents();
            }
        }
    }
#endif
}