using Input;
using Script.Map;
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
        
        [Header("HorizontalMovement")]
        [SerializeField] private float moveMultiplier;
        [SerializeField] private float normalMaxVelocity;
        [SerializeField] private float sprintMaxVelocity;
        [SerializeField] [Range(0.0001f, 2f)] private float noGroundedVelocityMultiplier;
        [SerializeField] private float frictionForce;
        private float _targetRotation;
        private float _rotationVelocity;
        
        [Header("GroundCheck")]
        [SerializeField] private float radius;
        [SerializeField] private CapsuleCollider bodyCollider;
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
        
        [Header("Pass Through")]
        [SerializeField] private float maxHeightPassThrough;
        
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

        private void LateUpdate()
        { 
            Camera();
            BodyRotation();
        }
        
        
        
        private static float CampAngle(float value, float min,  float max)
        {
            if (value < -360) value += 360;
            if (value > 360) value -= 360;
            return Mathf.Clamp(value, min, max);
        }
        
        private void BodyRotation()
        {
            var headRotationY = head.transform.eulerAngles.y;

            body.transform.rotation = Quaternion.Euler(0.0f, headRotationY, 0.0f);
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
        
        

        private void CalculateMovement()
        {

            var movementV2 = new Vector2(MoveVector.x,MoveVector.y).normalized;

            var targetMultiplier = moveMultiplier;
            
            var maxVelocity = SprintBool ? sprintMaxVelocity : normalMaxVelocity;
            maxVelocity = _isGrounded ? maxVelocity : maxVelocity * noGroundedVelocityMultiplier;

            if (MoveVector != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(movementV2.x, movementV2.y) * Mathf.Rad2Deg +
                                  head.transform.eulerAngles.y;
                
            }
            else //stop player if no move input is pressed
            {
                targetMultiplier = 0.0f;
            }

            var speed = targetMultiplier;
            
            Friction();
            ApplyMovement(speed);
            ClampHorizontalVelocity(maxVelocity);
            
        }
        
        
        
        private void ApplyMovement(float speed)
        {
            var targetDirection = Quaternion.Euler(0f, _targetRotation, 0f) * Vector3.forward;
            
            _rigidbody.AddForce(targetDirection.normalized * (speed * 10f), ForceMode.Force);
        }

        private void Friction()
        {
            var rbVelocity = _rigidbody.velocity;
            
            var velocity = new Vector2(rbVelocity.x,rbVelocity.z);
            var friction = velocity * (Time.deltaTime * frictionForce);
            
            velocity -= friction;
            _rigidbody.velocity = new Vector3(velocity.x,rbVelocity.y,velocity.y);
        }

        private void ClampHorizontalVelocity(float maxVelocity)
        {
            var rbVelocity = _rigidbody.velocity;
            var newVelocity = new Vector2(rbVelocity.x,rbVelocity.z);
            newVelocity = Vector2.ClampMagnitude(newVelocity, maxVelocity);
            _rigidbody.velocity = new Vector3(newVelocity.x,rbVelocity.y,newVelocity.y);
        }
        
        
        private void PassThrough()
        {
            var transformPos = transform.position;

            var origin = new Vector3(transformPos.x, bodyCollider.bounds.min.y, transformPos.z);
            var direction = Quaternion.Euler(0.0f, head.transform.localEulerAngles.y, 0.0f) * Vector3.forward;
            
            if (Physics.Raycast(origin, direction, out var hit, 1.5f, groundMask))
            {
                var heightDif = hit.transform.GetComponent<MeshCollider>().bounds.max.y - bodyCollider.bounds.min.y;

                if (heightDif <= maxHeightPassThrough && MoveVector.y != 0f)
                {

                    transformPos.y += heightDif;
                    transform.position = transformPos;
                }
            }

        }

        
        private void GroundCheck()
        {
            var position = transform.position;
            var spherePosition = new Vector3(position.x, bodyCollider.bounds.min.y, position.z);
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

