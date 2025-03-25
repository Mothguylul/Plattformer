using Assets.Code.Enemy.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaticCultistBase : EnemyBase
{
	private Animator _enemyAnimator;
	private Rigidbody2D _enemyRigidbody;
	private SpriteRenderer _enemySpriteRenderer;

	private bool playerInRange = false;

	[Header("Raycasts")]
	[SerializeField] private Transform _rayCastStartingPosition;
	private float _angle = 15f;
	private float _rayLenght = 10f;

	// Start is called before the first frame update
	void Start()
	{
		_enemyAnimator = GetComponent<Animator>();
		_enemyRigidbody = GetComponent<Rigidbody2D>();
		_enemySpriteRenderer = GetComponent<SpriteRenderer>();

		base.Health = 20;
		base.Damage = 10;
		base.Speed = 4f;
		base.AttackRange = 15f;
		base.CurrentEnemy = this.gameObject;
		base.EnemySpriteRenderer = _enemySpriteRenderer;
		base.EnemyRigidBody = _enemyRigidbody;

		base.CurrentState = EnemyState.Patrol;

		CurrentEnemy = this.gameObject;


	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if (base.Health <= 0)
			Die();

		Vector2 forward = transform.right;

		Vector2[] directions = new Vector2[]
		{
			forward,
		   base.RotateVector(forward, _angle),
		   base.RotateVector(forward, -_angle),
		   base.RotateVector(forward, _angle / 2),
		   base.RotateVector(forward, -_angle / 2)
		};

		foreach (var direction in directions)
		{
			RaycastHit2D ray = Physics2D.Raycast(_rayCastStartingPosition.position, direction, _rayLenght);

			if (ray.collider != null)
			{
				if (ray.collider.CompareTag("Player"))
				{
					CurrentState = EnemyState.Chase;
				}
			}

			Debug.DrawRay(_rayCastStartingPosition.position, direction * _rayLenght, Color.red);
		}


		if (_enemyRigidbody.velocity.x > 0.01f)
			_enemySpriteRenderer.flipX = true;
		else if (_enemyRigidbody.velocity.x < -0.01f)
			_enemySpriteRenderer.flipX = false;

	}

	//hit and damage logic
	public override void TakeDamage(int damageTotake)
	{
		_enemyRigidbody.bodyType = RigidbodyType2D.Static;
		base.TakeDamage(damageTotake);
		Invoke("SetAnimationAndBodyType", 0.1f);
	}


	private void SetAnimationAndBodyType()
	{
		_enemyAnimator.SetTrigger("Hit");
		_enemyRigidbody.bodyType = RigidbodyType2D.Dynamic;

	}


	//death logic
	protected override void Die()
	{
		_enemyAnimator.SetTrigger("Death");
		Invoke("InvokeDie", 1f);
	}

	private void InvokeDie()
	{
		base.Die();
	}

}
