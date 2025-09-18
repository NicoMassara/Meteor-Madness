using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class SpriteAlphaSetter
    {
        private readonly SpriteRenderer _renderer;
        private readonly float _timeToAppear;
        private readonly float _timeToDisappear;

        private float _elapsedTime = 0;
        private float _currentAlpha = 0;

        public SpriteAlphaSetter(SpriteRenderer renderer, float timeToAppear, float timeToDisappear)
        {
            _renderer = renderer;
            _timeToAppear = timeToAppear;
            _timeToDisappear = timeToDisappear;
        }

        public void Appear(float deltaTime)
        {
            _currentAlpha += deltaTime / _timeToAppear;
            _currentAlpha = Mathf.Clamp01(_currentAlpha);
            SetSpriteAlpha(_currentAlpha);
        }

        public void Disappear(float deltaTime)
        {
            _currentAlpha -= deltaTime / _timeToDisappear;
            _currentAlpha = Mathf.Clamp01(_currentAlpha);
            SetSpriteAlpha(_currentAlpha);
        }

        public void RestartValues()
        {
            _elapsedTime = 0;
        }

        public void SetSpriteAlpha(float alpha)
        {
            _currentAlpha = alpha;
            _renderer.color = new Color(1,1,1, _currentAlpha);
        }
    }
}