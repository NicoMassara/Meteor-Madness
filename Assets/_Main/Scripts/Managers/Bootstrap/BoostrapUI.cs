using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace MeteorMadness.Managers.Boostrap
{
    public class BoostrapUI : MonoBehaviour
    {
        [SerializeField] private GameObject canvasObject;
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private TMP_Text assetsLoadingText;
        
        private void Awake()
        {
            canvasObject.SetActive(false);
            
            var boostrap = GetComponent<IBoostrap>();

            boostrap.OnStartLoading += () =>
            {
                canvasObject.SetActive(true);
            };

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