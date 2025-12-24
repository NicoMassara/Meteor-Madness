using System;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace _Main.Scripts.Vibration
{
#if UNITY_ANDROID
    public class VibrationController
    {
        private AndroidJavaObject _vibrator;
        private TimerManager.GeneratedId _timerId;

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
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Vibrator initialization failed: " + e.Message);
                _vibrator = null;
            }
        }

        public void Vibrate(long milliseconds = 100, int amplitude = -1)
        {
            if (_vibrator == null) return;

            try
            {
                AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
                int apiLevel = version.GetStatic<int>("SDK_INT");

                if (apiLevel >= 26) // Android O y superiores
                {
                    AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");

                    // Usar DEFAULT_AMPLITUDE si el valor de amplitude es -1
                    int useAmplitude = (amplitude < 0) 
                        ? vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE") 
                        : amplitude;

                    AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                        "createOneShot",
                        milliseconds,
                        useAmplitude
                    );

 
                    _vibrator.Call("vibrate", vibrationEffect);
                }
                else
                {
                    _vibrator.Call("vibrate", milliseconds);
                }
                
                _timerId = TimerManager.Add(new TimerData
                (
                    milliseconds / 1000f,
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
            catch (System.Exception e)
            {
                Debug.LogWarning("Vibration failed: " + e.Message);
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