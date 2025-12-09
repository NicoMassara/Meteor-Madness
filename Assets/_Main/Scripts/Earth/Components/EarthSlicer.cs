using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Slicer;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Earth
{
    public class EarthSlicer : ManagedBehavior, IUpdatable
    {
        [Header("Slice Values")]
        [SerializeField] private GameObject slicePlane; // defines where & how to slice
        [SerializeField] private GameObject sliceContainer;
        private SliceType _sliceType = SliceType.Default;

        private GameObject[] _slices;
            
        public enum SliceType
        {
            Default,
            Pizza
        }
        
        private bool _isSliced;
        private bool _canMove;
        private float _moveTargetDistance;
        private float _moveTargetTime;
        private float _deltaTime;
        private bool _shouldPreSlice = true;
        
        public event Action OnStartSlice;
        public event Action OnEndSlice;
        public event Action OnStartUnite;
        public event Action OnEndUnite;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Effects;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;


        public void ExecuteUpdate(float deltaTime)
        {
            _deltaTime = deltaTime;
            
            if (_canMove)
            {
                MoveSlicedParts(_moveTargetDistance, _moveTargetTime);
            }
        }
        
        #region Slice

        public void StartSlicing()
        {
            _moveTargetDistance = GetSliceDistance();
            var sliceTimes = GameConfigManager.Instance.GetGameplayData().EarthTimeData.Slice;
            _moveTargetTime = sliceTimes.MoveSlices;
            SetSliceQueue(sliceTimes);
        }
        
        private void SetSliceQueue(IEarthSlice sliceTimes)
        {
            var action = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    OnStartSlice?.Invoke();
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 0f);
                }))
                .Then(new WaitSecondsAction(sliceTimes.StartSlice))
                .Then(new InstantAction(() =>
                {
                    SetActiveContainer(true);
                    _canMove = true;
                }))
                .Then(new WaitSecondsAction(sliceTimes.MoveSlices))
                .Then(new InstantAction(() =>
                {
                    _canMove = false;
                }))
                .Then(new WaitSecondsAction(sliceTimes.ReturnToNormalTime))
                .Then(new InstantAction(() =>
                {
                    _isSliced = true;
                    OnEndSlice?.Invoke();
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 1f);
                }))
                .Build();
            
            
            ActionManager.Add(action,ActionManager.UpdateType.Update);
        }
        
        public void PreSlice()
        {
            if(_shouldPreSlice == false) return;

            _slices = _sliceType switch
            {
                SliceType.Default => MeshSlicer.SplitMesh(slicePlane, sliceContainer.transform),
                SliceType.Pizza => MeshSlicer.SplitTwiceMesh(slicePlane, sliceContainer.transform),
                _ => null
            };

            if (_slices == null)
            {
                Debug.Log("Slices could not be created");
            }
            
            SetActiveContainer(false);

            _shouldPreSlice = false;
        }

        #endregion
        
        #region Unite
        
        public void StartUnite()
        {
            if(!_isSliced) return;
            
            _moveTargetDistance = 0;
            var sliceTimes = GameConfigManager.Instance.GetGameplayData().EarthTimeData.Slice;
            _moveTargetTime = sliceTimes.ReturnSlices;
            SetUniteQueue(sliceTimes);
        }
        
        private void SetUniteQueue(IEarthSlice sliceTimes)
        {
            var action = ActionBuilder.Start()
                .Do(new InstantAction(() =>
                {
                    OnStartUnite?.Invoke();
                    _canMove = true;
                }))
                .Then(new WaitSecondsAction(sliceTimes.ReturnSlices))
                .Then(new InstantAction(() =>
                {
                    _isSliced = false;
                    _canMove = false;
                    OnEndUnite?.Invoke();
                }))
                .Build();
            
            ActionManager.Add(action,ActionManager.UpdateType.Update);
        }


        public void UniteMeshes()
        {
            SetActiveContainer(false);
        }

        #endregion

        #region Movement
        
        private void MoveSlicedParts(float targetDistance, float targetTime)
        {
            if (_sliceType == SliceType.Pizza)
            {
                for (int i = 0; i < _slices.Length; i++)
                {
                    float xDir = (i < 2) ? 1 : -1;
                    float yDir = (i % 2 == 0) ? 1 : -1;
                
                    HandlePartMovement(_slices[i].transform, new Vector2(xDir,yDir), targetDistance,targetTime);
                }
            }
            else
            {
                for (int i = 0; i < _slices.Length; i++)
                {
                    float xDir = (i % 2 == 0) ? 1 : -1;
                    HandlePartMovement(_slices[i].transform, new Vector2(xDir,_slices[i].transform.position.y), targetDistance,targetTime);
                }
            }
        }
        
        private void HandlePartMovement(Transform partTransform, Vector2 direction, float targetDistance, float targetTime)
        {
            var lastPosition = partTransform.localPosition;
            var targetPosition = new Vector2(targetDistance * direction.x, targetDistance * direction.y);
            var distance = Vector2.Distance(lastPosition, targetPosition);
            var speed = (distance / targetTime) * _deltaTime;
            var newX = Mathf.MoveTowards(lastPosition.x, targetPosition.x, speed);
            var newY = Mathf.MoveTowards(lastPosition.y, targetPosition.y, speed);
            partTransform.localPosition = new Vector2(newX,newY);
        }
        
        #endregion

        public void SetSliceType(SliceType sliceType)
        {
            if(_sliceType == sliceType)
                return;
            
            DestroyActiveSlices();
            _sliceType = sliceType;
            _shouldPreSlice = true;
        }

        private void SetActiveContainer(bool isActive)
        {
            sliceContainer.SetActive(isActive);
        }

        private void DestroyActiveSlices()
        {
            if(_slices == null) return;
            
            var tempArray = (GameObject[]) _slices.Clone();
            
            foreach (var item in tempArray)
            {
                Destroy(item);
            }

            _slices = null;
        }

        private float GetSliceDistance()
        {
            return _sliceType switch
            {
                SliceType.Default => 1.25f,
                SliceType.Pizza => 0.75f,
                _ => 1f
            };
        }
    }
}