using _Main.Scripts.Interfaces;
using _Main.Scripts.Localization;
using UnityEngine;

namespace _Main.Scripts.MultiPage
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