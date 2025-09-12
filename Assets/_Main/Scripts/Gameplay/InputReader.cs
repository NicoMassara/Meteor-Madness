using _Main.Scripts.MyCustoms;
using UnityEngine;

namespace _Main.Scripts.Gameplay
{
    public class InputReader : MonoBehaviour
    {
        public float MovementDirection { get; private set; }
        public bool IsPaused { get; private set; }

        private void Update()
        {
            IsPaused = Input.GetKeyDown(KeyCode.P);

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