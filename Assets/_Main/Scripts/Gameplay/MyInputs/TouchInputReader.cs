using System;
using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace _Main.Scripts.Gameplay.MyInputs
{
    public class TouchInputReader : ITouchInputReader
    {
        private const float TressHoldToCountDoubleTap = 0.5f;

        private float _lastTouchTime = -1f;
        private bool _prevBothTouched;
        private bool _rightTouched;
        private bool _leftTouched;

        private int _currentTouchCount;
        private int _lastTouchCount;
        
        public event Action<int> OnUpdateDirection;
        public event Action<bool> OnTriggerAbility;

        public void Enable()
        {
            TouchSimulation.Enable();
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerUp += OnFingerUp;
        }
        
        public void Disable()
        {
            _rightTouched = false;
            _leftTouched = false;
            
            TouchSimulation.Disable();
            if (EnhancedTouchSupport.enabled)
            {
                Touch.onFingerDown -= OnFingerDown;
                Touch.onFingerUp -= OnFingerUp;
                EnhancedTouchSupport.Disable();
            }
        }

        private void OnFingerDown(Finger input)
        {
            if(IsTouchOverUI(input)) return;
            var touchPos = input.screenPosition;
            if (!IsTouchInSafeZone(touchPos.y))
            {
                Debug.Log("Touch Input out of reach");
                return;
            }
            //
            
            _currentTouchCount++;
            
            if (_currentTouchCount == 2 &&
                _lastTouchCount != 2 &&
                GetHasTriggeredAbility()) return;
            
            _lastTouchCount = _currentTouchCount;
            _lastTouchTime = Time.time;
            
            
            if (IsTouchInLeftZone(touchPos.x))
            {
                _leftTouched = true;
            }
            else if (IsTouchInRightZone(touchPos.x))
            {
                _rightTouched = true;
            }
            
            UpdateRotateDirectionFromTouch();
        }


        private void OnFingerUp(Finger input)
        {
            if(IsTouchOverUI(input)) return;
            
            var touchPos = input.screenPosition;
            if (!IsTouchInSafeZone(touchPos.y))
            {
                Debug.Log("Touch Input out of reach");
                return;
            }
            //
            _currentTouchCount--;

            if (_prevBothTouched)
            {
                OnTriggerAbility?.Invoke(false);
                _prevBothTouched = false;
            }
            
            if(!IsTouchInSafeZone(touchPos.y)) return;
            
            if (IsTouchInLeftZone(touchPos.x))
            {
                _leftTouched = false;
            }
            else if (IsTouchInRightZone(touchPos.x))
            {
                _rightTouched = false;
            }
            
            UpdateRotateDirectionFromTouch();
        }
        
        private void UpdateRotateDirectionFromTouch()
        {
            if (_leftTouched && !_rightTouched)
            {
                OnUpdateDirection?.Invoke(1);
            }
            else if (!_leftTouched && _rightTouched)
            {
                OnUpdateDirection?.Invoke(-1);
            }
            else
            {
                OnUpdateDirection?.Invoke(0);
            }
        }
        
        private bool GetHasTriggeredAbility()
        {
            var tempTime = Time.time - _lastTouchTime;
            if (tempTime <= TressHoldToCountDoubleTap)
            {
                var isRightTouch = false;
                var isLeftTouch = false;

                foreach (var t in Touch.activeTouches)
                {
                    var pos = t.screenPosition;
                
                    if(!IsTouchInSafeZone(pos.y)) continue;
                    
                    if (IsTouchInLeftZone(pos.x))
                    {
                        isLeftTouch = true;
                    }
                    else if(IsTouchInRightZone(pos.x))
                    {
                        isRightTouch = true;
                    }
                
                    if (isRightTouch && isLeftTouch) break;
                }

                if (isRightTouch && isLeftTouch)
                {
                    Debug.Log("Ability Triggered by touch");
                    _prevBothTouched = true;
                    OnTriggerAbility?.Invoke(true);
                    return true;
                }
            }
            else
            {
                Debug.Log($"Ability Trigger Failed, time since last touch{tempTime}");
            }
            
            return false;
        }

        private bool IsTouchInSafeZone(float posY)
        {
            return
                posY >= GameConfigManager.Instance.GetGameplayData().TouchInputData.GetBottomBound()
                &&
                posY <= GameConfigManager.Instance.GetGameplayData().TouchInputData.GetTopBound();
        }


        private bool IsTouchInLeftZone(float posX)
        {
            // ReSharper disable once PossibleLossOfFraction
            return
                posX >= GameConfigManager.Instance.GetGameplayData().TouchInputData.GetLeftZoneBounds().x
                &&
                posX <= GameConfigManager.Instance.GetGameplayData().TouchInputData.GetLeftZoneBounds().y;
        }

        private bool IsTouchInRightZone(float posX)
        {
                // ReSharper disable once PossibleLossOfFraction
            return
                posX >= GameConfigManager.Instance.GetGameplayData().TouchInputData.GetRightZoneBounds().x
                &&
                posX <= GameConfigManager.Instance.GetGameplayData().TouchInputData.GetRightZoneBounds().y;
        }
        
        private bool IsTouchOverUI(Finger finger)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = finger.screenPosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }

    }
}