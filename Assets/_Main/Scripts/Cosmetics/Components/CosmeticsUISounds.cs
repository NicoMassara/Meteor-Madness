using _Main.Scripts.Cosmetics.MVC;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.Components
{
    public class CosmeticsUISounds : SoundBehaviour<CosmeticUIView>
    {
        private void Start()
        {
            GetComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}