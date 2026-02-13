using _Main.Scripts.DebugTools;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield;
using UnityEngine;

namespace MeteorMadness.Debug._Main.Scripts.DebugTools.GUIDebug
{
    public class ShieldRotationDebug : ComponentGuiDebug
    {
        [SerializeField] private bool isDebugEnable;
        
        protected override string DebugName => "Shield Rotation";
        protected override int DebugLines => 4;

        private ShieldRotationDebugData _data = new  ShieldRotationDebugData();


        protected override void Start()
        {
            base.Start();

            ShieldRotationDebugEvents.OnDebug += data =>
            {
                _data = data;
            };

        }

        protected override DebugGUIData[] GetDebugData()
        {
            DebugDataArray[0]
                .SetText($"State, C: {_data.State}, L: {_data.LastState}")
                .SetColor(Color.white);
            DebugDataArray[1]
                .SetText($"Input, A: {_data.InputAngle}, M: {_data.InputMagnitude}")
                .SetColor(Color.white);
            DebugDataArray[2]
                .SetText($"Target: {_data.TargetAngle}")
                .SetColor(Color.white);
            
            return DebugDataArray;
        }

        protected override bool GetCanShowGUI() => true;
        protected override Vector3 GetGUIPosition() => Vector2.zero;
        protected override bool GetIsDebugEnable() => isDebugEnable;
    }
}