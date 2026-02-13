using _Main.Scripts.Core.FlyingObject.Debug;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Debug
{
    internal class MeteorDebug : FlyingObjectDebug<IDebugMeteor>
    {
        protected override string[] GetLines()
        {
            string targetRatio = DebugObject.TargetRatio > 0 ? $"Target R: {DebugObject.TargetRatio}" : "Target R: ---";
            
            return new string[]
            {
                $"Pos: {DebugObject.Position}",
                $"Speed: {DebugObject.Speed:F2}",
                $"Slot: {DebugObject.Slot}",
                $"Targetable: {DebugObject.CanBeTargeted}",
                $"{targetRatio}",
            };
        }
        
        protected override Color[] GetLineColors()
        {
            return new Color[]
            {
                Color.white, // Pos
                Color.white, // Speed
                Color.white, // Slot
                DebugObject.CanBeTargeted ? Color.green : Color.red, // Targetable
                Color.yellow, // Ratio
            };
        }
    }
}