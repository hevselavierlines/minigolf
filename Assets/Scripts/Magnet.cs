using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour {
    public float magnetStrength = 10.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.name == "Ball")
        {
            col.attachedRigidbody.AddForce(((col.gameObject.transform.position - this.transform.position).normalized) * -magnetStrength);
        }
    }
}
