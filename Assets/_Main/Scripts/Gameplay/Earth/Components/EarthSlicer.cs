using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using EzySlice;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthSlicer : ManagedBehavior, IUpdatable
    {
        [SerializeField] private Material capMaterial;
        [SerializeField] private Transform slicePlane; // defines where & how to slice
        [Range(0,1.5f)]
        [SerializeField] private float sliceDistance;
        
        private MeshFilter meshA;
        private MeshFilter meshB;
        
        private bool _isSliced;
        private bool _canMove;
        private float _moveTargetDistance;
        private float _moveTargetTime;
        private float _deltaTime;
        
        public event Action OnStartSlice;
        public event Action OnEndSlice;
        public event Action OnStartUnite;
        public event Action OnEndUnite;

        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Effects;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;


        private void Start()
        {
            Slice();
        }

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
            _moveTargetDistance = sliceDistance;
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
                    SetActiveSlices(true);
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
                    OnEndSlice?.Invoke();
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 1f);
                }))
                .Build();
            
            
            ActionManager.Add(action);
        }
        
        private void Slice() 
        {
            GameObject planeObj = gameObject;
            
            SlicedHull hull = planeObj.Slice(slicePlane.position, slicePlane.right, capMaterial);

            if (hull != null) {
                GameObject upper = hull.CreateUpperHull(planeObj, planeObj.GetComponent<Renderer>().material);
                GameObject lower = hull.CreateLowerHull(planeObj, planeObj.GetComponent<Renderer>().material);

                upper.transform.SetParent(slicePlane);
                upper.transform.localPosition = Vector3.zero;
                
                lower.transform.SetParent(slicePlane);
                lower.transform.localPosition = Vector3.zero;
                
                upper.AddComponent<MeshCollider>().convex = true;
                lower.AddComponent<MeshCollider>().convex = true;
                
                meshA = upper.GetComponent<MeshFilter>();
                meshB = lower.GetComponent<MeshFilter>();
                
                meshA.gameObject.AddComponent<MeshSortingLayerSetter>().SetSortingLayer(
                    meshA.GetComponent<Renderer>(), "Earth", 0);
                meshB.gameObject.AddComponent<MeshSortingLayerSetter>().SetSortingLayer(
                    meshB.GetComponent<Renderer>(), "Earth", 0);

                _isSliced = true;

                planeObj.GetComponent<MeshRenderer>().enabled = false;
                
                SetActiveSlices(false);
            }
        }

        private void MoveSlicedParts(float targetDistance, float targetTime)
        {
            HandlePartMovement(meshA.transform, Vector2.right, targetDistance,targetTime);
            HandlePartMovement(meshB.transform, Vector2.left, targetDistance,targetTime);
        }

        private void HandlePartMovement(Transform partTransform, Vector2 direction, float targetDistance, float targetTime)
        {
            var lastPosition = partTransform.localPosition;
            var targetPosition = new Vector2(targetDistance * direction.x, lastPosition.y);
            var distance = Vector2.Distance(lastPosition, targetPosition);
            var speed = (distance / targetTime) * _deltaTime;
            var newX = Mathf.MoveTowards(lastPosition.x, targetPosition.x, speed);
            partTransform.localPosition = new Vector2(newX, lastPosition.y);
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
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 0f);
                    _canMove = true;
                }))
                .Then(new WaitSecondsAction(sliceTimes.ReturnSlices))
                .Then(new InstantAction(() =>
                {
                    UniteMeshes();
                    _canMove = false;
                }))
                .Then(new WaitSecondsAction(sliceTimes.ReturnSlices))
                .Then(new InstantAction(() =>
                {
                    UniteMeshes();
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 1f);
                    OnEndUnite?.Invoke();
                }))
                .Build();
            
            ActionManager.Add(action);
        }


        private void UniteMeshes()
        {
            SetActiveSlices(false);
        }

        #endregion

        private void SetActiveSlices(bool isActive)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = !isActive;
            meshA.gameObject.SetActive(isActive);
            meshB.gameObject.SetActive(isActive);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var start = new Vector3(slicePlane.position.x - sliceDistance, slicePlane.position.y, slicePlane.position.z);
            var end = new Vector3(slicePlane.position.x + sliceDistance, slicePlane.position.y, slicePlane.position.z);
            Gizmos.DrawLine(start,end);
        }
    }
}