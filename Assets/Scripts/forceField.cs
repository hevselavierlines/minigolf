using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forceField : MonoBehaviour {
    public float forceStrength = 10.4f;
    public Vector3 direction;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        direction = this.transform.up;
	}
    void OnTriggerStay(Collider col) {
        if (col.gameObject.name == "Ball") { 
            col.attachedRigidbody.AddForce(this.transform.up * forceStrength);
        }
    }
}
