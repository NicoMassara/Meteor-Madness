using System;
using _Main.Scripts.MyAnimations;
using UnityEngine;

namespace _Main.Scripts.Pause.So
{

    [CreateAssetMenu(fileName = "So_Animation_UI_Pause", menuName = "Scriptable Objects/Animation Data/UI/Pause", order = 0)]
    public class PauseUiAnimationData : ScriptableObject
    {
        #region Animation Data

        [Serializable]
        public class PanelData : IPausePanelOpenData, IPausePanelCloseData
        {
            [Header("Positions")]
            public AnimationHelper.Direction titleOffscreenPos = AnimationHelper.Direction.Up;
            public AnimationHelper.Direction leftButtonsPanelOffscreenPos = AnimationHelper.Direction.Left;
            public AnimationHelper.Direction pointsTextPanelOffscreenPos = AnimationHelper.Direction.Right;
            [Space]
            [Header("Time Values")]
            [Range(0,1)]
            public float movementDuration = 0.25f;
            [Range(0,1)]
            public float finishDelay = 0.25f;
            [Space]
            [Header("Queue Values")]
            [Range(0,1)]
            public float backgroundFadeDuration = 0.2f;
            [Range(0,1)]
            public float backgroundFadeIntensity = 0.25f;
            [Space]
            [Header("Offsets")]
            public Vector2 titleOffset;
            public Vector2 leftButtonsOffset;
            public Vector2 pointsTextOffset;

            // Interface properties
            public AnimationHelper.Direction TitleOffscreenPos => titleOffscreenPos;
            public AnimationHelper.Direction LeftButtonsPanelOffscreenPos => leftButtonsPanelOffscreenPos;
            public AnimationHelper.Direction PointsTextPanelOffscreenPos => pointsTextPanelOffscreenPos;
            public Vector2 TitleOffset => titleOffset;
            public Vector2 LeftButtonsOffset => leftButtonsOffset;
            public Vector2 PointsTextOffset => pointsTextOffset;
            public float BackgroundFadeDuration => backgroundFadeDuration;
            public float BackgroundFadeIntensity => backgroundFadeIntensity;
            public float MovementDuration => movementDuration;
            public float FinishDelay => finishDelay;
        }

        [SerializeField] private PanelData panelOpenData;
        [SerializeField] private PanelData panelCloseData;

        #endregion

        public IPausePanelOpenData PanelOpenData => panelOpenData;
        public IPausePanelCloseData PanelCloseData => panelCloseData;
    }

}