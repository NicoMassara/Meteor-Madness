using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldSpriteAlphaSetter
    {
        private readonly GameObject _normalSprite;
        private readonly GameObject _superSprite;
        
        private readonly SpriteAlphaSetter _normalSpriteSetter;
        private readonly SpriteAlphaSetter _superSpriteSetter;

        public ShieldSpriteAlphaSetter(GameObject normalSprite, GameObject superSprite,
            float fadeInTime, float fadeOutTime)
        {
            var normalSpriteRenderer = normalSprite.GetComponent<SpriteRenderer>();
            var superSpriteRenderer = superSprite.GetComponent<SpriteRenderer>();
            
            _normalSpriteSetter = new SpriteAlphaSetter(
                normalSpriteRenderer,fadeInTime,fadeOutTime);
            _superSpriteSetter = new SpriteAlphaSetter(
                superSpriteRenderer,fadeInTime,fadeOutTime);
            
            _normalSpriteSetter.SetSpriteAlpha(1f);
            _superSpriteSetter.SetSpriteAlpha(0f);
        }

        public void EnableSuper(float deltaTime)
        {
            _superSpriteSetter.Appear(deltaTime * 2);
            _normalSpriteSetter.Disappear(deltaTime);
        }

        public void EnableNormal(float deltaTime)
        {
            _normalSpriteSetter.Appear(deltaTime * 2);
            _superSpriteSetter.Disappear(deltaTime);
        }

        public void RestartValues()
        {
            _normalSpriteSetter.RestartValues();
            _superSpriteSetter.RestartValues();
        }
    }
}