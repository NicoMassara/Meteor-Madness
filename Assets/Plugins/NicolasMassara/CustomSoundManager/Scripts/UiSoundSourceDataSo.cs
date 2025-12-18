using System;
using UnityEngine;

namespace Plugins.NicolasMassara.CustomSoundManager
{
    
    [CreateAssetMenu(fileName = "So_UiSoundSourceData_NAME", menuName = "Sound Manager/Source/Ui Sound Data", order = 0)]
    public class UiSoundSourceDataSo : SoundSourceDataSo
    {
        [Header("UI Sound Data")]
        [Tooltip("Do not creat two of the same type, it won't be loaded")]
        [SerializeField] private UISoundType uiSoundType;
        
        public UISoundType UISoundType => uiSoundType;

        private void OnValidate()
        {
            ForceChannel(SoundChannel.UI);
        }
    }
}