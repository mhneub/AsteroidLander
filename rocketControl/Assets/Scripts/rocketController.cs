﻿using UnityEngine;
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
	public ParticleSystem explodeParticles;

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
	float timeSinceThrusterSwipe = 1000f;
	float timeSinceGunSwipe = 1000f;

	int flipped = 0;
	Vector3 originPosition;
	Vector3 originAngles;
	SpriteRenderer spriteRenderer;

	bool isThrusting;
	bool isShooting;
	bool rocketDead;

	ParticleSystem thrustParticleSystem;
	ParticleSystem thrustBurstParticleSystem;

	int lastLevel = 3;


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
		//vel = new Vector3(0f, 0f, 0f);
		//originPosition = new Vector3(0f, 0f, 0f);
		originAngles = new Vector3(0f, 0f, 0f);
		transform.localScale = new Vector3(rocketSizeScale, rocketSizeScale, rocketSizeScale);
		Init();

		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = spriteNoFlame;
		//thrustedLastFrame = false;
		//thrustedThisFrame = false;
		isThrusting = false;

		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem> ();
		thrustParticleSystem = ps [0];
		thrustBurstParticleSystem = ps [1];
		thrustParticleSystem.Stop ();
		thrustBurstParticleSystem.Stop ();
		rocketDead = false;
	}

	void shoot()
	{
		// don't shoot if rocket died
		if (timeSinceLastBullet >= bulletTimeInterval && !rocketDead) {
			Rigidbody2D instantiatedBullet = Instantiate(bullet, transform.position, transform.rotation) as Rigidbody2D;
			instantiatedBullet.velocity = transform.up * bulletSpeed;	
			Physics2D.IgnoreCollision(instantiatedBullet.collider2D, rigidbody2D.collider2D);

			timeSinceLastBullet = 0.0f;
		}
	}

	void thrust()
	{
		rigidbody2D.AddForce (thrusterSpeed * transform.up);
	}

	void rotateRocket(float axis)
	{
		Quaternion intermediateQuat = Quaternion.Euler(transform.eulerAngles);
		Quaternion targetQuat;
		targetQuat = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, rotationScale * axis * 180 / Pi + flipped);
		transform.rotation = Quaternion.Lerp(intermediateQuat, targetQuat, lowPassFilterFactor);
	}

	IEnumerator delayWin()
	{
		YouWinText.gameObject.renderer.enabled = true;
		yield return new WaitForSeconds(1);
		if (Application.loadedLevel == lastLevel) {
			Application.LoadLevel(0);
		} else {
			Application.LoadLevel(Application.loadedLevel + 1);
		}
	}

	IEnumerator delayLose()
	{
		yield return new WaitForSeconds(2);
		Application.LoadLevel(Application.loadedLevel);
	}

	void rocketDeath()
	{
		if (!rocketDead) {
			rocketDead = true;

			// actions on death
			gameObject.collider2D.enabled = false;
			gameObject.renderer.enabled = false;
			gameObject.GetComponentInChildren<CapsuleCollider> ().enabled = false;
			thrustParticleSystem.Stop ();

			Instantiate(explodeParticles, transform.position, transform.rotation);

			StartCoroutine (delayLose ());
			//Application.LoadLevel(Application.loadedLevel);
		}
	}

	void win()
	{
		StartCoroutine(delayWin());
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		Debug.Log ("rocket collided with " + col.gameObject.name);
		if (col.gameObject.name == "platform") {
			if (rigidbody2D.velocity.magnitude < allowedSpeed 
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

	void rotate180()
	{
		if (flipped == 180)
			flipped = 0;
		else if (flipped == 0)
			flipped = 180;
	}

	void OnThrustStart() {
		isThrusting = true;
		if (!rocketDead) {
			spriteRenderer.sprite = spriteWithFlame;
			thrustBurstParticleSystem.Emit(12);
			thrustParticleSystem.Play();
		}
	}

	void OnThrustStop() {
		isThrusting = false;
		spriteRenderer.sprite = spriteNoFlame;
		thrustParticleSystem.Stop();
	}

	void OnGunStart() {
		isShooting = true;
	}

	void OnGunStop() {
		isShooting = false;
	}

	void OnDoubleSwipe() {
		rotate180();
	}

	void ButtonDown(string buttonTag) {
		//Debug.Log (buttonTag + " down");

		if (buttonTag == "thruster") {
			OnThrustStart ();
		} else if (buttonTag == "gun") {
			OnGunStart ();
		}
	}
	void ButtonUp(string buttonTag) {	// means button was released and not swiped
		//Debug.Log (buttonTag + " up");

		if (buttonTag == "thruster") {
			OnThrustStop ();
		} else if (buttonTag == "gun") {
			OnGunStop ();
		}
	}

	void ButtonSwiped(string buttonTag) {
		//Debug.Log (buttonTag + " swiped");

		// see if double-swipe happened
		bool doubleSwiped = false;
		if (buttonTag == "thruster") {
			timeSinceThrusterSwipe = 0f;
			if (timeSinceGunSwipe <= swipeTimeInterval) {
				doubleSwiped = true;
			}
			OnThrustStop();
		} else if (buttonTag == "gun") {
			isShooting = false;
			timeSinceGunSwipe = 0f;
			if (timeSinceThrusterSwipe <= swipeTimeInterval) {
				doubleSwiped = true;
			}
			OnGunStop ();
		}

		if (doubleSwiped) {
			OnDoubleSwipe();
		}
	}


	void FixedUpdate()
	{
		// update timeSince counters
		timeSinceLastBullet += Time.deltaTime;
		timeSinceThrusterSwipe += Time.deltaTime;
		timeSinceGunSwipe += Time.deltaTime;

		if (isShooting) {
			shoot ();
		} 
		if (isThrusting) {
			thrust();
		}

		rotateRocket (Input.acceleration.x);
	}
}
