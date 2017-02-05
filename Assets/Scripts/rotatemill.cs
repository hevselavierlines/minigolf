using UnityEngine;
using System.Collections;

public class rotatemill : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up * Time.deltaTime * -100);
	}
}
