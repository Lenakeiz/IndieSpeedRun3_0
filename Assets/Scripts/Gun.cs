using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public Transform spawnPosition;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVel = 35; 

	float nextShotTime;

	public void Shoot()
	{
		if(Time.time > nextShotTime)
		{
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newprojectile = Instantiate(projectile,spawnPosition.position, spawnPosition.rotation) as Projectile;
			newprojectile.SetSpeed(muzzleVel);
		}
	}

}
