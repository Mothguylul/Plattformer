using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Components")]
	private SpriteRenderer playerSpriteRenderer;
	private Rigidbody2D playerRigidBody;
	private Animator playerAnimator;
	private TrailRenderer playerTrailRenderer;

	[Header("Movement")]
	private bool isGround;
	private bool canDoubleJump = false;
	[SerializeField] private float speed = 5;
	private float xdirection;

	[Header("Dashing")]
	[SerializeField] private float _dashingVelocity = 20f, dashingTime = 0.35f, dashingCooldown;
	private Vector2 _dashingDirection;
	private bool _canDash = true, _isDashing;

	// Start is called before the first frame update
	void Start()
	{

		playerRigidBody = GetComponent<Rigidbody2D>();
		playerSpriteRenderer = GetComponent<SpriteRenderer>();
		playerAnimator = GetComponent<Animator>();
		playerTrailRenderer = GetComponent<TrailRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		xdirection = Input.GetAxisRaw("Horizontal");
		var dashingInput = Input.GetButtonDown("Dash");
		dashingCooldown -= Time.deltaTime;

		//dasing
		if (dashingInput && _canDash && dashingCooldown <= 0)
		{
			_isDashing = true;
			_canDash = false;
			_dashingDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			if (_dashingDirection == Vector2.zero)
			{
				_dashingDirection = new Vector2(0, 0);
			}
			StartCoroutine(StopDashing());
		}

		if (_isDashing)
		{
			playerRigidBody.velocity = _dashingDirection.normalized * _dashingVelocity;
			playerTrailRenderer.emitting = true;
			return;

		}

		if (isGround) { _canDash = true; }

		// handle jumping
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (isGround)
			{
				playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 5);

			}
			if (canDoubleJump)
			{
				playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 5);
				canDoubleJump = false;

			}
		}

		if(playerRigidBody.velocity.y > -0.01f && Input.GetMouseButtonDown(0))
		{
			return;
		}

		// handle left and right movement	
		playerRigidBody.velocity = new Vector2(xdirection * speed, playerRigidBody.velocity.y);

		playerAnimator.SetBool("isRunning", xdirection != 0);	
		playerAnimator.SetFloat("yVelocity", playerRigidBody.velocity.y);

		//handle sprite flipping
		if (xdirection > 0.01)
		{
			playerSpriteRenderer.flipX = false;
		}
		else if (xdirection < -0.01)
		{
			playerSpriteRenderer.flipX = true;
		}

	}

	//dashing
	private IEnumerator StopDashing()
	{
		yield return new WaitForSeconds(dashingTime);
		dashingCooldown = 1.25f;
		_isDashing = false;
		yield return new WaitForSeconds(0.2f);
		playerTrailRenderer.emitting = false;
	}

	// ground management
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("IsGround"))
		{
			isGround = true;
			canDoubleJump = false;
		}

	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("IsGround"))
		{
			isGround = false;
			canDoubleJump = true;
		}
	}

}
