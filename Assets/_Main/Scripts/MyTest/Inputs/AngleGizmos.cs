using UnityEngine;

namespace _Main.Scripts.MyTest.Inputs
{
    public class AngleGizmos : MonoBehaviour
    {
        private readonly int _divisions = GameParameters.GameplayValues.AngleSlots;
        [Range(1,10f)]
        public float radius = 1f;
        public Color gizmoColor = Color.yellow;

        void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;

            for (int i = 0; i < _divisions; i++)
            {
                float angle = i * (360f / _divisions);
                float rad = angle * Mathf.Deg2Rad;

                Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
                Gizmos.DrawLine(transform.position, transform.position + dir * radius);
            }
        }
    }
}