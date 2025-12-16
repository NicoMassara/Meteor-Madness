using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Plugins.NicolasMassara.CustomSoundManager
{
    public class SoundManager : MonoBehaviour
    {
        #region Private Classes

        #region Trackers
        
        private class AudioTracker
        {
            private readonly List<ISoundSource> _playing = new List<ISoundSource>();
            private readonly HashSet<ISoundSource> _toRemove = new HashSet<ISoundSource>();
            private readonly HashSet<ISoundSource> _toAdd = new HashSet<ISoundSource>();
            private readonly HashSet<ISoundSource> _toCancel = new HashSet<ISoundSource>();
            
            private Dictionary<SoundChannel, int> _activeByChannel = new();
            public event Action<SoundChannel, int> OnChannelUpdated;

            public AudioTracker()
            {
                for (int i = 1; i < (int)SoundChannel.UI+1; i++)
                {
                    _activeByChannel.Add((SoundChannel)i,0);
                }
            }

            public void Execute()
            {
                ApplyPending();
                UpdateLogic();
            }
            
            private void UpdateLogic()
            {
                foreach (var item in _playing)
                {
                    if (item.DoesLoop)
                    {
                        if (item.GetShouldFadeOut())
                        {
                            item.FinishLoop();
                        }
                        else if (item.GetPlayRatio() >= 0.99f)
                        {
                            item.FinishLoop();
                        }
                    }
                    else
                    {
                        if (item.GetShouldFadeOut())
                        {
                            item.Stop();
                        }
                        else if (item.GetPlayRatio() >= 0.99f)
                        {
                            item.Stop();
                        }
                    }
                }
            }

            #region Public API

            public void RegisterSound(ISoundSource soundSource, bool isIsolated = false)
            {
                if (isIsolated)
                {
                    var selectedChannel = soundSource.Channel;
                    ClearChannel(selectedChannel);
                }
                
                _toAdd.Add(soundSource);
            }
            
            public void Unregister(ISoundSource trackedAudio)
            { 
                _toRemove.Add(trackedAudio);
            }

            public void ClearAllChannels()
            {
                for (int i = 0; i < (int)SoundChannel.UI; i++)
                {
                    ClearChannel((SoundChannel)i);
                }
            }

            public void ClearChannel(SoundChannel channelToClear)
            {
                if (_activeByChannel.ContainsKey(channelToClear) == false)
                {
                    SoundTools.DebugSound($"{channelToClear} Channel is empty");
                    return;
                }
                    
                foreach (var item in _playing)
                {
                    if(item.Channel != channelToClear)
                        continue;

                    Unregister(item);
                }
                
                SoundTools.DebugSound($"{channelToClear} Channel Cleared!");
            }


            public void RemoveSound(ISoundSource trackedAudio)
            {
                if (_toAdd.Contains(trackedAudio))
                {
                    _toCancel.Add(trackedAudio);
                    return;
                }
                
                if (_playing.Contains(trackedAudio))
                {
                    Unregister(trackedAudio);
                }
            }

            public int GetChannelActiveCount(SoundChannel channel)
            {
                return _activeByChannel.GetValueOrDefault(channel, 0);
            }

            public bool GetIsChannelFull(SoundChannel channel)
            {
                return GetChannelActiveCount(channel) >= SoundTools.GetSoundChannelLimit(channel) ;
            }
            
            public bool GetCanAddToChannel(SoundChannel channel)
            {
                return GetChannelActiveCount(channel) < SoundTools.GetSoundChannelLimit(channel);
            }

            public bool GetCanAddToChannel(ISoundSource trackedAudio)
            {
                return GetCanAddToChannel(trackedAudio.Channel);
            }

            #endregion

            #region Private API

            private bool TryAddToChannel(SoundChannel channel, string itemSourceName)
            {
                _activeByChannel ??= new Dictionary<SoundChannel, int>();

                if (_activeByChannel.TryGetValue(channel, out var channelAmount) == false)
                {
                    _activeByChannel.Add(channel, 0);
                }
                
                if (channelAmount >= SoundTools.GetSoundChannelLimit(channel))
                {
                    SoundTools.DebugSound($"{channel} Channel limit reached! Sound {itemSourceName} could not be added to channel!");
                    return false;
                }
                else
                {
                    _activeByChannel[channel]++;
                    SoundTools.DebugSound($"Sound {itemSourceName}, Added to {channel} Channel");
                    OnChannelUpdated?.Invoke(channel, _activeByChannel[channel]);
                    return true;
                }
            }
            private void RemoveFromChannel(SoundChannel channel, string itemSourceName)
            {
                _activeByChannel ??= new Dictionary<SoundChannel, int>();

                if (_activeByChannel.ContainsKey(channel) == false)
                {
                    SoundTools.DebugSound($"{channel} not active!");
                    return;
                }

                _activeByChannel[channel]--;
                    
                SoundTools.DebugSound($"Sound {itemSourceName} Removed from {channel} Channel");
                    
                OnChannelUpdated?.Invoke(channel, _activeByChannel[channel]);
            }

            private void ApplyPending()
            {
                if (_toAdd.Count > 0)
                {
                    foreach (var item in _toAdd)
                    {
                        if (_toCancel.Contains(item))
                        {
                            continue;
                        }

                        if(TryAddToChannel(item.Channel,item.SourceName) == false)
                            continue;
                        
                        _playing.Add(item);
                        if(item.DoesLoop)
                            item.OnLoopFinished += Sound_OnLoopFinishedHandler;
                        item.OnStopped += SoundSource_OnStoppedHandler;
                        item.Play();
                    }
                    
                    _toAdd.Clear();
                    _toCancel.Clear();
                }

                if (_toRemove.Count > 0)
                {
                    foreach (var item in _toRemove)
                    {
                        if(item.DoesLoop)
                            item.OnLoopFinished -= Sound_OnLoopFinishedHandler;
                        item.OnStopped -= SoundSource_OnStoppedHandler;
                        RemoveFromChannel(item.Channel, item.SourceName);
                        item.FinishSound();
                        _playing.Remove(item);
                    }
                    
                    _toRemove.Clear();
                }
            }

            #region Handler
            
            private void SoundSource_OnStoppedHandler(ISoundSource input)
            {
                Unregister(input);
            }
            
            private void Sound_OnLoopFinishedHandler(ISoundSource input)
            {
                input.OnFadeOutComplete -= Sound_OnLoopFinishedHandler;
                
                //Add Timer
                
                input.Resume();
            }
            
            #endregion

            #endregion
            
            private Rect _windowRect = new Rect(10, 10, 320, 420);
            private GUIStyle _titleStyle;
            private GUIStyle _labelStyle;
            
            public void ShowGUI()
            {
                InitStyles();
                _windowRect = GUI.Window(1, _windowRect, DrawWindow, "Audio Debug");
            }

            private void DrawWindow(int id)
            {
                GUILayout.Space(6);

                GUILayout.Label("Active Sounds by Channel", _titleStyle);
                GUILayout.Space(10);

                foreach (var kvp in _activeByChannel)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(kvp.Key.ToString(), _labelStyle, GUILayout.Width(180));
                    GUILayout.Label(kvp.Value.ToString(), _labelStyle, GUILayout.Width(60));
                    GUILayout.EndHorizontal();
                }

                GUI.DragWindow();
            }

            private void InitStyles()
            {
                if (_titleStyle != null) return;

                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 18
                };

                _labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16
                };
            }
            
        }
        private class MusicController
        {
            public SoundManager.GeneratedId PlayingSoundId { get; private set; }
            public SoundManager.GeneratedId PausedSoundId { get; private set; }
            public SoundManager.GeneratedId BackgroundSoundId { get; private set; }

            private bool IsSoundValid(SoundManager.GeneratedId soundId)
            {
                return soundId != null && soundId.IsActive;
            }

            #region Public Getters
            
            public bool IsThisSoundPlaying(SoundManager.GeneratedId soundId)
            {
                if (IsSoundValid(soundId))
                {
                    return PlayingSoundId == soundId;
                }
                
                return false;
            }
            
            public bool IsThisPaused(SoundManager.GeneratedId soundId)
            {
                if (IsSoundValid(soundId))
                {
                    return PausedSoundId == soundId;
                }
                
                return false;
            }
            
            public bool IsThisInBackground(SoundManager.GeneratedId soundId)
            {
                if (IsSoundValid(soundId))
                    return BackgroundSoundId == soundId;
                
                return false;
            }

            public bool HasMusicPlaying() => IsSoundValid(PlayingSoundId);
            public bool HasMusicPaused() => IsSoundValid(PausedSoundId);
            public bool HasBackgroundMusicPlaying() => IsSoundValid(BackgroundSoundId);
            
            #endregion

            #region Music Actions

            public void Play(SoundManager.GeneratedId soundId)
            {
                if (IsSoundValid(soundId))
                {
                    if (HasBackgroundMusicPlaying() && 
                        BackgroundSoundId == soundId)
                    {
                        BackgroundSoundId = null;
                    }

                    if (HasMusicPlaying())
                    {
                        if (soundId == PlayingSoundId)
                        {
                            Debug.LogWarning("Can't play again the same music!");
                            return;
                        }
                        
                        ClearPlayingMusic();
                    }

                    PlayingSoundId = soundId;
                }
                else
                {
                    Debug.LogWarning("Music id is not valid!");
                }
            }

            public void Pause()
            {
                if (IsSoundValid(PlayingSoundId))
                {
                    PausedSoundId = PlayingSoundId;
                    PlayingSoundId = null;
                }
                else
                {
                    Debug.LogWarning("Music is not playing!");
                }
            }
            
            public void Resume()
            {
                if (IsSoundValid(PausedSoundId))
                {
                    PlayingSoundId = PausedSoundId;
                    PausedSoundId = null;
                }
                else
                {
                    Debug.LogWarning("Music is not paused!");
                }
            }

            public void SendToBackground()
            {
                if (IsSoundValid(PlayingSoundId))
                {
                    BackgroundSoundId = PlayingSoundId;
                    PlayingSoundId = null;
                }
                else
                {
                    Debug.LogWarning("Music is not playing!");
                }
            }

            public void ClearPlayingMusic()
            {
                if (!IsSoundValid(PlayingSoundId)) return;
                
                PlayingSoundId = null;

            }
            
            public void ClearPausedMusic()
            {
                if (!IsSoundValid(PausedSoundId)) return;
                
                PausedSoundId = null;
            }
            
            public void ClearBackgroundMusic()
            {
                if (!IsSoundValid(BackgroundSoundId)) return;
            
            BackgroundSoundId = null;
        }

        #endregion
        
    }
        
        #endregion
        
        #region Sound Generator
        
        private class UIDefaultSounds
        {
            private readonly Dictionary<UISoundType, ISoundSourceData> _uiSounds = new Dictionary<UISoundType, ISoundSourceData>();
            private const string Path = "ScriptableObjects/DefaultUISounds";
        
            public UIDefaultSounds()
            {
                Initialize();
            }

            private void Initialize()
            {
                var loaded = Resources.LoadAll<UiSoundSourceDataSo>(Path);

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

            public ISoundSourceData GetSound(UISoundType uiSoundType)
            {
                return _uiSounds[uiSoundType];
            }

        }
        private class SoundGenerator
        {
            #region Private Classes
            
            private class SoundFactory
            {
                private readonly GenericPool<IPoolableSound> _pool;
                
                public SoundFactory(SoundSource prefab, int defaultSize = 15, int maxSize = 100)
                {
                    _pool = new GenericPool<IPoolableSound>(() => UnityEngine.Object.Instantiate(prefab), 
                        defaultSize, maxSize)
                    {
                        PoolName = "Sound Pool"
                    };
                }

                public ISoundSource GetSound(ISoundSourceData soundData, Transform parent)
                {
                    var tempPoolable = _pool.Get();

                    var tempSound = (ISoundSource)tempPoolable;
                    tempSound.SetSoundData(soundData,parent);
                    return tempSound;
                }
                
                public void RecycleAll()
                {
                    _pool.RecycleAll();
                }
            }

            #endregion
            
            private readonly SoundFactory _factory;
            private readonly SoundManager.RandomIdGenerator _idGenerator;
            private readonly Dictionary<SoundManager.GeneratedId, ISoundSource> _soundsDic;
            private readonly Dictionary<ISoundSource, SoundManager.GeneratedId> _idsDic;

            public SoundGenerator(int defaultSize = 15, int maxSize = 50)
            {
                _soundsDic = new Dictionary<SoundManager.GeneratedId, ISoundSource>();
                _idsDic = new Dictionary<ISoundSource, SoundManager.GeneratedId>();
                _factory = new SoundFactory(SoundTools.GetSoundSourcePrefab(), defaultSize, maxSize);
                _idGenerator = new SoundManager.RandomIdGenerator(15);
            }
            
            public SoundClass GetSound(ISoundSourceData soundData, Transform parent)
            {
                var id = _idGenerator.Generate();
                var soundSource = _factory.GetSound(soundData, parent);
                soundSource.OnFinished += Sound_OnFinishedHandler;
                
                _soundsDic.Add(id, soundSource);
                _idsDic.Add(soundSource, id);

                return new SoundClass(id, soundSource);
            }

            private void ReturnSound(ISoundSource input)
            {
                if(_idsDic.TryGetValue(input, out var id) == false) return;
                
                input.OnFinished -= Sound_OnFinishedHandler;
                
                _soundsDic.Remove(id);
                _idsDic.Remove(input);
                id.Reset();
            }

            private void Sound_OnFinishedHandler(ISoundSource input)
            {
                ReturnSound(input);
            }

            public bool DoesContain(SoundManager.GeneratedId soundId)
            {
                if (soundId == null)
                {
                    SoundTools.DebugSound($"Given sound id is null.");
                    return false;
                }

                return _soundsDic.ContainsKey(soundId);
            }

            public ISoundSource GetSoundById(SoundManager.GeneratedId soundId)
            {
                if (soundId == null)
                {
                    SoundTools.DebugSound($"Given sound id is null.");
                    return null;
                }
                
                return _soundsDic[soundId];
            }
        }
        private class SoundClass
        {
            public SoundManager.GeneratedId Id { get; private set; }
            public ISoundSource Source { get; private set; }

            public SoundClass(SoundManager.GeneratedId id, ISoundSource source)
            {
                Id = id;
                Source = source;
            }
        }

        #endregion
        
        #endregion
        
        #region ID Generator

        public class GeneratedId
        {
            public ushort Id { get; private set; }
            public bool IsActive => Id > 0;
            private event Action<GeneratedId> OnRelease;
    
            public GeneratedId(ushort id, Action<GeneratedId> onRelease)
            {
                Id = id;
                this.OnRelease = onRelease;
            }
    
            public void Release()
            {
                OnRelease?.Invoke(this);
            }
    
            public void Reset()
            {
                Id = 0;
            }
        }
        public class RandomIdGenerator
        {
            private const ushort NullId = 0; // Default ID used as null

            private readonly HashSet<ushort> _inUseId;// IDs currently in use
            private ushort _nextId = 1; // Start from 1 (0 = NullId)

            public RandomIdGenerator(int initialSize = 10)
            {
                initialSize = Math.Clamp(initialSize, 0, ushort.MaxValue);
                _inUseId = new HashSet<ushort>(initialSize);
            }
        

            /// <summary>
            /// Generates an incremental GeneratedId
            /// </summary>
            public GeneratedId Generate()
            {
                if (_inUseId.Count >= ushort.MaxValue - 1)
                    throw new InvalidOperationException("All available IDs are in use.");

                // Find the next free ID
                while (_inUseId.Contains(_nextId) || _nextId == NullId)
                {
                    _nextId++;

                    if (_nextId == ushort.MaxValue)
                        _nextId = 1; // Wrap around if overflow
                }

                ushort value = _nextId;
                _inUseId.Add(value);
                _nextId++;

                return new GeneratedId(value, Release);
            }

            private void Release(GeneratedId idData)
            {
                _inUseId.Remove(idData.Id);
                idData.Reset();
            }
        }

        #endregion
        
        #region Singleton
        public static SoundManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        
        private static SoundManager _instance;
        
        private static SoundManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(SoundManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<SoundManager>();
        }

        // Empty Method just to call the Manager and create the Instance
        private void Internal_LoadInstance() {}
        public static void LoadInstance() => Instance.Internal_LoadInstance();
        
        #endregion
        
        private bool _hasInitialized;
        private AudioTracker _audioTracker;
        private SoundGenerator _soundGenerator;
        private MusicController _musicController;
        private UIDefaultSounds _uiDefaultSounds;
        
        private AudioMixer _mixer;
        private float _mixerVolume;

        private void Awake()
        {
            SoundEvents.OnInitializeSoundManager += Initialize;
        }

        private void Initialize()
        {
            _audioTracker = new AudioTracker();
            _soundGenerator = new SoundGenerator();
            _musicController = new MusicController();
            _uiDefaultSounds = new UIDefaultSounds();
            _mixer = SoundTools.GetAudioMixer();
            //
            SoundEvents.SoundManagerInitialized();
            _hasInitialized = true;
        }

        private void Update()
        {
            if(_hasInitialized == false)
                return;
            
            _audioTracker?.Execute();
        }


        #region Public API
        public static void SetMainVolume(string paramName, float volume) 
            => Instance.Internal_SetMainVolume(paramName, volume);
        public static void PlayUISound(UISoundType uiSoundType, bool isIsolated) 
            => Instance.Internal_PlayUISound(uiSoundType, isIsolated);
        public static GeneratedId PlaySound(ISoundSourceData soundData, Transform parent = null, bool isIsolated = false) 
            => Instance.Internal_PlaySound(soundData, parent, isIsolated); 
        public static GeneratedId PlayMusic(ISoundSourceData soundData, Transform parent = null, bool pauseCurrent = false, 
            GeneratedId soundId = null)
            => Instance.Internal_PlayMusic(soundData, parent, pauseCurrent, soundId); 
        public static void StopSound(GeneratedId soundId) 
            => Instance.Internal_StopSound(soundId);
        public static void PauseSound(GeneratedId soundId)  
            => Instance.Internal_PauseSound(soundId);
        public static void ResumeSound(GeneratedId soundId) 
            => Instance.Internal_ResumeSound(soundId);      
        public static void ResumeMusic(GeneratedId soundId, bool pauseCurrent = false) 
            => Instance.Internal_ResumeMusic(soundId, pauseCurrent);
        public static void SetSoundVolume(GeneratedId soundId, float volume)
            => Instance.Internal_SetSoundVolume(soundId, volume);

        public static void SetMusicBackground(GeneratedId soundId, float volumeTarget)
            => Instance.Internal_SetMusicBackground(soundId,volumeTarget);
        public static void SetMusicForeground(GeneratedId soundId)
            => Instance.Internal_SetMusicForeground(soundId);

        public static void ClearAllSounds() 
            => Instance.Internal_ClearAllSounds();
        
        public static void StopMusic() 
            => Instance.Internal_StopMusic();
        public static void DetachSoundFromParent(GeneratedId soundId)
            => Instance.Internal_DetachSoundFromParent(soundId);

        #endregion

        #region Internal

        private void Internal_PlayUISound(UISoundType uiSoundType, bool isIsolated)
        {
            Internal_PlaySound(_uiDefaultSounds.GetSound(uiSoundType), null, isIsolated);
        }
        
        private GeneratedId Internal_PlaySound(ISoundSourceData soundData, Transform parent = null, bool isIsolated = false)
        {
            if (soundData == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
                SoundTools.DebugSound("Sound data is null");

#endif
                return null;
            }
            
            if (_audioTracker.GetIsChannelFull(soundData.ChannelData.Channel))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
                SoundTools.DebugSound($"{soundData.ChannelData.Channel} channel is full, sound wont be played");

#endif
                return null;
            }
            
            var soundClass = _soundGenerator.GetSound(soundData, parent);
            
            _audioTracker.RegisterSound(soundClass.Source, isIsolated);
            

            
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            SoundTools.DebugSound($"Sound {soundClass.Source.SourceName} is now Playing!");
#endif
            
            return soundClass.Id;
        }

        private GeneratedId Internal_PlayMusic(ISoundSourceData soundData, Transform parent = null, bool pauseCurrent = false, 
            GeneratedId soundId = null)
        {
            if (soundId != null && soundId.IsActive)
            {
                if (_musicController.IsThisInBackground(soundId))
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    
                    SoundTools.DebugSound("Sound is already playing in background! It'll be set as foreground!");
#endif
                    Internal_SetMusicForeground(soundId);
                    
                    return soundId;
                }
                else if(_musicController.IsThisPaused(soundId))
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    
                    SoundTools.DebugSound("Sound is paused! It'll be resumed!");
#endif
                    
                    Internal_ResumeMusic(soundId, pauseCurrent);
                    
                    return soundId;
                } 
            }

            var musicId = Internal_PlaySound(soundData, parent, false);
            
            if (_musicController.HasMusicPaused())
            {
                if (pauseCurrent == false)
                {
                    StopSound(_musicController.PausedSoundId);
                    _musicController.ClearPausedMusic();
                }
            }
            else
            {
                ClearOrPauseMusic(pauseCurrent);
            }

            _musicController.Play(musicId);
            
            return musicId;
        }

        private void Internal_StopSound(GeneratedId soundId)
        {
            if(IsSoundValid(soundId, out var soundSource) == false)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                SoundTools.DebugSound("Sound is not valid, Id is null, is not active or SoundGenerator does not has it");
#endif
                return;
            }
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            var soundName = _soundGenerator.GetSoundById(soundId).SourceName;
            
            SoundTools.DebugSound($"Sound {soundName} has been stopped");
