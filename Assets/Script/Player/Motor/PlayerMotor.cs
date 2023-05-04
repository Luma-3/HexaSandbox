using Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Motor
{ 
    [RequireComponent(typeof(PlayerInput))] [RequireComponent(typeof(Rigidbody))]
    public class PlayerMotor : PlayerInput
    {
        
        [Header("View")]
        [SerializeField] private float bottomClamp;
        [SerializeField] private float topClamp;
        [SerializeField] private float sensitivity;
        [SerializeField] private GameObject head;
        [SerializeField] private GameObject body;
        private float _cineMachineYaw;
        private float _cineMachinePitch;
        private float _targetRotation;
        
        [Header("HorizontalMovement")]
        [SerializeField] private float normalMaxVelocity;
        [SerializeField] private float moveMultiplier;
        [SerializeField] private float sprintMaxVelocity;
        [SerializeField] private float frictionForce;
        
        [Header("GroundCheck")]
        [SerializeField] private float radius;
        [SerializeField] private new CapsuleCollider collider;
        [SerializeField] private LayerMask groundMask;
        private bool _isGrounded;
        
        [Header("Gravity")]
        [SerializeField] private float initialForce;
        [SerializeField] private float forceMultiplier;
        [SerializeField] private float gravityMaxForce;
        private float _gravity;

        [Header("Jump")]
        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpBufferTime;
        private float _jumpBuffer; 
        private bool _isJumping;
        
        private Rigidbody _rigidbody;

        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            GroundCheck();
            CalculateGravity();
            CalculateMovement();
            Jump();
            PassThrough();
        }

        private void PassThrough()
        {
            var position = transform.position;
            var origin = new Vector3(position.x, collider.bounds.min.y, position.z);
            var ray = new Ray(origin, Vector3.forward);
            Physics.Raycast(ray, 1f, groundMask);
            Debug.DrawRay(origin,Vector3.forward,Color.red,0.5f);
        }

        private void LateUpdate()
        { 
            Camera();
            BodyRotation();
        }

        private void BodyRotation()
        {
            var headRotation = head.transform.rotation;
            body.transform.rotation = new Quaternion(0.0f, headRotation.y, 0.0f, 0.0f);
        }

        private void Camera()
        {

            if (LookVector.sqrMagnitude >= 0.01f) 
            {
                _cineMachineYaw += LookVector.x * 1f * sensitivity;
                _cineMachinePitch += LookVector.y * 1f * sensitivity;
            }

            _cineMachineYaw = CampAngle(_cineMachineYaw, float.MinValue, float.MaxValue);
            _cineMachinePitch = CampAngle(_cineMachinePitch, bottomClamp, topClamp);

            head.transform.rotation = Quaternion.Euler(_cineMachinePitch,
                                                        _cineMachineYaw, 0.0f);
        }

        private static float CampAngle(float value, float min,  float max)
        {
            if (value < -360) value += 360;
            if (value > 360) value -= 360;
            return Mathf.Clamp(value, min, max);
        }

        private void CalculateMovement()
        {

            var movementV2 = new Vector2(MoveVector.x,MoveVector.z).normalized;

            var targetMultiplier = moveMultiplier;
            
            var maxVelocity = SprintBool ? sprintMaxVelocity : normalMaxVelocity;

            if (movementV2 != Vector2.zero)
            { 
                _targetRotation = Mathf.Atan2(movementV2.x, movementV2.y) * Mathf.Rad2Deg +
                                head.transform.eulerAngles.y;
            }
            else //stop player if no move input is pressed
            {
                Friction();
                targetMultiplier = 0.0f;
            }

            var speed = targetMultiplier;
            
            ApplyMovement(speed);
            ClampHorizontalVelocity(maxVelocity);
        }

        private void Friction()
        {
            var velocity = _rigidbody.velocity;
            var friction = velocity * (Time.deltaTime * frictionForce);
            velocity -= friction;
            _rigidbody.velocity = velocity;
        }

        private void ClampHorizontalVelocity(float maxVelocity)
        {
            var rbVelocity = _rigidbody.velocity;
            rbVelocity.x = Mathf.Clamp(rbVelocity.x, -maxVelocity, maxVelocity);
            rbVelocity.z = Mathf.Clamp(rbVelocity.z, -maxVelocity, maxVelocity);
            _rigidbody.velocity = rbVelocity;
        }

        private void ApplyMovement(float speed)
        {
            var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            _rigidbody.AddForce(targetDirection.normalized * speed + new Vector3(0f,MoveVector.normalized.y,0f));
        }
        
        private void GroundCheck()
        {
            var position = transform.position;
            var spherePosition = new Vector3(position.x, collider.bounds.min.y, position.z);
            _isGrounded = Physics.CheckSphere(spherePosition, radius, groundMask);
        }

        private void CalculateGravity()
        {
            if (_isGrounded)
            {
                _gravity = initialForce;
            }
            else if (_gravity <= gravityMaxForce)
            {
                _gravity += initialForce * forceMultiplier;
            }
            
            _rigidbody.AddForce(Vector3.down * _gravity);
        }
        
        private void Jump()
        {
            if (_isGrounded)
            {
                if (JumpBool && !_isJumping && _jumpBuffer <= 0f)
                {
                    _isJumping = true;
                    _rigidbody.AddForce(Vector3.up * (jumpForce * _gravity), ForceMode.Impulse);
                }
                else if (_isJumping)
                {
                    _isJumping = false;
                    _jumpBuffer = jumpBufferTime;
                }
            }
            JumpTimer();
        }

        private void JumpTimer()
        {
            if (_jumpBuffer >= 0f)
            {
                _jumpBuffer -= Time.deltaTime;
            } 
        }
        
    }
}

