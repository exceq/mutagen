using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	public CameraController cam;

	public float speed;
	protected Rigidbody r;
	private Vector3 vertical;
	private Vector3 horizontal;

	public float jumpForce;
	private bool isJumping;

	void Start()
	{
		r = GetComponent<Rigidbody>();
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

	}

    private void LateUpdate()
    {
		cam.LateUpdate();
		RotateCharacter();
	}

    private void RotateCharacter()
	{
		transform.rotation = Quaternion.LookRotation(new Vector3(cam.transform.forward.x, 0f, cam.transform.forward.z));
	}
}
