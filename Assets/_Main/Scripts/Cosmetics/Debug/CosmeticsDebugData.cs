using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Cosmetics
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public class CosmeticsDebugData
    {
        public SkinType CurrentSkin { get; set; }
        
        public CosmeticsDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Managers, DebugGUISortingOrder.Group.Skin)
                ?.AddEntry(
                    () => "-- Skin -- ",
                    () => $"Current: {CurrentSkin.ToString()}"
                );
        }
    }
#endif
}