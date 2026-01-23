using MeteorMadness.Contracts.Interfaces;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield
{
    public class ShieldInput : MonoBehaviour
    {
        public IInputReader GetInputReader() => GetComponent<IInputReader>();
    }
}