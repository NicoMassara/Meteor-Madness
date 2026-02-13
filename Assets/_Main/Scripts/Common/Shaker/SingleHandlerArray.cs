using UnityEngine;

namespace _Main.Scripts.Common
{
    public class SingleHandlerArray<T>
    {
        private readonly T[] _array;
        private int _maxAvailableIndex;

        public bool IsEmpty => _maxAvailableIndex == -1;

        public SingleHandlerArray(T[] array)
        {
            _array = array;
            RestartIndex();
        }

        public void RestartIndex()
        {
            _maxAvailableIndex = _array.Length - 1;
        }
        
        public void IncreaseMaxIndex()
        {
            _maxAvailableIndex++;
            _maxAvailableIndex = Mathf.Clamp(_maxAvailableIndex, 0, _array.Length - 1);
        }

        public void DecreaseMaxIndex()
        {
            if(IsEmpty) return;
            
            _maxAvailableIndex--;
            _maxAvailableIndex = Mathf.Clamp(_maxAvailableIndex, -1, _array.Length - 1);
        }

        public T GetLastItem()
        {
            if(IsEmpty) return default;
            
            T selectedItem = _array[_maxAvailableIndex];
            
            DecreaseMaxIndex();
            
            return selectedItem;
        }

        public T GetFirstItem()
        {
            if(IsEmpty) return default;
            
            T selectedItem = _array[0];
            T tempItem = _array[_maxAvailableIndex];
            _array[_maxAvailableIndex] = selectedItem;
            _array[0] = tempItem;
            
            DecreaseMaxIndex();
            
            return selectedItem;
        }

        public T GetRandomItem()
        {
            if(IsEmpty) return default;
            
            int randomIndex = Random.Range(0, _maxAvailableIndex+1);
            T selectedItem = _array[randomIndex];
            T tempItem = _array[_maxAvailableIndex];
            _array[_maxAvailableIndex] = selectedItem;
            _array[randomIndex] = tempItem;
            DecreaseMaxIndex();
            
            return selectedItem;
        }
    }
}