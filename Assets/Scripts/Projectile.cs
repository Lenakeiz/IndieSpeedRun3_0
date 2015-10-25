using UnityEngine;
using System.Collections;

public class Projectile : Photon.MonoBehaviour {

	protected float speed = 10.0f;
	protected float shrinkPower = 0.1f;
	public LayerMask collisionMask;

	public float lifeTime = 5.0f;
	protected float width = .1f;

	protected MapController mc;

	public GameObject parent;

	protected virtual void Start(){
		Collider[] initialCollision = Physics.OverlapSphere(transform.position, .1f, collisionMask);

		if(initialCollision.Length > 0){
			OnHitObject(initialCollision[0]);
		}

		mc = GameObject.FindGameObjectWithTag("Map").GetComponent<MapController>();
	}

	public void SetSpeed(float newspeed)
	{
		speed = newspeed;
	}

	public void SetShrinkPower(float aShrinkPower)
	{
		shrinkPower = aShrinkPower;

	}

	public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (lifeTime);
		} else {
			lifeTime = (float)stream.ReceiveNext();
		}
	}

	protected virtual void CheckCollision(float distanceMove)
	{
		Ray ray = new Ray(transform.position, transform.forward);
		//Debug.DrawLine(transform.position, transform.forward * distanceMove, Color.magenta);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, distanceMove + width, collisionMask))
		{
			OnHitObject(hit);
		}
		else
		{
//			Plane ground = new Plane(Vector3.up,Vector3.zero);
//			float rayDistance;
//
//			if(ground.Raycast(ray, out rayDistance))
//			{
//				Vector3 rayPoint = ray.GetPoint(rayDistance);
//				Debug.Log ("Hitted ground pos x: " + rayPoint.x + " pos y: " + rayPoint.y);
//				OnHitGround(rayPoint);
//			}
		}
	}

	protected virtual void OnHitGround(Vector3 pos)
	{
		Transform hittedTile;
		if(mc.GetTileFromPosition(pos, out hittedTile))
		{
			Debug.Log ("Hitted a tile");
			IReshrink reshrinkObject = hittedTile.GetComponent<IReshrink>();
			if(reshrinkObject == null)
				reshrinkObject = hittedTile.transform.parent.GetComponent<IReshrink>();
			if(reshrinkObject != null)
			{
				//Debug.Log("Hitted :" + hit.collider.gameObject.name);
				hittedTile.GetComponent<PhotonView>().RPC("Reshrink",PhotonTargets.All,shrinkPower);
			}
		}
			
		if (photonView.isMine)
			PhotonNetwork.Destroy (gameObject);

	}

	protected virtual void OnHitObject(RaycastHit hit)
	{

		GameObject targetObj = hit.collider.gameObject;
		IReshrink reshrinkObject = targetObj.GetComponent<IReshrink>();
		bool shouldDestroy = true;
		if (reshrinkObject == null) {
			reshrinkObject = targetObj.transform.parent.GetComponent<IReshrink> ();
			targetObj = targetObj.transform.parent.gameObject;
		}
		if(reshrinkObject != null)
		{

			//Debug.Log("Hitted :" + hit.collider.gameObject.name);

			if(photonView.isMine && hit.collider.name != "OurPlayer")
			{
				targetObj.GetComponent<PhotonView>().RPC("Reshrink",PhotonTargets.All,shrinkPower);
			}
			else
				shouldDestroy = false;
			//reshrinkObject.Reshrink(shrinkPower);
		}
		//Debug.Log(hit.collider.gameObject.name);
		if (shouldDestroy && photonView.isMine)
			PhotonNetwork.Destroy (gameObject);
	}

	protected virtual void OnHitObject(Collider hit)
	{

		GameObject targetObj = hit.gameObject;
		IReshrink reshrinkObject = targetObj.GetComponent<IReshrink>();
		bool shouldDestroy = true;
		if (reshrinkObject == null) {
			reshrinkObject = targetObj.transform.parent.GetComponent<IReshrink> ();
			targetObj = targetObj.transform.parent.gameObject;
		}
		if(reshrinkObject != null)
		{
			//Debug.Log("Hitted :" + hit.collider.gameObject.name);
			if(photonView.isMine && hit.gameObject.name != "OurPlayer")
			{
				targetObj.GetComponent<PhotonView>().RPC("Reshrink",PhotonTargets.All,shrinkPower);
			}
			else
				shouldDestroy = false;
		}
		//Debug.Log(hit.collider.gameObject.name);
		if (shouldDestroy && photonView.isMine)
			PhotonNetwork.Destroy (gameObject);
	}

	protected virtual void Update () {
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0) {
			if (photonView.isMine)
				PhotonNetwork.Destroy (gameObject);
		}


		float distanceMove =  Time.deltaTime * speed;
		CheckCollision(distanceMove);
		gameObject.transform.Translate(Vector3.forward * distanceMove);
	}
}
