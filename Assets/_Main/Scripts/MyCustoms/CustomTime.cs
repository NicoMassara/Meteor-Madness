using System.Collections.Generic;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.MyCustoms
{
    public static class CustomTime
    {
        private static readonly Dictionary<UpdateGroup, TimeChannel> Channels = new();
        
        public static TimeChannel GetChannel(UpdateGroup key)
        {
            if (!Channels.ContainsKey(key))
                Channels[key] = new TimeChannel();
            return Channels[key];
        }

        internal static void UpdateAll(float unscaledDeltaTime)
        {
            foreach (var kv in Channels)
                kv.Value.Update(unscaledDeltaTime);
        }
        
        internal static void FixedUpdateAll(float unscaledDeltaTime)
        {
            foreach (var kv in Channels)
                kv.Value.UpdateFixed(unscaledDeltaTime);
        }
    }

    public class TimeChannel
    {
        public float DeltaTime { get; private set; }
        public float FixedDeltaTime { get; private set; }
        public float TimeScale = 1f;
        public bool IsPaused = false;
        
        public void SetTimeScale(float scale) => TimeScale = Mathf.Max(0f, scale);
        public void SetPaused(bool value) => IsPaused = value;

        public void Update(float unscaledDeltaTime)
        {
            DeltaTime = IsPaused ? 0f : unscaledDeltaTime * TimeScale;
        }
        
        public void UpdateFixed(float fixedUnscaledDeltaTime)
        {
            FixedDeltaTime = IsPaused ? 0f : fixedUnscaledDeltaTime * TimeScale;
        }
    }
}