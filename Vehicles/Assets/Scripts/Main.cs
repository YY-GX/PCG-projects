using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using space_craft;

public class Main : MonoBehaviour
{
    public int seed = 7;
    public int numOfsc = 10;
    public float scDistance = 100f;
    private List<space_craft.SpaceCraft> spaceCrafts;
    // Start is called before the first frame update
    void Start()
    {
        // Initialization
        System.Random rnd = new System.Random(seed);
        this.spaceCrafts = new List<space_craft.SpaceCraft>();
        for (int i = 0; i < this.numOfsc; i++)
        {
            float z = i * this.scDistance;
            space_craft.SpaceCraft spaceCraft = new space_craft.SpaceCraft(new Vector3(0, 0, z), rnd);
            this.spaceCrafts.Add(spaceCraft);
        }
        GameObject.Find("Main Camera").transform.position = new Vector3(32, 0, 36);
        GameObject.Find("Main Camera").transform.rotation = new Quaternion(0f, -0.9f, 0f, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        // Control camera
        // get the horizontal and verticle controls (arrows, or WASD keys)
        float dx = Input.GetAxis ("Horizontal");
        float dz = Input.GetAxis ("Vertical");
        // Debug.Log(dx + ", " +  dz);
        
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
        
        // Quaternion cam_pos = Camera.main.transform.rotation;
        // Debug.Log(cam_pos);
        // Debug.LogFormat ("x z: {0} {1}", cam_pos.x, cam_pos.z);

        // update components
        foreach (var spaceCraft in this.spaceCrafts)
        {
            spaceCraft.update();
        }
    }
}
