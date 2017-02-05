using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour {

	public int bounciness;
	public float angularDrag;
	public Rigidbody rb;

	private Vector3 golfclubOffset;
	public float speed;
	public GameObject golfclub;
	public GolfClubControl gc;

	private int notMoving = -1;
	private LvlManager levelTracker;
	public GameObject lvlManager;
	public Text hitsText;
	private int hits;

	void Start() {
		rb = GetComponent<Rigidbody> ();
		golfclubOffset = golfclub.transform.position - transform.position;

		bounciness = 10;
		angularDrag = 1.0f;

		levelTracker = (LvlManager)lvlManager.GetComponent(typeof(LvlManager));
	}

	public Vector3 getPosition() {
		return transform.position;
	}

	void ballStopped() {
		gc.clubAppear ();
	}

	public bool IsBallRolling() {
		return notMoving != -1;
	}

	public void HitBall() {
		notMoving = 0;
		hits++;
		hitsText.text = "Hits: " + hits;
	}

	public void holeBall() {
		gc.finishVibrate ();

		hits = 0;
		hitsText.text = "Hits: " + hits;
	}

	void FixedUpdate() {
		if (Input.GetKey (KeyCode.R)) {
			reset ();
		}
		if (notMoving != -1 && Mathf.Abs(rb.velocity.x) < 0.15 && Mathf.Abs(rb.velocity.z) < 0.15 
			&& Mathf.Abs(rb.velocity.y) < 0.05) {
			notMoving++;
			if (notMoving > 180) {
				notMoving = -1;
				ballStopped ();
			}
		}
		if (notMoving == -1) {
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

		if (transform.position.y < 0.0) {
			reset ();
		}
	}

	public void stop() {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero; 
		notMoving = -1;
		golfclub.transform.eulerAngles = new Vector3 (25f, 180f, 0.0f);

		ballStopped ();
	}

	void reset() {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero; 
		notMoving = -1;
		golfclub.transform.eulerAngles = new Vector3 (25f, 180f, 0.0f);

		levelTracker.reset ();
		ballStopped ();
	}

	// Update is called once per frame
	void Update () {
		golfclub.transform.position = transform.position + golfclubOffset;
        angularDrag = 1.0f;

	}
	void OnCollisionEnter(Collision col){
		if(col.collider.gameObject.tag == "environment"){
			foreach(ContactPoint contact in col.contacts){
				Debug.DrawRay(contact.point, contact.normal, Color.white);
				rb.AddForce(contact.normal * col.relativeVelocity.magnitude * bounciness);
			}
		}
	}
	void OnCollisionStay(Collision col){
		if(col.collider.gameObject.tag == "environment"){
			rb.angularDrag = angularDrag;
		}
		if(rb.velocity.magnitude < 0.2){
			rb.velocity = new Vector3(0,0,0);
		}
	}
}
