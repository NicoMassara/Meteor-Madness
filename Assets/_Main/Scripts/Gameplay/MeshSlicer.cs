using EzySlice;
using UnityEngine;

namespace MeteorMadness.Gameplay
{
    public class MeshSlicer
    {
        public static GameObject[] SplitMesh(GameObject target,Transform newParent = null)
        {
            var renderer = target.GetComponent<MeshRenderer>();
            
            var crossSectionMaterial = renderer.material;
            
            return SplitMesh(target,Vector3.zero,Vector3.right,crossSectionMaterial, newParent);
        }
        public static GameObject[] SplitMesh(GameObject target,Vector3 planeNormal,Transform newParent = null)
        {
            var renderer = target.GetComponent<MeshRenderer>();
            
            var crossSectionMaterial = renderer.material;
            
            return SplitMesh(target,Vector3.zero,planeNormal,crossSectionMaterial, newParent);
        }
        public static GameObject[] SplitMesh(GameObject target,Vector3 planePoint,Vector3 planeNormal,Transform newParent = null)
        {
            var renderer = target.GetComponent<MeshRenderer>();
            
            var crossSectionMaterial = renderer.material;
            
            return SplitMesh(target,planePoint,planeNormal,crossSectionMaterial, newParent);
        }
        private static GameObject[] SplitMesh(GameObject target, Vector3 planePoint, Vector3 planeNormal, Material material, Transform newParent = null)
        {
            SlicedHull hull = target.Slice(planePoint, planeNormal);

            if (hull != null)
            {
                GameObject upper = hull.CreateUpperHull(target, material);
                GameObject lower = hull.CreateLowerHull(target, material);
                
                upper.transform.SetParent(newParent);
                lower.transform.SetParent(newParent);
                
                upper.transform.localPosition = Vector3.zero;
                lower.transform.localPosition = Vector3.zero;

                target.SetActive(false);
                
                return new[] { upper, lower};
            }

            return null;
        }
        public static GameObject[] SplitTwiceMesh(GameObject target, Transform newParent = null)
        {
            var startSlices = MeshSlicer.SplitMesh(target,Vector3.zero, Vector3.right, newParent);
            
            if(startSlices == null) return null;

            var sliceCount = 0;
            
            var sliceArray = new GameObject[4];

            foreach (var slice in startSlices)
            {
                Vector3 planePoint = slice.transform.TransformPoint(Vector3.up);
                var slicedSlices = SplitMesh(slice, planePoint, Vector3.up, newParent);
                
                if(slicedSlices == null) return null;

                foreach (var slice2 in slicedSlices)
                {
                    sliceArray[sliceCount] = slice2;
                    sliceCount++;
                }
            }

            foreach (var item in startSlices)
            {
                Object.Destroy(item);
            }

            return sliceArray;
        }
    }
}