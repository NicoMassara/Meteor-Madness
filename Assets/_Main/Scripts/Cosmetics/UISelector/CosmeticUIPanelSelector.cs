using System;
using _Main.Scripts.Cosmetics.Components;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.Cosmetics
{
    public class CosmeticUIPanelSelector : UiComponentsSelector<CosmeticUIComponents> { }

    [Serializable]
    public class CosmeticUIComponents : UiComponentsData
    {
        [Space]
        [Header("Buttons")]
        [SerializeField] private Button backButton;
        [Space]
        [Header("Select Buttons")]
        public Transform ButtonsContainer;
        public SkinSelectButton SkinSelectButton;
        [Space]
        [Header("Scroll")]
        [SerializeField] private ButtonsScrollRect scrollRect;
        [Space]
        [Header("Description")] 
        [SerializeField] private GameObject descriptionPanel;
        [SerializeField] private TMP_Text descriptionText;
        [Space]
        [Header("Locked")] 
        [SerializeField] private GameObject lockedPanel;
        [SerializeField] private TMP_Text requirementText;
        [SerializeField] private TMP_Text lockedText;
        [SerializeField] private Button unlockButton;
        [Header("Coins")]
        [SerializeField] private TMP_Text coinsText;
        [Space]
        [Header("First Open")]
        [SerializeField] private GameObject firstOpenPanel;
        [SerializeField] private Button closeFirstOpenButton;
        
        // === Buttons === //
        
        #region Buttons
        public void AddListenerToBackButton(UnityAction onClick) 
            => AddListenerToButton(backButton,onClick);
        public void RemoveListenerBackButton(UnityAction onClick) 
            => RemoveListenerFromButton(backButton,onClick);
        #endregion
        
        // === Scroll Rect === //
        
        #region Scroll Rect
        public void AddScrollListener(Action<int> onScroll) 
            => scrollRect.OnScrolled += onScroll;

        public void RemoveScrollListener(Action<int> onScroll) 
            => scrollRect.OnScrolled -= onScroll;

        #endregion

        // === Description === //
        
        #region Description
        
        public void SetActiveDescriptionPanel(bool active)
            => SetActiveObject(descriptionPanel,active);
        public void SetDescriptionText(string textCode) 
            => SetText(descriptionText, GetLocalizedString(textCode));
        #endregion

        // === Locked === //
        
        #region Locked
        public void SetActiveLockedPanel(bool active) 
            => SetActiveObject(lockedPanel,active);
        public void SetRequirementText(string requirementTextCode,int coinsAmount, string coinsTextCode)
        {
            SetText(requirementText,
                $"{GetLocalizedString(requirementTextCode)}" + " " + 
                $"{coinsAmount}" + " " +
                $"{GetLocalizedString(coinsTextCode)}");
        }

        public void ClearRequirementText() => ClearText(requirementText);
        public void SetLockedText(string textCode) 
            => SetText(lockedText, GetLocalizedString(textCode));
        public void SetLockedTextGreen() => SetTextColor(lockedText, new Color(0,0.75f,0,1));
        public void SetLockedTextRed() => SetTextColor(lockedText, new Color(0.75f,0,0,1));

        public void AddListenerToUnlockButton(UnityAction onClick) 
            => AddListenerToButton(unlockButton,onClick);
        public void RemoveListenerToUnlockButton(UnityAction onClick) 
            => RemoveListenerFromButton(unlockButton,onClick);

        public void SetUnlockButtonInteractable(bool interactable) 
            => SetButtonInteractable(unlockButton, interactable);
        
        public void DisableUnlockButton() 
            => SetActiveObject(unlockButton.gameObject, false);
        public void EnableUnlockButton()
            => SetActiveObject(unlockButton.gameObject, true);
        #endregion

        // === Coins === //

        #region Coins
        
        public void SetCoinsText(string textCode, uint coinsAmount)
        {
            SetText(coinsText, $"{GetLocalizedString(textCode)}:{coinsAmount:D6}");
        }
        
        #endregion
        
        // === First Open === //

        #region First Open

        public void SetActiveFirstOpenPanel(bool active) 
            => SetActiveObject(firstOpenPanel,active);
        
        public void AddListenerToCloseFirstOpenButton(UnityAction onClick) 
            => AddListenerToButton(closeFirstOpenButton,onClick);
        public void RemoveListenerToCloseFirstOpenButton(UnityAction onClick) 
            => RemoveListenerFromButton(closeFirstOpenButton,onClick);
        
        public void SetInteractiveCloseFirstOpenButton(bool active)
            => SetButtonInteractable(closeFirstOpenButton,active);

        #endregion
    }
}
