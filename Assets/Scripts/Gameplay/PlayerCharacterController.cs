using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
	[Header("References")]
	public Camera PlayerCamera;

	[Header("General")]
	[Tooltip("Force applied downward when in the air")]
	public float GravityDownForce = 20f;

	[Tooltip("Physic layers checked to consider the player grounded")]
	public LayerMask GroundCheckLayers = -1;

	[Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
	public float GroundCheckDistance = 0.05f;

	[Header("Movement")]
	[Tooltip("Max movement speed when grounded (when not sprinting)")]
	public float MaxSpeedOnGround = 10f;

	[Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
	public float MovementSharpnessOnGround = 15;

	[Tooltip("Max movement speed when not grounded")]
	public float MaxSpeedInAir = 10f;

	[Tooltip("Acceleration speed when in the air")]
	public float AccelerationSpeedInAir = 25f;

	[Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
	public float SprintSpeedModifier = 2f;

	[Tooltip("Height at which the player dies instantly when falling off the map")]
	public float KillHeight = -50f;

	[Header("Rotation")]
	[Tooltip("Rotation speed for moving the camera")]
	public float RotationSpeed = 200f;

	[Header("Jump")]
	[Tooltip("Force applied upward when jumping")]
	public float JumpForce = 9f;

	[Header("Stance")]
	[Tooltip("Ratio (0-1) of the character height where the camera will be at")]
	public float CameraHeightRatio = 0.95f;

	public bool IsGrounded { get; private set; }
	public bool HasJumpedThisFrame { get; private set; }
	public Vector3 CharacterVelocity { get; set; }
	public float Scale { get; set; } = 1f;


	Actor m_Actor;
	CharacterController m_Controller;
	PlayerInputHandler m_InputHandler;

	Vector3 m_GroundNormal;
	Vector3 m_LatestImpactSpeed;

	float speedModifier = 1f;
	float m_CameraVerticalAngle = 0f;
	float m_LastTimeJumped;
	float m_TargetCharacterHeight = 1.8f;
	//float m_TargetCharacterScale = 1f;
	const float k_JumpGroundingPreventionTime = 0.2f;
	const float k_GroundCheckDistanceInAir = 0.07f;
	void Awake()
	{
		ActorsManager actorsManager = FindObjectOfType<ActorsManager>();
		if (actorsManager != null)
			actorsManager.SetPlayer(gameObject);
	}

	void Start()
	{
		m_Actor = GetComponent<Actor>();
		m_Controller = GetComponent<CharacterController>();
		m_InputHandler = GetComponent<PlayerInputHandler>();
		m_Controller.enableOverlapRecovery = true;
		UpdateCharacterHeight(true);
	}

	private void Update()
	{
		HasJumpedThisFrame = false;
		//GroundCheck();
		UpdateCharacterHeight(false);
		HandleCharacterMovement();
		//GetComponent<Transform>().localScale = Vector3.one * Scale;
	}

	void HandleCharacterMovement()
	{
		// horizontal character rotation
		{
			// rotate the transform with the input speed around its local Y axis
			transform.Rotate(
				new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * RotationSpeed),
					0f), Space.Self);
		}

		// vertical camera rotation
		{
			// add vertical inputs to the camera's vertical angle
			m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed;

			// limit the camera's vertical angle to min/max
			m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

			// apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
			PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0) * -1f;
		}

		// character movement handling
		{
			// converts move input to a worldspace vector based on our character's transform orientation
			Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

			// handle grounded movement
			if (m_Controller.isGrounded)
			{
				// calculate the desired velocity from inputs, max speed, and current slope
				Vector3 targetVelocity = worldspaceMoveInput * MaxSpeedOnGround * speedModifier * Scale;

				// smoothly interpolate between our current velocity and the target velocity based on acceleration speed
				CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
					MovementSharpnessOnGround * Time.deltaTime);

				// jumping
				if (m_InputHandler.GetJumpInputDown())
				{
					
					// start by canceling out the vertical component of our velocity
					CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

					// then, add the jumpSpeed value upwards
					CharacterVelocity += Vector3.up * JumpForce * Scale;

					// remember last time we jumped because we need to prevent snapping to ground for a short time
					m_LastTimeJumped = Time.time;
					HasJumpedThisFrame = true;

					// Force grounding to false
					//IsGrounded = false;
					m_GroundNormal = Vector3.up;					
				}
			}	
			// handle air movement
			else
			{
				// add air acceleration
				CharacterVelocity += worldspaceMoveInput * AccelerationSpeedInAir * Time.deltaTime;

				// limit air speed to a maximum, but only horizontally
				float verticalVelocity = CharacterVelocity.y;
				Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
				horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
				CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

				// apply the gravity to the velocity
				CharacterVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
			}
		}

        m_Controller.Move(CharacterVelocity * Time.deltaTime);
		// apply the final calculated velocity value as a character movement
		Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);

        // detect obstructions to adjust velocity accordingly
        m_LatestImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius,
            CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            // We remember the last impact speed because the fall damage logic might need it
            m_LatestImpactSpeed = CharacterVelocity;

            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
        }
    }

	// Gets the center point of the bottom hemisphere of the character controller capsule    
	Vector3 GetCapsuleBottomHemisphere()
	{
		return transform.position + (transform.up * m_Controller.radius);
	}

	// Gets the center point of the top hemisphere of the character controller capsule    
	Vector3 GetCapsuleTopHemisphere(float atHeight)
	{
		return transform.position + (transform.up * (atHeight - m_Controller.radius));
	}

	void UpdateCharacterHeight(bool force)
	{
		// Update height instantly
		if (force)
		{
			m_Controller.height = m_TargetCharacterHeight;
			//m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
			PlayerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio + Vector3.forward * 0.2f;
			m_Actor.AimPoint.transform.localPosition = m_Controller.center;
			//GetComponent<Transform>().localScale = Vector3.up;
		}
		// Update smooth height
		/*
		else if (m_Controller.height != m_TargetCharacterHeight)
		{
			// resize the capsule and adjust camera position
			m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight,
				CrouchingSharpness * Time.deltaTime);
			m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
			PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
				Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
			m_Actor.AimPoint.transform.localPosition = m_Controller.center;
		}
		*/
	}

	void GroundCheck()
	{
		// Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
		float chosenGroundCheckDistance =
			IsGrounded ? (m_Controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

		// reset values before the ground check
		IsGrounded = false;
		m_GroundNormal = Vector3.up;

		// only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
		if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
		{
			// if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
			if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height),
				m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
				QueryTriggerInteraction.Ignore))
			{
				// storing the upward direction for the surface found
				m_GroundNormal = hit.normal;

				// Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
				// and if the slope angle is lower than the character controller's limit
				if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(m_GroundNormal))
				{
					IsGrounded = true;

					// handle snapping to the ground
					if (hit.distance > m_Controller.skinWidth)
					{
						m_Controller.Move(Vector3.down * hit.distance);
					}
				}
			}
		}
	}
	bool IsNormalUnderSlopeLimit(Vector3 normal)
	{
		return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
	}
}
