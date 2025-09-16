using System;
using System.Collections;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;
using _Main.Scripts.MyCustoms;
using EzySlice;
using UnityEngine;

namespace _Main.Scripts
{
    public class PlaneSlicer : ManagedBehavior, IUpdatable
    {
        [SerializeField] private Material capMaterial;
        [SerializeField] private Transform slicePlane; // defines where & how to slice
        [Range(0,3f)]
        [SerializeField] private float sliceDistance;
        [Range(0, 5f)]
        [SerializeField] private float timeToMove;

        private ActionQueue _sliceQueue = new ActionQueue();
        private MeshFilter meshA;
        private MeshFilter meshB;
        private bool _isSliced;
        private bool _canMove;

        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Effects;

        public void ManagedUpdate()
        {
            if (_canMove)
            {
                MoveSlicedParts();
            }
        }

        public void RestartValues()
        {
            SetSliceQueue();
            UniteMeshes();
        }
        
        #region Slice

        private void SetSliceQueue()
        {
            _sliceQueue.AddAction(new ActionData(()=> CustomTime.GetChannel(UpdateGroup.UI).SetTimeScale(0f), 0));
            _sliceQueue.AddAction(new ActionData(()=> CustomTime.GetChannel(UpdateGroup.Gameplay).SetTimeScale(0f), 0));
            _sliceQueue.AddAction(new ActionData(()=> Slice(), 1f));
            _sliceQueue.AddAction(new ActionData(()=> _canMove = true, 0f));
            _sliceQueue.AddAction(new ActionData(()=> _canMove = false, timeToMove));
            _sliceQueue.AddAction(new ActionData(()=> CustomTime.GetChannel(UpdateGroup.UI).SetTimeScale(1), 2f));
            _sliceQueue.AddAction(new ActionData(()=> CustomTime.GetChannel(UpdateGroup.Gameplay).SetTimeScale(1f), 0));
        }
        
        public void StartSlicing()
        {
            StartCoroutine(RunQueue());
        }

        private IEnumerator RunQueue()
        {
            while (!_sliceQueue.IsEmpty)
            {
                _sliceQueue.Run(CustomTime.GetChannel(SelfUpdateGroup).DeltaTime);
                
                yield return null;
            }
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

        private void MoveSlicedParts()
        {
            HandlePartMovement(meshA.transform, Vector2.right);
            HandlePartMovement(meshB.transform, Vector2.left);
        }

        private void HandlePartMovement(Transform partTransform, Vector2 direction)
        {
            var lastPosition = partTransform.localPosition;
            var targetPosition = new Vector2(sliceDistance * direction.x, lastPosition.y);
            var distance = Vector2.Distance(lastPosition, targetPosition);
            var speed = (distance / timeToMove) * CustomTime.GetChannel(SelfUpdateGroup).DeltaTime;
            var newX = Mathf.MoveTowards(lastPosition.x, targetPosition.x, speed);
            partTransform.localPosition = new Vector2(newX, lastPosition.y);
        }

        #endregion


        
        private void UniteMeshes() 
        {
            if(!_isSliced) return;  
            
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var start = new Vector3(slicePlane.position.x - sliceDistance, slicePlane.position.y, slicePlane.position.z);
            var end = new Vector3(slicePlane.position.x + sliceDistance, slicePlane.position.y, slicePlane.position.z);
            Gizmos.DrawLine(start,end);
        }
    }
}