using MeteorMadness.ScreenFlow.Base;

namespace MeteorMadness.ScreenFlow.Interfaces
{
    public interface IPanelSelector<T> 
        where T : UiComponentsData
    {
        public T GetPanelData();
    }
}