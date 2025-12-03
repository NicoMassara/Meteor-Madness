using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Bootstrap
{
    public class BoostrapUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private TMP_Text assetsLoadingText;
        
        private void Awake()
        {
            var boostrap = GetComponent<IBoostrap>();

            boostrap.OnLoadingAsset += value =>
            {
                assetsLoadingText.text = value;
            };
        }

        private void Start()
        {
            StartCoroutine(AnimateDots());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }


        private IEnumerator AnimateDots()
        {
            string baseText = loadingText.text;
            int count = 0;

            while (true)
            {
                loadingText.text = baseText + new string('.', count);
                count = (count + 1) % 4;
                yield return new WaitForSeconds(0.25f);
            }
            
            // ReSharper disable once IteratorNeverReturns
        }
    }
}