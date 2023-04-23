using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float damage;
    public int bulletsPerMag = 30;
    public int bulletsLeft;
    public AudioClip shootSound;

    public float fireRate;
}
