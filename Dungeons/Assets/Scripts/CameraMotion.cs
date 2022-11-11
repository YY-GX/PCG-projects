// This sample code demonstrates how to create geometry "on demand" based on camera motion.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {

	int max_plane = -1;       // the number of planes that we've made
	float plane_size = 3.0f;  // size of the planes

	void Start () {
		
		// start with one plane
		create_new_plane();
	}
	
	// Move the camera, and maybe create a new plane
	void Update () {

		// get the horizontal and verticle controls (arrows, or WASD keys)
		float dx = Input.GetAxis ("Horizontal");
		float dz = Input.GetAxis ("Vertical");

		// sensitivity factors for translate and rotate
		float translate_factor = 0.3f;
		float rotate_factor = 5.0f;

		// move the camera based on the keyboard input
		if (Camera.current != null) {
			// translate forward or backwards
			Camera.current.transform.Translate (0, 0, dz * translate_factor);

			// rotate left or right
			Camera.current.transform.Rotate (0, dx * rotate_factor, 0);

		}

		// grab the main camera position
		Vector3 cam_pos = Camera.main.transform.position;
		//Debug.LogFormat ("x z: {0} {1}", cam_pos.x, cam_pos.z);

		// if the camera has moved far enough, create another plane
		if (cam_pos.z > (max_plane + 0.5) * plane_size * 2) {
			create_new_plane ();
		}

	}

	// create a new plane
	void create_new_plane() {

		int index = max_plane + 1;
		float plane_scale = plane_size / 5.0f;

		// create a new plane
		GameObject s = GameObject.CreatePrimitive(PrimitiveType.Plane);
		s.name = index.ToString("Plane 0");  // give this plane a name

		// modify the size and position of the plane
		s.transform.localScale = new Vector3 (plane_scale, plane_scale, plane_scale);
		// move plane to proper location in z
		s.transform.position = new Vector3 (0.0f, 0.0f, 2 * plane_size * (index + 0.0f));

		// change the color of the plane
		Renderer rend = s.GetComponent<Renderer>();
		// alternate between two colors
		if (max_plane % 2 == 0)
			rend.material.color = new Color (0.2f, 0.2f, 0.7f, 1.0f);
		else
			rend.material.color = new Color (0.7f, 0.1f, 0.1f, 1.0f);
		
		// increment the number of planes that we've created
		max_plane++;
	}
}
