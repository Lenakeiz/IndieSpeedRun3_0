using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityStandardAssets.Characters.FirstPerson;

[CustomEditor(typeof(ReshrikingEntity))]
public class SchrinkEntityEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		
		ReshrikingEntity re = target as ReshrikingEntity;
		if(GUILayout.Button ("Reshrink"))
		{
			re.Reshrink(0.1f);
		}
		
	}

}

[CustomEditor(typeof(RigidbodyFirstPersonController))]
public class RigidbodyFirstPersonControllerSchrinkEditor : Editor {
	
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		
		RigidbodyFirstPersonController re = target as RigidbodyFirstPersonController;
		if(GUILayout.Button ("Reshrink"))
		{
			re.Reshrink(0.1f);
		}
		
	}
	
}
