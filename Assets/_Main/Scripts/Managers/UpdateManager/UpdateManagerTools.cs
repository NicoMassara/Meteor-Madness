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
                TickGroup.EveryFrame => adaptiveFrameTime,
                TickGroup.HalfTarget => adaptiveFrameTime * 2,
                TickGroup.QuarterTarget => adaptiveFrameTime *4,
                TickGroup.EightTarget => adaptiveFrameTime * 8,
                TickGroup.EverySecond => 1f,
                _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
            };

            return tickValue;
        }
    }
}