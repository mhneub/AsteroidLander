using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class rocketController : MonoBehaviour
{

	// public variables
	public LayerMask ignore;
	//public GameObject YouLoseText;
	public GameObject YouWinText;
	public Rigidbody2D bullet;
	public Sprite spriteNoFlame;
	public Sprite spriteWithFlame;

	// constant variables
	const float Pi = 3.14159f;

	// physics parameters
	float allowedSpeed = 0.5f;
	float allowedAngle = 5f;
	float rocketSizeScale = 0.5f;
	float thrusterSpeed = 1f;
	float rotationScale = -8f;
	float bulletSpeed = 10f;
	float lowPassFilterFactor = 0.2f;
	float bulletTimeInterval = 0.1f;	// minimum time between bullets in seconds
	float timeSinceLastBullet = 0.0f;
	float swipeTimeInterval = 0.5f;
	float timeSincePrevSwipe = 0.0f;
	bool flipped = false;
	bool swipedThruster = false;
	bool swipedGun = false;
	Vector3 originPosition;
	Vector3 originAngles;
	Vector2 vel;
	SpriteRenderer spriteRenderer;
	bool thrustedLastFrame;
	bool thrustedThisFrame;

	ParticleSystem thrustParticleSystem;
	ParticleSystem thrustBurstParticleSystem;

	int lastLevel = 3;

	//old physics parameters
	/*float gravity = -0.5f;
	Vector3 acc;
	Vector3 velocity;
	Vector3 jerk;
	Vector3 G;
	float gunRecoil = -10000f;
	float friction = 0.5f;
	float rotationSpeed = -5f;
	int rotationScale = 5;*/

	void Init()
	{
		YouWinText.gameObject.renderer.enabled = false;
		//transform.position = originPosition;
		transform.eulerAngles = originAngles;
		rigidbody2D.velocity = Vector3.zero;
		rigidbody2D.angularVelocity = 0f;
	}

	// Use this for initialization
	void Start()
	{
		Screen.orientation = ScreenOrientation.LandscapeRight;
		vel = new Vector3(0f, 0f, 0f);
		//originPosition = new Vector3(0f, 0f, 0f);
		originAngles = new Vector3(0f, 0f, 0f);
		transform.localScale = new Vector3(rocketSizeScale, rocketSizeScale, rocketSizeScale);
		Init();

		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = spriteNoFlame;
		thrustedLastFrame = false;
		thrustedThisFrame = false;

		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem> ();
		thrustParticleSystem = ps [0];
		thrustBurstParticleSystem = ps [1];
		thrustParticleSystem.Stop ();
		thrustBurstParticleSystem.Stop ();
	}

	void thrust()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up);
		if (hit.collider != null) {
			GameObject recipient = hit.transform.gameObject;
			if (recipient.tag == "enemy") {
				float dist = (recipient.transform.position - transform.position).magnitude;
				recipient.rigidbody2D.AddForce(-transform.up * 10 / (dist)); //add distance parameter maybe
			}
		}
		thrustedThisFrame = true;
	}

	void shoot()
	{
		if (timeSinceLastBullet >= bulletTimeInterval) {
			Rigidbody2D instantiatedBullet = Instantiate(bullet, transform.position, transform.rotation) as Rigidbody2D;
			instantiatedBullet.velocity = transform.up * bulletSpeed;	
			Physics2D.IgnoreCollision(instantiatedBullet.collider2D, rigidbody2D.collider2D);

			timeSinceLastBullet = 0.0f;
		}
	}

	void rotateRocket(float axis)
	{
		Quaternion intermediateQuat = Quaternion.Euler(transform.eulerAngles);
		Quaternion targetQuat;
		if (flipped) {
			targetQuat = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -rotationScale * axis * 180 / Pi + 180);
		} else {
			targetQuat = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, rotationScale * axis * 180 / Pi);
		}
		transform.rotation = Quaternion.Lerp(intermediateQuat, targetQuat, lowPassFilterFactor);
	}

	IEnumerator delay(bool win)
	{
		YouWinText.gameObject.renderer.enabled = true;
		yield return new WaitForSeconds(1);
		if (Application.loadedLevel == lastLevel) {
			Application.LoadLevel(0);
		} else {
			Application.LoadLevel(Application.loadedLevel + 1);
		}

	}

	void rocketDeath()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	void win()
	{
		StartCoroutine(delay(true));
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.name == "platform") {
			if (vel.magnitude < allowedSpeed 
				&& (Mathf.Abs(transform.eulerAngles.z) < allowedAngle || Mathf.Abs(transform.eulerAngles.z) > (360 - allowedAngle))) {
				win();
			} else {
				rocketDeath();
			}
		}
		else if (col.gameObject.tag == "enemy" || col.gameObject.tag == "terrain") {
			rocketDeath();
		}
	}

	/*void rotate180()
	{
		//flipped = -flipped;
		if (flipped == 180)
			flipped = 0;
		else if (flipped == 0)
			flipped = 180;
	}*/

	void ButtonPressed(string buttonTag){
		if (buttonTag == "thruster") {
			rigidbody2D.AddForce(thrusterSpeed * transform.up);
			thrust ();
		}
		if (buttonTag == "gun") {
			rigidbody2D.AddForce(-transform.up);
			shoot();
		}
	}

	void ButtonSwiped(string buttonTag){
		if (buttonTag == "thruster") {
			swipedThruster = true;
			timeSincePrevSwipe = 0.0f;
		}
		if (buttonTag == "gun") {
			swipedGun = true;
			timeSincePrevSwipe = 0.0f;
		}
	}

	bool detectTwoFingerSwipe(){
		if (timeSincePrevSwipe < swipeTimeInterval && swipedGun && swipedThruster) {
			swipedGun = false;
			swipedThruster = false;
			timeSincePrevSwipe = 0.0f;
			return true;
		} else if (timeSincePrevSwipe > swipeTimeInterval) {
			swipedGun = false;
			swipedThruster = false;
			return false;
		}
		return false;
	}

	void FixedUpdate()
	{
		// used in shoot()
		timeSinceLastBullet += Time.deltaTime;

		// check for two button swipe to flip rocket
		timeSincePrevSwipe += Time.deltaTime;
		if(detectTwoFingerSwipe()){
			flipped = !flipped;
		}

		// switch rocket sprite to flame/no-flame if thrusting started/stopped this frame
		if (!thrustedThisFrame && thrustedLastFrame) {
			spriteRenderer.sprite = spriteNoFlame;
			//thrustParticleSystem.Stop ();
		} else if (thrustedThisFrame && !thrustedLastFrame) {
			spriteRenderer.sprite = spriteWithFlame;
		
			thrustBurstParticleSystem.Emit(25);
			//thrustParticleSystem.Play();
		}
		thrustedLastFrame = thrustedThisFrame;
		thrustedThisFrame = false;

		//velocity used in OnCollisionEnter2D
		vel = rigidbody2D.velocity; 

		rotateRocket (Input.acceleration.x);

		// rocket cannot go offscreen
		if (Camera.main.WorldToScreenPoint(transform.position).x < 0
			|| Camera.main.WorldToScreenPoint(transform.position).x > Screen.width
			|| Camera.main.WorldToScreenPoint(transform.position).y < 0
			|| Camera.main.WorldToScreenPoint(transform.position).y > Screen.height) {
			rocketDeath();
		}
	}
}


