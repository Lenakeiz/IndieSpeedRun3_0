using UnityEngine;
using System.Collections;

public class NetworkedObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (this.transform.position);
			stream.SendNext (this.transform.rotation);
		} else {
			this.transform.position = Vector3.Lerp(this.transform.position,
			                                       (Vector3)stream.ReceiveNext(),0.5f);
			this.transform.rotation = Quaternion.Lerp(this.transform.rotation,
			                                       (Quaternion)stream.ReceiveNext(),0.5f);
		
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
