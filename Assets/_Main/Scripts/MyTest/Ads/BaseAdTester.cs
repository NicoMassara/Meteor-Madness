using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MyTest.Ads
{
    public abstract class BaseAdTester : MonoBehaviour
    {
        [SerializeField] protected Button openAddButton;
        
        private void Awake()
        {
            InitializeButton();
        }
        
        protected virtual void DisableButton()
        {
            openAddButton.interactable = false;
        }

        public virtual void EnableButton()
        {
            openAddButton.interactable = true;
        }
        
        private void InitializeButton()
        {
            openAddButton.onClick.AddListener(() =>
            {
                DisableButton();
                Button_OnClick();
            });
            DisableButton();
        }

        protected abstract void Button_OnClick();
    }
}