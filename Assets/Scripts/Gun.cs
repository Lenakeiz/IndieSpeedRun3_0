using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public Transform spawnPosition;
	public float msBetweenShots = 100;
	public float muzzleVel = 35; 
	public float shrinkAmount = 0.01f;
	public string projectileName = "Prefabs/Projectile";

	float nextShotTime;

	private void PlaySound(string clipname)
	{
		GameObject go = GameObject.FindGameObjectWithTag("Map");
		if(go != null)
		{
			AudioManager am = go.GetComponent<AudioManager>();
			am.PlaySound(clipname);
		}
	}

	public virtual void Shoot()
	{
		if(Time.time > nextShotTime)
		{
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newprojectile = PhotonNetwork.Instantiate(projectileName,spawnPosition.position, spawnPosition.rotation,0).GetComponent<Projectile>();
			newprojectile.SetSpeed(muzzleVel);
			newprojectile.SetShrinkPower(shrinkAmount);
			newprojectile.parent = transform.root.gameObject;
			PlaySound("Shoot");
		}
	}

	public virtual void Shoot(float scaleMultiplier)
	{
		if(Time.time > nextShotTime)
		{
			nextShotTime = Time.time + msBetweenShots / 1000;
			Projectile newprojectile = PhotonNetwork.Instantiate(projectileName,spawnPosition.position, spawnPosition.rotation,0).GetComponent<Projectile>();
			Vector3 scale = newprojectile.transform.localScale;
			newprojectile.transform.localScale = scale * scaleMultiplier;
			newprojectile.SetSpeed(muzzleVel);
			newprojectile.SetShrinkPower(shrinkAmount);
			newprojectile.parent = transform.root.gameObject;
			PlaySound("Shoot");
		}
	}

}
