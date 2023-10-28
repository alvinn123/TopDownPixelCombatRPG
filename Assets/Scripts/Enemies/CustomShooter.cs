using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShooter : MonoBehaviour, IEnemy
{
    [SerializeField] private BulletPattern bulletPattern;
    [SerializeField] private GameObject bulletPrefab;

    private void OnValidate()
    {

    }

    public void Attack()
    {
        bulletPattern.Shoot(transform, bulletPrefab);
    }

}
