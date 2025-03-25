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

	private float _attackRange = 0;

	private EnemyState _currentState;

	[Header("Movement elements")]

	[SerializeField] protected Transform PlayerTransform;

	[SerializeField] protected Transform EnemyFirstPosition, EnemySecondPosition;

	protected GameObject CurrentEnemy;

	protected SpriteRenderer EnemySpriteRenderer;

	protected Rigidbody2D EnemyRigidBody;

	[Header("Others")]
	[SerializeField] private GameObject _player;

	virtual protected float AttackRange
	{
		get => _attackRange;
		set => _attackRange = value;
	}

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
				StopCoroutine(Patrol());
				
				break;

			case EnemyState.Attack:
				break;

		}
		
		if (CurrentState != EnemyState.Patrol)
			_isPatrolling = false;

		/*if ((CurrentEnemy.transform.position - _player.transform.position).magnitude < AttackRange)
		{
			Debug.Log("Attack state");
			CurrentState = EnemyState.Attack;
		}*/

	}


	public virtual void TakeDamage(int damageTotake)
	{
		Health -= damageTotake;
	}

	protected virtual IEnumerator Patrol()
	{
		Transform nextTargetPoint = EnemyFirstPosition;

		while (true)
		{
			if (nextTargetPoint != null)
			{
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

	/// <summary>
	/// Helper method to rotate vectors 
	/// </summary>
	/// <param name="v"></param>
	/// <param name="degrees"></param>
	/// <returns></returns>
	protected virtual Vector2 RotateVector(Vector2 v, float degrees)
	{
		float radians = degrees * Mathf.Deg2Rad;
		float cos = Mathf.Cos(radians);
		float sin = Mathf.Sin(radians);
		return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
	}

	protected virtual void Die()
	{
		Destroy(this.gameObject);

	}

	// empty methods that will be set in each enemy individually
	protected virtual void Chase() { }
	protected virtual void Attack() { } 
}
