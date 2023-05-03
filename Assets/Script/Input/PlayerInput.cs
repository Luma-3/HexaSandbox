using Motor.Player;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Input
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerInput : MonoBehaviour
    {
        PlayerInputMap inputs;

        public Vector2 LookVector { get; private set; }
        public Vector3 MoveVector { get; private set; }

        private void Awake()
        {
            inputs = new PlayerInputMap();
        }

        void OnEnable()
        {
            inputs.Enable();
            inputs.PhysicInput.Enable();
            inputs.PhysicInput.Move.performed += OnMove;
            inputs.PhysicInput.Move.canceled += OnMove;
            inputs.PhysicInput.Look.performed += OnLook;
            inputs.PhysicInput.Look.canceled += OnLook;

        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            MoveVector = ctx.ReadValue<Vector3>();
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            LookVector = ctx.ReadValue<Vector2>();
        }
    }
}



