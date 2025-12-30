using System;
using System.Collections.Generic;
using System.Linq;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.Save;

namespace _Main.Scripts.Cosmetics
{
    /// <summary>
    /// I don't know how to call it, but it controls the locked or unlocked skin data.
    /// </summary>
    public class LockedSkinController
    {
        #region Tools
        private static bool GetBit(ref UnlockBits128 bits, int index)
        {
            if (index < 64)
                return (bits.low & (1UL << index)) != 0;
            else
                return (bits.high & (1UL << (index - 64))) != 0;
        }

        private static void SetBit(ref UnlockBits128 bits, int index)
        {
            if (index < 64)
                bits.low |= 1UL << index;
            else
                bits.high |= 1UL << (index - 64);
        }

        private static void ClearBit(ref UnlockBits128 bits, int index)
        {
            if (index < 64)
                bits.low &= ~(1UL << index);
            else
                bits.high &= ~(1UL << (index - 64));
        }
        
        #endregion
        
        private UnlockBits128 _bits;
        private GeneratedId _secureId;

        public LockedSkinController()
        {

        }

        public void Initialize(DataManager.SkinSaveData data)
        {
            _bits = data.UnlockedSkins;

#if UNITY_EDITOR
            
            if (GameParameters.GameplayValues.HasAllSkinsUnlocked)
            {
                _bits.low = ulong.MaxValue;
                _bits.high = ulong.MaxValue;
            }
#endif
        }

        #region Public Methods
        
        public UnlockBits128 GetUnlockedSkins() => _bits;
        public void UnlockSkin(int skinIndex)
        {
            if (skinIndex > 128)
            {
                skinIndex = 128;
            }

            SetBit(ref _bits, skinIndex-1);
        }

        public bool GetIsLocked(int skinIndex)
        {
            if (skinIndex > 128)
            {
                skinIndex = 128;
            }
            
            return GetBit(ref _bits, skinIndex-1) == false;
        }

        #endregion
    }
}