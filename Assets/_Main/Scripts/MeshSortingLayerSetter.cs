using UnityEngine;

namespace _Main.Scripts
{
    public class MeshSortingLayerSetter : MonoBehaviour
    {
        [SerializeField] private MeshRenderer modelMeshRenderer;
        [SerializeField] private string sortingLayer;
        [Range(-10,10)]
        [SerializeField] private int sortingOrder;
        
        private void Awake()
        {
            modelMeshRenderer.sortingLayerName = sortingLayer;
            modelMeshRenderer.sortingOrder = sortingOrder;
        }
    }
}