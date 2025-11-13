using _Main.Scripts.MainMenu.MVC;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Menu
{
    public class MainMenuUISounds : SoundBehaviour<MainMenuUiView>
    {
        
        private void Start()
        {
            GetComponentToSound.OnConfirmButtonClicked += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            GetComponentToSound.OnCancelButtonClicked += () =>
            {
                PlayUISound(UISoundType.Default);
            };
            GetComponentToSound.OnBackButtonClicked += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}