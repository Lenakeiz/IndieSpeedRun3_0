using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {


	bool isConnected;
	bool isHost;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnJoinedRoom()
	{
		isConnected = true;
	}

	void OnPhotonRandomJoinFailed()
	{
		isHost = true;
	}
}
