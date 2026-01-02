using System;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Menu
{
    [ExecuteInEditMode]
    public class TextVersionSetter : MonoBehaviour
    {
        private void Start()
        {
            var text = GetComponent<TMP_Text>();
            text.text = $"Version: {Application.version}";
        }
    }
}