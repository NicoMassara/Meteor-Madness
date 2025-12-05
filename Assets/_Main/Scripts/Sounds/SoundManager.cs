using System;
using System.Collections.Generic;
using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MySettings;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using UnityEngine.Audio;

namespace _Main.Scripts.Sounds
{
    public enum SoundChannel
    {
        Sfx,
        Music,
        Collision,
        Deflection,
        UI
    }
    
    public class SoundManager : ManagedBehavior, IUpdatable
    {
        #region Tools
        private class MusicController
        {
            private GeneratedId _playingId;
            private GeneratedId _pausedId;

            public GeneratedId PlayingId => _playingId;
            public GeneratedId PausedId => _pausedId;

            private bool IsSoundValid(GeneratedId soundId)
            {
                return soundId != null && soundId.IsValid;
            }

            public bool IsSoundInvalid(GeneratedId soundId)
            {
                return !IsSoundValid(soundId);
            }

            public bool IsThisPlaying(GeneratedId soundId)
            {
                if (IsSoundValid(soundId) == false) return false;
                
                return _playingId == soundId;
            }

            public bool IsThisPaused(GeneratedId soundId)
            {
                if (IsSoundValid(soundId) == false) return false;
                
                return _pausedId == soundId;
            }

            public bool HasMusicPlaying()
            {
                return IsSoundValid(_playingId) && _playingId.IsValid;
            }

            public bool HasMusicPaused()
            {
                return IsSoundValid(_pausedId) && _pausedId.IsValid;
            }

            #region Music Actions
            
            public void SetNewMusic(GeneratedId musicId)
            {
                if (IsSoundInvalid(musicId))
                {
                    Debug.Log("Couldn't set new music");
                    return;
                }

                _playingId = musicId;
            }

            public void PauseCurrenMusic()
            {
                if (IsSoundInvalid(_playingId))
                {
                    Debug.Log("Couldn't pause music");
                    return;
                }
                
                _pausedId = _playingId;
                _playingId = null;
            }

            public void ResumePausedMusic()
            {
                if (IsSoundInvalid(_pausedId))
                {
                    Debug.Log("Couldn't resume paused music");
                    return;
                }
                
                _playingId = _pausedId;
                _pausedId = null;
            }

            public void ClearPausedMusic()
            {
                _pausedId = null;
            }

            public void ClearMusicPlaying()
            {
                _playingId = null;
            }
            
            #endregion
        }
        private class UIDefaultSounds
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
        private class SoundGenerator
        {
            #region Factory

            private class SoundBehaviourFactory
            {
                private readonly GenericPool<SoundSource> _pool;

                public SoundBehaviourFactory(SoundSource prefab, int defaultSize = 15, int maxSize = 100)
                {
                    _pool = new GenericPool<SoundSource>(prefab, defaultSize, maxSize);
                }
        
                public SoundSource GetSound(ISoundData soundData, Transform parent)
                {
                    var tempSound = _pool.Get();
                    tempSound.SetData(soundData);

                    if (soundData.Is3DSound)
                    {
                        tempSound.SetParent(parent);
                    }
                    
                    tempSound.OnRecycle += OnRecycleHandler;
                    return tempSound;
                }
        
                public void RecycleAll()
                {
                    _pool.RecycleAll();
                }

                private void OnRecycleHandler(SoundSource soundBehavior)
                {
                    soundBehavior.OnRecycle -= OnRecycleHandler;
                    _pool.Release(soundBehavior);
                }
            }

            #endregion
            
            private readonly SoundBehaviourFactory _factory;
            private readonly CustomIdGenerator _idGenerator;
            private readonly Dictionary<GeneratedId, ITrackedAudio> _soundsDic = new Dictionary<GeneratedId, ITrackedAudio>();
            private readonly Dictionary<ITrackedAudio, GeneratedId> _idsDic = new Dictionary<ITrackedAudio, GeneratedId>();
        
            public SoundGenerator(SoundSource prefab)
            {
                _factory = new SoundBehaviourFactory(prefab);
                _idGenerator = new CustomIdGenerator(50);
            }

            public Tuple<GeneratedId,ITrackedAudio> Create(ISoundData soundData, Transform transform)
            {
                var id = _idGenerator.Generate();
                var sound = _factory.GetSound(soundData, transform);

                sound.OnFinished += Return;

                _soundsDic.Add(id,sound);
                _idsDic.Add(sound, id);
            
                return new Tuple<GeneratedId, ITrackedAudio>(id, sound);
            }

            public bool DoesContain(GeneratedId id)
            {
                return _soundsDic.ContainsKey(id);
            }

            public ITrackedAudio Get(GeneratedId id)
            {
                return DoesContain(id) ? _soundsDic[id] : null;
            }

