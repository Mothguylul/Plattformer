using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCollision : MonoBehaviour
{
	/// <summary>
	/// Enemy who took the hit gets referenced and health of that object gets subtracted
	/// </summary>
	/// <param name="collision"></param>
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();	
			if(enemy != null)
			{
				enemy.TakeDamage(5);
			}
		}
	}
}
