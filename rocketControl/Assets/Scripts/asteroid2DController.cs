using UnityEngine;
using System.Collections;

public class asteroid2DController : MonoBehaviour {
	public int health = 10;
	public Vector2 initialVelocity;

	Vector3 axis;
	float angle;
	Vector3 vel;
	float rotationSpeed;

	// Use this for initialization
	void Start () {
		transform.rotation.ToAngleAxis (out angle, out axis);
		float speed = Random.Range (0f, 1f);
		if (initialVelocity.x == 0 && initialVelocity.y == 0) {
			Vector2 velocityDir = new Vector2 (Random.Range (-1F, 1F), Random.Range (-1F, 1F)).normalized;
			vel = speed * velocityDir;
		} else {
			vel = initialVelocity;
		}
		rotationSpeed = 0.1f / transform.localScale.x;
	}

	void updateKinematics(){
		transform.RotateAround (transform.position, axis, angle * rotationSpeed);
		transform.position += vel * Time.fixedDeltaTime;
	}

	void explode(){
		Destroy (gameObject);
	}

	// Update is called once per frame
	void FixedUpdate () {
		Debug.Log (health);
		if (health <= 0) {
			explode();
		}
		if (Camera.main.WorldToScreenPoint (transform.position).x < 0
						|| Camera.main.WorldToScreenPoint (transform.position).x > Screen.width){
			vel.x = -vel.x;
		}
		if (Camera.main.WorldToScreenPoint (transform.position).y < 0
		    || Camera.main.WorldToScreenPoint (transform.position).y > Screen.height){
			vel.y = -vel.y;
		}
		updateKinematics ();
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "enemy" || col.gameObject.tag == "terrain") {
			Vector2 impulse = new Vector2(-vel.x, -vel.y);
			rigidbody2D.AddForce(impulse);
		}
		if (col.gameObject.name == "bullet") {
			health--;
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "bullet") {
			health--;
		}
	}
}