#endif
            
            soundSource.Stop(); 
            
            if (_musicController.IsThisPaused(soundId))
            {
                _musicController.ClearPausedMusic();
            }
            else if (_musicController.IsThisSoundPlaying(soundId))
            {
                _musicController.ClearPlayingMusic();
            }
            else if(_musicController.IsThisInBackground(soundId))
            {
                _musicController.ClearBackgroundMusic();
            }
        }

        private void Internal_PauseSound(GeneratedId soundId)
        {
            if(IsSoundValid(soundId, out var soundSource) == false)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                SoundTools.DebugSound("Sound is not valid, Id is null, is not active or SoundGenerator does not has it");
#endif
                return;
            }
            
            soundSource.Pause();
            
            if (_musicController.IsThisSoundPlaying(soundId))
            {
                _musicController.Pause();
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
           var soundName = _soundGenerator.GetSoundById(soundId).SourceName;
            
            SoundTools.DebugSound($"Sound {soundName} has been paused");
#endif
        }
        
        private void Internal_ResumeSound(GeneratedId soundId)
        {
            if(IsSoundValid(soundId, out var soundSource) == false)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                SoundTools.DebugSound("Sound is not valid, Id is null, is not active or SoundGenerator does not has it");
#endif
                return;
            }
            
            soundSource.Resume();
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var soundName = _soundGenerator.GetSoundById(soundId).SourceName;
            
            SoundTools.DebugSound($"Sound {soundName} has been resumed");
