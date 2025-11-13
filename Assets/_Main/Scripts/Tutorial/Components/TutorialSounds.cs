using _Main.Scripts.Sounds;
using _Main.Scripts.Tutorial.MVC;
using UnityEngine;

namespace _Main.Scripts.Tutorial.Components
{
    public class TutorialSounds : SoundBehaviour<TutorialView>
    {
        [SerializeField] private SoundClassSo music;

        private void Start()
        {
            ComponentToSound.OnTutorialEnable += () =>
            {
                PlayMusic(music);
            };
        }
    }
}