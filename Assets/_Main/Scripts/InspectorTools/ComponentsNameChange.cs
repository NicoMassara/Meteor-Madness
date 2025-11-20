using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.InspectorTools
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class ComponentsNameChange : MonoBehaviour
    {
        [Header("Replace")]
        [SerializeField] private string textToRemove = "";
        [SerializeField] private string textToAdd = "";

        [Header("Add Prefix")]
        [SerializeField] private string prefixToAdd = "";

        [Header("Object")]
        [SerializeField] private GameObject componentParent;
        [SerializeField] private bool renameParent;

        [Header("Prefix Count")]
        [Range(1, 10)]
        [SerializeField] private int prefixRemoveCount = 2;

        #region Inspector Actions

        public void AddPrefix()
        {
            if (componentParent == null || string.IsNullOrEmpty(prefixToAdd)) return;

            int count = 0;

            if (renameParent)
                AddText(componentParent.transform);

            AddPrefixRecursive(componentParent.transform, ref count);
            Debug.Log($"Se agregó prefijo a {count} objetos.");
        }

        public void RemovePrefix()
        {
            if (componentParent == null) return;

            int count = 0;

            if (renameParent)
                TrimPrefix(componentParent.transform);

            RemovePrefixRecursive(componentParent.transform, ref count);
            Debug.Log($"Se removió prefijo de {count} objetos.");
        }

        public void ReplaceText()
        {
            if (componentParent == null || 
                (string.IsNullOrEmpty(textToRemove) && string.IsNullOrEmpty(textToAdd))) return;

            int count = 0;

            if (renameParent)
                Replace(componentParent.transform);

            ReplaceRecursive(componentParent.transform, ref count);
            Debug.Log($"Se reemplazó texto en {count} objetos.");
        }

        #endregion

        #region Recursive

        private void AddPrefixRecursive(Transform parent, ref int count)
        {
            foreach (Transform child in parent)
            {
                AddText(child);
                count++;
                AddPrefixRecursive(child, ref count);
            }
        }

        private void RemovePrefixRecursive(Transform parent, ref int count)
        {
            foreach (Transform child in parent)
            {
                TrimPrefix(child);
                count++;
                RemovePrefixRecursive(child, ref count);
            }
        }

        private void ReplaceRecursive(Transform parent, ref int count)
        {
            foreach (Transform child in parent)
            {
                Replace(child);
                count++;
                ReplaceRecursive(child, ref count);
            } 
        }
    #endregion

    #region Base Actions

    private void Replace(Transform item)
    {
        if (!string.IsNullOrEmpty(textToRemove))
            item.name = item.name.Replace(textToRemove, textToAdd);
    }

    private void AddText(Transform item)
    {
        item.name = prefixToAdd + item.name;
    }

    private void TrimPrefix(Transform item)
    {
        if (item.name.Length > prefixRemoveCount)
            item.name = item.name.Substring(prefixRemoveCount);
        else
            item.name = "";
    }

    #endregion
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
                script.AddPrefix();
            }
            
            if (GUILayout.Button("Remove Prefix"))
            {
                // Llama al método normalmente
                script.RemovePrefix();
            }
            
            if (GUILayout.Button("Replace Text"))
            {
                // Llama al método normalmente
                script.ReplaceText();
            }
        }
    }
#endif
}