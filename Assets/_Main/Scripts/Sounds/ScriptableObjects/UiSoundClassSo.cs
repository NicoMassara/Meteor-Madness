using UnityEngine;

namespace _Main.Scripts.Sounds
{
    [CreateAssetMenu(fileName = "SO_UISoundData_Sound", menuName = "Scriptable Objects/Sound/UI Sound Class", order = 0)]
    public class UiSoundClassSo : SoundClassSo
    {
        [Header("UI Sound Data")]
        [Tooltip("Do not creat two of the same type, it won't be loaded")]
        [SerializeField] private UISoundType uiSoundType;
        
        public UISoundType UISoundType => uiSoundType;

        protected override void OnValidate()
        {
            base.OnValidate();
            ForceChannel(SoundChannel.UI);
        }
    }
}