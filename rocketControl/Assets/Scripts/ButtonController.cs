using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonController : MonoBehaviour {

	public Sprite redButton;
	public Sprite yellowButton;


	GameObject rocket;
	private Vector2 initialTouchPos;
	private float swipeDist;
	//Dictionary<int,Vector2> initialTouchPos;
	//Dictionary<int, float> swipeDist;
	float minSwipeDist = 20;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<SpriteRenderer> ().sprite = redButton;
		rocket = GameObject.Find ("rocket");
		//initialTouchPos = new Dictionary<int, Vector2>();
		//swipeDist = new Dictionary<int, float>();
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Touch touch in Input.touches){
			swipeDist = 0;
			if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
				if(hit.collider != null){
					GameObject recipient = hit.transform.gameObject;
					if (recipient == gameObject){
						initialTouchPos = touch.position;
						//Debug.Log ("touch " + gameObject.name + " " + touch.position);
						gameObject.GetComponent<SpriteRenderer> ().sprite = yellowButton;
						rocket.SendMessage("ButtonPressed", gameObject.name);
					}
				}
			}
			else if (touch.phase == TouchPhase.Ended){
				gameObject.GetComponent<SpriteRenderer> ().sprite = redButton;
				/*swipeDist = (new Vector2(0, touch.position.y) - new Vector2(0, initialTouchPos.y)).magnitude;
				if(swipeDist > minSwipeDist){
					rocket.SendMessage("ButtonSwiped", gameObject.name);
				}*/
			}
		}
	}
}
