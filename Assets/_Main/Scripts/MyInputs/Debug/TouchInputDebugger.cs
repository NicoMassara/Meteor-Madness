using _Main.Scripts.GameConfig;
using UnityEngine;

namespace _Main.Scripts.MyInputs
{
    public class TouchInputDebugger : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            if(GameConfigManager.Instance == null) return;
            
            Gizmos.color = Color.red;
            
            var zoneBottom = GameConfigManager.Instance.GetGameplayData().TouchInputData.GetBottomBound();
            var zoneTop = GameConfigManager.Instance.GetGameplayData().TouchInputData.GetTopBound();
            var cameraZ = 10f;
            
            // [ LEFT ] 
            
            var leftZoneLeft = GameConfigManager.Instance.GetGameplayData().TouchInputData.GetLeftZoneBounds().x;
            var leftZoneRight = GameConfigManager.Instance.GetGameplayData().TouchInputData.GetLeftZoneBounds().y;
            
            // Middle
            
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f,Screen.height,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f,0,cameraZ)));
            
            // Upper 
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(leftZoneLeft,zoneTop,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(leftZoneRight,zoneTop,cameraZ)));
            // Lower 
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(leftZoneLeft,zoneBottom,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(leftZoneRight,zoneBottom,cameraZ)));
            // Left 
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(leftZoneLeft,zoneTop,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(leftZoneLeft,zoneBottom,cameraZ)));
            // Right
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(leftZoneRight,zoneTop,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(leftZoneRight,zoneBottom,cameraZ)));
            
            // [ RIGHT ]
            
            var rightZoneLeft = GameConfigManager.Instance.GetGameplayData().TouchInputData.GetRightZoneBounds().x;
            var rightZoneRight = GameConfigManager.Instance.GetGameplayData().TouchInputData.GetRightZoneBounds().y;
            
            // Upper 
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(rightZoneLeft,zoneTop,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(rightZoneRight,zoneTop,cameraZ)));
            // Lower 
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(rightZoneLeft,zoneBottom,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(rightZoneRight,zoneBottom,cameraZ)));
            // Left 
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(rightZoneLeft,zoneTop,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(rightZoneLeft,zoneBottom,cameraZ)));
            // Right
            Gizmos.DrawLine( 
                Camera.current.ScreenToWorldPoint(new Vector3(rightZoneRight,zoneTop,cameraZ)),
                Camera.current.ScreenToWorldPoint(new Vector3(rightZoneRight,zoneBottom,cameraZ)));
        }
    }
}