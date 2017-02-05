using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GolfClubColliderControl : MonoBehaviour {

	Vector3 anchor;
	public int moveCoef;
	public int rotateCoef;
	public static bool swingcheck;
	public float force;
	public Vector3 shootDir;
	private Ball movePlayer;
	public float DEBUG_POWER = 2;
	// Use this for initialization
	void Start () {
		moveCoef = 10;
		rotateCoef = 50;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Space)){
			transform.Rotate(Vector3.right, Time.deltaTime*rotateCoef);
		}
		if(Input.GetKey(KeyCode.B)){
			transform.Rotate(-Vector3.right, Time.deltaTime*rotateCoef);
		}
		if(Input.GetKey(KeyCode.W)){
			transform.Translate(Vector3.forward * Time.deltaTime*moveCoef, Space.World);

		}if(Input.GetKey(KeyCode.S)){
			transform.Translate(Vector3.back* Time.deltaTime*moveCoef, Space.World);
		}
		if(Input.GetKey(KeyCode.A)){
			transform.Translate(Vector3.left* Time.deltaTime*moveCoef, Space.World);
		}
		if(Input.GetKey(KeyCode.D)){
			transform.Translate(Vector3.right* Time.deltaTime*moveCoef, Space.World);
		}
		if(Input.GetKey(KeyCode.X)){
			transform.Translate(Vector3.up* Time.deltaTime*moveCoef, Space.World);
		}
		if(Input.GetKey(KeyCode.Z)){
			transform.Translate(Vector3.down* Time.deltaTime*moveCoef, Space.World);
		}

		if(Input.GetKey(KeyCode.Q)){
			transform.Rotate(Vector3.up, Time.deltaTime*rotateCoef);
		}
		if(Input.GetKey(KeyCode.E)){
			transform.Rotate(-Vector3.up, Time.deltaTime*rotateCoef);
		}

	}

	void OnTriggerEnter(Collider col) {

		if (col.gameObject.name == "Ball") {
			if (movePlayer == null) {
				movePlayer = col.GetComponentsInParent<Ball> () [0];
			}
			if (!movePlayer.IsBallRolling()) {
				GolfClubControl gc = GetComponentsInParent<GolfClubControl> () [0];
				if (DEBUG_POWER > 0) {
					col.attachedRigidbody.AddForce (this.transform.forward * force * DEBUG_POWER);
				} else {
					col.attachedRigidbody.AddForce (this.transform.forward * force * gc.power);
				}
				gc.clubDisappear ();
				movePlayer.HitBall ();
				movePlayer.gc = gc;
			}
		}
	}
}

