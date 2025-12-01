using System;
using _Main.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Cosmetics.Components
{
    public class SkinButtonSelector : MonoBehaviour, IButtonSelector
    {
        [SerializeField] private Button defaultButton;
        [SerializeField] private Button pizzaButton;

        public event Action<int> OnSkinSelected;

        private void OnEnable()
        {
            var current = SkinManager.Instance.GetCurrentSkinType();

            if (current == 0)
            {
                defaultButton.interactable = false;
                pizzaButton.interactable = true;
            }
            else
            {
                defaultButton.interactable = true;
                pizzaButton.interactable = false;
            }

            defaultButton.onClick.AddListener(() =>
            {
                OnSkinSelected?.Invoke((int)SkinType.Default);
                defaultButton.interactable = false;
                pizzaButton.interactable = true;
            });
            
            pizzaButton.onClick.AddListener(() =>
            {
                OnSkinSelected?.Invoke((int)SkinType.Pizza);
                defaultButton.interactable = true;
                pizzaButton.interactable = false;
            });
        }

        private void OnDisable()
        {
            defaultButton.onClick.RemoveAllListeners();
            pizzaButton.onClick.RemoveAllListeners();
        }
    }
}