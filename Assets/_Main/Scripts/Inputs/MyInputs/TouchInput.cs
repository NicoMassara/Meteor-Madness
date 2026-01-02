using System;
using System.Collections.Generic;
using System.Linq;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Contracts.Interfaces.GameplayData;
using NicolasMassara.CustomTimerManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace _Main.Scripts.MyInputs
{
#if UNITY_ANDROID

    public class TouchInput : IInput
    {
        private const int MaxTouchCount = 2;
        private const float TressHoldToCountDoubleTap = 0.05f;
        private readonly Dictionary<int, TouchData> _touchesDic = new Dictionary<int, TouchData>();
        private readonly List<int> _indexList = new List<int>();
        private TimerManager.GeneratedId _addTouchTimerId;
        private ITouchInputData _touchInputData;
        private int CurrentTouches => _touchesDic.Count;
        private double _lastTouchTime = ulong.MaxValue;
        private bool _hasTriggeredAbility;
        
        private TouchType _currentTouchType;
        
        public event Action<int> OnUpdateDirection;
        public event Action<bool> OnTriggerAbility;
        
        
#pragma warning disable CS0067 
        // This event is used by KeyInput to use the Keyboard inputs
        // Mobile only can Pause the game on the UI so this event is useless here
        public event Action OnPaused;
#pragma warning restore CS0067

        public TouchInput(ITouchInputData touchInputData)
        {
            _touchInputData = touchInputData;
            
            OnTriggerAbility += (isTriggered) =>
            {
                _hasTriggeredAbility = isTriggered;
            };
        }

        public void Enable()
        {
            //Adds -1 so can be null
            _indexList.Add(-1);
            TouchSimulation.Enable();
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerUp += OnFingerUp;
        }

        public void Disable()
        {
            _touchesDic.Clear();
            _indexList.Clear();
            TryRemoveTimer();
            
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
            if(_touchInputData == null) return; 
            
            var touchType = GetTouchType(input.screenPosition);
            if(touchType == TouchType.OutOfBounds) return;

            TryRemoveTimer();
            
            var lastCount = CurrentTouches;
            
            AddTouchDataToDic(new TouchData(touchType, input.currentTouch, input.index));
            
            if (lastCount == 0)
            {
                _lastTouchTime = Time.realtimeSinceStartup;

                _addTouchTimerId = TimerManager.Add(new TimerData
                (TressHoldToCountDoubleTap + Time.deltaTime, () =>
                {
                    UpdateRotateDirection();
                }));
            }
            else if (lastCount == 1)
            {
                if (GetCanTriggerAbility())
                {
                    TriggerAbility();
                }
                else
                {
                    UpdateRotateDirection();
                }
            }
        }
        
        private void OnFingerUp(Finger input)
        {
            int fingerIndex = input.index;
            
            if (GetIsActiveTouch(fingerIndex))
            {
                if (_hasTriggeredAbility)
                {
                    OnTriggerAbility?.Invoke(false);
                }
                
                RemoveTouchData(fingerIndex);
                UpdateRotateDirection();
            }
        }
        
        private void UpdateRotateDirection()
        {
            _currentTouchType = GetTouchTypeByIndex(GetLastTouchIndex());
            
            if (_currentTouchType == TouchType.Left)
            {
                OnUpdateDirection?.Invoke(1);
            }
            else if (_currentTouchType == TouchType.Right)
            {
                OnUpdateDirection?.Invoke(-1); 
            }
            else
            {
                OnUpdateDirection?.Invoke(0);
            }
        }

        private bool GetCanTriggerAbility()
        {
            var tempTime = Time.realtimeSinceStartup - _lastTouchTime;
            return tempTime < TressHoldToCountDoubleTap;
        }

        private void TriggerAbility()
        {
            var isRightTouch = false;
            var isLeftTouch = false;

            var tempList = _touchesDic.Values.ToList();
            
            foreach (var touchData in tempList)
            {
                if (touchData.TouchType == TouchType.Left)
                {
                    isLeftTouch = true;
                }
                else if(touchData.TouchType == TouchType.Right)
                {
                    isRightTouch = true;
                }
                
                if (isRightTouch && isLeftTouch) break;
            }
            
            if (isRightTouch && isLeftTouch)
            {
                OnTriggerAbility?.Invoke(true);
            }
        }

        private bool IsTouchInSafeZone(float posY)
        {
            return
                posY >= _touchInputData.GetBottomBound()
                &&
                posY <= _touchInputData.GetTopBound();
        }


        private bool IsTouchInLeftZone(float posX)
        {
            // ReSharper disable once PossibleLossOfFraction
            return
                posX >= _touchInputData.GetLeftZoneBounds().x
                &&
                posX <= _touchInputData.GetLeftZoneBounds().y;
        }

        private bool IsTouchInRightZone(float posX)
        {
                // ReSharper disable once PossibleLossOfFraction
            return
                posX >= _touchInputData.GetRightZoneBounds().x
                &&
                posX <= _touchInputData.GetRightZoneBounds().y;
        }
        
        private bool IsTouchOverUI(Vector2 touchPosition)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = touchPosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }

        private TouchType GetTouchType(Vector2 touchPosition)
        {
            if (IsTouchOverUI(touchPosition) || !IsTouchInSafeZone(touchPosition.y))
            {
                return TouchType.OutOfBounds;
            }

            if (IsTouchInLeftZone(touchPosition.x))
            {
                return TouchType.Left;
            }
            
            if (IsTouchInRightZone(touchPosition.x))
            {
                return TouchType.Right;
            }
            
            return TouchType.OutOfBounds;
        }

        private void AddTouchDataToDic(TouchData dataToAdd)
        {
            _touchesDic.Add(dataToAdd.Index, dataToAdd);
            _indexList.Add(dataToAdd.Index);
        }

        private void RemoveTouchData(int index)
        {
            if (_touchesDic.ContainsKey(index))
            {
                _touchesDic.Remove(index);
                _indexList.Remove(index);
            }
        }
        
        private void TryRemoveTimer()
        {
            TimerManager.Remove(_addTouchTimerId);
        } 

        private TouchType GetTouchTypeByIndex(int index)
        {
            if (_touchesDic.TryGetValue(index, out var touch))
            {
                return touch.TouchType;
            }

            return TouchType.None;
        }

        private bool GetIsActiveTouch(int index)
        {
            return _touchesDic.ContainsKey(index);
        }

        private int GetLastTouchIndex()
        {
            return _indexList[^1];
        }

    }

    public enum TouchType
    {
        None,
        OutOfBounds,
        Right,
        Left
    }

    public class TouchData
    {
        public TouchType TouchType;
        public Touch StartTouch;
        public readonly int Index;
        
        public TouchData(TouchType touchType, Touch startTouch, int index)
        {
            TouchType = touchType;
            StartTouch = startTouch;
            Index = index;
        }
    }

#endif
}