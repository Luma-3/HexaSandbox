using Player.Motor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerInput : MonoBehaviour
    {
        private PlayerInputMap.PhysicInputActions _inputs;

        protected Vector2 LookVector { get; private set; }
        protected Vector2 MoveVector { get; private set; }
        protected bool SprintBool { get; private set; }
        protected bool JumpBool { get; private set; }

        private void OnEnable()
        {
            _inputs = new PlayerInputMap().PhysicInput;
            _inputs.Enable();
            _inputs.Move.performed += OnMove;
            _inputs.Move.canceled += OnMove;
            _inputs.Look.performed += OnLook;
            _inputs.Look.canceled += OnLook;
            _inputs.Sprint.started += OnSprint;
            _inputs.Sprint.canceled += OnSprint;
            _inputs.Jump.started += OnJump;
            _inputs.Jump.canceled += OnJump;

        }

        private void OnDisable()
        {
            _inputs.Disable();
            _inputs.Move.performed -= OnMove;
            _inputs.Move.canceled -= OnMove;
            _inputs.Look.performed -= OnLook;
            _inputs.Look.canceled -= OnLook;
            _inputs.Sprint.started -= OnSprint;
            _inputs.Sprint.canceled -= OnSprint;
            _inputs.Jump.started -= OnJump;
            _inputs.Jump.canceled -= OnJump;

        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            MoveVector = ctx.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            LookVector = ctx.ReadValue<Vector2>();
        }

        private void OnSprint(InputAction.CallbackContext ctx)
        {
            SprintBool = ctx.started;
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            JumpBool = ctx.started;
        }
    }
}



