using UnityEngine;
using System.Collections;

public class bulletController : MonoBehaviour {

	public GameObject explodeParticles;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// now handled by box colliders of boundary_invisible
		/*if (Camera.main.WorldToScreenPoint (transform.position).x < 0
		    || Camera.main.WorldToScreenPoint (transform.position).x > Screen.width
		    || Camera.main.WorldToScreenPoint (transform.position).y < 0
		    || Camera.main.WorldToScreenPoint (transform.position).y > Screen.height ) {
			Destroy(gameObject);
		}*/
	}

	void OnTriggerEnter2D(Collider2D col) {
		// destroy bullet if it hits asteroid or terrain
		if (col.gameObject.tag == "terrain" || col.gameObject.tag == "enemy") {
			Destroy(gameObject);

			// explode
			Instantiate(explodeParticles, transform.position, transform.rotation);
		}
	}
}