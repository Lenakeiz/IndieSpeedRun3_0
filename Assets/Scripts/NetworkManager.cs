using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

	public bool debugVerbose;
	public string userName;
	public string roomName;

	GameObject mainMenu;

	// Use this for initialization
	void OnEnable () {
		PhotonNetwork.playerName = "Cool Dude";
		mainMenu = GameObject.Find ("MainMenu");
	}

	void OurLog(string message)
	{
		if (debugVerbose)
			Debug.Log (message);
	}

	public void SetPlayerUsername(string name)
	{
		PhotonNetwork.playerName = GameObject.Find ("NameField").GetComponent<InputField> ().text;

	}

	public void SetRoomName(string name)
	{
		roomName = GameObject.Find ("RoomField").GetComponent<InputField> ().text;
	
	}

	public void Connect()
	{
		OurLog ("Connect");
		PhotonNetwork.ConnectUsingSettings ("FPs v.0.1");
	}

	void OnGUI(){
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
	}

	void OnJoinedLobby() {
		OurLog ("OnJoinedLobby");
		if (roomName.Length > 0)
			PhotonNetwork.JoinRoom (roomName);
		else 
			PhotonNetwork.JoinRandomRoom ();
	}

	void OnPhotonJoinRoomFailed()
	{
		OurLog ("OnPhotonJoinFialed");
		
		OurLog ("Creating new room");

		PhotonNetwork.CreateRoom (roomName);
	}

	void OnPhotonRandomJoinFailed(){
		OurLog ("OnPhotonRandomJoinFialed");

		OurLog ("Creating new room");

		PhotonNetwork.CreateRoom ("");
	}

	void OnJoinedRoom(){
		OurLog ("OnJoinedRoom");
		if (mainMenu)
			mainMenu.SetActive (false);
		SpawnMyPlayer ();
	}

	void SpawnMyPlayer(){
		OurLog ("SpawnMyPlayer");
		GameObject mainCam = GameObject.Find ("Main Camera");
		mainCam.GetComponent<Camera> ().enabled = false;
		mainCam.GetComponent<AudioListener> ().enabled = false;

		GameObject player = (GameObject)PhotonNetwork.Instantiate ("Prefabs/NetworkedPlayer",new Vector3 (0, 2, 0), Quaternion.identity, 0);
		player.name = "OurPlayer";
		player.GetComponent<ReshrikingEntity> ().enabled = true;
		player.GetComponentInChildren<Camera> ().enabled = true;
		player.GetComponentInChildren<AudioListener> ().enabled = true;
		player.GetComponentInChildren<HeadBob> ().enabled = true;
	}
}
