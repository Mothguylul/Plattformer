using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEditor.Build;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
	private Animator playerAnimator;
	private Rigidbody2D playerRigidBody;
	private float attackTimer;
	private int direction;
	[SerializeField] private BoxCollider2D hitBox;
	private bool isGround;

	private int numberOfCicks;
	private float lastClickTime;
	// Start is called before the first frame update
	void Start()
	{
		playerAnimator = GetComponent<Animator>();
		playerRigidBody = GetComponent<Rigidbody2D>();
		hitBox.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		direction = transform.localScale.x > 0 ? 1 : -1;
		attackTimer -= Time.deltaTime;

		if (Time.time - lastClickTime > 0.5f)
		{
			numberOfCicks = 0;
		}

		if (isGround && playerRigidBody.velocity.x == 0)
		{

			if (Input.GetMouseButtonDown(0))
			{
				lastClickTime = Time.time;
				numberOfCicks++;

				if (numberOfCicks == 1 && attackTimer <= 0)
					Attack();

			}
		}

		if (playerRigidBody.velocity.y > 0.01)
		{
			CancelGroundAttack();

			if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
			{
				JumpAttack();
			}
		}
	}

	public void ComboAttakTransition()
	{
		if (numberOfCicks >= 2)
		{
			playerAnimator.SetTrigger("Attack2");
			ActiveHitbox();
			Invoke("DeactivateHitBox", 0.2f);
		}
	}

	private void Attack()
	{
		playerAnimator.SetTrigger("Attack1");
		ActiveHitbox();
		Invoke("DeactivateHitBox", 0.2f);
		attackTimer = 1;

	}

	private void CancelGroundAttack()
	{
		DeactivateHitBox();
		playerAnimator.ResetTrigger("Attack1");
		playerAnimator.ResetTrigger("Attack2");

	}

	void ActiveHitbox()
	{
		hitBox.gameObject.SetActive(true);
	}

	private void DeactivateHitBox()
	{
		hitBox.gameObject.SetActive(false);
	}

	void JumpAttack()
	{
		playerAnimator.SetTrigger("isJumpAttacking");
		ActiveHitbox();
		Invoke("DeactivateHitBox", 0.5f);
		playerRigidBody.AddForce(new Vector2(direction * 7, 4), ForceMode2D.Impulse);
		attackTimer = 1;
	}


	// ground management
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("IsGround"))
		{
			isGround = true;
		}

	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("IsGround"))
		{
			isGround = false;
		}
	}
}
