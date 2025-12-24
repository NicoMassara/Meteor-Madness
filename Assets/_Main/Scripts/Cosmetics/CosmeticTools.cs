using System;

namespace _Main.Scripts.Cosmetics
{
    public class CosmeticTools
    {
        private const int PointsToCoin = 20;
        
        public static uint ScoreToCoinsConverter(uint score)
        {
            return score / PointsToCoin;
        }
    }
}