using UnityEngine;
using System.Collections;

public class asteroid2DController : MonoBehaviour {

	public int health = 10;
	public Vector2 initialVelocity;
	public float initialRotation;	// degrees/second
	public ParticleSystem explodeParticles;
	public Sprite fullHealthAsteroid;
	public Sprite damagedAsteroid1;
	public Sprite damagedAsteroid2;

	int originalHealth;
	SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {

		originalHealth = health;
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = fullHealthAsteroid;

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

		Instantiate(explodeParticles, transform.position, transform.rotation);
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (health <= 0) {
			explode();
		} else if (health == originalHealth * 2 / 3) {
			spriteRenderer.sprite = damagedAsteroid1;
		} else if (health == originalHealth / 3) {
			spriteRenderer.sprite = damagedAsteroid2;
		}

	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "bullet") {
			health--;
		}
	}
}
