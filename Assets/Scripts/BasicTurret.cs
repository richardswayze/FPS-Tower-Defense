using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : TurretStats {

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;
    [SerializeField] Transform firingPoint;
    [SerializeField] GameObject turret;
    [SerializeField] Vector3 lookOffset;

    bool canFire;
    float fireCoolDownTime;
    List<Transform> enemiesInRange;
    Transform currentTarget;

    // Use this for initialization
    void Start() {
        canFire = true;
        enemiesInRange = new List<Transform>();
        fireCoolDownTime = 0;
    }

    // Update is called once per frame
    void Update() {
        if (!currentTarget && canFire && Spawner.enemyList.Count > 0 || currentTarget == null)
        {
            currentTarget = FindNearestTarget();
        }
        if (currentTarget)
        {
            turret.transform.LookAt(currentTarget.transform.position + lookOffset);
        }
        if (canFire)
        {
            if (currentTarget && EnemyInRange(currentTarget))
            {
                Attack(currentTarget);
                canFire = false;
            } 
            else
            {
                currentTarget = FindNearestTarget();
            }
        }
        if (!canFire && fireCoolDownTime <= Time.time)
        {
            canFire = true;
        }
        //if (enemiesInRange.Count > 0 && canFire)
        //{
        //    if (EnemyInRange(currentTarget))
        //    {
                
        //    }
        //    else
        //    {
        //        currentTarget = enemiesInRange[enemiesInRange.Count - 1]; //Shoots at most recently added enemy, should provide most attacking time.
        //        //currentTarget = FindNearestTarget();
        //        Attack(currentTarget);
        //        canFire = false;
        //    }
        //}
    }

    void Attack(Transform enemy)
    {
        GameObject newProjectile = Instantiate(projectilePrefab);
        newProjectile.transform.position = firingPoint.position;
        Projectile projectile = newProjectile.GetComponent<Projectile>();
        projectile.target = enemy.transform.position;
        projectile.SetSpeed(projectileSpeed);
        projectile.SetDamage(damage);
        projectile.SetEnemyTarget(enemy);
        fireCoolDownTime = Time.time + firingRate;
    }

    Transform FindNearestTarget()
    {
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;
        foreach (Transform enemy in Spawner.enemyList)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < shortestDistance && EnemyInRange(enemy))
            {
                shortestDistance = Vector3.Distance(transform.position, enemy.transform.position);
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    bool EnemyInRange(Transform currentTarget)
    {
        return Vector3.Distance(currentTarget.transform.position, transform.position) <= firingRange;
    }

}
