#pragma strict

 #if UNITY_EDITOR

import UnityEngine;
import UnityEditor;
import System.Linq;
  
  

class LightCakeExtension extends EditorWindow {

		

		// Main GUI Function
		function OnGUI () {
	
		//apply window to selected object
		var Selected=Selection.activeGameObject;
	if(!Selected){
	return;
	}
		//check if object is a light source
		if(Selected.GetComponent.<Light>()){
		//check if the script is already added
	if(!Selected.GetComponent(LightCake)){
	GUILayout.Label("There is no Cake on this light source");

	GUI.color = Color(0.049,0.9,0.075);
	}
	else{
	 GUILayout.Label("This light is already prepared for cookie animation");
	 GUI.color = Color(1,0.51,0.51); 
	 }
	
		//only show the button if a light is selected and add a lightcake
		if(!Selected.GetComponent(LightCake)){
	if( GUILayout.Button("add Cake")){
	for(var transform in Selection.transforms){
	   transform.gameObject.AddComponent.<LightCake>();
	  }
	  }
	  
	}
	 //delete lightcake
	 if(Selected.GetComponent(LightCake)){
	 if( GUILayout.Button("Remove Cake")){
	 for(var transform in Selection.transforms){
	 DestroyImmediate(transform.gameObject.GetComponent(LightCake));	 
	 }
	 }
	   //sort out the array
	   GUI.color = new Color(0.049,0.5,1); 
	  if( GUILayout.Button("Sort array")){
				
 						
		Selected.GetComponent(LightCake).CookieBatch = Selected.GetComponent(LightCake).CookieBatch.OrderBy( function (x) x.name).ToList();
			} 
			 
			 
	 
	 GUILayout.Label("The sorter works alphabetically/numerically");
	 GUILayout.Label("For example: Frame_001, Frame_002, Frame_003, etc...");
	 
	 }
	 
	 
	 }
	//hide everything if the selected object is not compatible for the script
	else{
	GUILayout.Label("This object is not a light source");
	GUI.color = Color(1,0.51,0.51);  
	}
	
	
	
}


		
		//Always refresh the window, even when the inspector is changed.
		function OnInspectorUpdate(){
	
		  Repaint();
		  
		  //place the first cookie in the light cookie slot (for preview purposes).
		  	var Selected=Selection.activeGameObject;
		  	//return if no object is selected
		  		if(!Selected){
	return;
	}
	//return if selected object does not have a cake
	  		if(!Selected.GetComponent(LightCake)){
	return;
	}
	
	//return if array is empty
	 if(!Selected.GetComponent(LightCake).CookieBatch.Count){
	 return;
	 }
	
		  	if(Selected.GetComponent(LightCake)){
		  if(Selected.GetComponent(LightCake).CookieBatch.Count>=1){
		  Selected.GetComponent.<Light>().cookie=Selected.GetComponent(LightCake).CookieBatch[0];
		  }
		  }
		}
			
		// Add menu item to show the window.
		@MenuItem ("Window/LightCake")
		static function Init () {
			EditorWindow.GetWindow (LightCakeExtension);
		}
	
		
	}
	
	 #endif