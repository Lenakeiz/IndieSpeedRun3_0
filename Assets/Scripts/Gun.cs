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
			Projectile newprojectile = PhotonNetwork.Instantiate("Prefabs/Projectile",spawnPosition.position, spawnPosition.rotation,0).GetComponent<Projectile>();
			newprojectile.SetSpeed(muzzleVel);
		}
	}

}
