using System;
using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class SoundManager : ManagedBehavior, IUpdatable
    {
        [Header("Prefab References")]
        [SerializeField] private SoundBehavior soundPrefab;
        [Header("Music Data")]
        [SerializeField] private SoundClassSo menuMusic;
        [SerializeField] private SoundClassSo gameplayMusic;
        [SerializeField] private SoundClassSo defeatMusic;
        
        private SoundBehaviourFactory _factory;
        private AudioPlaybackTracker _playbackTracker = new AudioPlaybackTracker();
        private MusicController _musicController = new MusicController();

        private readonly Dictionary<SoundChannel, int> _channelLimits = new()
        {
            { SoundChannel.Sfx, 5},
            { SoundChannel.Collision, 3},
            { SoundChannel.Deflection, 3},
            { SoundChannel.UI, 3},
        };
        
        private readonly Dictionary<SoundChannel, List<SoundBehavior>> _activeByChannel = new()
        {
            { SoundChannel.Sfx, new List<SoundBehavior>() },
            { SoundChannel.Collision, new List<SoundBehavior>()},
            { SoundChannel.Deflection, new List<SoundBehavior>()},
            { SoundChannel.UI, new List<SoundBehavior>()},
        };

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        
        private void Awake()
        {
            _factory = new SoundBehaviourFactory(soundPrefab);
            
            AddMusic(MusicType.MainMenu, menuMusic);
            AddMusic(MusicType.Gameplay, gameplayMusic);
            AddMusic(MusicType.EndGame, defeatMusic);
            
            SetEventBus();
        }
        
        public void ManagedUpdate()
        {
            _playbackTracker.Execute();
        }

        private void AddMusic(MusicType type, ISoundData soundData)
        {
            if (soundData == null)
            {
                Debug.LogWarning($"{type} Music SoundData is not assigned");
                return;
            }

            var tempSound = _factory.GetSound();
            tempSound.SetData(soundData);
            _musicController.AddMusic(type, tempSound);
        }

        private void SpawnSound(ISoundData soundData, Transform soundParent, ILoopableSound loopableSound)
        {
            if (soundData == null)
            {
                Debug.Log("Sound Data is NULL");
                return;
            }

            if (GetIsChannelFull(soundData.Channel))
            {
                Debug.Log($"{soundData.Channel} channel is full");
                return;
            }
            
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
                tempSound.PlayAudio();
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
            GameEventCaller.Subscribe<SoundEvents.PlaySound>(EventBus_Sounds_PlaySound);
            GameEventCaller.Subscribe<SoundEvents.PlayMusic>(EventBus_Sounds_PlayMusic);
            GameEventCaller.Subscribe<SoundEvents.StopMusic>(EventBus_Sounds_StopMusic);
            GameEventCaller.Subscribe<SoundEvents.SetMusicLevel>(EventBus_Sounds_SetMusicLevel);
        }

        private void EventBus_Sounds_PlaySound(SoundEvents.PlaySound input)
        {
            SpawnSound(input.Data, input.SoundParent, input.LoopableSound);
        }

        private void EventBus_Sounds_PlayMusic(SoundEvents.PlayMusic input)
        {
            _musicController.PlayMusic(input.Type);
        }

        private void EventBus_Sounds_StopMusic(SoundEvents.StopMusic input)
        {
            _musicController.StopCurrentMusic();
        }

        private void EventBus_Sounds_SetMusicLevel(SoundEvents.SetMusicLevel input)
        {
            _musicController.SetMusicVolume(input.Volume);
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