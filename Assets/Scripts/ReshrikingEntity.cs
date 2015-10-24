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

	public virtual void Start () {
		minimumReached = false;
		multiplier = startingMultiplier;
		initialScale = transform.localScale;
	}

//	public virtual void Update () {
//	
//	}
//
//	public virtual void FixedUpdate(){
//
//	}

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

	public virtual void Reshrink(){
		if(!minimumReached)
		{
			multiplier -= reshrinkAmount;
			UpdateScale();

			if(multiplier < minimumMultiplier)
			{
				MinimumReached();
			}
		}
	}
}
