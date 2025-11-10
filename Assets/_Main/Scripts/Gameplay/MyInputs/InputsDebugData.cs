using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.Gameplay.MyInputs
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    
    public class InputsDebugData
    {
        public int HorizontalAxis;
        public bool TriggerAbility;
        
        public InputsDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Input, DebugGUISortingOrder.Group.Input)
                ?.AddEntry(
                    () => $"Axis: {HorizontalAxis}",
                    () => $"Ability: {TriggerAbility}"
                );
        }
    }

#endif
}