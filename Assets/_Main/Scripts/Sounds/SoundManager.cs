using System;
using System.Collections.Generic;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MySettings;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using UnityEngine.Audio;

namespace _Main.Scripts.Sounds
{
    public class SoundManager : SingletonManagedBehaviour<SoundManager>, IUpdatable
    {
        [Header("Prefab References")]
        [SerializeField] private SoundComponent soundPrefab;
        [Header("Mixer References")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private string mainMixerParameter;
        
        private readonly AudioPlaybackTracker _playbackTracker = new AudioPlaybackTracker();
        private readonly MusicController _musicController = new MusicController();
        private SoundBehaviourFactory _factory;
        private UIDefaultSounds _uiDefaultSounds;
        private SoundIdStorage _idStorage;

        private readonly Dictionary<SoundChannel, List<SoundComponent>> _activeByChannel = new()
        {
            { SoundChannel.Sfx, new List<SoundComponent>() },
            { SoundChannel.Collision, new List<SoundComponent>()},
            { SoundChannel.Deflection, new List<SoundComponent>()},
            { SoundChannel.UI, new List<SoundComponent>()},
            { SoundChannel.Music, new List<SoundComponent>()},
        };

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.QuarterTarget;
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private SoundDebugData _debugData;
        
#endif

        private void Start()
        {
            _factory = new SoundBehaviourFactory(soundPrefab);
            _uiDefaultSounds = new UIDefaultSounds();
            _idStorage = new SoundIdStorage();

            SetMainVolume(SettingsManager.Instance.GetMasterVolume());
            SettingsManager.Instance.OnMasterVolumeChanged += Settings_OnMasterVolumeChangedHandler;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData = new SoundDebugData();
            
            _musicController.OnMusicPlaying += (musicName) =>
            {
                if (_idStorage.TryGetSound(musicName, out var sound))
                {
                    _debugData.CurrentMusic = sound.SoundClass.ClassName;
                }
            };
            
            _musicController.OnMusicPaused += (musicName) =>
            {
                if (_idStorage.TryGetSound(musicName, out var sound))
                {
                    _debugData.PausedMusic = sound.SoundClass.ClassName;
                }
            };
            
            _musicController.OnMusicStopped += () =>
            {
                _debugData.CurrentMusic = "None";
            };
            
            _musicController.OnMusicResumed += (id) =>
            {
                _debugData.PausedMusic = "None";
                
                if (_idStorage.TryGetSound(id, out var sound))
                {
                    _debugData.CurrentMusic = sound.SoundClass.ClassName;
                }
            };
#endif
        }
        
        public void ExecuteUpdate(float deltaTime)
        {
            _playbackTracker.Execute();
        }

        #region Mixer Actions

        private void SetMainVolume(float volume)
        {
            mainMixer.SetFloat(mainMixerParameter, SoundManagerTools.GetDbFrom01Value(volume));
        }

        #endregion
        
        #region Sounds

        public void PlayUISound(UISoundType uiSoundType)
        {
            PlaySound(_uiDefaultSounds.GetSound(uiSoundType), null);
        }

        #region Music

        public SoundId PlayMusic(ISoundData soundData, SoundId soundId)
        {
            if (soundData == null)
            {
                Debug.Log("Music is null");
                return soundId;
            }

            if (_musicController.IsThisPlaying(soundId))
            {
                return soundId;
            }

            if (_musicController.HasMusicPlaying())
            {
                StopMusic();
            }

            var gottenId = PlaySound(soundData, null);
            
            if (gottenId == null)
            {
                gottenId = PlaySound(soundData, null);

                if (gottenId == null)
                {
                    Debug.Log("Music playing failed");
                    return soundId;
                }
            }

            _musicController?.Play(gottenId.Id);
            
            return gottenId;
            
        }
        
        public ulong StopMusic()
        {
            var gottenId = _musicController.Stop();

            if (_idStorage.TryGetSound(gottenId, out var sound))
            {
                _playbackTracker.Unregister(sound);
                _idStorage.Unregister(gottenId);
            }
            else
            {
                //Debug.LogWarning("Sound to [STOP] could not be found");
            }
            
            return gottenId;
        }

        public void PauseMusic()
        {
            var gottenId = _musicController.Pause();

            if (_idStorage.TryGetSound(gottenId, out var sound))
            {
                sound.PauseSound();
            }
            else
            {
                //Debug.LogWarning("Sound to [PAUSE] could not be found");
            }
        }

        public void ResumeMusic()
        {
            if (_musicController.HasMusicPlaying())
            {
                StopMusic();
            }
            
            var gottenId = _musicController.Resume();
            
            if (_idStorage.TryGetSound(gottenId, out var sound))
            {
                sound.ResumeSound();
            }
            else
            {
                //Debug.LogWarning("Sound to [RESUME] could not be found");
            }
        }

        #endregion
        
        public SoundId PlaySound(ISoundData soundData, Transform soundParent)
        {
            if (soundData == null)
            {
                //Debug.Log("Sound Data is NULL");
                return null;
            }

            if (GetIsChannelFull(soundData.Channel))
            {
                //Debug.Log($"{soundData.Channel} channel is full");
                return null;
            }
            
            var tempSound = _factory.GetSound();
            var soundId = _idStorage.RegisterSound(tempSound);

            if (soundId == null)
            {
                Debug.LogWarning($"Sound Id for {soundData.ClassName} could not be registered");
            }

            tempSound.SetData(soundData);

            if (soundData.Is3DSound)
            {
                tempSound.SetParent(soundParent);
            }
            
            _playbackTracker.Register(tempSound);
            
            AddToChannel(tempSound,soundData.Channel);
            
            tempSound.OnFinished += Sound_OnFinishedHandler;
            
            return soundId;
        }

        public void StopSound(ref SoundId soundId)
        {
            if(soundId == null) return;
            
            if (_idStorage.TryGetSound(soundId.Id, out var soundComponent))
            {
                _playbackTracker.Unregister(soundComponent);
                _idStorage.Unregister(soundId.Id);
                soundId.Reset();
            }
        }

        #endregion

        #region Channel Setters

        private void AddToChannel(SoundComponent soundData, SoundChannel channel)
        {
            _activeByChannel[channel].Add(soundData);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            _debugData.ChannelCount[channel] = _activeByChannel[channel].Count;
            
#endif
        }

        private void RemoveFromChannel(SoundComponent soundData, SoundChannel channel)
        {
            if (_activeByChannel[channel].Contains(soundData))
            {
                _activeByChannel[channel].Remove(soundData);
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
                _debugData.ChannelCount[channel] = _activeByChannel[channel].Count;
#endif
            }
        }

        #endregion

        #region Channel Getters

        private bool GetIsChannelFull(SoundChannel channel)
        {
            return _activeByChannel[channel].Count >= SoundManagerTools.GetChannelLimit(channel);;
        }

        #endregion
        
        #region Handlers

        private void Sound_OnFinishedHandler(SoundComponent soundBehavior)
        {
            RemoveFromChannel(soundBehavior, soundBehavior.SoundClass.Channel);
            
            soundBehavior.OnFinished -= Sound_OnFinishedHandler;
        }
        
        private void Settings_OnMasterVolumeChangedHandler(float volume)
        {
            SetMainVolume(volume);
        }


        #endregion
    }

