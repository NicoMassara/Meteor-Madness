using _Main.Scripts.Core.FlyingObject.Debug;
using UnityEngine;

namespace _Main.Scripts.Environment.Comet.Test
{
    public class CometDebug : FlyingObjectDebug<IDebugComet>
    {
        protected override string[] GetLines()
        {
            return new string[]
            {
                $"Pos: {DebugObject.Position}",
                $"Speed: {DebugObject.Speed:F2}",
                $"Ratio: {DebugObject.TravelRatio:F3}",
                $"Dist: {DebugObject.Distance:F3}",
                $"Scale: {DebugObject.Scale}",
            };
        }
        
        protected override Color[] GetLineColors()
        {
            return new Color[]
            {
                Color.white, // Pos
                Color.white, // Speed
                Color.white, // Ratio
                Color.white, // Dist
                Color.white, // Scale
            };
        }

    }
}