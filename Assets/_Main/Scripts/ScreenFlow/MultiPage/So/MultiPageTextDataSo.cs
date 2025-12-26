using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Managers.Localization;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.MultiPage
{
    [CreateAssetMenu(fileName = "SO_MultiPageTextData_Name", menuName = "Scriptable Objects/Multi Page", order = 0)]
    public class MultiPageTextDataSo : ScriptableObject, IMultiPageData
    {
        [SerializeField] private string textCode;

        public string TextsCode => $"{textCode}.Texts";
        public int TextCount => GetTextCount();
        public string LastButtonCode => $"{textCode}.LastButton";
        public int MaxTextIndex => GetTextCount()-1;

        private int GetTextCount()
        {
            return LocalizationManager.Instance.GetArrayLength(TextsCode);
        }
    }
}