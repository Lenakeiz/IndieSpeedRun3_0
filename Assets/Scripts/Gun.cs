using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public Transform spawnPosition;
	public Projectile projectile;
	public float msBetweenShots = 100;
	public float muzzleVel = 35; 
	public float shrinkAmount = 0.01f;

	float nextShotTime;

	public void Shoot()
	{
		if(Time.time > nextShotTime)
		{
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newprojectile = PhotonNetwork.Instantiate("Prefabs/Projectile",spawnPosition.position, spawnPosition.rotation,0).GetComponent<Projectile>();
			newprojectile.SetSpeed(muzzleVel);
			newprojectile.SetShrinkPower(shrinkAmount);
		}
	}

	public void Shoot(float scaleMultiplier)
	{
		if(Time.time > nextShotTime)
		{
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newprojectile = PhotonNetwork.Instantiate("Prefabs/Projectile",spawnPosition.position, spawnPosition.rotation,0).GetComponent<Projectile>();
			Vector3 scale = newprojectile.transform.localScale;
			newprojectile.transform.localScale = scale * scaleMultiplier;
			newprojectile.SetSpeed(muzzleVel);
			newprojectile.SetShrinkPower(shrinkAmount);
		}
	}

}
