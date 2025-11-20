using System;
using _Main.Scripts.Sounds;

namespace _Main.Scripts.Gameplay.GameMode.Pause
{
    public class PausePanelSounds : SoundBehaviour<PauseUiPanel>
    {
        private void Start()
        {
            GetComponentToSound.OnResumeButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            GetComponentToSound.OnOptionsButtonPressed += () =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            GetComponentToSound.OnMainMenuButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}