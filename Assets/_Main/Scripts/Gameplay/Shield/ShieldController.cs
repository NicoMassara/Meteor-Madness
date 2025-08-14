using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    public class ShieldController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject spriteObject;
        [Header("Values")]
        [Range(1,10)]
        [SerializeField] private float rotateSpeed;

        public void Rotate(float direction = 1)
        {
            transform.RotateAround(transform.position, Vector3.forward, (rotateSpeed/10) * direction);
        }

        public void SetActiveSprite(bool isActive)
        {
            spriteObject.SetActive(isActive);
        }
    }
}