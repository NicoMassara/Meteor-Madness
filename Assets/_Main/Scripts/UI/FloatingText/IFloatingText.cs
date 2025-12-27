using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Managers;

namespace MeteorMadness.UI.FloatingText
{
    public interface IFloatingText : IPoolable<IFloatingText>
    {
        public void SetValues(FloatingTextValues values);
    }
}