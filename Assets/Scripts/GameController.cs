using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public Transform[] spawnPoints;
	bool[] spawnIsTaken;

	// Use this for initialization
	void Start () {
		spawnIsTaken = new bool[spawnPoints.Length];
	}


	public Transform GetSpawnPoint()
	{
		for (int i = 0; i < spawnPoints.Length; ++i) {
			if(spawnIsTaken[i])
				continue;

			GetComponent<PhotonView>().RPC("ClaimSpawn",PhotonTargets.All,i);
			return spawnPoints[i];
		}
		return null;
	}

	[PunRPC]
	void ClaimSpawn(int index)
	{
		spawnIsTaken [index] = true;
	}

	void OnPlayerDeath()
	{


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