#endif
        }

        private void Internal_ResumeMusic(GeneratedId soundId, bool pauseCurrent)
        {
            if (soundId != null && soundId.IsActive)
            {
                if (_musicController.IsThisInBackground(soundId))
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    
                    SoundTools.DebugSound("Sound is already playing in background! It'll be set as foreground!");
#endif
                    Internal_SetMusicForeground(soundId);
                }
            }
            
            if(IsSoundValid(soundId, out var soundSource) == false)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                SoundTools.DebugSound("Sound is not valid, Id is null, is not active or SoundGenerator does not has it");
#endif
                return;
            }
            
            ClearOrPauseMusic(pauseCurrent);
            soundSource.Resume();
            
            if (_musicController.IsThisPaused(soundId))
            {
                _musicController.Resume();
            }
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var soundName = _soundGenerator.GetSoundById(soundId).SourceName;
            
            SoundTools.DebugSound($"Sound {soundName} has been resumed");
#endif
        }
        
        private void Internal_SetSoundVolume(GeneratedId soundId, float volume)
        {
            if(IsSoundValid(soundId, out var soundSource) == false)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                SoundTools.DebugSound("Sound is not valid, Id is null, is not active or SoundGenerator does not has it");
#endif
                return;
            }
            
            soundSource.SetVolume(volume);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var soundName = _soundGenerator.GetSoundById(soundId).SourceName;
            
            SoundTools.DebugSound($"Sound {soundName} volume has been set to {volume} ");
