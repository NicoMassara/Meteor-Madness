using System;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace MeteorMadness.Vibration
{
#if UNITY_ANDROID
    public class VibrationController
    {
        private AndroidJavaObject _vibrator;
        private TimerManager.GeneratedId _timerId;
        private AndroidJavaClass _vibrationEffectClass;
        private int _apiLevel;
        private int _defaultAmplitude;

        public event Action OnVibrate;
        public event Action OnStopVibration;
        
        public VibrationController()
        {
            Initialize();
        }

        public bool IsVibrating { get; private set; }

        private void Initialize()
        {
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                
                using var version = new AndroidJavaClass("android.os.Build$VERSION");
                _apiLevel = version.GetStatic<int>("SDK_INT");

                if (_apiLevel >= 26)
                {
                    _vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                    _defaultAmplitude = _vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Vibrator initialization failed: " + e.Message);
                _vibrator = null;
            }
        }
        
        public void VibrateInternal(AndroidJavaObject effect, long fallbackDurationMs)
        {
            _vibrator.Call("cancel");

            if (_apiLevel >= 26 && effect != null)
                _vibrator.Call("vibrate", effect);
            else
                _vibrator.Call("vibrate", fallbackDurationMs);

            StartTimer(fallbackDurationMs);
        }

        private void StartTimer(long durationMs)
        {
            if (_timerId != null && _timerId.IsActive)
            {
                TimerManager.Remove(_timerId);
                IsVibrating = false;
            }

            _timerId = TimerManager.Add(new TimerData
            (
                durationMs / 1000f,
                onStartAction: () =>
                {
                    IsVibrating = true;
                    OnVibrate?.Invoke();
                },
                onEndAction: () =>
                {
                    IsVibrating = false;
                    OnStopVibration?.Invoke();
                }
            ));
        }

        public void Vibrate(long[] timings, int[] amplitudes)
        {
            if (_vibrator == null || timings == null || timings.Length == 0)
                return;

            if (_apiLevel >= 26 &&
                amplitudes != null &&
                amplitudes.Length != timings.Length)
            {
                Debug.LogWarning("Timings and amplitudes length mismatch");
                return;
            }

            long totalDuration = VibrationTools.GetTotalDuration(timings);

            try
            {
                AndroidJavaObject effect = null;

                if (_apiLevel >= 26)
                {
                    effect = _vibrationEffectClass.CallStatic<AndroidJavaObject>(
                        "createWaveform",
                        timings,
                        amplitudes,
                        -1
                    );
                }

                VibrateInternal(effect, totalDuration);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Waveform vibration failed: {e.Message}");
            }
        }
        
        public void Vibrate(long milliseconds = 100, int amplitude = -1)
        {
            if (_vibrator == null)
                return;

            try
            {
                AndroidJavaObject effect = null;

                if (_apiLevel >= 26)
                {
                    int useAmplitude = amplitude < 0
                        ? _defaultAmplitude
                        : amplitude;

                    effect = _vibrationEffectClass.CallStatic<AndroidJavaObject>(
                        "createOneShot",
                        milliseconds,
                        useAmplitude
                    );
                }

                VibrateInternal(effect, milliseconds);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"OneShot vibration failed: {e.Message}");
            }
        }

        public void CancelVibration()
        {
            if (_vibrator != null)
            {
                Debug.Log("Vibration cancelled");
                _vibrator.Call("cancel");
                if (_timerId != null)
                {
                    TimerManager.Remove(_timerId);
                }
                IsVibrating = false;
                OnStopVibration?.Invoke();
            }
        }
    }
#endif
}