using System;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities
{
    [RequireComponent(typeof(AbilityUIView))]
    [RequireComponent(typeof(AbilityViewAnimation))]
    public class AbilityUISetup : MonoBehaviour
    {
        #region Motor

        private class AbilityUIMotor
        {
        
        }

        #endregion

        //Todo: Implemente Ability MVC from Gameplay with This
        
        private AbilityUIView.IAbilityUIView _ui;
        private AbilityViewAnimation.IAbilityViewAnimation _animation;

        private void Awake()
        {
            var ui = GetComponent<AbilityUIView>();
            var anim = GetComponent<AbilityViewAnimation>();
            
            _ui = ui;
            _animation = anim;
        }
    }


}