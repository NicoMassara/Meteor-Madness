using System;
using UnityEngine;

namespace _Main.Scripts
{
    public class PanelDeleter : MonoBehaviour
    {
        public GameObject[] mobileCanvas;
        public GameObject[] desktopCanvas;
        
        private void Awake()
        {
            DeletePanels();
            
            Destroy(this,3f);
        }

        protected bool GetIsMobile()
        {
#if UNITY_EDITOR
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else

            return false;
#endif
#else
            return SystemInfo.deviceType == DeviceType.Handheld;
#endif
        }
        
        public void DeletePanels()
        {
            var isMobile = GetIsMobile();
            string panelType = isMobile ? "Mobile" : "Desktop";
            if (isMobile)
            {
                if(desktopCanvas != null)
                    foreach (var panel in desktopCanvas)
                    {
                        Destroy(panel.gameObject);
                    }
            }
            else
            {
                if(mobileCanvas != null)
                    foreach (var panel in mobileCanvas)
                    {
                        Destroy(panel.gameObject);
                    }
                
            }

            mobileCanvas = null;
            desktopCanvas = null;

            //Debug.Log($"Device:{panelType}, non device panels deleted");
        }
    }
}