            private void Return(SoundSource sound)
            {
                if(_idsDic.TryGetValue(sound, out var id) == false) return;
            
                sound.OnFinished -= Return;
            
                _soundsDic.Remove(id);
                id.Reset();
                _idsDic.Remove(sound);
            }
        }
        private class AudioPlaybackTracker
        {
            private readonly List<ITrackedAudio> _trackedAudios = new List<ITrackedAudio>();
            private readonly List<ITrackedAudio> _toRemove = new List<ITrackedAudio>();
            private readonly List<ITrackedAudio> _toAdd = new List<ITrackedAudio>();
            
            private Dictionary<SoundChannel, int> _activeByChannel = new();
            public event Action<SoundChannel, int> OnChannelUpdated;

            public void Execute()
            {
                ApplyPending();

                foreach (var item in _trackedAudios)
                {
                    if (item.GetIsPlaying()) continue;
                    if (item.GetIsPaused()) continue;

                    _toRemove.Add(item);
                }
            
                ApplyPending();
            }

            #region Public API

            public void Register(ITrackedAudio trackedAudio, bool isUrgent = false)
            {
                if (isUrgent)
                {
                    var trackChannel = trackedAudio.SoundClass.Channel;
                    
                    // Could check if the Channel is full before Clearing it
                    
                    ClearChannel(trackChannel);
                }

                _toAdd.Add(trackedAudio);
            }

            public void Unregister(ITrackedAudio trackedAudio)
            {
                _toRemove.Add(trackedAudio);
            }

            public bool IsChannelFull(SoundChannel channel)
            {
                if (_activeByChannel.ContainsKey(channel) == false) return false; 
                
                var limit = SoundManagerTools.GetChannelLimit(channel);
                var amount = _activeByChannel[channel];
                
                return amount >= limit;
            }

            public void ClearChannel(SoundChannel channelToClear)
            {
                if (_activeByChannel.ContainsKey(channelToClear) == false)
                {
                    //Debug.LogWarning($"{channelToClear} Channel is empty");
                    return;
                }
                
                foreach (var item in _trackedAudios)
                {
                    if(item.SoundClass.Channel != channelToClear)
                        continue;
                    
                    _toRemove.Add(item);
                }
            }

            #endregion
            
            #region Private API

            private void AddToChannel(SoundChannel channel)
            {
                _activeByChannel ??= new Dictionary<SoundChannel, int>();

                if (_activeByChannel.ContainsKey(channel) == false)
                {
                    _activeByChannel.Add(channel, 0);
                }

                _activeByChannel[channel]++;
                
                OnChannelUpdated?.Invoke(channel, _activeByChannel[channel]);
                
            }

            private void RemoveFromChannel(SoundChannel channel)
            {
                _activeByChannel ??= new Dictionary<SoundChannel, int>();
                
                if(_activeByChannel.ContainsKey(channel) == false) return;

                _activeByChannel[channel]--;
                
                OnChannelUpdated?.Invoke(channel, _activeByChannel[channel]);
            }
            

            private void ApplyPending()
            {
                if (_toAdd.Count > 0)
                {
                    foreach (var item in _toAdd)
                    {
                        //Debug.Log($"Adding: {item.AudioName}");
                        _trackedAudios.Add(item);
                        item.PlayAudio();
                        AddToChannel(item.SoundClass.Channel);
                    }
                
                    _toAdd.Clear();
                }

                if (_toRemove.Count > 0)
                {
                    foreach (var item in _toRemove)
                    {
                        item.TriggerFinish();
                        _trackedAudios.Remove(item);
                        RemoveFromChannel(item.SoundClass.Channel);
                    }
                
                    _toRemove.Clear();
                }
            }

            #endregion
        }
        
        #endregion
        
        [Header("Prefab References")]
        [SerializeField] private SoundSource soundPrefab;
        [Header("Mixer References")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private string mainMixerParameter;
        
        private readonly AudioPlaybackTracker _playbackTracker = new AudioPlaybackTracker();
        private UIDefaultSounds _uiDefaultSounds;
        private SoundGenerator _soundGenerator;
        private MusicController _musicController;
        private bool _hasInitialized;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.QuarterTarget;


        public static SoundManager Instance => _instance;
        protected static SoundManager _instance;
        
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private SoundDebugData _debugData;
        
#endif
        
        private void MakeSingleton()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(_instance);
            }
        }

        protected void Awake()
        {
            MakeSingleton();
            
            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnMainSystemRequestInitialize -= Initialize;
            //
            _uiDefaultSounds = new UIDefaultSounds();

            if (soundPrefab == null)
            {
                Debug.Log("Prefab Reference is null");
            }
            else
            {
                _soundGenerator = new SoundGenerator(soundPrefab);
            }
            
            _musicController = new MusicController();

            SetMainVolume(SettingsManager.Instance.GetMasterVolume());
            SettingsManager.Instance.OnMasterVolumeChanged += Settings_OnMasterVolumeChangedHandler;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            _debugData = new SoundDebugData();
            
            _playbackTracker.OnChannelUpdated += (channel, amount) =>
            {
                _debugData.UpdateChannels(channel, amount);
            };
            
#endif

            BootEvents.MainSystemInitialized();
            _hasInitialized = true;
        }
        
