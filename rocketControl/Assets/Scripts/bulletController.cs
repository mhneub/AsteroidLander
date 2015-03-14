using UnityEngine;
using System.Collections;

public class bulletController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.main.WorldToScreenPoint (transform.position).x < 0
		    || Camera.main.WorldToScreenPoint (transform.position).x > Screen.width
		    || Camera.main.WorldToScreenPoint (transform.position).y < 0
		    || Camera.main.WorldToScreenPoint (transform.position).y > Screen.height ) {
			Destroy(gameObject);
		}
	}

	/*void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "enemy") {
			Destroy(col.gameObject);
			//Physics2D.IgnoreCollision(col.gameObject.collider2D, rigidbody2D.collider2D);
		}
	}*/

	void OnTriggerEnter2D(Collider2D col) {
		//if (col.gameObject.name == "platform") {
		if (col.gameObject.tag == "terrain" || col.gameObject.tag == "enemy") {
			Destroy(gameObject);
		}
	}
}