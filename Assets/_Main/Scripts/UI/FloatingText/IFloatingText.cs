using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;

namespace MeteorMadness.UI.FloatingText
{
    public interface IFloatingText : IPoolable<IFloatingText>
    {
        public void SetValues(FloatingTextValues values);
    }
}