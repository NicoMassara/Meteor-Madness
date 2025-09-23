using System;
using System.Collections;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using EzySlice;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthSlicer : ManagedBehavior, IUpdatable
    {
        [SerializeField] private Material capMaterial;
        [SerializeField] private Transform slicePlane; // defines where & how to slice
        [Range(0,1.5f)]
        [SerializeField] private float sliceDistance;

        private ActionQueue _actionQueue = new ActionQueue();
        private MeshFilter meshA;
        private MeshFilter meshB;
        private bool _isSliced;
        private bool _canMove;

        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Effects;

        public void ManagedUpdate()
        {

        }
        
        #region Slice

        public void StartSlicing()
        {
            SetSliceQueue();
            StartCoroutine(RunQueue(sliceDistance, EarthSliceTimeValues.MoveSlices));
        }
        
        private void SetSliceQueue()
        {
            var temp = new[]
            {
                new ActionData(() =>
                {
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 0f);
                }),
                new ActionData(() =>
                {
                    Slice();
                    _canMove = true;
                }, EarthSliceTimeValues.StartSlice),
                
                new ActionData(() => _canMove = false, EarthSliceTimeValues.MoveSlices),
                
                new ActionData(() =>
                {
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 1f);
                }, EarthSliceTimeValues.ReturnToNormalTime),
            };
            
            _actionQueue.AddAction(temp);
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

                _isSliced = true;

                planeObj.GetComponent<MeshRenderer>().enabled = false;
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
            var speed = (distance / targetTime) * CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup);
            var newX = Mathf.MoveTowards(lastPosition.x, targetPosition.x, speed);
            partTransform.localPosition = new Vector2(newX, lastPosition.y);
        }

        #endregion
        
        #region Unite
        
        public void StartUnite()
        {
            if(!_isSliced) return;
            
            SetUniteQueue();
            StartCoroutine(RunQueue(0, EarthSliceTimeValues.ReturnSlices));
        }
        
        private void SetUniteQueue()
        {
            var temp = new[]
            {
                new ActionData(() =>
                {
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 0f);
                    _canMove = true;
                }),
                
                new ActionData(() =>
                {
                    UniteMeshes();
                    _canMove = false;
                    
                }, EarthSliceTimeValues.ReturnSlices),
                
                new ActionData(() =>
                {
                    UniteMeshes();
                    CustomTime.SetChannelTimeScale(
                        new []{UpdateGroup.UI, UpdateGroup.Gameplay, UpdateGroup.Earth}, 1f);
                }, EarthSliceTimeValues.ReturnSlices),
            };
            
            _actionQueue.AddAction(temp);
        }


        private void UniteMeshes() 
        {
            Destroy(meshA);
            Destroy(meshB);
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            
            /*meshA.transform.localPosition = Vector3.zero;
            meshB.transform.localPosition = Vector3.zero;

            MeshFilter filterA = meshA.GetComponent<MeshFilter>();
            MeshFilter filterB = meshB.GetComponent<MeshFilter>();

            if (filterA == null || filterB == null) return;

            // Prepare combine array
            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = filterA.sharedMesh;
            combine[0].transform = filterA.transform.localToWorldMatrix;
            combine[1].mesh = filterB.sharedMesh;
            combine[1].transform = filterB.transform.localToWorldMatrix;

            // Create new mesh
            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combine);

            // Create a new GameObject with the merged mesh
            GameObject united = new GameObject("UnitedMesh");
            united.transform.SetParent(slicePlane);
            MeshFilter mf = united.AddComponent<MeshFilter>();
            MeshRenderer mr = united.AddComponent<MeshRenderer>();

            mf.mesh = newMesh;
            mr.material = meshA.GetComponent<MeshRenderer>().sharedMaterial;

            capMaterial = mr.material;

            // Optional: destroy the originals
            Destroy(meshA);
            Destroy(meshB);

            _isSliced = false;*/
        }

        #endregion
        
        private IEnumerator RunQueue(float targetDistance, float targetTime)
        {
            while (!_actionQueue.IsEmpty)
            {
                _actionQueue.Run(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
                
                if (_canMove)
                {
                    MoveSlicedParts(targetDistance, targetTime);
                }
                
                yield return null;
            }
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