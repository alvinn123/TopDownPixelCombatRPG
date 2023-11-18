using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Shooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletMoveSpeed;
    [SerializeField] private int burstCount;
    [SerializeField] private float projectilesPerBurst;
    [SerializeField][Range(0, 359)]protected float angleSpread;
    [SerializeField] private float startingDistance = 0.1f;
    [SerializeField] private float timeBetweenBursts;
    [SerializeField] private float restTime = 1f;
    [SerializeField] protected bool stagger;
    [Tooltip("Stagger has to be enabled for oscillate to work properly")]
    [SerializeField] protected bool oscillate;

    private bool isShooting = false;
    protected SpriteRenderer spriteRenderer;

    private void OnValidate()
    {
        // Ensure that if "oscillate" is true, "stagger" is set to true; if "oscillate" is false, "stagger" is set to false.
        if (oscillate) { stagger = true; }
        if (!oscillate) { stagger = false; }

        // Validate and set minimum values for various parameters.
        if (projectilesPerBurst < 1) { projectilesPerBurst = 1; }
        if (burstCount < 1) { burstCount = 1; }
        if (timeBetweenBursts < 0.1f) { timeBetweenBursts = 0.1f; }
        if (restTime < 0.1f) { restTime = 0.1f; }
        if (startingDistance < 0.1f) { startingDistance = 0.1f; }

        // Ensure that if "angleSpread" is set to 0, "projectilesPerBurst" is set to 1.
        if (angleSpread == 0) { projectilesPerBurst = 1; }

        // Validate and set a minimum value for "bulletMoveSpeed."
        if (bulletMoveSpeed <= 0) { bulletMoveSpeed = 0.1f; }
    }

    public virtual void Attack()
    {
        if(!isShooting)
        {
            StartCoroutine(ShootRoutine());
        }
    }
    private IEnumerator ShootRoutine() 
    {
        // Set the flag to indicate that shooting is in progress.
        isShooting = true;

        // Define variables to track angle values for bullet spread.
        float startAngle, currentAngle, angleStep, endAngle;
        float timeBetweenProjectiles = 0f;

        // Calculate the initial cone of influence for targeting.
        TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);

        // If "stagger" is enabled, calculate the time between firing individual projectiles.
        if (stagger)
        {
            timeBetweenProjectiles = timeBetweenBursts / projectilesPerBurst;
        }

        // Loop through the burst count to fire bullets.
        for (int i = 0; i < burstCount; i++)
        {
            // If "oscillate" is disabled, re-calculate the cone of influence for each burst.
            if (!oscillate)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }

            // If "oscillate" is enabled and "i" is even, re-calculate the cone of influence.
            if (oscillate && i % 2 != 1)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }
            // If "oscillate" is enabled and "i" is odd, reverse the direction of oscillation.
            else if (oscillate)
            {
                currentAngle = endAngle;
                endAngle = startAngle;
                startAngle = currentAngle;
                angleStep *= -1;
            }

            // Loop through firing multiple projectiles in the current burst.
            for (int j = 0; j < projectilesPerBurst; j++)
            {
                // Calculate the position for spawning the bullet.
                Vector2 pos = FindBulletSpawnPos(currentAngle);

                // Create a new bullet GameObject and set its orientation.
                GameObject newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                newBullet.transform.right = newBullet.transform.position - transform.position;

                // If the bullet has a Projectile component, update its move speed.
                if (newBullet.TryGetComponent(out Projectile projectile))
                {
                    projectile.UpdateMoveSpeed(bulletMoveSpeed);
                }

                // Increment the angle for the next projectile.
                currentAngle += angleStep;

                // If "stagger" is enabled, wait for a specified time before firing the next projectile.
                if (stagger) { yield return new WaitForSeconds(timeBetweenProjectiles); }
            }

            // Reset the current angle for the next burst.
            currentAngle = startAngle;

            // If "stagger" is not enabled, wait for a specified time before starting the next burst.
            if (!stagger) { yield return new WaitForSeconds(timeBetweenBursts); }
        }

        // Wait for the rest time after the shooting routine is completed.
        yield return new WaitForSeconds(restTime);

        // Set the flag to indicate that shooting has finished.
        isShooting = false;
    }

    // Calculates the parameters for a shooting cone of influence based on the specified angle spread, allowing the shooter to target a position within that cone.
    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        startAngle = targetAngle;
        endAngle = targetAngle;
        currentAngle = targetAngle;
        float halfAngleSpread;
        angleStep = 0;
        if (angleSpread != 0f)
        {
            angleStep = angleSpread / (projectilesPerBurst - 1);
            halfAngleSpread = angleSpread / 2f;
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }

    private Vector2 FindBulletSpawnPos(float currentAngle)
    {
        float x = transform.position.x + startingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = transform.position.y + startingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        Vector2 pos = new Vector2(x, y);

        return pos;
    }
}
