using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldTrail))]
    public class ShieldAppereance : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private GameObject normalSprite;
        [SerializeField] private GameObject superSprite;
        [Header("Values")]
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToEnableSuperShield;
        [Range(0.1f, 5f)]
        [SerializeField] private float timeToDisableSuperShield;
        
        private ShieldSpriteAlphaSetter _spriteAlphaSetter;
        private ShieldTrail _shieldTrail;

        public float TimeToEnableSuperShield => timeToEnableSuperShield;

        public float TimeToDisableSuperShield => timeToDisableSuperShield;

        private void Awake()
        {
            _spriteAlphaSetter = new ShieldSpriteAlphaSetter(normalSprite,superSprite, 
                timeToEnableSuperShield,timeToDisableSuperShield);

            _shieldTrail = GetComponent<ShieldTrail>();
        }
        
        private void Start()
        {
            SetActiveSuperShieldSprite(false);
        }

        #region AlphaSetter

        public void RestartValues()
        {
            _spriteAlphaSetter.RestartValues();
        }

        public void EnableSuperShield(float deltaTime)
        {
            _spriteAlphaSetter.EnableSuper(deltaTime);
        }

        public void EnableNormalShield(float deltaTime)
        {
            _spriteAlphaSetter.EnableNormal(deltaTime);
        }

        #endregion

        #region Sprites Color

        public void SetAutomaticEnable(bool isActive)
        {
            var color = isActive ? Color.red : Color.white;
            if (isActive)
            {
                _shieldTrail.SetTrailColor(color);
            }
            else
            {
                _shieldTrail.SetDefault();
            }
            
            normalSprite.GetComponent<SpriteRenderer>().color = color;
        }
        
        public void SetGoldEnable(bool isActive)
        {
            var color = isActive ? Color.yellow : Color.white;
            if (isActive)
            {
                _shieldTrail.SetTrailColor(color);
            }
            else
            {
                _shieldTrail.SetDefault();
            }
            normalSprite.GetComponent<SpriteRenderer>().color = color;
        }

        public void SetSlowEnable(bool isActive)
        {
            var color = isActive ? Color.green : Color.white;
            if (isActive)
            {
                _shieldTrail.SetTrailColor(color);
            }
            else
            {
                _shieldTrail.SetDefault();
            }
            normalSprite.GetComponent<SpriteRenderer>().color = color;
        }

        #endregion

        public void SetActiveSuperShieldSprite(bool isActive)
        {
            superSprite.SetActive(true);
        }

    }
}