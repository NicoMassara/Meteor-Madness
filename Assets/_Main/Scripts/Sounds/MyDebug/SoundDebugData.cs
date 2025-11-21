using System;
using System.Collections.Generic;
using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Sounds
{
    
#if UNITY_EDITOR || DEVELOPMENT_BUILD

    public class SoundDebugData
    {
        private readonly Dictionary<SoundChannel, int> _channelCount = new Dictionary<SoundChannel, int>();
        public string CurrentMusic { get; set; } = "None";
        public string PausedMusic { get; set; } = "None";
        
        public SoundDebugData()
        {
            var soundGroup = DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Sounds, DebugGUISortingOrder.Group.Sounds);

            if (soundGroup != null)
            {
                soundGroup.CreateSubGroup(DebugGUIKeys.SubGroup.Music, DebugGUISortingOrder.SubGroup.Music)
                    ?.AddEntry(
                        () => $"Current: {CurrentMusic}",
                        () => $"Paused: {PausedMusic}"
                    );
                
                soundGroup.CreateSubGroup(DebugGUIKeys.SubGroup.SoundChannel, DebugGUISortingOrder.SubGroup.SoundChannel)
                    ?.AddEntry(
                        ()=> $"Sfx: {(_channelCount.GetValueOrDefault(SoundChannel.Sfx, 0))}",
                        ()=> $"Collision: {(_channelCount.GetValueOrDefault(SoundChannel.Collision, 0))}",
                        ()=> $"Deflection: {(_channelCount.GetValueOrDefault(SoundChannel.Deflection, 0))}",
                        ()=> $"UI: {(_channelCount.GetValueOrDefault(SoundChannel.UI, 0))}",
                        ()=> $"Music: {(_channelCount.GetValueOrDefault(SoundChannel.Music, 0))}"
                    );
            }
        }

        public void UpdateChannels(SoundChannel channel, int count)
        {
            _channelCount[channel] = count;
        }

    }    

#endif
}