    #region Extra Clases

    public class SoundId
    {
        public ulong Id { get; private set; }
        public bool IsActive => Id > 0;

        public SoundId(ulong id)
        {
            this.Id = id;
        }

        public void Reset()
        {
            Id = 0;
        }
    }

    public class SoundIdStorage
    {
        private readonly Dictionary<ulong, SoundComponent> _soundByIdDic = new Dictionary<ulong, SoundComponent>();
        private readonly RandomIdGenerator _idGenerator = new RandomIdGenerator();

        public SoundIdStorage()
        {
            
        }

        public bool TryGetSound(ulong id, out SoundComponent soundComponent)
        {
            return _soundByIdDic.TryGetValue(id, out soundComponent);
        }

        public bool HasSound(ulong id)
        {
            return _soundByIdDic.ContainsKey(id);
        }

        public SoundId RegisterSound(SoundComponent soundComponent)
        {
            var id = _idGenerator.Generate();

            soundComponent.OnFinished += (soundComponent) =>
            {
                Unregister(id);
            };
            
            _soundByIdDic.Add(id, soundComponent);
            
            return new SoundId(id);
        }
        
        public void Unregister(ulong id)
        {
            if (_soundByIdDic.ContainsKey(id))
            {
                _soundByIdDic.Remove(id);
                _idGenerator.Release(id);
            }
        }
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
                
                //Debug.Log($"{soundType} UI Sound loaded from Resources/{Path}");
#endif
                
                _uiSounds.Add(soundType, loaded[i]);
            }
        }

        public ISoundData GetSound(UISoundType uiSoundType)
        {
            return _uiSounds[uiSoundType];
        }

    }
    
    #endregion
}