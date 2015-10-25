using UnityEngine;
using System.Collections;

public class PlayerNetworkedObject : MonoBehaviour {

	Animator m_anim;	
	
	void OnAwake()
	{

	}
	// Use this for initialization
	void Start () {
		m_anim = gameObject.GetComponent<Animator>();
		if(m_anim == null)
		{
			Debug.Log("Doesn't found the animator");
		}
	}
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			if(m_anim != null){
				stream.SendNext (m_anim.GetBool("isJumping"));
				stream.SendNext (m_anim.GetBool("isBack"));
				stream.SendNext(m_anim.GetBool("isRunning"));
			}

		} else {
			if(m_anim != null)
			{
				m_anim.SetBool("isJumping",(bool)stream.ReceiveNext());
           		m_anim.SetBool("isBack",(bool)stream.ReceiveNext());
				m_anim.SetBool("isRunning",(bool)stream.ReceiveNext());
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//transform.position = Vector3.Lerp(transform.position,realPosition,0.5f);
		//transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.5f);
	}
}
