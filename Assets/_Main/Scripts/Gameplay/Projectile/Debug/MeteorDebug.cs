using _Main.Scripts.Core.FlyingObject.Debug;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Debug
{
    internal class MeteorDebug : FlyingObjectDebug<IDebugMeteor>
    {
        protected override string[] GetLines()
        {
            return new string[]
            {
                $"Pos: {DebugObject.Position}",
                $"Speed: {DebugObject.Speed:F2}",
                $"Targetable: {DebugObject.CanBeTargeted}",
            };
        }
        
        protected override Color[] GetLineColors()
        {
            return new Color[]
            {
                Color.white, // Pos
                Color.white, // Speed
                DebugObject.CanBeTargeted ? Color.green : Color.red, // Targetable
            };
        }
    }
}