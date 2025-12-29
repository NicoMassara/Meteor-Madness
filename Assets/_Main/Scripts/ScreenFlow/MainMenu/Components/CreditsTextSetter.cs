using MeteorMadness.Contracts.Events;
using MeteorMadness.Managers.Localization;
using MeteorMadness.ScreenFlow.Menu;
using UnityEngine;

namespace _Main.Scripts.Menu
{
    public class CreditsTextSetter : MonoBehaviour
    {
        [SerializeField] private string textKey;
        [SerializeField] private string linkedin;
        [SerializeField] private string itchIo;
        [SerializeField] private string github;
        [SerializeField] private MainMenuUiPanelComponents uiSelector;
        
        private void Awake()
        {
            LocalizationEvents.OnLanguageChanged += UpdateText;
        }
        
        private void UpdateText()
        {
            if(uiSelector.GetPanelData() == null) return;

            var localizatedText = LocalizationManager.Instance.GetText(textKey);
            
            uiSelector.GetPanelData().CreditsText.text =
                $"{localizatedText} <b>Nicolas Massara</b>\n" +
                $"<color=#0077B5><u>l{linkedin}</u></color>\n" +
                $"<color=#AAAFFA><u>{github}</u></color>\n" +
                $"<color=#FA5C5C><u>{itchIo}</u></color>";
        }
    }
}