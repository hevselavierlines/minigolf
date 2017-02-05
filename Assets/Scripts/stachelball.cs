using UnityEngine;
using System.Collections;

public class stachelball : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3(Time.deltaTime * 15, Time.deltaTime * 35, Time.deltaTime * 20));
	}
}
