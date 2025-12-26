using _Main.Scripts.CustomId;
using _Main.Scripts.Managers;
using _Main.Scripts.Save;
using _Main.Scripts.SecurityData;
using UnityEngine;

namespace _Main.Scripts.Cosmetics
{
    public class CoinsController
    {
        private GeneratedId _secureId;

        public CoinsController()
        {

        }

        public void Initialize(DataManager.SkinSaveData data)
        {
            _secureId = SecureValueManager.RegisterValue(data.SkinCoins);
            
        }

        #region Public Methods
        
        public bool TryAddCoins(uint score)
        {
            var coins = CosmeticTools.ScoreToCoinsConverter(score);

            if (SecureValueManager.GetDoesContainValue<uint>(_secureId, out var storedCoins) == false) return false;
            
            storedCoins += coins;
            
            UpdateStoredCoins(storedCoins);
            return true;
        }

        public uint GetCoins()
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out uint storedCoins) == false)
                return 0;
            
            return storedCoins;
        }

        public bool GetContainsEnoughCoins(uint coinsAmount)
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out uint storedCoins) == false)
                return false;
            
            return storedCoins >= coinsAmount;
        }

        public bool TryRemoveCoins(uint coinsToRemove)
        {
            if (SecureValueManager.GetDoesContainValue<uint>(_secureId, out var storedCoins) == false) return false;
            
            storedCoins -= coinsToRemove;
            UpdateStoredCoins(storedCoins);
            
            return true;
        }

        public void SaveStoredCoins()
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out uint storedCoins) == false)
                return;
            
            var data = DataManager.Instance.GetData<DataManager.SkinSaveData>(DataManager.SaveDataType.Skin);
            data.SkinCoins = storedCoins;
            DataManager.Instance.SaveGameData(data, DataManager.SaveDataType.Skin);
        }
        
        #endregion

        #region Private Methods
        
        private void UpdateStoredCoins(uint value)
        {
            if (SecureValueManager.GetDoesContainValue(_secureId, out uint storedCoins) == false)
                return;
            
            storedCoins = value;
            
            SecureValueManager.ModifyValue(_secureId, storedCoins);
        }
        
        #endregion
    }
}