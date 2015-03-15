using UnityEngine;
using System.Collections;

public class asteroid2DController : MonoBehaviour {

	public int health = 10;
	public Vector2 initialVelocity;
	public float initialRotation;	// degrees/second
	public GameObject explodeParticles;

	// Use this for initialization
	void Start () {
		Vector3 vel;
		float rotationSpeed;

		if (initialVelocity.x == 0 && initialVelocity.y == 0) {
			// this is a non-uniform sampling of possible directions!!
			//Vector2 velocityDir = new Vector2 (Random.Range (-1F, 1F), Random.Range (-1F, 1F)).normalized;
			//vel = speed * velocityDir;

			float speed = Random.Range (0f, 0.2f);
			float theta = Random.Range (0f, 2f * Mathf.PI);
			Vector2 velocityDir = new Vector2(Mathf.Cos (theta), Mathf.Sin (theta));
			vel = speed * velocityDir;
		} else {
			vel = initialVelocity;
		}

		if (initialRotation == 0f) {
			float max = 90f;	// deg/sec
			rotationSpeed = Random.Range(-max, max);
		} else {
			rotationSpeed = initialRotation;
		}

		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
		rb.velocity = vel;
		rb.angularVelocity = rotationSpeed;
	}

	void explode(){
		Destroy (gameObject);

		GameObject particles = Instantiate(explodeParticles, transform.position, transform.rotation) as GameObject;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (health <= 0) {
			explode();
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "bullet") {
			health--;
		}
	}
}
