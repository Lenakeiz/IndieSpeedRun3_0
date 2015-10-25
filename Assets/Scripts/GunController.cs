using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

	public Transform weaponHolder;
	public Gun[] startingGuns;
	Gun equippedGun;
	int gunIndex =0;

	public void EquipGun(Gun equip)
	{
		if(equippedGun != null)
		{
			GameObject.Destroy(equippedGun.gameObject);
		}

		equippedGun = Instantiate(equip, weaponHolder.position, weaponHolder.rotation) as Gun;
		equippedGun.transform.SetParent(weaponHolder,true);

	}

	public void Shoot()
	{
		if(equippedGun != null)
		{
			equippedGun.Shoot();
		}
	}

	public void Shoot(float scaleMultiplier)
	{
		if(equippedGun != null)
		{
			equippedGun.Shoot(scaleMultiplier);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Q)) {
			gunIndex --;
			if(gunIndex == -1)
			{
				gunIndex = startingGuns.Length -1;
			}
			EquipGun(startingGuns[gunIndex]);
		}
		else if (Input.GetKeyDown (KeyCode.E)) {
			gunIndex ++;
			if(gunIndex >= startingGuns.Length)
			{
				gunIndex = 0;
			}
			EquipGun(startingGuns[gunIndex]);
		}

	}

	void Start () {
		if(startingGuns.Length > 0 && startingGuns[gunIndex] != null)
		{
			EquipGun(startingGuns[gunIndex]);
		}
	}

}
