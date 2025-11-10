using _Main.Scripts.DebugGUI;

namespace _Main.Scripts.GameScreens
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public class GameScreenDebugData
    {
        public ScreenType CurrentScreen { get; set; }
        public ScreenType LastScreen { get; set; }
        
        public GameScreenDebugData()
        {
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.GameScreen, DebugGUISortingOrder.Group.GameScreen)
                ?.AddEntry(
                    () => $"Current: {CurrentScreen}",
                    () => $"Last: {LastScreen}"
                );
        }
    }
#endif
}