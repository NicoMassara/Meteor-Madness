using System.Collections.Generic;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.Managers.UpdateManager.Interfaces;

namespace _Main.Scripts.Gameplay.Abilies
{
    public class AbilityController : ManagedBehavior, IUpdatable
    {
        private Dictionary<AbilityType, AbilityData> _abilities = new Dictionary<AbilityType, AbilityData>();
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Ability;


        public void TriggerAbility(AbilityType enumType)
        {
            
        }

        public void ManagedUpdate()
        {
            
        }

        private void CreateAbilityData()
        {
            #region TotalShield
            
            //Start Queue
            ActionQueue shieldStartQueue = new ActionQueue(new []
            {
                new ActionData(() =>
                {
                    //Disable Player Inputs
                }, 0f),
                new ActionData(() =>
                {
                    // Starts to decrease Gameplay TimeScale
                    // until stops
                }, 0f),
                new ActionData(() =>
                {
                    // Zooms In
                }, 1f),
                new ActionData(() =>
                {
                    // Shield Starts to Move 
                    // Extremely Quick
                }, 1f),
                new ActionData(() =>
                {
                    // Changes to Total Shield Sprite
                }, 1f),
                new ActionData(() =>
                {
                    // Spawns Ring Meteor
                }, 1f),
                new ActionData(() =>
                {
                    // Zooms Out
                }, 1f),
                new ActionData(() =>
                {
                    // Starts to increase Gameplay TimeScale
                    // Until reaches 1
                }, 1f),
            });
            
            //End Queue
            ActionQueue shieldEndQueue = new ActionQueue(new []
            {
                new ActionData(() =>
                {
                    // 
                }, 0f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
            });

            var shieldData = new AbilityData
            {
                AbilityType = AbilityType.TotalShield,
                StartActionQueue = shieldStartQueue,
                EndActionQueue = shieldEndQueue,
            };
            
            _abilities.Add(shieldData.AbilityType, shieldData);

            #endregion
            
            #region SlowMotion

            //TotalShield
            ActionQueue slowQueue = new ActionQueue();
            ActionData[] slowActionData = new []
            {
                new ActionData(() =>
                {
                    //Disable Player Inputs
                    //Stops GameplayTime
                }, 0f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
                new ActionData(() =>
                {
                    //
                }, 1f),
            };

            #endregion
        }
        
    }

    public enum AbilityType
    {
        TotalShield,
        SlowMotion,
        Health
    }

    public class AbilityData
    {
        public AbilityType AbilityType;
        public ActionQueue StartActionQueue;
        public ActionQueue EndActionQueue;
        

        public ActionQueue GetStartActionQueue()
        {
            return StartActionQueue;
        }

        public ActionQueue GetEndActionQueue()
        {
            return EndActionQueue;
        }
    }
}