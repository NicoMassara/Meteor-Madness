using System;

namespace _Main.Scripts.Cosmetics
{
    public class CosmeticTools
    {
        private const int PointsToCoin = 5000;
        
        public static uint ScoreToCoinsConverter(uint score)
        {
            return score / PointsToCoin;
        }

        public static int GetSkinPrice(SkinType skinType)
        {
            return skinType switch
            {
                SkinType.Default => 0,
                SkinType.Pizza => 100,
                _ => int.MaxValue
            };
        }
    }
}