using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Cosmetics.Components
{
    public class SkinSelectButton : MonoBehaviour, ISkinButton
    {
        [SerializeField] private TMP_Text skinNameText;
        [SerializeField] private Button selectButton;
        
        private SkinType _skinType;
        
        public SkinType SkinType => _skinType;
        public event Action<SkinType> OnSelect;

        private void OnEnable()
        {
            selectButton?.onClick.AddListener(() =>
            {
                OnSelect?.Invoke(_skinType);
            });
        }

        private void OnDisable()
        {
            selectButton.onClick.RemoveAllListeners();
        }


        public SkinSelectButton SetData(string skinName, SkinType skinType)
        {
            skinNameText.text = skinName;
            _skinType = skinType;
            
            return this;
        }

        public void SetInteractable(bool isInteractable) => selectButton.interactable = isInteractable;
    }
}   