using System;
using UnityEngine;

namespace Plugins.Demigiant.DOTween
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine.UI;

    public static class DOTweenTMPExtensions
    {
        /// <summary>
        /// Anima un entero y actualiza el texto (TMP) durante la duración.
        /// format: opcional, ejemplo "Score: {0:N0}" (N0 = separador de miles).
        /// </summary>
        public static Tweener DOCounter(this TMP_Text target, string baseText ,int fromValue, int endValue, float duration, string format = "{0}")
        {
            int v = fromValue;
            return DOTween.To(() => v, x =>
            {
                v = x;
                target.text = $"{baseText}{string.Format(format, v)}";
            }, endValue, duration).SetTarget(target);
        }

        /// <summary>
        /// Mismo para UnityEngine.UI.Text
        /// </summary>
        public static Tweener DOCounter(this Text target, int fromValue, int endValue, float duration, string format = "{0}")
        {
            int v = fromValue;
            return DOTween.To(() => v, x =>
            {
                v = x;
                target.text = string.Format(format, v);
            }, endValue, duration).SetTarget(target);
        }
    }
    
    public static class TweenUtils
    {
        public static Tweener AnimateScore(Action<TMP_Text,long> action, TMP_Text text, 
            long targetScore, float duration, Ease ease = Ease.OutQuad)
        {
            return DOTween.To(
                () => 0,
                x => action(text,(long)x),
                targetScore,  
                duration 
            ).SetEase(ease);
        }
            
        public static float GetDurationLinear(int number, float baseTime = 0.5f, float timePerUnit = 0.01f)
        {
            return baseTime + number * timePerUnit;
        }
            
        public static float GetDurationLog(uint number, float scale = 0.15f)
        {
            return Mathf.Log(number + 1) * scale;
        }
    }
}