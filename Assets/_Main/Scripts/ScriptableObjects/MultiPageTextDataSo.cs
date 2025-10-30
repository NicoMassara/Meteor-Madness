using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_MultiPageTextData_Name", menuName = "Scriptable Objects/UI/Multi Page", order = 0)]
    public class MultiPageTextDataSo : ScriptableObject, IMultiPageData
    {
        [Header("Texts")]
        [TextArea]
        [SerializeField] private string[] textsArray;
        [SerializeField] private string lastPageNextButtonText = "Finish";

        public string[] TextsArray => textsArray;
        public string LastPageNextButtonText => lastPageNextButtonText;
        public int MaxTextIndex => textsArray.Length - 1;
    }
}