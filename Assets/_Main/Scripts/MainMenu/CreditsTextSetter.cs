using _Main.Scripts.Localization;
using UnityEngine;

namespace _Main.Scripts.Menu
{
    public class CreditsTextSetter : MonoBehaviour
    {
        [SerializeField] private string textKey;
        [SerializeField] private string linkedin;
        [SerializeField] private string itchIo;
        [SerializeField] private string github;
        [SerializeField] private MainMenuUiPanelSelector uiSelector;
        
        private void Start()
        {
            if(uiSelector.GetPanelData() == null) return;
            
            uiSelector.GetPanelData().CreditsText.text =
                $"{LocalizationManager.Instance.GetText(textKey)} <b>Nicolas Massara</b>\n" +
                $"<color=#0077B5><u>l{linkedin}</u></color>\n" +
                $"<color=#000000><u>{github}</u></color>\n" +
                $"<color=#FA5C5C><u>{itchIo}</u></color>";
        }
    }
}