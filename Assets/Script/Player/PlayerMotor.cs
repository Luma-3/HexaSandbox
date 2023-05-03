using UnityEngine;
using Input;

namespace Motor
{
    namespace Player
    {
        [RequireComponent(typeof(PlayerInput))]
        public class PlayerMotor : PlayerInput
        {
            public float bottomClamp;
            public float topClamp;
            public float sensitivity;
            public GameObject head;

            float cinemachineYaw;
            float cinemachinePitch;

            GameObject mainCamera;
            float targetRotation;
            [SerializeField] float speed;
            [SerializeField] float moveSpeed;

            private void Start()
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            void FixedUpdate()
            {
                Movement();
            }

            private void LateUpdate()
            {
                Camera();
            }


            private void Camera()
            {

                if (LookVector.sqrMagnitude >= 0.01f) 
                {
                    cinemachineYaw += LookVector.x * 1f * sensitivity;
                    cinemachinePitch += LookVector.y * 1f * sensitivity;
                }

                cinemachineYaw = CampAngle(cinemachineYaw, float.MinValue, float.MaxValue);
                cinemachinePitch = CampAngle(cinemachinePitch, bottomClamp, topClamp);

                head.transform.rotation = Quaternion.Euler(cinemachinePitch,
                                                         cinemachineYaw, 0.0f);
            }

            private float CampAngle(float value, float min,  float max)
            {
                if (value < -360) value += 360;
                if (value > 360) value -= 360;
                return Mathf.Clamp(value, min, max);
            }

            private void Movement()
            {

                Vector2 movementV2 = new Vector2(MoveVector.x,MoveVector.z).normalized;

                float targetSpeed = moveSpeed;

                if (movementV2 != Vector2.zero)
                {
                    targetRotation = Mathf.Atan2(movementV2.x, movementV2.y) * Mathf.Rad2Deg +
                                  mainCamera.transform.eulerAngles.y;
                }
                else //stop player if no move input is pressed
                {
                    targetSpeed = 0.0f;
                }

                speed = targetSpeed;

                Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

                transform.Translate(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, MoveVector.normalized.y, 0.0f));
            }
        }
    }
}

