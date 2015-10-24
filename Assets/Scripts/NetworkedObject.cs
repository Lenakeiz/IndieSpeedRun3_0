using UnityEngine;
using System.Collections;

public class NetworkedObject : MonoBehaviour {

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;


	void OnAwake()
	{
		realPosition = transform.position;
		realRotation = transform.rotation;
	}
	// Use this for initialization
	void Start () {
		realPosition = transform.position;
		realRotation = transform.rotation;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (this.transform.position);
			stream.SendNext (this.transform.rotation);
		} else {
			this.transform.position = (Vector3)stream.ReceiveNext();
			this.transform.rotation = (Quaternion)stream.ReceiveNext();
		}
	}
	
	// Update is called once per frame
	void Update () {
			//transform.position = Vector3.Lerp(transform.position,realPosition,0.5f);
			//transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.5f);
	}
}
