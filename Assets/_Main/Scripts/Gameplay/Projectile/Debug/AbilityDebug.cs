using _Main.Scripts.Core.FlyingObject.Debug;
using MeteorMadness.GlobalValues.Utilities;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Debug
{
    internal class AbilityDebug : FlyingObjectDebug<IDebugAbilitySphere>
    {
        protected override string[] GetLines()
        {
            string targetRatio = DebugObject.TargetRatio > 0 ? $"Target R: {DebugObject.TargetRatio}" : "Target R: ---";
            
            return new string[]
            {
                $"Pos: {DebugObject.Position}",
                $"Speed: {DebugObject.Speed:F2}",
                $"Targetable: {DebugObject.CanBeTargeted}",
                $"{targetRatio}",
                $"Ability: {DebugObject.DebugAbility}"
            };
        }

        protected override Color[] GetLineColors()
        {
            return new Color[]
            {
                Color.white, // Pos
                Color.white, // Speed
                DebugObject.CanBeTargeted ? Color.green : Color.red, // Targetable
                AbilityColorHelper.GetColor(DebugObject.DebugAbility), // Ability
            };
        }
    }
}