#endif
        }

        private void Internal_ClearAllSounds()
        {
            _audioTracker.ClearAllChannels();
        }

        private void Internal_SetMainVolume(string paramName, float volume)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            SoundTools.DebugSound($"Volume set to {volume} in {paramName}");
#endif
            _mixerVolume = SoundTools.GetDbFrom01Value(volume);
            GetAudioMixer().SetFloat(paramName, _mixerVolume);
        }
        
        private void Internal_SetMusicBackground(GeneratedId soundId, float volumeTarget = 0.25f)
        {
            if(IsSoundValid(soundId, out var soundSource) == false)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                SoundTools.DebugSound("Sound is not valid, Id is null, is not active or SoundGenerator does not has it");
#endif
                return;
            }
            
            if(_musicController.IsThisSoundPlaying(soundId) == false) return;
            
            soundSource.SetAsBackground(volumeTarget);
            _musicController.SendToBackground();
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var soundName = _soundGenerator.GetSoundById(soundId).SourceName;
            
            SoundTools.DebugSound($"Sound {soundName} volume has been set to background.");
#endif
        }
        
        private void Internal_SetMusicForeground(GeneratedId soundId)
        {
            if(IsSoundValid(soundId, out var soundSource) == false)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                SoundTools.DebugSound("Sound is not valid, Id is null, is not active or SoundGenerator does not has it");
#endif
                return;
            }
            
            if(_musicController.IsThisInBackground(soundId) == false) return;
            
            soundSource.SetAsForeground();
            _musicController.Play(soundId);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var soundName = _soundGenerator.GetSoundById(soundId).SourceName;
            
            SoundTools.DebugSound($"Sound {soundName} volume has been set to foreground.");
