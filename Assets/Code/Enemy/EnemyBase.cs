using Assets.Code.Enemy.Enemies;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
	[Header("Base values")]
	private int _health = 0;

	private int _damage = 0;

	private int _speed = 0;

	private EnemyState _currentState;

	[Header("Movement elements")]

	protected bool playerInRange = false;

	[SerializeField] protected CapsuleCollider2D SpotRange; // the range the enemy will be able to see the player 

	[SerializeField] protected Transform PlayerTransform;

	[SerializeField] protected Transform EnemyFirstPosition, EnemySecondPosition;

	protected GameObject CurrentEnemy;

	virtual protected int Speed
	{
		get => _speed;
		set => _speed = value;
	}

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
	void Update()
	{
		switch (CurrentState)
		{
			case EnemyState.Patrol:
				StartCoroutine(Patrol());
				break;

			case EnemyState.Chase:
				MoveTowardsPlayer();
				break;

			case EnemyState.Attack:
				Attack();
				break;

		}
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
			else if (nextTargetPoint == null)
			{
				nextTargetPoint = EnemyFirstPosition.transform;
			}
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
