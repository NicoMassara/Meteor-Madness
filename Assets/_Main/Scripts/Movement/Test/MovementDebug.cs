using _Main.Scripts.TestTools;
using UnityEngine;

namespace _Main.Scripts.Movement.Test
{
    public class MovementDebug : GUIDebug<IDebugMovement>
    {
        private string _lastState;

        protected override int DebugLines => 10;

        protected override DebugGUIData[] GetDebugData()
        {
            var component = ComponentToDebug.DebugComponent;

            DebugDataArray[0]
                .SetText($"C Speed: {component.CurrentSpeed:F2} - (R:{component.SpeedRatio:F2})")
                .SetColor(Color.white);
            DebugDataArray[1]
                .SetText($"Speed Mag: {component.CurveMagnitude:F2}")
                .SetColor(Color.white);
            DebugDataArray[2]
                .SetText($"C Angle: {component.CurrentAngle:F2}")
                .SetColor(Color.white);
            DebugDataArray[3]
                .SetText($"C Dir: {component.CurrentDirection:F0}")
                .SetColor(Color.white);
            // === Target Angle Data === //
            DebugDataArray[4]
                .SetText($"T Angle : {component.TargetAngle}")
                .SetColor(Color.yellow)
                .SetShowCondition(component.HasTargetAngle);
            DebugDataArray[5]
                .SetText($"T Distance: {component.DistanceToTarget:F1} - (R:{component.DistanceToTargetRatio:F2})")
                .SetColor(Color.yellow)
                .SetShowCondition(component.HasTargetAngle);
            // === State === //
            DebugDataArray[6]
                .SetText($"C State: {component.CurrentState}")
                .SetColor(Color.cyan);
            DebugDataArray[7]
                .SetText($"L State: {component.LastState}")
                .SetColor(Color.cyan);
            // === Input === //
            DebugDataArray[8]
                .SetText($"Has Input: {component.HasInput}")
                .SetColor(Color.red);
            
            return DebugDataArray;
        }
        protected override Vector3 GetGUIPosition()
        {
            return MainCamera.WorldToScreenPoint(ComponentToDebug.DebugPosition);
        }

        protected override void DrawDebugGizmos()
        {
            var component = ComponentToDebug.DebugComponent;
            
            if(component.HasTargetAngle == false) return;
            
            var debugPos = (Vector2)ComponentToDebug.DebugPosition;
            var angleDistance = 3;
            var targetAnglePos = PositionFromAngle(debugPos, component.TargetAngle, angleDistance);
            const float angleRadius = 0.15f;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetAnglePos, angleRadius);
        }
        
        private Vector2 PositionFromAngle(Vector2 pos, float angle, float distance)
        {
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            return pos + dir * (distance) * -1;
        }
    }
}