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
}