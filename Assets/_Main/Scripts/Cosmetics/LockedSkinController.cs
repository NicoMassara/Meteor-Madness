using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.CustomId;
using _Main.Scripts.Save;
using _Main.Scripts.SecurityData;

namespace _Main.Scripts.Cosmetics
{
    /// <summary>
    /// I don't know how to call it, but it controls the locked or unlocked skin data.
    /// </summary>
    public class LockedSkinController
    {
        private struct UnlockSnapshot
        {
            public int[] SnapshotArray;
        }
        
        private GeneratedId _secureId;

        public LockedSkinController()
        {

        }

        public void Initialize(DataManager.SkinSaveData data)
        {
            _secureId = SecureValueManager.RegisterValue(new UnlockSnapshot
            {
                SnapshotArray = data.UnlockedSkins.ToArray()
            });
        }

        #region Public Methods


        public List<int> GetUnlockedSkins()
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out UnlockSnapshot storedData) == false)
                return new List<int>();
            
            return storedData.SnapshotArray.ToList();
        }

        public void UnlockSkin(int skinIndex)
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out UnlockSnapshot storedData) == false)
                return;

            var hashSet = storedData.SnapshotArray.ToHashSet();
            
            hashSet.Add(skinIndex);
            
            storedData.SnapshotArray = hashSet.ToArray();

            SecureValueManager.ModifyValue(_secureId, storedData);
        }

        public bool GetIsLocked(int skinIndex)
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out UnlockSnapshot storedData) == false)
                return true;
            
            var hashSet = storedData.SnapshotArray.ToHashSet();
            
            return hashSet.Contains(skinIndex) == false;
        }
        
        #endregion
    }
}