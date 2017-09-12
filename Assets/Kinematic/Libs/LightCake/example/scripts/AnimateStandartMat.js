#pragma strict
var NormalMap1:Texture[];
private var BatchNr:int=0;
var fps:float=25;

//manual repeat/refresh
function Start () {
if(NormalMap1.Length==0){
return;
}

AnimateTexture();


}

//execute animation
function AnimateTexture(){
if(Application.isPlaying){
//reset array for animation loop.
if(BatchNr>=NormalMap1.Length||BatchNr==NormalMap1.Length){
BatchNr=0;
}

//set the next image
transform.GetComponent.<Renderer>().material.SetTexture("_BumpMap",NormalMap1[BatchNr]);
transform.GetComponent.<Renderer>().material.SetTexture("_DetailNormalMap",NormalMap1[BatchNr]);

//set number of next image
BatchNr++;

//go back to start
Loop();
}
}

function Loop(){
//speed of animation
yield WaitForSeconds(1f/fps);
AnimateTexture();
}