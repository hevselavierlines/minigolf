using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LvlManager: MonoBehaviour {
    GameObject Player;
    private int nextLevelNumber;
	private int currentLevel = 3;
	public Text levelText;
    //Use this for initialization
	void Start () {
        Player = GameObject.Find("Ball");
		GameObject gameObj = GameObject.Find ("Start" + currentLevel);
		if (gameObj != null) {
			Player.transform.position = gameObj.transform.position;
		} 
		levelText.text = "Level " + currentLevel;
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    public void nextLevel() {
        nextLevelNumber = ++currentLevel;
		levelText.text = "Level " + currentLevel;

		GameObject gameObj = GameObject.Find ("Start" + currentLevel);
		if (gameObj != null) {
			Player.transform.position = gameObj.transform.position;
		} 
    }

	public void reset() {
		Player.transform.position =  GameObject.Find("Start" + currentLevel).transform.position;   
	}
}
