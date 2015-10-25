using UnityEngine;
using System.Collections;

public class Grenade : Projectile {
	
	public float explosionRadius;
	public float explosionForce;

	protected override void Start()
	{
		base.Start ();
		this.GetComponent<Rigidbody> ().AddForce (transform.forward.normalized * speed,ForceMode.Impulse);
	}
	
	public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (lifeTime);
		} else {
			lifeTime = (float)stream.ReceiveNext();
		}
	}

	protected override void CheckCollision(float distanceMove)
	{
	}
	
	protected override void OnHitGround(Vector3 pos)
	{
		
	}
	
	protected override void OnHitObject(RaycastHit hit)
	{
	
	}
	
	protected override void OnHitObject(Collider hit)
	{

	}

	void Explode()
	{
		Hashtable alreadyHitTable = new Hashtable();
		Collider[] hits = Physics.OverlapSphere (this.transform.position, explosionRadius);
		for (int i = 0; i < hits.Length; ++i) {
			GameObject hitObj = hits[i].gameObject;
			if(hitObj.GetComponent<Rigidbody>())
			{
				hitObj.GetComponent<Rigidbody>().AddExplosionForce(
					explosionForce,this.transform.position,explosionRadius,1.0f,ForceMode.Impulse);
			}
			if(hitObj.GetComponent<ReshrikingEntity>() && !alreadyHitTable.ContainsKey(hitObj))
			{
				hitObj.GetComponent<PhotonView>().RPC("Reshrink",PhotonTargets.All,shrinkPower);
			}
			if(!alreadyHitTable.ContainsKey(hitObj))
			{
				alreadyHitTable.Add(hitObj,true);
			}
		}
	}
	
	protected override void  Update () {
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0) {
			Explode ();
			if (photonView.isMine)
				PhotonNetwork.Destroy (gameObject);
		}

	}
}
