using _Main.Scripts.DebugGUI;
using _Main.Scripts.MyTools;

namespace _Main.Scripts.Managers.UpdateManager
{
    
#if UNITY_EDITOR || DEVELOPMENT_BUILD

    public class UpdateManagerDebugData
    {
        private readonly FPSCounter _fpsCounter;
        public int ManagedUpdateCount;
        public int ManagedFixedUpdateCount;
        public int ManagedLateUpdateCount;
        
        public UpdateManagerDebugData()
        {
            _fpsCounter = new FPSCounter(0.5f);
            
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Fps, DebugGUISortingOrder.Group.Fps)?.AddEntry(
                () => $"FPS: {_fpsCounter.Current:F1}",
                () => $"AVG: {_fpsCounter.AVG:F1}"
            );
            
            DebugGUIManager.Instance.CreateGroup(DebugGUIKeys.Group.Managers, DebugGUISortingOrder.Group.Managers)
                ?.CreateSubGroup(DebugGUIKeys.SubGroup.UpdateManager, DebugGUISortingOrder.SubGroup.UpdateManager)
                ?.AddEntry(
                () => $"Update Count: {ManagedUpdateCount}",
                () => $"Fixed Count: {ManagedFixedUpdateCount}",
                () => $"Late Count: {ManagedLateUpdateCount}"
            );
            
        }

        public void UpdateFps(float deltaTime)
        {
            _fpsCounter.Update(deltaTime);
        }
    }
    
#endif

}