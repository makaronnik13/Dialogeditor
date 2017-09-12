#pragma strict
var caustics:GameObject;

function OnGUI () {
if(GUILayout.Button("Models created by VYTAUTAS RAMANAUSKAS")) {
Application.OpenURL("http://vytautasramanauskas.wix.com/portfolio");
}
GUILayout.Label ("");
/*if(GUILayout.Button("Buy")) {
Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/publisher/4847");
}*/
GUILayout.Label ("");
GUILayout.Label ("animation speed (fps)");
caustics.GetComponent(LightCake).fps = GUILayout.HorizontalSlider (caustics.GetComponent(LightCake).fps, 4, 60);
GUILayout.Label(""+caustics.GetComponent(LightCake).fps);

GUILayout.Label ("");

GUILayout.Label ("light intensity");
caustics.GetComponent.<Light>().intensity = GUILayout.HorizontalSlider (caustics.GetComponent.<Light>().intensity, 0, 8);
GUILayout.Label(""+caustics.GetComponent.<Light>().intensity);
}