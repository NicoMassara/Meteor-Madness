using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay
{
    public class InputReader : MonoBehaviour
    {
        public float MovementDirection { get; private set; }

        private void Update()
        {
            if (GetIsLeftPressed())
            {
                MovementDirection = 1;
            }
            else if (GetIsRightPressed())
            {
                MovementDirection = -1;
            }
            else
            {
                MovementDirection = 0;
            }


            if (Input.GetKeyDown(KeyCode.P))
            {
                UpdateManager.Instance.IsPaused = !UpdateManager.Instance.IsPaused;
            }

            CustomTime.GetChannel(UpdateGroup.Gameplay).SetTimeScale(Input.GetKey(KeyCode.Y) ? 0.25f : 1f);
            CustomTime.GetChannel(UpdateGroup.Inputs).SetTimeScale(Input.GetKey(KeyCode.U) ? 0.25f : 1f);
            

        }

        private bool GetIsLeftPressed()
        {
            return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        }

        private bool GetIsRightPressed()
        {
            return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        }
    }
}