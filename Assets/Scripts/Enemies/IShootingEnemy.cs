using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IShootingEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private BulletPattern bulletPattern;
    [SerializeField] private GameObject bulletPrefab;

    public void Attack()
    {
        bulletPattern.Shoot(transform, bulletPrefab);
    }
}
