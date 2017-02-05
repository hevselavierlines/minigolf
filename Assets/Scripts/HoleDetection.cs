using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HoleDetection : MonoBehaviour {
    private int currentLevel = 1;
	private int holeWait = 0;
	public Text holeText;
    GameObject LvlManager;
	Ball ball;
    LvlManager LevelTracker;
    // Use this for initialization
	void Start () {
        GameObject LvlManager = GameObject.Find("LevelTracker");
        LevelTracker = (LvlManager)LvlManager.GetComponent(typeof(LvlManager));
		holeText.text = "";
    }
	
	// Update is called once per frame
	void Update () {
		if (holeWait > 0) {
			holeWait++;
			if (holeWait > 180) {
				holeWait = 0;

				ball.stop ();
				holeText.text = "";
				LevelTracker.nextLevel();
				currentLevel++;
			}
		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.name == "Ball") {
			holeText.text = "You did it!";
			if (ball == null) {
				ball = col.gameObject.GetComponent<Ball> ();
			}
			holeWait = 1;
			ball.holeBall ();
		}
	}
}