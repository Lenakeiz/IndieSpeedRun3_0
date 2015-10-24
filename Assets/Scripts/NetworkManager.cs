using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class NetworkManager : MonoBehaviour {

	public bool debugVerbose;

	// Use this for initialization
	void OnEnable () {
		Connect ();
	}

	void OurLog(string message)
	{
		if (debugVerbose)
			Debug.Log (message);
	}

	void Connect()
	{
		OurLog ("Connect");
		PhotonNetwork.ConnectUsingSettings ("FPs v.0.1");
	}

	void OnGUI(){
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
	}

	void OnJoinedLobby() {
		OurLog ("OnJoinedLobby");
		PhotonNetwork.JoinRandomRoom ();
	}

	void OnPhotonRandomJoinFailed(){
		OurLog ("OnPhotonRandomJoinFialed");

		OurLog ("Creating new room");

		PhotonNetwork.CreateRoom ("");
	}

	void OnJoinedRoom(){
		OurLog ("OnJoinedRoom");
		SpawnMyPlayer ();
	}

	void SpawnMyPlayer(){
		OurLog ("SpawnMyPlayer");
		GameObject mainCam = GameObject.Find ("Main Camera");
		mainCam.GetComponent<Camera> ().enabled = false;
		mainCam.GetComponent<AudioListener> ().enabled = false;

		GameObject player = (GameObject)PhotonNetwork.Instantiate ("Prefabs/NetworkedPlayer",new Vector3 (0, 2, 0), Quaternion.identity, 0);
		player.GetComponent<ReshrikingEntity> ().enabled = true;
		player.GetComponentInChildren<Camera> ().enabled = true;
		player.GetComponentInChildren<AudioListener> ().enabled = true;
		player.GetComponentInChildren<HeadBob> ().enabled = true;
	}
}
