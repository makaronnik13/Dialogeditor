#pragma strict
import UnityEditor;

@CustomEditor(LightCake)

class LightCakeEditorButton extends Editor {

		

		// Main GUI Function
		function OnInspectorGUI () {
	
		//apply window to selected object
		var Selected=Selection.activeGameObject;
		
		if(GUILayout.Button ("Window")) {
			EditorWindow.GetWindow (LightCakeExtension);
		
			}
		
		// Show default inspector property editor
			DrawDefaultInspector ();
		
		
		}
		
		
		
}