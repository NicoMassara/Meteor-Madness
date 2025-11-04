using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_MultiPageTextData_Name", menuName = "Scriptable Objects/UI/Multi Page", order = 0)]
    public class MultiPageTextDataSo : ScriptableObject, IMultiPageData
    {
        [SerializeField] private string textCode;
        [SerializeField] private int textCount;

        public string TextsCode => $"{textCode}.Texts";
        public int TextCount => textCount;
        public string LastButtonCode => $"{textCode}.LastButton";
        public int MaxTextIndex => textCount-1;
    }
}