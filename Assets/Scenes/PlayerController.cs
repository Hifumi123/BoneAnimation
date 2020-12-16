using UnityEngine;

public enum MovementState
{
    Stand,
    Crouch,
    Jump
}

public enum JumpState
{
    None,
    Up,
    Loop,
    Down
}

public class PlayerController : MonoBehaviour
{
    public Transform gunHandTransform;

    public float maxWalkSpeed = 2;

    public float maxRunSpeed = 4.5f;

    public float maxSprintSpeed = 7;

    public float maxSpeed = 0;

    public float acceleration = 0.2f;

    public float drag = 0.5f;

    public float rotationSpeed = 6.5f;

    public float maxUpSpeed = 6;

    public float jumpXZSpeedScale = 0.02f;

    private CharacterController m_CharacterController;

    private Animator m_Animator;

    private Vector2 m_MovementXZ = Vector2.zero;

    private bool m_HaveMoved = true;

    private MovementState m_MovementState = MovementState.Stand;

    private JumpState m_JumpState = JumpState.None;

    private float m_UpSpeed = 0;

    private float m_GravityAcceleration = -20;

    private Vector3 m_MovementForJump = Vector3.zero;

    private bool m_HasJumpAgain = false;

    void Start()
    {
        maxSpeed = maxRunSpeed;

        m_CharacterController = GetComponent<CharacterController>();

        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            if (m_MovementState == MovementState.Crouch)
            {
                m_MovementState = MovementState.Stand;

                m_Animator.SetInteger("MovementState", 0);
            }
            else if (m_MovementState == MovementState.Stand)
            {
                m_MovementState = MovementState.Crouch;

                m_Animator.SetInteger("MovementState", 1);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (m_MovementState == MovementState.Crouch)
            {
                m_MovementState = MovementState.Stand;

                m_JumpState = JumpState.None;

                m_Animator.SetInteger("MovementState", 0);
            }
            else if (m_MovementState == MovementState.Stand)
            {
                m_MovementState = MovementState.Jump;

                m_JumpState = JumpState.Up;

                m_Animator.SetInteger("MovementState", 2);

                m_UpSpeed = maxUpSpeed;
            }
            else
            {
                if (m_JumpState == JumpState.Up && !m_HasJumpAgain)
                {
                    m_JumpState = JumpState.Loop;

                    m_Animator.SetInteger("MovementState", 4);

                    m_UpSpeed = maxUpSpeed * 1.5f;

                    m_HasJumpAgain = true;
                }
            }
        }

        if (m_MovementState == MovementState.Stand)
        {
            if (Input.GetKey(KeyCode.LeftControl))
                maxSpeed = maxWalkSpeed;
            else if (Input.GetKey(KeyCode.LeftShift))
                maxSpeed = maxSprintSpeed;
            else
                maxSpeed = maxRunSpeed;
        }

        if (m_HaveMoved)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            if (x > 0)
            {
                m_MovementXZ.x += acceleration;

                if (m_MovementXZ.x > maxSpeed)
                    m_MovementXZ.x = maxSpeed;

                m_HaveMoved = false;
            }
            else if (x < 0)
            {
                m_MovementXZ.x -= acceleration;

                if (m_MovementXZ.x < -maxSpeed)
                    m_MovementXZ.x = -maxSpeed;

                m_HaveMoved = false;
            }
            else if (m_MovementXZ.x > 0)
            {
                m_MovementXZ.x -= drag;

                if (m_MovementXZ.x < 0)
                    m_MovementXZ.x = 0;
                
                m_HaveMoved = false;
            }
            else if (m_MovementXZ.x < 0)
            {
                m_MovementXZ.x += drag;

                if (m_MovementXZ.x > 0)
                    m_MovementXZ.x = 0;
                
                m_HaveMoved = false;
            }

            if (y > 0)
            {
                m_MovementXZ.y += acceleration;

                if (m_MovementXZ.y > maxSpeed)
                    m_MovementXZ.y = maxSpeed;

                m_HaveMoved = false;
            }
            else if (y < 0)
            {
                m_MovementXZ.y -= acceleration;

                if (m_MovementXZ.y < -maxSpeed)
                    m_MovementXZ.y = -maxSpeed;

                m_HaveMoved = false;
            }
            else if (m_MovementXZ.y > 0)
            {
                m_MovementXZ.y -= drag;

                if (m_MovementXZ.y < 0)
                    m_MovementXZ.y = 0;
                
                m_HaveMoved = false;
            }
            else if (m_MovementXZ.y < 0)
            {
                m_MovementXZ.y += drag;

                if (m_MovementXZ.y > 0)
                    m_MovementXZ.y = 0;
                
                m_HaveMoved = false;
            }
        }

        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");

