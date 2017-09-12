#pragma strict

import UnityEngine;
import System.Collections.Generic;


var CookieBatch: List.<Texture>;
private var BatchNr:int=0;
var fps:float=25;

//manual repeat/refresh
function Start () {
if(CookieBatch.Count==0){
return;
}
yield WaitForEndOfFrame;
AnimateTexture();
}

//execute animation
function AnimateTexture(){
if(Application.isPlaying){
//reset array for animation loop.
if(BatchNr>=CookieBatch.Count||BatchNr==CookieBatch.Count){
BatchNr=0;
} 

//set the next image
transform.GetComponent.<Light>().cookie=null;
transform.GetComponent.<Light>().cookie=CookieBatch[BatchNr];
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