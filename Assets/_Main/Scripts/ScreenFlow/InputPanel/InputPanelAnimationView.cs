using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel.UISelector;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;

namespace MeteorMadness.ScreenFlow._Main.Scripts.ScreenFlow.InputPanel
{
    public class InputPanelAnimationView : BaseViewAnimation<InputPanelUIAnimationSelector,InputPanelUIAnimationComponents>
    {
        public interface IInputPanelAnimationView : IBaseViewAnimation
        {
            
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case InputPanelObserverMessage.Enable:
                    HandleEnable();
                    break;
                
                case InputPanelObserverMessage.Disable:
                    HandleDisable();
                    break;
            }
        }

        private void HandleEnable()
        {

        }

        private void HandleDisable()
        {

        }
    }
}