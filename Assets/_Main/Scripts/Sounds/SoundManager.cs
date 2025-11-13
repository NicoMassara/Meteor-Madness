using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class SoundManager : SingletonManagedBehaviour<SoundManager>, IUpdatable
    {
        [Header("Prefab References")]
        [SerializeField] private SoundComponent soundPrefab;
        
        private SoundBehaviourFactory _factory;
        private readonly AudioPlaybackTracker _playbackTracker = new AudioPlaybackTracker();
        private readonly MusicController _musicController = new MusicController();
        private Dictionary<ulong, SoundComponent> _soundIdDic = new Dictionary<ulong, SoundComponent>();
        private UIDefaultSounds _uiDefaultSounds;
        
        private readonly Dictionary<SoundChannel, List<SoundComponent>> _activeByChannel = new()
        {
            { SoundChannel.Sfx, new List<SoundComponent>() },
            { SoundChannel.Collision, new List<SoundComponent>()},
            { SoundChannel.Deflection, new List<SoundComponent>()},
            { SoundChannel.UI, new List<SoundComponent>()},
        };

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.QuarterTick;
        public float LastUpdateTime { get; set; }
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private SoundDebugData _debugData;
        
#endif

        private void Start()
        {
            _factory = new SoundBehaviourFactory(soundPrefab);
            _uiDefaultSounds = new UIDefaultSounds();
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData = new SoundDebugData();
            _musicController.OnMusicPlaying += (musicName) =>
            {
                _debugData.CurrentMusic = musicName;
            };
            _musicController.OnMusicArriving += (musicName) =>
            {
                _debugData.ArrivingMusic = musicName;
            };
            _musicController.OnMusicLeaving += (musicName) =>
            {
                _debugData.LeavingMusic = musicName;
            };
#endif
        }
        
        public void ExecuteUpdate()
        {
            _playbackTracker.Execute();
            _musicController.Execute(CustomTime.GetDeltaTimeByChannel(SelfUpdateGroup));
        }
        
        #region Sounds

        public void PlayUISound(UISoundType uiSoundType)
        {
            PlaySound(_uiDefaultSounds.GetSound(uiSoundType), null);
        }

        public void PlayMusic(ISoundData soundData)
        {
            if (soundData == null)
            {
                return;
            }
            
            var tempSound = _factory.GetSound();
            tempSound.SetData(soundData);
            _musicController.PlayMusic(tempSound);
        }

        public void StopMusic()
        {
            _musicController.StopMusic();
        }

        public ulong PlaySound(ISoundData soundData, Transform soundParent)
        {
            if (soundData == null)
            {
                //Debug.Log("Sound Data is NULL");
                return 0;
            }

            if (GetIsChannelFull(soundData.Channel))
            {
                //Debug.Log($"{soundData.Channel} channel is full");
                return 0;
            }
            
            var tempSound = _factory.GetSound(out var soundId);
            tempSound.SetData(soundData);

            if (soundData.Is3DSound)
            {
                tempSound.SetParent(soundParent);
            }
            
            _playbackTracker.Register(tempSound);
            
            _activeByChannel[soundData.Channel].Add(tempSound);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.ChannelCount[soundData.Channel] = _activeByChannel[soundData.Channel].Count;
            
#endif
            
            tempSound.OnFinished += Sound_OnFinishedHandler;
            
            return soundId;
        }

        public void StopSound(ulong soundId)
        {
            if (_soundIdDic.ContainsKey(soundId))
            {
                var soundComponent = _soundIdDic[soundId];
                soundComponent.OnFinished -= Sound_OnFinishedHandler;
                _soundIdDic.Remove(soundId);
            }
        }

        #endregion

        #region Channel Getters

        private int GetChannelLimit(SoundChannel channel)
        {
            return SoundManagerTools.GetChannelLimit(channel);
        }

        private bool GetIsChannelFull(SoundChannel channel)
        {
            return _activeByChannel[channel].Count >= GetChannelLimit(channel);
        }

        #endregion
        
        #region Handlers

        private void Sound_OnFinishedHandler(SoundComponent soundBehavior)
        {
            if (_activeByChannel[soundBehavior.SoundClass.Channel].Contains(soundBehavior))
            {
                _activeByChannel[soundBehavior.SoundClass.Channel].Remove(soundBehavior);
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
                _debugData.ChannelCount[soundBehavior.SoundClass.Channel] = _activeByChannel[soundBehavior.SoundClass.Channel].Count;
            
#endif
            }
        }


        #endregion
    }

    public class UIDefaultSounds
    {
        private readonly Dictionary<UISoundType, ISoundData> _uiSounds = new Dictionary<UISoundType, ISoundData>();
        private const string Path = "ScriptableObjects/DefaultUISounds";
        
        public UIDefaultSounds()
        {
            Initialize();
        }

        private void Initialize()
        {
            var loaded = Resources.LoadAll<UiSoundClassSo>(Path);

            for (int i = 0; i < loaded.Length; i++)
            {
                var soundType = loaded[i].UISoundType;

                if (_uiSounds.ContainsKey(soundType))
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                
                    Debug.LogWarning($"{soundType} UI Sound duplicated found in Resources/{Path} was not loaded, check the UISoundType and changed to load it!");
#endif
                    continue;
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                
                Debug.Log($"{soundType} UI Sound loaded from Resources/{Path}");
#endif
                
                _uiSounds.Add(soundType, loaded[i]);
            }
        }

        public ISoundData GetSound(UISoundType uiSoundType)
        {
            return _uiSounds[uiSoundType];
        }

    }
}