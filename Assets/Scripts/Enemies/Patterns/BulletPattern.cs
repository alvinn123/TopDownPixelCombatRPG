using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletPattern : ScriptableObject
{
    public abstract void Shoot(Transform shooterTransform, GameObject bulletPrefab);
}