        public void ExecuteUpdate(float deltaTime)
        {
            if(_hasInitialized == false) return;
            
            _playbackTracker.Execute();
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            if (_musicController.HasMusicPlaying())
            {
                var soundId = _musicController.PlayingId;
                
                var hasSound = _soundGenerator.DoesContain(soundId);
                
                _debugData.CurrentMusic = hasSound ? 
                    _soundGenerator.Get(soundId).SoundClass.ClassName : "None";
                
            }
            else
            {
                _debugData.CurrentMusic = "None";
            }
            
            if (_musicController.HasMusicPaused())
            {
                var soundId = _musicController.PausedId;
                
                var hasSound = _soundGenerator.DoesContain(soundId);
                
                _debugData.PausedMusic = hasSound ? 
                    _soundGenerator.Get(soundId).SoundClass.ClassName : "None";
            }
            else
            {
                _debugData.PausedMusic = "None";
            }
#endif
        }

        #region Mixer Actions

        private void SetMainVolume(float volume)
        {
            mainMixer.SetFloat(mainMixerParameter, SoundManagerTools.GetDbFrom01Value(volume));
        }

        #endregion

        #region Sounds

        #region Public

        public GeneratedId PlayMusic(ISoundData soundData, bool isIsolated, GeneratedId soundId)
        {
            // If isolated is TRUE, it'll remove all existing music, even if is paused

            // Music Controller Checks
            
            if (_musicController.IsThisPlaying(soundId))
            {
                Debug.Log("Already Playing This");
                return soundId;
            }
            
            if (_musicController.IsThisPaused(soundId))
            {
                if (_musicController.HasMusicPlaying())
                {
                    StopSound(_musicController.PlayingId);
                    
                    Debug.Log("Music was playing and now has stopped");
                }

                ResumeSound(soundId);
                Debug.Log("This was paused and now is Resume");
                return soundId;
            }
            
            if (_musicController.HasMusicPlaying())
            {
                StopSound(_musicController.PlayingId);
            }
            
            // New TrackedAudio creation and ID generation

            var tempId = PlaySound(soundData, null,isIsolated);

            if (tempId == null)
            {
                return null;
            }
            
            _musicController.SetNewMusic(tempId);
            
            return tempId;
        }

        public void PlayUISound(UISoundType uiSoundType)
        {
            PlaySound(_uiDefaultSounds.GetSound(uiSoundType), null);
        }
        
        public GeneratedId PlaySound(ISoundData soundData, Transform soundParent = null, bool isIsolated = false)
        {
            if (soundData == null)
            {
                //Debug.LogWarning("Sound data is null");
                return null;
            }
            
            if (_playbackTracker.IsChannelFull(soundData.Channel))
            {
                //Debug.Log($"{soundData.Channel} channel is full");
                return null;
            }
            
            var createdItems = _soundGenerator.Create(soundData, soundParent);
            
            _playbackTracker.Register(createdItems.Item2, isIsolated);

            return createdItems.Item1;
        }


        public void StopAllMusic()
        {
            _musicController.ClearMusicPlaying();
            _musicController.ClearPausedMusic();
            _playbackTracker.ClearChannel(SoundChannel.Music);
        }

        public void StopSound(GeneratedId soundId)
        {
            if(IsSoundValid(soundId, out var trackedAudio) == false)
            {
                //Debug.LogWarning("Sound could not be stopped");
                return;
            }
            
            trackedAudio.StopSound();
            
            if (_musicController.IsThisPlaying(soundId))
            {
                _musicController.ClearMusicPlaying();
            }
        }

        public void PauseSound(GeneratedId soundId)
        {
            if (IsSoundValid(soundId, out var trackedAudio) == false)
            {
                //Debug.LogWarning("Sound could not be paused");
                return;
            }

            trackedAudio.PauseSound();

            if (_musicController.IsThisPlaying(soundId))
            {
                _musicController.PauseCurrenMusic();
            }
        }
        
        public void ResumeSound(GeneratedId soundId)
        {
            if(IsSoundValid(soundId, out var trackedAudio) == false)
            {
                //Debug.LogWarning("Sound could not be resumed");
                return;
            }
            
            trackedAudio.ResumeSound();
            
            if (_musicController.IsThisPaused(soundId))
            {
                _musicController.ResumePausedMusic();
            }
        }
        
        #endregion

        #region Private

        private bool IsSoundValid(GeneratedId soundId, out ITrackedAudio trackedAudio)
        {
            trackedAudio = null;
            
            if(soundId == null) return false;
            if(soundId.IsValid == false) return false;
            if (_soundGenerator.DoesContain(soundId) == false) return false;

            trackedAudio = _soundGenerator.Get(soundId);
            
            return true;
        }

        #endregion
        
        #endregion
        
        #region Handlers
        
        private void Settings_OnMasterVolumeChangedHandler(float volume)
        {
            SetMainVolume(volume);
        }


        #endregion
    }

    #region Extra Clases
    



    
    #endregion
}