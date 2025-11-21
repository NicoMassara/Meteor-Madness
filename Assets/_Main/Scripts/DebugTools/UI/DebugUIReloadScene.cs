using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Main.Scripts.DebugTools
{
    public class DebugUIReloadScene : MonoBehaviour
    {
        [SerializeField] private Button reloadButton;

        public event Action OnReload;
        
        private void Awake()
        {
            reloadButton.onClick.AddListener(() =>
            {
                OnReload?.Invoke();
            });
        }
    }
}