using _Main.Scripts.MyInputs;
using _Main.Scripts.Shield.Rotation;
using UnityEngine;

namespace _Main.Scripts.MyTest.Inputs
{
    public class InputMovementTest : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Transform spriteContainer;
        
        [SerializeField] private RotationDataSo rotationData;
        private IShieldMovement _shieldMovement;


        private void Awake()
        {
            _shieldMovement = new RotationMovement(rotationData,spriteContainer);
        }

        private void Start()
        {
            var inputReader = GetComponent<InputReader>();
            inputReader.OnMovementDirectionChanged += (value) =>
            {
                _shieldMovement.SetDirection(value);
            };
        }

        private void Update()
        {
            _shieldMovement.ExecuteMovement(Time.deltaTime);
        }
    }
    
}
