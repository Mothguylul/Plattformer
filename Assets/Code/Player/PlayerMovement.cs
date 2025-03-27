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
	[SerializeField]private Collider2D _hitBox;
	//private TrailRenderer _playerTrailrenderer;

	[Header("Movement")]
	private bool isGround;
	private bool canDoubleJump = false;
	[SerializeField] private float speed = 5;
	private float xdirection;

	[Header("Dashing")]
	[SerializeField] private float _dashingVelocity = 20f, dashingTime = 0.45f, dashingCooldown;
	private Vector2 _dashingDirection;
	private bool _canDash = true, _isDashing;

	// Start is called before the first frame update
	void Start()
	{

		playerRigidBody = GetComponent<Rigidbody2D>();
		playerSpriteRenderer = GetComponent<SpriteRenderer>();
		playerAnimator = GetComponent<Animator>();
		//_playerTrailrenderer = GetComponent<TrailRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		xdirection = Input.GetAxisRaw("Horizontal");
		dashingCooldown -= Time.deltaTime;
		var dashingInput = Input.GetButtonDown("Dash");

		HandleJumping();
		HandleMovementAndAnimation();
		SpriteFlippingAndHitbox();	
		
		//dashing
		if (dashingInput && _canDash && dashingCooldown <= 0)
		{
			_canDash = false;
			_isDashing = true;
			_dashingDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			StartCoroutine(StopDashing());
		}

		if (_isDashing)
		{
			playerRigidBody.velocity = _dashingDirection.normalized * _dashingVelocity;
			//_playerTrailrenderer.emitting = true;
			return;
		}

		if (isGround) { _canDash = true; }
	}

	private void SpriteFlippingAndHitbox()
	{
		float offsetX = Mathf.Abs(_hitBox.transform.localPosition.x);

		if (xdirection > 0.01)
		{
			playerSpriteRenderer.flipX = false;
			_hitBox.transform.localPosition = new Vector2(offsetX, _hitBox.transform.localPosition.y);

		}
		else if (xdirection < -0.01)
		{
			playerSpriteRenderer.flipX = true;
			_hitBox.transform.localPosition = new Vector2(-offsetX, _hitBox.transform.localPosition.y);
		}
	
	}

	private void HandleMovementAndAnimation()
	{
		playerRigidBody.velocity = new Vector2(xdirection * speed, playerRigidBody.velocity.y);

		playerAnimator.SetBool("isRunning", xdirection != 0);
		playerAnimator.SetFloat("yVelocity", playerRigidBody.velocity.y);

	}

	private void HandleJumping()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (isGround || canDoubleJump)
			{
				playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 5);

				if (!isGround) canDoubleJump = false;
			}
		}

	}

	//dashing
	private IEnumerator StopDashing()
	{
		yield return new WaitForSeconds(dashingTime);
		dashingCooldown = 1.25f;
		_isDashing = false;
		yield return new WaitForSeconds(0.2f);
		//_playerTrailrenderer.emitting = false;
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