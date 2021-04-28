using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	public Camera PlayerCamera;

	public float speed;
	protected Rigidbody r;
	private Vector3 vertical;
	private Vector3 horizontal;

	public float jumpForce;

	public float rotationSensitivity = 3f;
	public float yMinLimit = -89f;
	public float yMaxLimit = 89f;

	CharacterController m_Controller;
	private bool isJumping;
	private float x, y;
	void Start()
	{
		m_Controller = GetComponent<CharacterController>();
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		r = GetComponent<Rigidbody>();
		m_Controller.enableOverlapRecovery = true;
	}

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			isJumping = true;
		}
	}

    private void FixedUpdate()
	{
		Vector3 velocity = r.velocity;

		vertical = transform.forward * speed * Input.GetAxis("Vertical");
		horizontal = transform.right * speed * Input.GetAxis("Horizontal");
		velocity = (vertical + horizontal) * Time.fixedDeltaTime;
		velocity.y = r.velocity.y;

		r.velocity = velocity;

		if (isJumping)
		{
			r.AddForce(transform.up * jumpForce);
			isJumping = false;
		}


		//HandleCameraRotation();

		//RotateCharacter();

	}

    private void LateUpdate()
    {
		HandleCameraRotation();
		RotateCharacter();
    }

    private void RotateCharacter()
	{
		transform.rotation = Quaternion.LookRotation(new Vector3(PlayerCamera.transform.forward.x, 0f, PlayerCamera.transform.forward.z));
	}

    void HandleCameraRotation()
	{
		Cursor.lockState = CursorLockMode.Locked;

		x += Input.GetAxis("Mouse X") * rotationSensitivity;
		y = ClampAngle(y - Input.GetAxis("Mouse Y") * rotationSensitivity, yMinLimit, yMaxLimit);

		// Rotation
		PlayerCamera.transform.rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
	}

	// Clamping Euler angles
	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360) angle += 360;
		if (angle > 360) angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}
