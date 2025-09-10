using _Main.Scripts.Managers.UpdateManager.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Managers.UpdateManager
{
    public class ManagedBehavior : MonoBehaviour, IManagedObject
    {
        protected virtual void OnEnable()
        {
            this.RegisterInManager();
        }

        protected virtual void OnDisable()
        {
            this.UnregisterInManager();
        }
    }
}