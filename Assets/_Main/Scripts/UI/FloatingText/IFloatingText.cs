using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Managers;

namespace MeteorMadness.UI.FloatingText
{
    public interface IFloatingText : IPoolable<IFloatingText>
    {
        public void SetValues(FloatingTextValues values);
    }
}