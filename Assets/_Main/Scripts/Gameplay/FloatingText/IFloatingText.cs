using _Main.Scripts.Interfaces;

namespace _Main.Scripts.Gameplay.FloatingScore
{
    public interface IFloatingText : IPoolable<IFloatingText>
    {
        public void SetValues(FloatingTextValues values);
    }
}