using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.DebugTools
{
    public class DebugUIMeteor : MonoBehaviour
    {
        [Header("Amount Components")]
        [SerializeField] private Button increaseAmount;
        [SerializeField] private Button decreaseAmount;
        [SerializeField] private TMP_InputField amountText;
        [Header("Spawn Components")]
        [SerializeField] private Button spawnSingle;
        [SerializeField] private Button spawnRing;
        
        public UnityAction OnSingleSpawned;
        public UnityAction OnRingSpawned;

        private int _amountToSpawn = 1;

        private void Awake()
        {
            spawnSingle.onClick.AddListener(() =>
            {
                for (int i = 0; i < _amountToSpawn; i++)
                {
                    OnSingleSpawned?.Invoke();
                }
            });
            spawnRing.onClick.AddListener(() =>
            {
                OnRingSpawned?.Invoke();
            });
            
            increaseAmount.onClick.AddListener(() =>
            {
                _amountToSpawn++;
                amountText.text = _amountToSpawn.ToString();
                _amountToSpawn = Mathf.Clamp(_amountToSpawn, 1, 100);
            });
            decreaseAmount.onClick.AddListener(() =>
            {
                _amountToSpawn--;
                _amountToSpawn = Mathf.Clamp(_amountToSpawn, 1, 100);
                amountText.text = _amountToSpawn.ToString();
            });

            amountText.onValueChanged.AddListener(text =>
            {
                _amountToSpawn = int.Parse(text);
                _amountToSpawn = Mathf.Clamp(_amountToSpawn, 1, 100);
            });
            
            amountText.contentType = TMP_InputField.ContentType.DecimalNumber;
            amountText.text = _amountToSpawn.ToString();
        }
    }
}