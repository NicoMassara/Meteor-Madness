using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldView : MonoBehaviour, IObserver
    {
        [Header("Components")] 
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private GameObject normalSprite;
        [SerializeField] private GameObject superSprite;
        [Header("Sounds")]
        [SerializeField] private SoundBehavior hitSound;
        [SerializeField] private SoundBehavior moveSound;
        [Space] 
        [Header("Movement Values")]
        [SerializeField] private float rotateSpeed = 6.75f;
        [Header("Values")]
        [SerializeField] private ShakeDataSo hitShakeData;
        [SerializeField] private ShakeDataSo cameraShakeData;
        [SerializeField] private ParticleDataSo deflectParticleData;

        //Multiplier added to handle lower numbers in inspector
        private float GetRotateSpeed => rotateSpeed * 50f; 
        private GameObject _activeSprite;
        private ShakerController _shakerController;

        private void Awake()
        {
            
        }

        private void Start()
        {
            _shakerController = new ShakerController(transform);
            _shakerController.SetShakeData(hitShakeData);
        }

        private void Update()
        {
            if (_shakerController.IsShaking)
            {
                _shakerController.HandleShake();
            }
        }

        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case ShieldObserverMessage.Rotate:
                    HandleRotation((float)args[0]);
                    break;
                case ShieldObserverMessage.StopRotate:
                    HandleStopRotate();
                    break;
                case ShieldObserverMessage.Deflect:
                    HandleDeflect((Vector3)args[0]);
                    break;
                case ShieldObserverMessage.PlayMoveSound:
                    PlayMoveSound();
                    break;
                case ShieldObserverMessage.RestartPosition:
                    HandleRestartPosition();
                    break;
                case ShieldObserverMessage.SetActiveShield:
                    HandleSetActiveShield((bool)args[0]);
                    break;
                case ShieldObserverMessage.SetSpriteType:
                    HandleSetSpriteType((SpriteType)args[0]);
                    break;
            }
        }
        
        #region Sprites

        private void HandleSetActiveShield(bool isActive)
        {
            spriteContainer.SetActive(isActive);
        }
        
        private void HandleSetSpriteType(SpriteType spriteType)
        {
            _activeSprite?.SetActive(false);

            _activeSprite = spriteType switch
            {
                SpriteType.Normal => normalSprite,
                SpriteType.Super => superSprite,
                _ => _activeSprite
            };
            
            _activeSprite?.SetActive(true);
        }

        #endregion

        #region Movement
        private void HandleRotation(float direction)
        {
            transform.RotateAround(transform.position, Vector3.forward, 
                ((GetRotateSpeed) * direction) * Time.deltaTime);
        }
        private void HandleStopRotate()
        {
            
        }
        
        private void PlayMoveSound()
        {
            moveSound?.PlaySound();
        }
        
        private void HandleRestartPosition()
        {
            transform.rotation = Quaternion.Euler(0,0,0);
        }

        #endregion

        private void HandleDeflect(Vector3 position)
        {
            _shakerController.StartShake();
            hitSound?.PlaySound();
            
            GameManager.Instance.EventManager.Publish
            (
                new SpawnParticle
                {
                    ParticleData = deflectParticleData,
                    Position = position,
                    Rotation = Quaternion.identity
                }
            );
            
            GameManager.Instance.EventManager.Publish(new CameraShake{ShakeData = cameraShakeData});
        }
        
    }
}