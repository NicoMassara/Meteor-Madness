using System;
using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class SoundManager : ManagedBehavior, IUpdatable
    {
        [SerializeField] private SoundBehavior soundPrefab;
        private SoundBehaviourFactory _factory;
        private AudioPlaybackTracker _playbackTracker = new AudioPlaybackTracker();
        private MusicController _musicController = new MusicController();

        private readonly Dictionary<SoundChannel, int> _channelLimits = new()
        {
            { SoundChannel.Sfx, 5},
            { SoundChannel.Collision, 3},
            { SoundChannel.Deflection, 3},
        };
        
        private readonly Dictionary<SoundChannel, List<SoundBehavior>> _activeByChannel = new()
        {
            { SoundChannel.Sfx, new List<SoundBehavior>() },
            { SoundChannel.Collision, new List<SoundBehavior>()},
            { SoundChannel.Deflection, new List<SoundBehavior>()},
        };

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        
        private void Awake()
        {
            _factory = new SoundBehaviourFactory(soundPrefab);
            
            SetEventBus();
        }
        
        public void ManagedUpdate()
        {
            _playbackTracker.Execute();
        }


        private void Play2dSound(ISoundData soundData, ILoopableSound loopableSound)
        {
            if (GetIsChannelFull(soundData.Channel))
            {
                return;
            }

            SpawnSound(soundData, null, loopableSound);
        }

        private void Play3dSound(ISoundData soundData, Transform soundParent, ILoopableSound loopableSound)
        {
            if (GetIsChannelFull(soundData.Channel))
            {
                return;
            }
            
            SpawnSound(soundData, soundParent, loopableSound);
        }

        private void AddMusic(MusicType type, ISoundData soundData)
        {
            var tempSound = _factory.GetSound();
            tempSound.SetData(soundData);
            _musicController.AddMusic(type, tempSound);
        }

        private void SpawnSound(ISoundData soundData, Transform soundParent, ILoopableSound loopableSound)
        {
            var tempSound = _factory.GetSound();
            tempSound.SetData(soundData);

            if (soundData.Is3DSound)
            {
                tempSound.SetParent(soundParent);
            }
            
            if (soundData.DoesLoop == false)
            {
                _playbackTracker.Register(tempSound);
            }
            else
            {
                loopableSound.OnLoopFinished += tempSound.TriggerFinish;
            }
            _activeByChannel[soundData.Channel].Add(tempSound);
            
            tempSound.OnFinished += Sound_OnFinishedHandler;
        }
        private int GetChannelLimit(SoundChannel channel)
        {
            return _channelLimits[channel];
        }

        private bool GetIsChannelFull(SoundChannel channel)
        {
            return _activeByChannel[channel].Count >= GetChannelLimit(channel);
        }


        #region Event Bus

        private void SetEventBus()
        {
            //Add Event Manager Events and Set Them
        }

        #endregion

        #region Handlers

        private void Sound_OnFinishedHandler(SoundBehavior soundBehavior)
        {
            if (_activeByChannel[soundBehavior.SoundClass.Channel].Contains(soundBehavior))
            {
                _activeByChannel[soundBehavior.SoundClass.Channel].Remove(soundBehavior);
            }
        }


        #endregion
    }
}