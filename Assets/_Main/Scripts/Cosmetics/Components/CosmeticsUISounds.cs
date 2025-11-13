using _Main.Scripts.Cosmetics.MVC;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.Components
{
    public class CosmeticsUISounds : SoundBehaviour<CosmeticUIView>
    {
        [SerializeField] private SoundClassSo buttonConfirm;
        [SerializeField] private SoundClassSo buttonBack;
        
        private void Start()
        {
            ComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlaySound(buttonBack);
            };
        }
    }
}