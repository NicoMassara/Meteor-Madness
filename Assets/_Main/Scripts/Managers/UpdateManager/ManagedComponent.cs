using System;

namespace _Main.Scripts.Managers.UpdateManager
{
    public class ManagedComponent : IManagedObject, IDisposable
    {
        protected ManagedComponent()
        {
            this.RegisterInManager();
        }

        public void Dispose()
        {
            this.UnregisterInManager();
            GC.SuppressFinalize(this);
        }
    }
}