using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapController))]
public class MapEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();

		MapController map = target as MapController;
		if(GUILayout.Button ("Build Map"))
		{
			map.GenerateMap();
		}

	}
}
