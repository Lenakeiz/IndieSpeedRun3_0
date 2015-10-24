using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	float speed = 10.0f;
	public LayerMask collisionMask;

	float lifeTime = 5.0f;
	float width = .1f;

	MapController mc;

	void Start(){
		Destroy(gameObject, lifeTime);

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

	void CheckCollision(float distanceMove)
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

	void OnHitGround(Vector3 pos)
	{
		Transform hittedTile;
		if(mc.GetTileFromPosition(pos, out hittedTile))
		{
			Debug.Log ("Hitted a tile");
			IReshrink reshrinkObject = hittedTile.GetComponent<IReshrink>();
			if(reshrinkObject != null)
			{
				//Debug.Log("Hitted :" + hit.collider.gameObject.name);
				reshrinkObject.Reshrink();
			}
		}

		GameObject.Destroy(gameObject);

	}

	void OnHitObject(RaycastHit hit)
	{
		IReshrink reshrinkObject = hit.collider.gameObject.GetComponent<IReshrink>();
		if(reshrinkObject != null)
		{
			//Debug.Log("Hitted :" + hit.collider.gameObject.name);
			reshrinkObject.Reshrink();
		}
		//Debug.Log(hit.collider.gameObject.name);
		GameObject.Destroy(gameObject);
	}

	void OnHitObject(Collider hit)
	{
		IReshrink reshrinkObject = hit.GetComponent<IReshrink>();
		if(reshrinkObject != null)
		{
			//Debug.Log("Hitted :" + hit.collider.gameObject.name);
			reshrinkObject.Reshrink();
		}
		//Debug.Log(hit.collider.gameObject.name);
		GameObject.Destroy(gameObject);
	}

	void Update () {

		float distanceMove =  Time.deltaTime * speed;
		CheckCollision(distanceMove);
		gameObject.transform.Translate(Vector3.forward * distanceMove);

	}
}
