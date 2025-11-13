using System;
using System.Collections.Generic;
using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Sounds
{
    
#if UNITY_EDITOR || DEVELOPMENT_BUILD

    public class SoundDebugData
    {
        public Dictionary<SoundChannel, int> ChannelCount = new Dictionary<SoundChannel, int>();
        public string CurrentMusic = "None";
        public string ArrivingMusic = "None";
        public string LeavingMusic = "None";
        
        public SoundDebugData()
        {
            var soundGroup = DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Sounds, DebugGUISortingOrder.Group.Sounds);

            if (soundGroup != null)
            {
                soundGroup.CreateSubGroup(DebugGUIKeys.SubGroup.Music, DebugGUISortingOrder.SubGroup.Music)
                    ?.AddEntry(
                        ()=> $"CurrentMusic: {CurrentMusic}",
                        ()=> $"ArrivingMusic: {ArrivingMusic}",
                        ()=> $"LeavingMusic: {LeavingMusic}"
                    );
                
                soundGroup.CreateSubGroup(DebugGUIKeys.SubGroup.SoundChannel, DebugGUISortingOrder.SubGroup.SoundChannel)
                    ?.AddEntry(
                        ()=> $"Sfx: {(ChannelCount.GetValueOrDefault(SoundChannel.Sfx, 0))}",
                        ()=> $"Collision: {(ChannelCount.GetValueOrDefault(SoundChannel.Collision, 0))}",
                        ()=> $"Deflection: {(ChannelCount.GetValueOrDefault(SoundChannel.Deflection, 0))}",
                        ()=> $"UI: {(ChannelCount.GetValueOrDefault(SoundChannel.UI, 0))}"
                    );
            }
        }
        
    }    

#endif
}