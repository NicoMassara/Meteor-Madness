
using UnityEngine;

namespace _Main.Scripts.Inputs
{
    public class DesktopInput : DeviceInput<IDesktopInputData>
    {
        public DesktopInput(IDesktopInputData inputData, Camera mainCamera)
            : base(inputData, mainCamera)
        {
        }

        protected override bool GetIsPressingToMove() => Input.GetMouseButton(0);
        protected override Vector2 GetInputPressPosition() => GetMousePosition();
        protected override Vector2 GetInputPosition() => GetMousePosition();
        
        private Vector2 GetMousePosition() => Input.mousePosition;
    }
}