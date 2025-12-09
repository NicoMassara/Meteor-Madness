using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class GameModeSounds : MusicBehavior<IGameModeSounds>
    {
        [Header("Music")]
        [SerializeField] private SoundClassSo gameMusicData;
        [Space]
        [Header("Countdown")]
        [SerializeField] private SoundClassSo countdownSound;
        [SerializeField] private SoundClassSo countdownFinish;
        
        private GeneratedId _gameMusicId;
        
        private void Start()
        {
            // Music
            
            ComponentToSound.OnInitialized += () =>
            {
                StopAllMusic();
            };  
            
            ComponentToSound.OnPlayMusic += () =>
            {
                // Adds a delay
                _gameMusicId = PlayMusic(gameMusicData, _gameMusicId);
            };
            
            ComponentToSound.OnStopMusic += () =>
            {
                StopAllMusic();
            };  
            
            // Sounds
            
            ComponentToSound.OnCountdownUpdated += (value) =>
            {
                if (value > 1)
                {
                    PlaySound(countdownSound);
                }
                else if(value >= 0)
                {
                    PlaySound(countdownFinish);
                }


            };
            
            ComponentToSound.OnCountdownUpdatedFinished += () =>
            {
            };
        }
    }
}