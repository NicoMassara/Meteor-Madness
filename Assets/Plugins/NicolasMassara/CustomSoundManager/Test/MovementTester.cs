using System;
using UnityEngine;

namespace Plugins.NicolasMassara.CustomSoundManager.Test
{
    public class MovementTester : MonoBehaviour
    {
        [Range(1,10f)]
        [SerializeField] private float speed = 5f;

        void Update()
        {
            float x = 0f;
            float y = 0f;

            if (Input.GetKey(KeyCode.A)) x -= 1f;
            if (Input.GetKey(KeyCode.D)) x += 1f;
            if (Input.GetKey(KeyCode.S)) y -= 1f;
            if (Input.GetKey(KeyCode.W)) y += 1f;

            Vector2 direction = new Vector2(x, y);


            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            var finalSpeed = (Time.deltaTime * speed) * 2.5f;
            
            transform.position += (Vector3)(direction * finalSpeed);
        }
    }
}