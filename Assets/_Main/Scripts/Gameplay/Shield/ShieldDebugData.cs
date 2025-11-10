using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Gameplay.Shield
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    
    public class ShieldDebugData
    {
        public float Rotation;
        public ShieldDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Gameplay, DebugGUISortingOrder.Group.Gameplay)
                ?.CreateSubGroup(DebugGUIKeys.SubGroup.Shield, DebugGUISortingOrder.SubGroup.Shield)
                ?.AddEntry(
                    () => $"Rotation: {Rotation}"
                );
        }
    }
    
#endif
    
}