using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonController : MonoBehaviour {

	public Sprite redButton;
	public Sprite yellowButton;


	GameObject rocket;
	Vector2 initialTouchPos;
	float swipeDist;
	float distToButton;
	
	float minSwipeDistVert = 20;
	float maxSwipeDistHor = 100;
	Vector2 buttonPosX;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<SpriteRenderer> ().sprite = redButton;
		rocket = GameObject.Find ("rocket");
		buttonPosX = new Vector2 (Camera.main.WorldToScreenPoint (gameObject.transform.position).x, 0);
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
						gameObject.GetComponent<SpriteRenderer> ().sprite = yellowButton;
						rocket.SendMessage("ButtonPressed", gameObject.tag);
					}
				}
			}
			else if (touch.phase == TouchPhase.Ended){
				gameObject.GetComponent<SpriteRenderer> ().sprite = redButton;
				swipeDist = (new Vector2(0, touch.position.y) - new Vector2(0, initialTouchPos.y)).magnitude;
				distToButton = (new Vector2(touch.position.x, 0) - buttonPosX).magnitude;
				if(swipeDist > minSwipeDistVert && distToButton < maxSwipeDistHor){
					rocket.SendMessage("ButtonSwiped", gameObject.tag);
				}
			}
		}
	}
}
