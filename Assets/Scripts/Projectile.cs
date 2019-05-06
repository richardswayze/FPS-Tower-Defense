using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public Vector3 target;

    float speed;
    float damage;
    Transform enemyToDamage;

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    public void SetEnemyTarget(Transform _enemy)
    {
        enemyToDamage = _enemy;
    }

	// Update is called once per frame
	void Update () {
        Vector3 moveDirection = target - transform.position;
        float distanceToMove = speed * Time.deltaTime;
        if (distanceToMove > moveDirection.magnitude)
        {
            if (enemyToDamage == null)
            {
                Destroy(gameObject);
                return;
            }
            enemyToDamage.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
	}
}
