using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bullet Patterns/Star Pattern")]
public class StarPattern : BulletPattern
{
    private float bulletSpeed = 5f;
    private int numberOfBullets = 5;

    public override void Shoot(Transform shooterTransform, GameObject bulletPrefab)
    {
        Debug.Log("Star Pattern");
        float angleStep = 360f / numberOfBullets;
        float angle = 0f;

        for (int i = 0; i < numberOfBullets; i++)
        {
            float dirX = Mathf.Sin((angle * Mathf.PI) / 180f);
            float dirY = Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 bulletDirection = new Vector3(dirX, dirY, 0f);
            GameObject bullet = Instantiate(bulletPrefab, shooterTransform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = bulletDirection.normalized * bulletSpeed;

            angle += angleStep;
        }
    }
}