            transform.Rotate(0, x * rotationSpeed, 0);
        }

        if (Input.GetKeyUp(KeyCode.R))
            m_Animator.SetInteger("UpperState", 1);
    }

    private void FixedUpdate()
    {
        if (m_MovementState != MovementState.Jump && !m_HaveMoved)
        {
            m_Animator.SetFloat("RightSpeed", m_MovementXZ.x);
            m_Animator.SetFloat("ForwardSpeed", m_MovementXZ.y);

            m_HaveMoved = true;
        }

        if (m_MovementState == MovementState.Jump && m_JumpState == JumpState.Down && m_CharacterController.isGrounded)
        {
            m_MovementState = MovementState.Stand;

            m_UpSpeed = 0;

            m_JumpState = JumpState.None;

            m_MovementForJump.y = 0;

            m_Animator.SetInteger("MovementState", 0);

            m_HasJumpAgain = false;
        }

        if (m_JumpState == JumpState.Up)
        {
            if (m_UpSpeed <= 0)
            {
                m_UpSpeed = 0;

                m_JumpState = JumpState.Down;

                m_Animator.SetInteger("MovementState", 3);
            }
            else
            {
                m_MovementForJump.y = m_UpSpeed * Time.fixedDeltaTime;

                if (!m_HaveMoved)
                {
                    m_MovementForJump.x = m_MovementXZ.x * jumpXZSpeedScale;
                    m_MovementForJump.z = m_MovementXZ.y * jumpXZSpeedScale;

                    m_HaveMoved = true;

                    m_Animator.SetFloat("RightSpeed", m_MovementXZ.x);
                    m_Animator.SetFloat("ForwardSpeed", m_MovementXZ.y);
                }

                m_CharacterController.Move(transform.TransformVector(m_MovementForJump));

                m_MovementForJump.x = 0;
                m_MovementForJump.z = 0;

                m_UpSpeed += m_GravityAcceleration * Time.fixedDeltaTime;
            }
        }
        else if (m_JumpState == JumpState.Down)
        {
            m_MovementForJump.y = m_UpSpeed * Time.fixedDeltaTime;

            if (!m_HaveMoved)
            {
                m_MovementForJump.x = m_MovementXZ.x * jumpXZSpeedScale;
                m_MovementForJump.z = m_MovementXZ.y * jumpXZSpeedScale;

                m_HaveMoved = true;

                m_Animator.SetFloat("RightSpeed", m_MovementXZ.x);
                m_Animator.SetFloat("ForwardSpeed", m_MovementXZ.y);
            }

            m_CharacterController.Move(transform.TransformVector(m_MovementForJump));

            m_MovementForJump.x = 0;
            m_MovementForJump.z = 0;

            m_UpSpeed += m_GravityAcceleration * Time.fixedDeltaTime;
        }
        else if (m_JumpState == JumpState.Loop)
        {
            if (m_UpSpeed <= 0)
            {
                m_UpSpeed = 0;

                m_JumpState = JumpState.Down;

                m_Animator.SetInteger("MovementState", 5);
            }
            else
            {
                m_MovementForJump.y = m_UpSpeed * Time.fixedDeltaTime;

                if (!m_HaveMoved)
                {
                    m_MovementForJump.x = m_MovementXZ.x * jumpXZSpeedScale;
                    m_MovementForJump.z = m_MovementXZ.y * jumpXZSpeedScale;

                    m_HaveMoved = true;

                    m_Animator.SetFloat("RightSpeed", m_MovementXZ.x);
                    m_Animator.SetFloat("ForwardSpeed", m_MovementXZ.y);
                }

                m_CharacterController.Move(transform.TransformVector(m_MovementForJump));

                m_MovementForJump.x = 0;
                m_MovementForJump.z = 0;

                m_UpSpeed += m_GravityAcceleration * Time.fixedDeltaTime;
            }
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex == 1 && gunHandTransform != null)
        {
            m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, gunHandTransform.position);
            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        }
    }
}
