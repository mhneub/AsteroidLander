using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonController : MonoBehaviour {

	public float lowAlpha;
	public float highAlpha;


	GameObject rocket;

	Touch buttonTouchLastFrame;
	bool buttonTouchedLastFrame;	// used as null flag

	SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {

		spriteRenderer = GetComponent<SpriteRenderer> ();
		spriteRenderer.color = new Color (1f, 1f, 1f, lowAlpha);

		rocket = GameObject.Find ("rocket");
		//buttonPosX = new Vector2 (Camera.main.WorldToScreenPoint (gameObject.transform.position).x, 0);

		buttonTouchLastFrame = new Touch ();
		buttonTouchedLastFrame = false;
	}
	
	// Update is called once per frame
	void Update () {

		// see if the button was touched this frame.  If so, get that touch
		Touch buttonTouch = new Touch();
		bool buttonTouched = false;			// used as null flag
		foreach(Touch touch in Input.touches){
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
			if(hit.collider != null && hit.transform.gameObject == gameObject){
				buttonTouch = touch;
				buttonTouched = true;
				break;
			}
		}

		// check if button started or stopped being pressed this frame
		if (!buttonTouchedLastFrame && buttonTouched) {
			// button started being pressed this frame
			spriteRenderer.color = new Color (1f, 1f, 1f, highAlpha);
			rocket.SendMessage("ButtonDown", gameObject.tag);

		} else if (buttonTouchedLastFrame && !buttonTouched) {
			// button stopped being pressed this frame
			spriteRenderer.color = new Color (1f, 1f, 1f, lowAlpha);

			// check if this was due to a swipe from inside to outside the button:
			// search for a move touch with the same fingerid as the button touch from last frame
			bool swiped = false;
			foreach(Touch touch in Input.touches){
				if (touch.fingerId == buttonTouchLastFrame.fingerId && touch.phase == TouchPhase.Moved) {
					swiped = true;
					break;
				}
			}

			if (swiped) {
				rocket.SendMessage("ButtonSwiped", gameObject.tag);
			} else {
				rocket.SendMessage("ButtonUp", gameObject.tag);
			}
		}

		buttonTouchLastFrame = buttonTouch;
		buttonTouchedLastFrame = buttonTouched;
	}
}
