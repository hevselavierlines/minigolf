using UnityEngine;
using System.Collections;

public class cameraClippin : MonoBehaviour {
    Camera MainCam;
	// Use this for initialization
	void Start () {
	    MainCam = Camera.current;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerStay(Collider col){
        if (col.gameObject.name == "Ball"){
            MainCam.nearClipPlane = 2;
        }
    }
}
