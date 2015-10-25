using UnityEngine;
using System.Collections;

public class ReshrikingEntity : MonoBehaviour, IReshrink {

	public float startingMultiplier = 1.0f;
	public float minimumMultiplier = 0.1f;
	public float reshrinkAmount = 0.1f;
	public bool minimumReached;
	public bool isDead = false;
	public float destroyThreshold = -10.0f;
	protected Vector3 initialScale;
	protected float initialMass;

	protected float multiplier;

	public event System.Action OnMinumSizeReached;
	public event System.Action OnDeath;

	public virtual void Awake () {
		minimumReached = false;
		multiplier = startingMultiplier;
		SetInitialScale ();
	}

	public void SetInitialScale()
	{
		initialScale = transform.localScale;
		if(this.GetComponent<Rigidbody>())
			initialMass = this.GetComponent<Rigidbody> ().mass;
	}

	public virtual void Update () {
		if(transform.position.y < destroyThreshold && !isDead)
		{
			isDead = true;
			if(OnDeath != null)
			{
				OnDeath();
			}
		}
	}

	public void SetMultipler(float mul)
	{
		multiplier = mul;
		if (multiplier > minimumMultiplier)
			minimumReached = false;
	}

	public float GetMultipler()
	{
		return multiplier;

	}
//
//	public virtual void FixedUpdate(){
//
//	}

	public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (multiplier);
		} else {
			multiplier = (float)stream.ReceiveNext ();
			UpdateScale();
		}	
	}

	private void MinimumReached()
	{
		minimumReached = true;
		Debug.Log("Reshrink minimum reached");
		if(OnMinumSizeReached != null)
		{
			OnMinumSizeReached();
		}
	}

	private void PlaySound(string clipname)
	{
		GameObject go = GameObject.FindGameObjectWithTag("AudioPlayer");
		if(go != null)
		{
			AudioManager am = go.GetComponent<AudioManager>();
			am.PlaySound(clipname);
		}
	}

	protected virtual void UpdateScale()
	{
		string sound = "Shrink"+Random.Range(1,3).ToString();
		PlaySound(sound);
		transform.localScale = initialScale * multiplier;
		if(this.GetComponent<Rigidbody>())
		{
			this.GetComponent<Rigidbody> ().mass = initialMass * multiplier/2;
			this.GetComponent<Rigidbody> ().WakeUp();
		}
	}

	[PunRPC]
	public virtual void Reshrink(float shrinkAmount){
		if (!minimumReached && !isDead) {
			float newMultiplier = multiplier - shrinkAmount;

			if(newMultiplier <= minimumMultiplier)
			{
				multiplier = minimumMultiplier;

				MinimumReached();
			}
			else{
				multiplier = newMultiplier;
			}
			UpdateScale();
		}
	}

	[PunRPC]
	public virtual void Reshrink(){
		if(!minimumReached)
		{
			multiplier -= reshrinkAmount;
			if(multiplier <= minimumMultiplier)
			{
				multiplier = minimumMultiplier;
				MinimumReached();
			}
			UpdateScale();
		}
	}
}
