using _Main.Scripts.MainMenu.MVC;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Menu
{
    public class MainMenuUISounds : SoundBehaviour<MainMenuUiView>
    {
        [SerializeField] private SoundClassSo cancelButton;
        [SerializeField] private SoundClassSo confirmButton;
        [SerializeField] private SoundClassSo backButton;
        
        private void Start()
        {
            ComponentToSound.OnConfirmButtonClicked += () =>
            {
                PlaySound(confirmButton);
            };
            ComponentToSound.OnCancelButtonClicked += () =>
            {
                PlaySound(cancelButton);
            };
            ComponentToSound.OnBackButtonClicked += () =>
            {
                PlaySound(backButton);
            };
        }
    }
}