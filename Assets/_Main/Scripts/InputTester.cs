using _Main.Scripts.Managers;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts
{
    public class InputTester : MonoBehaviour
    {
        [SerializeField] private AbilityType abilityToAdd;

        private bool _timeScaleHalved;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                AddAbility();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Deflect();
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                ChangeTimeScale();
            }
        }

        private void Kill()
        {
            for (int i = 0; i < 10; i++)
            {
                ProjectileEventCaller.Collision(new CollisionData());
            }
        }
        
        private void AddAbility()
        {
            AbilitiesEventCaller.Add(new AbilityAddData{AbilityType = abilityToAdd});
        }

        private void Deflect()
        {
            ProjectileEventCaller.Deflected(new DeflectData{Value = 1f});
        }

        private void ChangeTimeScale()
        {
            _timeScaleHalved = !_timeScaleHalved;
            
            CustomTime.GlobalTimeScale = _timeScaleHalved ? 1f : 0.1f;
            CustomTime.GlobalFixedTimeScale = _timeScaleHalved ? 1f : 0.1f;
        }
    }
}