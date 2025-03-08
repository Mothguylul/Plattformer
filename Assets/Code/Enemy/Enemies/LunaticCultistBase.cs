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
	

	// Start is called before the first frame update
	void Start()
    {
        _enemyAnimator = GetComponent<Animator>();
		_enemyRigidbody = GetComponent<Rigidbody2D>();
		_enemySpriteRenderer = GetComponent<SpriteRenderer>();

		base.Health = 20;
		base.Damage = 10;
		base.Speed = 10;
		base.CurrentEnemy = this.gameObject;

		base.CurrentState = EnemyState.Patrol;

		CurrentEnemy = this.gameObject;


    }

    // Update is called once per frame
    void Update()
    {
		if (base.Health <= 0)
			Die();

		if (playerInRange)
		{

			if (PlayerTransform.position.x < this.gameObject.transform.position.x)
			{
				_enemySpriteRenderer.flipX = true;
			}
			else
			{
				_enemySpriteRenderer.flipX = false;
			}
		}
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
