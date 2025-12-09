using System;
using UnityEditor;
using UnityEngine;

namespace _Main.Slicer
{
#if UNITY_EDITOR
    public class MeshTester : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject target;
        [SerializeField] private GameObject container;
        [SerializeField] private float offset;

        private void Awake()
        {
            Restart();
        }

        private GameObject[] Slice(GameObject targetMesh,Vector3 planePoint,Vector3 planeNormal)
        {
            var items = MeshSlicer.SplitMesh(targetMesh,planePoint, planeNormal, container.transform);

            if (items != null)
            {
                items[1].transform.position = Vector2.left * offset;
                items[0].transform.position = Vector2.right * offset;
            }
            else
            {
                Debug.LogWarning("No Mesh could not be sliced");
            }
            
            return items;
        }

        public void SliceInHalf()
        {
            Slice(target, Vector3.zero, Vector3.right);
        }

        public void SliceInQuarter()
        { 
            var items = MeshSlicer.SplitTwiceMesh(target, container.transform);
            
            for (int i = 0; i < items.Length; i++)
            {
                float xOffset = (i < 2) ? offset : -offset;
                float yOffset = (i % 2 == 0) ? -offset : offset;
                
                
                var x = (Vector2.right * xOffset).x;
                var y = (Vector2.down * yOffset).y;
                
                Debug.Log($"{x},{y}");
                
                items[i].transform.position = new Vector3(x, y, items[i].transform.position.z);
            }
        }
        

        public void Restart()
        {
            target.SetActive(true);

            for (int i = container.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(container.transform.GetChild(i).gameObject);
            }
            
        }
    }
#endif
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(MeshTester))]
    public class ComponentsNameChangeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Dibuja el inspector normal
            DrawDefaultInspector();

            // Agrega el botón
            MeshTester script = (MeshTester)target;
            if (GUILayout.Button("Slice In Half"))
            {
                script.SliceInHalf();
            }
            
            if (GUILayout.Button("Slice In Quarter"))
            {
                script.SliceInQuarter();
            }
            
            if (GUILayout.Button("Restart"))
            {
                script.Restart();
            }
        }
    }
    
#endif
}