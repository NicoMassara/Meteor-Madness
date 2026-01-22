using UnityEngine;

namespace _Main.Scripts.Inputs
{
    public class InputHelper
    {
        public static float GetAngleFromDirection(Vector2 direction, float offset = 0)
        {
            // Force Normalize in Direction
            
            if(direction == Vector2.zero)
                return 0f;
            
            direction.Normalize();
            float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            angle += offset;
            return (angle + 360) % 360;
        }
    }
}