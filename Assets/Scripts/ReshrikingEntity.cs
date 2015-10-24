using UnityEngine;
using System.Collections;

public class ReshrikingEntity : MonoBehaviour, IReshrink {

	public float startingMultiplier = 1.0f;
	public float minimumMultiplier = 0.1f;
	public float reshrinkAmount = 0.1f;
	public bool minimumReached;
	protected Vector3 initialScale;

	protected float multiplier;

	public event System.Action OnMinumSizeReached;

	public virtual void Awake () {
		minimumReached = false;
		multiplier = startingMultiplier;
	}

	public void SetInitialScale()
	{
		initialScale = transform.localScale;
	}

//	public virtual void Update () {
//	
//	}
//
//	public virtual void FixedUpdate(){
//
//	}

	public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
		
			stream.SendNext (multiplier);
		} else {
			transform.localScale = (float)stream.ReceiveNext () * initialScale;
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

	protected virtual void UpdateScale()
	{
		transform.localScale = initialScale * multiplier;		
	}

	public virtual void Reshrink(float shrinkAmount){
		if (!minimumReached) {
			float newMultiplier = multiplier - shrinkAmount;

			if(newMultiplier <= minimumMultiplier)
			{
				multiplier = minimumMultiplier;
				UpdateScale();
				MinimumReached();
			}
			else{
				multiplier = newMultiplier;
				UpdateScale();
			}

		}
	}

	public virtual void Reshrink(){
		if(!minimumReached)
		{
			multiplier -= reshrinkAmount;
			if(multiplier < minimumMultiplier)
			{
				multiplier = minimumMultiplier;
				MinimumReached();
			}
			UpdateScale();
		}
	}
}
