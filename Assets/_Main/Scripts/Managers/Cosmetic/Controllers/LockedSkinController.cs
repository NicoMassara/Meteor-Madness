using System.Collections.Generic;
using System.Linq;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.Save;

namespace _Main.Scripts.Cosmetics
{
    /// <summary>
    /// I don't know how to call it, but it controls the locked or unlocked skin data.
    /// </summary>
    public class LockedSkinController
    {
        
        public List<int> UnlockedSkins;
        private GeneratedId _secureId;

        public LockedSkinController()
        {

        }

        public void Initialize(DataManager.SkinSaveData data)
        {
            UnlockedSkins = data.UnlockedSkins;
        }

        #region Public Methods


        public List<int> GetUnlockedSkins()
        {
            return UnlockedSkins.ToList();
        }

        public void UnlockSkin(int skinIndex)
        {
            var hashSet = UnlockedSkins.ToHashSet();
            
            hashSet.Add(skinIndex);
            
            UnlockedSkins = hashSet.ToList();
        }

        public bool GetIsLocked(int skinIndex)
        {
            var hashSet = UnlockedSkins.ToHashSet();
            
            return hashSet.Contains(skinIndex) == false;
        }
        
        #endregion
    }
}