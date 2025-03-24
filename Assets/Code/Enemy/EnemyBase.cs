using Assets.Code.Enemy.Enemies;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class EnemyBase : MonoBehaviour
{
	[Header("Base values")]
	private int _health = 0;

	private int _damage = 0;

	private float _speed = 0;

	private EnemyState _currentState;

	[Header("Movement elements")]

	protected bool playerInRange = false;

	[SerializeField] protected CapsuleCollider2D SpotRange; // the range the enemy will be able to see the player 

	[SerializeField] protected Transform PlayerTransform;

	[SerializeField] protected Transform EnemyFirstPosition, EnemySecondPosition;

	protected GameObject CurrentEnemy;

	private System.Random _random = new System.Random();

	virtual protected float Speed
	{
		get => _speed;
		set => _speed = value;
	}

	private bool _isPatrolling;

	virtual protected EnemyState CurrentState
	{
		get => _currentState;
		set => _currentState = value;
	}

	virtual protected int Health
	{
		get => _health;
		set => _health = value;
	}

	virtual protected int Damage
	{
		get => _damage;
		set => _damage = value;
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		switch (CurrentState)
		{
			case EnemyState.Patrol:
				if (_isPatrolling)
				{
					return;
				}
				else
				{
					_isPatrolling = true;
					StartCoroutine(Patrol());

				}
				break;

			case EnemyState.Chase:

				break;

			case EnemyState.Attack:
				Attack();
				break;

		}

		if (CurrentState != EnemyState.Patrol)
			_isPatrolling = false;
	}


	public virtual void TakeDamage(int damageTotake)
	{
		Health -= damageTotake;
	}

	protected virtual void MoveTowardsPlayer()
	{

	}

	protected virtual void Attack() { } //Empty, will be set in each enemy class individually

	protected virtual IEnumerator Patrol()
	{
		Transform nextTargetPoint = EnemyFirstPosition;

		while (true)
		{
			if (nextTargetPoint != null)
			{
				Debug.Log("move enemy");
				CurrentEnemy.transform.position = Vector2.MoveTowards(
					CurrentEnemy.transform.position,
					nextTargetPoint.transform.position,
					  Speed * Time.deltaTime
				); 

				if ((CurrentEnemy.transform.position - nextTargetPoint.transform.position).magnitude < 0.1f) //check if the distance between the enemy and the first target point is low, same as Vector.Distance()
				{
					yield return new WaitForSeconds(1);

					nextTargetPoint = (nextTargetPoint == EnemyFirstPosition) ? EnemySecondPosition : EnemyFirstPosition;
				}
			}
			yield return null;	
		}
	}


	protected virtual void Rest()
	{

	}

	protected virtual void Die()
	{
		Destroy(this.gameObject);

	}
	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInRange = true;
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInRange = false;
		}
	}


}
