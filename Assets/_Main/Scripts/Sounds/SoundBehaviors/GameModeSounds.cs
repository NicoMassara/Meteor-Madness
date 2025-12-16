using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class GameModeSounds : MusicBehavior<IGameModeSounds>
    {
        [Header("Music")]
        [SerializeField] private SoundSourceDataSo gameMusicData;
        [Space]
        [Header("Countdown")]
        [SerializeField] private SoundSourceDataSo countdownSound;
        [SerializeField] private SoundSourceDataSo countdownFinish;
        
        private SoundManager.GeneratedId _gameMusicId;
        
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
                _gameMusicId = PlayMusic(gameMusicData, _gameMusicId, false);
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