using System;
using UnityEngine;

namespace _Main.Scripts.Managers.UpdateManager
{
    public class UpdateManagerTools
    {
        public static float GetTickByGroup(TickGroup group, float frameTime)
        {
            float targetFPS = Application.targetFrameRate > 0 ? Application.targetFrameRate : (1f / frameTime);
            float baseFrameTime = 1f / targetFPS;
            
            float adaptiveFrameTime = Mathf.Lerp(baseFrameTime, frameTime, 0.2f);
            
            var tickValue = group switch
            {
                TickGroup.FullTick => adaptiveFrameTime,
                TickGroup.HalfTick => adaptiveFrameTime/2,
                TickGroup.QuarterTick => adaptiveFrameTime/4,
                TickGroup.SecondTick => 1f,
                _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
            };

            return tickValue;
        }
    }
}