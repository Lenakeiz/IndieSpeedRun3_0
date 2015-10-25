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
	public bool[] spawnIsTaken;

	Text winText;

	public GameObject aMainMenu;
	public GameObject aWinText;
	public Toggle aSandboxToggle;
	public Text numPlayerText;
	public GameObject aWaitMenu;
	public GameObject waitTextPanal;


	public int totalPlayers = 0;
	public int totalDead = 0;
	public float minPlayerWaitTime = 5.0f;

	public bool createSandboxRoom = false;
	bool roomOwner = false;
	bool weAreDead = false;
	public bool forceStartGame = false;
	bool inGame = false;

	Transform spawnPoint;


	// Use this for initialization
	void OnEnable () {
		PhotonNetwork.playerName = "Cool Dude";
		mainMenu = aMainMenu;
		if (mainMenu == null)
			Debug.LogError ("You need to put the MainMenu into NetworkManager");
		if (aWinText == null)
			Debug.LogError ("You need to put the WinText into NetworkManager");
		if (aSandboxToggle == null)
			Debug.LogError ("You need to put the SandboxToggle into NetworkManager");
		if (numPlayerText == null) 
			Debug.LogError("You need to put the NumPlayerText into NetworkManager");
		if (aWaitMenu == null)
			Debug.LogError ("You need to put the WaitingGameMenu into NetworkManager");
		if (waitTextPanal == null)
			Debug.LogError ("You need to put the WaitTextPanel into NetworkManager");

		mainMenu .SetActive (true);

		aWaitMenu.SetActive (false);
		spawnIsTaken = new bool[spawnPoints.Length];
		winText = aWinText.GetComponent<Text> ();
		waitTextPanal.SetActive (false);
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
		createSandboxRoom = aSandboxToggle.isOn;
	}

	public void SetRoomName(string name)
	{
		roomName = GameObject.Find ("RoomField").GetComponent<InputField> ().text;
	}

	public void ForceGameStart()
	{
		if (roomOwner) {
			forceStartGame = true;
		}
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
			PhotonNetwork.JoinRandomRoom (null,4);
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
		RoomOptions r = new RoomOptions();
		r.maxPlayers = 4;
		if (random) {
			OurLog ("Making new random room");
			PhotonNetwork.CreateRoom(null,r,null);
		} else {
			OurLog ("Making new room");
			PhotonNetwork.CreateRoom (roomName,r,null);
		}
		roomOwner = true;
	}

	void OnJoinedRoom(){
		OurLog ("OnJoinedRoom");
		if (mainMenu)
			mainMenu.SetActive (false);

		if (roomOwner) {
			ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable ();
			table.Add ("TotalPlayers", 0);
			table.Add ("TotalDead", 0);
			PhotonNetwork.room.SetCustomProperties (table);
		} else {
			totalPlayers = (int)PhotonNetwork.room.customProperties ["TotalPlayers"];
			totalDead = (int)PhotonNetwork.room.customProperties ["TotalDead"];
		}
		spawnPoint = GetSpawnPoint ();
		waitTextPanal.SetActive (true);
		if (roomOwner) {
			aWaitMenu.SetActive (true);
		}
		StartCoroutine ("WaitForPlayers");

	}

	[PunRPC]
	void StartGame()
	{
		aWaitMenu.SetActive (false);
		waitTextPanal.SetActive (false);
		SpawnMyPlayer ();
		
		ExitGames.Client.Photon.Hashtable customPropTable = new ExitGames.Client.Photon.Hashtable ();
		customPropTable.Add ("TotalPlayers", PhotonNetwork.room.playerCount);
		customPropTable.Add ("TotalDead", 0);
		if (!createSandboxRoom && roomOwner) {
			PhotonNetwork.room.open = false;
		}

		PhotonNetwork.room.SetCustomProperties (customPropTable);
		inGame = true;
	}

	public Transform GetSpawnPoint()
	{
		return spawnPoints [PhotonNetwork.room.playerCount - 1];
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
		weAreDead = true;
		this.GetComponent<PhotonView>().RPC("KillPlayer",PhotonTargets.All,name);
	}

	IEnumerator WaitForPlayers()
	{
		float timeWaited = 0.0f;
		int minimumPlayers = 4;
		while (PhotonNetwork.room.playerCount <minimumPlayers) {

			if(timeWaited > minPlayerWaitTime)
			{
				timeWaited = 0;
				minimumPlayers --;
			}
			if(numPlayerText)
			{
				numPlayerText.text = "Waiting for players : " + PhotonNetwork.room.playerCount + "/" + minimumPlayers;
			}

			if( forceStartGame)
			{
				break;
			}

			if(inGame)
				yield break;

			timeWaited += 0.2f;
			yield return new WaitForSeconds(0.2f);
		}

		GetComponent<PhotonView> ().RPC ("StartGame", PhotonTargets.All, null);
		yield break;
	}

	[PunRPC]
	void KillPlayer(string name)
	{
		totalDead ++;
		if( PhotonNetwork.room.playerCount - totalDead == 1 || totalPlayers == 1)
		{
			StartCoroutine("GameOver");
		}
	}


	[PunRPC]
	void RegisterWinner(string name)
	{

		winText.text = name + " Wins!";
	}

	IEnumerator GameOver()
	{
		winText.text = "You Lose!";
		if(!weAreDead)
		{
			this.GetComponent<PhotonView>().RPC("RegisterWinner",PhotonTargets.All,
			                                    PhotonNetwork.playerName);
		}
		
		yield return new WaitForSeconds (1);
		winText.enabled = true;
		winText.rectTransform.localScale = Vector3.zero;
		for (float currentScale = 0; currentScale <=1; currentScale+=Time.deltaTime) {
			winText.rectTransform.localScale = new Vector3(currentScale,
			                                               currentScale,
			                                               currentScale);
			yield return null;
		}
		
		yield return new WaitForSeconds (2);
		winText.enabled = false;
		GameObject.Find ("Map").GetComponent<MapController> ().Reset ();
		GameObject deathCam = GameObject.FindGameObjectWithTag ("DeadCamera");
		deathCam.GetComponent<Camera> ().enabled = false;
		deathCam.GetComponent<AudioListener> ().enabled = false;

		totalPlayers = 0;
		totalDead = 0;
		createSandboxRoom = false;
		roomOwner = false;
		weAreDead = false;
		forceStartGame = false;
		inGame = false;
		GameObject mainCam = GameObject.Find ("Main Camera");
		mainCam.GetComponent<Camera> ().enabled = true;
		mainCam.GetComponent<AudioListener> ().enabled = true;
		mainMenu.SetActive (true);
		weAreDead = false;
		roomOwner = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		if(player != null)
			PhotonNetwork.Destroy(player);

		PhotonNetwork.Disconnect ();
		yield break;
	}

	void SpawnMyPlayer(){
		OurLog ("SpawnMyPlayer");
		GameObject mainCam = GameObject.Find ("Main Camera");
		mainCam.GetComponent<Camera> ().enabled = false;
		mainCam.GetComponent<AudioListener> ().enabled = false;

		PhotonNetwork.playerName = RegisterPlayerName(PhotonNetwork.playerName);


		if (spawnPoint == null) {
			Debug.LogError("No spawn points left in GlobalScripts - NetworkManager");
		}
		player = (GameObject)PhotonNetwork.Instantiate ("Prefabs/BenNetworkedPlayer",spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
		player.name = "OurPlayer";
		player.GetComponent<RigidbodyFirstPersonController> ().Ownership = true;
		player.GetComponentInChildren<Camera> ().enabled = true;
		player.GetComponentInChildren<AudioListener> ().enabled = true;
		player.GetComponentInChildren<HeadBob> ().enabled = true;
		player.transform.FindChild("Rigged_Model").gameObject.GetComponentInChildren<Renderer>().enabled = false;
	}

}
