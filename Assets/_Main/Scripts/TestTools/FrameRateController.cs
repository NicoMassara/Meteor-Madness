using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace _Main.Scripts.TestTools
{
    public class FrameRateController : MonoBehaviour
    {
        #region GUI Config
        
        [System.Serializable]
        private class DebugGUIConfig
        {
            public Vector2 PositionOffset = new Vector2(0, 0);
            [Min(1)]
            public float Width = 260f;
            [Min(1)]
            public float Height = 230f;
        }

        [SerializeField] private DebugGUIConfig guiConfig;
        
        #endregion

        private float _timeScale = 1;
        private float _lastTimeScale;
        private int _frameCountStep = 1;


        #region Frame Counter
        private class FrameCounter
        {
            private const float UpdateInterval = 0.5f;
            
            private float _accum;
            private int _frames;
            private float _timeLeft;
            public float FPS { get; private set; }

            public FrameCounter()
            {
                _timeLeft = UpdateInterval;
            }

            public void Update(float deltaTime)
            {
                _timeLeft -= deltaTime;
                _accum += deltaTime;
                _frames++;

                if (_timeLeft <= 0f)
                {
                    FPS = _frames / _accum ;
                    _timeLeft = UpdateInterval;
                    _accum = 0f;
                    _frames = 0;
                }

                if (deltaTime == 0)
                {
                    FPS = 0;
                }
            }
        }
        
        
        #endregion
        
        private FrameCounter _scaledFrameCounter;
        private FrameCounter _unScaledFrameCounter;

        private void Start()
        {
            _scaledFrameCounter = new FrameCounter();
            _unScaledFrameCounter = new FrameCounter();
        }

        private void Update()
        {
            if (_lastTimeScale != _timeScale)
            {
                Time.timeScale = _timeScale;
                _lastTimeScale = _timeScale;
            }
            
            _scaledFrameCounter?.Update(Time.deltaTime);
            _unScaledFrameCounter?.Update(Time.unscaledDeltaTime);
        }

        #region Actions

        private void StepFrames(int frames)
        {
            StartCoroutine(Coroutine_StepFrame(frames));
        }
        
        private IEnumerator Coroutine_StepFrame(int frames)
        {
            var target = frames;
            
            _timeScale = 1f;

            while (target > 0)
            {
                target--;
                
                yield return null;
            }

            _timeScale = 0f;
        }

        private void Pause()
        {
            _timeScale = 0;
        }

        private void Resume()
        {
            _timeScale = 1;
        }
        #endregion
        
        private void OnGUI()
        {
            float scale = Mathf.Min(
                Screen.width / 1920f,
                Screen.height / 1080f
            );

            GUI.matrix = Matrix4x4.TRS(
                Vector3.zero,
                Quaternion.identity,
                Vector3.one * scale
            );

            Rect area = new Rect(
                10 / scale + guiConfig.PositionOffset.x, 
                10 / scale + guiConfig.PositionOffset.y ,
                guiConfig.Width, guiConfig.Height);

            GUILayout.BeginArea(area, "Frame Control", GUI.skin.window);

            GUILayout.Space(5);
            
            // === Frame Rate === //
            
            GUILayout.Label($"Scaled FPS: {_scaledFrameCounter.FPS:F2}");
            GUILayout.Label($"Unscaled FPS: {_unScaledFrameCounter.FPS:F2}");
            
            GUILayout.Space(5);
            
            // === Time Scale === //

            GUILayout.Label($"Time Scale: {_timeScale:F2}");

            _timeScale = GUILayout.HorizontalSlider(_timeScale, 0, 1);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Pause", GUILayout.Height(30)))
                Pause();

            if (GUILayout.Button("Resume", GUILayout.Height(30)))
                Resume();

            GUILayout.Space(5);

            // === Frame Step === //
            
            if (GUILayout.Button("Step Single Frame", GUILayout.Height(30)))
                StepFrames(1);

            GUILayout.Space(10);

            GUILayout.Label($"Frames to step: {_frameCountStep}");

            _frameCountStep = Mathf.RoundToInt(
                GUILayout.HorizontalSlider(_frameCountStep, 1, 300)
            );

            if (GUILayout.Button($"Step {_frameCountStep} Frames", GUILayout.Height(30)))
                StepFrames(_frameCountStep);

            GUILayout.EndArea();

            GUI.matrix = Matrix4x4.identity;
        }
    }
}