#endif
        }

        private void Internal_StopMusic()
        {
            if (_musicController.HasMusicPlaying())
            {
                StopSound(_musicController.PlayingSoundId);
                _musicController.ClearPlayingMusic();
            }
            if (_musicController.HasBackgroundMusicPlaying())
            {
                StopSound(_musicController.BackgroundSoundId);
                _musicController.ClearBackgroundMusic();
            }
            if (_musicController.HasMusicPaused())
            {
                StopSound(_musicController.PausedSoundId);
                _musicController.ClearPausedMusic();
            }
        }

        private void Internal_DetachSoundFromParent(GeneratedId soundId)
        {
            if(IsSoundValid(soundId, out var soundSource) == false)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                SoundTools.DebugSound("Sound is not valid, Id is null, is not active or SoundGenerator does not has it");
#endif
                return;
            }
            
            soundSource.DetachFromParent();
        }

        #endregion

        #region Private Methods
        
        private void ClearOrPauseMusic(bool pauseCurrent)
        {
            if (_musicController.HasMusicPlaying())
            {
                if (pauseCurrent)
                {
                    _musicController.Pause();
                    PauseSound(_musicController.PausedSoundId);
                }
                else
                {
                    StopSound(_musicController.PlayingSoundId);
                    _musicController.ClearPlayingMusic();
                }
            }
        }

        private AudioMixer GetAudioMixer()
        {
            return _mixer ??= SoundTools.GetAudioMixer();
        }

        private bool IsSoundValid(GeneratedId soundId, out ISoundSource soundSource)
        {
            soundSource = null;
            if(soundId == null) return false;
            if(soundId.IsActive == false) return false;
            if (_soundGenerator.DoesContain(soundId) == false) return false;

            soundSource = _soundGenerator.GetSoundById(soundId);
            return true;
        }
        
        #endregion
        
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        private void OnGUI()
        {
            if(_hasInitialized == false) return;
            
            _audioTracker.ShowGUI();
        }

#endif
        
    }
}