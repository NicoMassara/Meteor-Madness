using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Gameplay.FloatingScore
{
    public class FloatingTextBehaviour : ManagedBehavior,IUpdatable,IFloatingText
    {
        [SerializeField] private TextMeshPro meshText;
        [Range(1, 5)] 
        [SerializeField] private float movementSpeed = 2;

        [Range(0.1f, 3f)] [SerializeField] private float movementTime = 1f;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public event Action<IFloatingText> OnRecycle;
        private bool _canMove;
        
        public void ManagedUpdate()
        {
            if (_canMove)
            {
                HandleMovement();
            }
        }

        private void HandleMovement()
        {
            var finalSpeed = (CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup) * movementSpeed);
            transform.position += Vector3.up * finalSpeed ;
        }

        public void SetValues(FloatingTextValues values)
        {
            transform.position = values.Position;
            meshText.text = values.Text;
            meshText.color = values.Color;

            _canMove = true;
            
            TimerManager.Add(new TimerData
            {
                Time = movementTime,
                OnEndAction = Recycle
            });
        }

        public void Recycle()
        {
            _canMove = false;
            OnRecycle?.Invoke(this);
        }
    }

    public class FloatingTextValues
    {
        public Vector2 Position;
        public string Text;
        public Color Color;
    }
}