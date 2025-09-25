using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class MeshSortingLayerSetter : MonoBehaviour
    {
        [SerializeField] private Renderer _modelMeshRenderer;
        [SerializeField] private string _sortingLayer = "Default";
        [SerializeField] private int _sortingOrder = 0;
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
            if(_modelMeshRenderer == null) return;
            
            _modelMeshRenderer.sortingLayerName = _sortingLayer;
            _modelMeshRenderer.sortingOrder = _sortingOrder;
        }

        public void SetLayerData(Renderer meshRenderer, string sortingLayer, int sortingOrder )
        {
            _modelMeshRenderer = meshRenderer;
            _sortingLayer = sortingLayer;
            _sortingOrder = sortingOrder;
            
            ApplySortingLayer();
        }
    }
}