///////OLD CODE FOR BUTTON PRESS AND SWIPE//////////

	/*void translateRocket(Vector3 touchPosition)
	{
		RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
		if (hit.collider != null) {
			GameObject recipient = hit.transform.gameObject;
			if (recipient.name == "Thruster") {
				rigidbody2D.AddForce(thrusterSpeed * transform.up);
				thrust();
			} else if (recipient.name == "Gun") {
				rigidbody2D.AddForce(-transform.up);
				shoot();
			}
		}
	}*/


	/*bool detectSwipe(Touch touch)
	{
		swipeDist[touch.fingerId] = 0;
		if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
			if (hit.collider != null) {
				GameObject recipient = hit.transform.gameObject;
				if (recipient.name == "Thruster") {
					initialTouchPos[touch.fingerId] = touch.position;
					swipedThruster = true;
				} else if (recipient.name == "Gun") {
					initialTouchPos[touch.fingerId] = touch.position;
					swipedGun = true;
				}
			}
		} else if (touch.phase == TouchPhase.Ended) {
			swipeDist[touch.fingerId] = (new Vector2(0, touch.position.y) - new Vector2(0, initialTouchPos[touch.fingerId].y)).magnitude;
		}
		return swipeDist[touch.fingerId] > minSwipeDist;
	}

	bool detectTwoFingerSwipe()
	{
		foreach (Touch touch in Input.touches) {
			bool swipe = detectSwipe(touch);
			if (swipedGun && swipedThruster && swipe) {
				swipedGun = false;
				swipedThruster = false;
				return true;
			}
		}
		return false;
	}*/




		/*if (Input.touchCount > 0) {

			bool swiped = false;
			if (Input.touchCount == 2)
				swiped = detectTwoFingerSwipe();
			if (swiped) {
				flipped =  !flipped;
				//rotate180();
			} /*else {
				foreach (Touch touch in Input.touches) {
					translateRocket(Camera.main.ScreenToWorldPoint(touch.position));
				}
			}

		}*/
