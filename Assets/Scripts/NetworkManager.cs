using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

	public bool debugVerbose;
	public string roomName;

	GameObject player;

	GameObject mainMenu;

	public Transform[] spawnPoints;
	bool[] spawnIsTaken;

	Text winText;


	public int totalPlayers = 0;
	public int totalDead = 0;

	bool roomOwner = false;
	bool weAreDead = false;

	// Use this for initialization
	void OnEnable () {
		PhotonNetwork.playerName = "Cool Dude";
		mainMenu = GameObject.Find ("MainMenu");
		spawnIsTaken = new bool[spawnPoints.Length];
		
		winText = GameObject.Find ("WinText").GetComponent<Text> ();
		if(winText)
			winText.enabled = false;
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

		CreateRoom (false);
	}

	void OnPhotonRandomJoinFailed(){
		OurLog ("OnPhotonRandomJoinFialed");

		OurLog ("Creating new room");

		CreateRoom (true);
	}

	void CreateRoom(bool random)
	{

		if (random) {
			OurLog ("Making new random room");
			PhotonNetwork.CreateRoom(null);
		} else {
			OurLog ("Making new room");
			PhotonNetwork.CreateRoom (roomName);
		}
		roomOwner = true;
	}

	void OnJoinedRoom(){
		OurLog ("OnJoinedRoom");
		if (mainMenu)
			mainMenu.SetActive (false);

		if (roomOwner) {
			ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable ();
			table.Add ("AvailableSpawns", spawnIsTaken);
			table.Add ("TotalPlayers", 0);
			table.Add ("TotalDead", 0);
			PhotonNetwork.room.SetCustomProperties (table);
		} else {
			spawnIsTaken = (bool[])PhotonNetwork.room.customProperties ["AvailableSpawns"];
			totalPlayers = (int)PhotonNetwork.room.customProperties ["TotalPlayers"];
			totalDead = (int)PhotonNetwork.room.customProperties ["TotalDead"];
		}
		SpawnMyPlayer ();

		ExitGames.Client.Photon.Hashtable customPropTable = new ExitGames.Client.Photon.Hashtable ();
		customPropTable.Add ("AvailableSpawns", spawnIsTaken);
		customPropTable.Add ("TotalPlayers", totalPlayers);
		customPropTable.Add ("TotalDead", 0);
		PhotonNetwork.room.SetCustomProperties (customPropTable);


	}

	public Transform GetSpawnPoint()
	{
		for (int i = 0; i < spawnPoints.Length; ++i) {
			if(spawnIsTaken[i])
				continue;

			spawnIsTaken[i] = true;
			return spawnPoints[i];
		}
		return null;
	}

	public string RegisterPlayerName(string name)
	{
		totalPlayers ++;
		string playerName = name;
		for(int i=1; i < 10; ++i)
		{
			if(PhotonNetwork.room.customProperties.ContainsKey(playerName))
			{
				playerName = name + i;
			}
			else{
				ExitGames.Client.Photon.Hashtable customPropTable = new ExitGames.Client.Photon.Hashtable ();
				customPropTable.Add (playerName, true);
				PhotonNetwork.room.SetCustomProperties (customPropTable);
				return playerName;
			}
		}
		Debug.LogError ("Too many people with the same name");
		return "WTF??";
	}

	public void OnPlayerDeath(string name)
	{
		this.GetComponent<PhotonView>().RPC("KillPlayer",PhotonTargets.All,name);
		weAreDead = true;
	}

	[PunRPC]
	void KillPlayer(string name)
	{
		totalDead ++;
		if( totalPlayers - totalDead == 1)
		{
			StartCoroutine("GameOver");
		}
	}

	IEnumerator GameOver()
	{

		if(!weAreDead)
		{
			ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable ();
			table.Add ("Winner", PhotonNetwork.playerName);
			PhotonNetwork.room.SetCustomProperties(table);

		}
		
		yield return new WaitForSeconds (1);
		
		winText.rectTransform.localScale = Vector3.zero;
		winText.enabled = true;
		winText.text = ((string)PhotonNetwork.room.customProperties["Winner"]) + " WINS!!";
		for (float currentScale = 0; currentScale <=1; currentScale+=Time.deltaTime) {
			winText.rectTransform.localScale = new Vector3(currentScale,
			                                               currentScale,
			                                               currentScale);
			yield return null;
		}
		
		yield return new WaitForSeconds (2);
		winText.enabled = false;
		PhotonNetwork.LeaveRoom ();
		GameObject.Find ("Map").GetComponent<MapController> ().Reset ();
	}

	void SpawnMyPlayer(){
		OurLog ("SpawnMyPlayer");
		GameObject mainCam = GameObject.Find ("Main Camera");
		mainCam.GetComponent<Camera> ().enabled = false;
		mainCam.GetComponent<AudioListener> ().enabled = false;

		PhotonNetwork.playerName = RegisterPlayerName(PhotonNetwork.playerName);

		Transform spawn = GetSpawnPoint ();

		if (spawn == null) {
			Debug.LogError("NO SPAWNS LEFT");
		}
		player = (GameObject)PhotonNetwork.Instantiate ("Prefabs/NetworkedPlayer",spawn.transform.position, spawn.transform.rotation, 0);
		player.name = "OurPlayer";
		player.GetComponent<RigidbodyFirstPersonController> ().Ownership = true;
		player.GetComponentInChildren<Camera> ().enabled = true;
		player.GetComponentInChildren<AudioListener> ().enabled = true;
		player.GetComponentInChildren<HeadBob> ().enabled = true;
	}

	void OnLeftRoom()
	{
		if (player != null) {
			PhotonNetwork.Destroy(player);
		}

		GameObject mainCam = GameObject.Find ("Main Camera");
		mainCam.GetComponent<Camera> ().enabled = false;
		mainCam.GetComponent<AudioListener> ().enabled = false;
		mainMenu.SetActive (true);
	}

}
