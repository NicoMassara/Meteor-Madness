using _Main.Scripts.MyTools;
using UnityEngine;

namespace _Main.Scripts.Gameplay.FloatingScore
{
    public interface IFloatingText : IPoolable<IFloatingText>
    {
        public void SetValues(FloatingTextValues values);
    }
}