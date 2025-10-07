using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    public class GameConfigUtilities
    {
        public static void UpdateArray<T>(ref T[] array, int maxSize)
        {
            if (array == null || array.Length != maxSize)
            {
                var oldArray = array;
                array = new T[maxSize];

                // Copiar valores existentes para no perder referencias
                if (oldArray != null)
                {
                    for (int i = 0; i < Mathf.Min(oldArray.Length, array.Length); i++)
                    {
                        array[i] = oldArray[i];
                    }
                }
            }
        }
        
        public static void UpdateArray<T>(ref T[] array, int maxSize, Action<int> setDefaultValues)
        {
            if (array == null || array.Length != maxSize)
            {
                var oldArray = array;
                array = new T[maxSize];
                setDefaultValues?.Invoke(maxSize);

                // Copiar valores existentes para no perder referencias
                if (oldArray != null)
                {
                    for (int i = 0; i < Mathf.Min(oldArray.Length, array.Length); i++)
                    {
                        array[i] = oldArray[i];
                    }
                }
            }
        }
    }
}