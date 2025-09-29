using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class MeshSortingLayerSetter : MonoBehaviour
    {
        [SerializeField] private Renderer modelMeshRenderer;
        [SerializeField] private string sortingLayer;
        [Range(-10,10)]
        [SerializeField] private int sortingOrder;

        private void OnValidate()
        {
            ApplySortingLayer();
        }

        private void Reset()
        {
            ApplySortingLayer();
        }


        private void ApplySortingLayer()
        {
            if(modelMeshRenderer == null) return;
            
            modelMeshRenderer.sortingLayerName = sortingLayer;
            modelMeshRenderer.sortingOrder = sortingOrder;
        }

        public void SetSortingLayer(Renderer model, string layer, int order)
        {
            modelMeshRenderer = model;
            sortingLayer = layer;
            sortingOrder = order;
            
            ApplySortingLayer();
        }
    }
}