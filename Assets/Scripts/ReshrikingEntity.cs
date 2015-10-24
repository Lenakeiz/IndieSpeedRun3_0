using UnityEngine;
using System.Collections;

public class ReshrikingEntity : MonoBehaviour, IReshrink {

	public float startingMultiplier = 1.0f;
	public float minimumMultiplier = 0.2f; 
	public bool minimumReached;

	protected float multiplier;

	public event System.Action OnMinumSizeReached;

	public virtual void Start () {
		minimumReached = false;
		multiplier = startingMultiplier;
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

	public virtual void Reshrink(float amount){
		if(!minimumReached)
		{
			multiplier -= amount;

			if(multiplier < minimumMultiplier)
			{
				MinimumReached();
			}
		}
	}
}
