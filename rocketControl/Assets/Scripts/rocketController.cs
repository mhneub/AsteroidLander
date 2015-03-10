using UnityEngine;
using System.Collections;

public class rocketController : MonoBehaviour {

	public LayerMask ignore;
	public GameObject YouLoseText;

	const float Pi = 3.14159f;

	public Rigidbody2D bullet;

	float allowedSpeed = 0.5f;
	float allowedAngle = 10f;
	float scale = 0.5f;
	float thrusterSpeed = 1f;
	float rotationSpeed = -5f;
	int rotationScale = 5;
	float bulletSpeed = 5;
	int bulletTimeInterval = 10;
	Vector3 originPosition;
	Vector3 originAngles;
	Vector2 vel;


	//old physics parameters
	float gravity = -0.5f;

	Vector3 acc;
	Vector3 velocity;
	//Vector3 jerk;
	Vector3 G;

	//float gunRecoil = -10000f;
	float friction = 0.5f;

	void Init() {
		YouLoseText.gameObject.renderer.enabled = false;
		transform.position = originPosition;
		transform.eulerAngles = originAngles;
		rigidbody2D.velocity = Vector3.zero;
		rigidbody2D.angularVelocity = 0f;

	}

	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		G = new Vector3 (0f, gravity, 0f);
		acc = new Vector3(0f, 0f, 0f);
		vel = new Vector3(0f, 0f, 0f);
		originPosition = new Vector3 (0f, 0f, 0f);
		originAngles = new Vector3 (0f, 0f, 0f);
		transform.localScale = new Vector3 (scale, scale, scale);
		Init ();
	}

	void thrust(){
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up);
		if (hit.collider != null) {
			GameObject recipient = hit.transform.gameObject;
			if (recipient.tag == "enemy") {
				float dist = (recipient.transform.position - transform.position).magnitude;
				recipient.rigidbody2D.AddForce(-transform.up * 10/(dist)); //add distance parameter maybe
			}
		}
	}

	void shoot(){
		if ((int)(Time.time * 100) % bulletTimeInterval == 0) {
			Rigidbody2D instantiatedBullet = Instantiate (bullet, transform.position, transform.rotation) as Rigidbody2D;
			instantiatedBullet.velocity = transform.up * bulletSpeed;	
			Physics2D.IgnoreCollision(instantiatedBullet.collider2D, rigidbody2D.collider2D);
		}
	}

	void translateRocket(Vector3 touchPosition, Touch touch){
		RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
		if(hit.collider != null){
			GameObject recipient = hit.transform.gameObject;
			if (recipient.name == "Thruster"){
				rigidbody2D.AddForce(thrusterSpeed * transform.up);
				thrust();
				//acc += thrusterSpeed*transform.up;
			}
			else if(recipient.name == "Gun") {
				//acc -= thrusterSpeed*transform.up;
				rigidbody2D.AddForce(-transform.up);
				shoot();
			}
		}

	}

	void rotateRocket(float axis){
		//int rotateStep = (int)(axis * 180/Pi) / rotationScale;
		//transform.Rotate(0, 0, rotateStep * rotationScale * rotationSpeed * Pi / 180);
		Vector3 rotation = new Vector3 (0, 0, -4 * axis * 180/Pi);
		transform.eulerAngles= rotation;
	}

	void updateKinematics(){
		velocity = acc * Time.fixedDeltaTime + G * Time.fixedTime;
		acc -= friction * velocity;
		transform.position += velocity * Time.fixedDeltaTime;
	}

	IEnumerator delay(){
		YouLoseText.gameObject.renderer.enabled = true;
		yield return new WaitForSeconds(1);
		Application.LoadLevel(Application.loadedLevel);
	}

	void rocketDeath(){
		StartCoroutine(delay ());
	}

	void win(){
		Application.LoadLevel(Application.loadedLevel);
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "enemy") {
			rocketDeath ();
		}

		if (col.gameObject.name == "platform") {
			Debug.Log ("win " + vel.magnitude + " " + Mathf.Abs(transform.eulerAngles.z) );
			if (vel.magnitude < allowedSpeed 
			    && (Mathf.Abs(transform.eulerAngles.z) < allowedAngle || Mathf.Abs(transform.eulerAngles.z) > (360 - allowedAngle))) {
				win();
			}
			else{
				rocketDeath ();
			}
		}
	}

	void FixedUpdate () {

/*#if UNITY_EDITOR
		if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(hit.collider != null){
				GameObject recipient = hit.transform.gameObject;;
				if (recipient.name == "Thruster"){
					acc += thrusterSpeed*transform.up;
				}
				else if(recipient.name == "Gun") {
					if (Input.GetMouseButtonDown(0)) {
						acc -= thrusterSpeed*transform.up;
					}
				}
			}
		}

#endif*/
		if(Input.touchCount > 0){
			foreach (Touch touch in Input.touches) {
				translateRocket(Camera.main.ScreenToWorldPoint(touch.position), touch);
			}
		}
		vel = rigidbody2D.velocity;
		rotateRocket (Input.acceleration.x);
		//updateKinematics();

		if (Camera.main.WorldToScreenPoint (transform.position).x < 0
						|| Camera.main.WorldToScreenPoint (transform.position).x > Screen.width
		    			|| Camera.main.WorldToScreenPoint (transform.position).y < 0
		    			|| Camera.main.WorldToScreenPoint (transform.position).y > Screen.height ) {
			rocketDeath();
		}
	}
}
