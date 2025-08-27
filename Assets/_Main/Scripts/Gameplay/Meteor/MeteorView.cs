using System;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorView : MonoBehaviour
    {
        [SerializeField] private GameObject normalSpriteObject;
        [SerializeField] private GameObject destroyedSpriteObject;
        
        private Meteor _meteor;
        private float _alpha = 1;
        private readonly Timer _startDissolveTimer = new Timer();

        private void Awake()
        {
            _meteor = GetComponent<Meteor>();
        }

        private void Start()
        {
            _meteor.OnHit += OnHitHandler;
            _meteor.OnValuesSet += OnValuesSetHandler;
        }

        private void Update()
        {
            if (_startDissolveTimer.HasEnded())
            {
                _alpha -= Time.deltaTime * GameValues.MeteorRecycleTime;
                destroyedSpriteObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, _alpha);
            }
        }

        private void OnValuesSetHandler()
        {
            normalSpriteObject.SetActive(true);
            destroyedSpriteObject.SetActive(false);
            _alpha = 1;
            destroyedSpriteObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            _startDissolveTimer.Reset();
        }

        private void OnHitHandler(Meteor meteor, bool hasHitShield)
        {
            if (hasHitShield)
            {
                normalSpriteObject.SetActive(false);
                destroyedSpriteObject.SetActive(true);
                _startDissolveTimer.Set(GameValues.MeteorRecycleTime/2);
            }
        }
    }
}