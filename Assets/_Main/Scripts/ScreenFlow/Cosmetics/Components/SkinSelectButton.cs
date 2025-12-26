using System;
using MeteorMadness.GlobalValues;
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
        
        private void TriggerOnSelect()
        {
            OnSelect?.Invoke(_skinType);
        }

        public SkinSelectButton SetData(string skinName, SkinType skinType)
        {
            skinNameText.text = skinName;
            _skinType = skinType;
            
            return this;
        }

        public void SetInteractable(bool isInteractable) => selectButton.interactable = isInteractable;

        public void RemoveListener() => selectButton?.onClick.RemoveListener(TriggerOnSelect);
        public void AddListener() => selectButton?.onClick.AddListener(TriggerOnSelect);
    }
}   