using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ca;
using System;

public class DungeonGeneration : MonoBehaviour
{
    // CA params
    public int seed = 7;
    private System.Random rnd;
    public int grid_width = 50;
    public int grid_height = 50; 
    public int CA_iteration_num = 10;
    public float init_fill_prob = 0.4f;

    // map definition
    private int[,] map; 

    // Map generation params
    public float grid_size = 1;
    public int wall_height = 5;
    public float chest_ratio = 0.1f;
    public float rock_ratio = 0.05f;
    public float teleportation_ratio = 0.01f;
    private float[] start_pos = new float[2];
    private int toggle = 1;
    public float human_height = 1f;
    private List<Tuple<int, int, GameObject>> chests = new List<Tuple<int, int, GameObject>>();
    private List<Tuple<int, int, GameObject>> transes = new List<Tuple<int, int, GameObject>>();
    private float prev_dz = 0f;    
    public int height_3 = 30;
    public bool is_collision_detection = false;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize params
        this.rnd = new System.Random(this.seed);
        // Create floor
        this.Create_floor();
        // Create map
        this.Create_map();
        // Initialize start position
        GameObject.Find("Main Camera").transform.position = new Vector3(this.start_pos[0], this.human_height, this.start_pos[1]);
        // Debug.LogFormat ("camera x y z: {0} {1} {2}", GameObject.Find("Main Camera").transform.position.x, GameObject.Find("Main Camera").transform.position.y, GameObject.Find("Main Camera").transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        bool is_changed = false;
        if (Input.GetKeyDown("space")) {
            is_changed = true;
            if (this.toggle == 1) {
                this.toggle = 3;
            } else {
                this.toggle = 1;
            }
        }
        if (this.toggle == 1) {
            if (is_changed) {
                // camera.transform.Translate (this.start_pos[0], 0, this.start_pos[1]);
                GameObject.Find("Main Camera").transform.position = new Vector3(this.start_pos[0], this.human_height, this.start_pos[1]);
                GameObject.Find("Main Camera").transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            // grab the main camera position
		    Vector3 cam_pos = Camera.main.transform.position;
            int x = (int) (Mathf.Floor(cam_pos.x / this.grid_size));
            int z = (int) (Mathf.Floor(cam_pos.z / this.grid_size));

            // get the horizontal and verticle controls (arrows, or WASD keys)
            float dx = Input.GetAxis ("Horizontal");
            float dz = Input.GetAxis ("Vertical");


            // sensitivity factors for translate and rotate
            float translate_factor = 0.1f;
            float rotate_factor = 2.0f;

            // move the camera based on the keyboard input
            if (Camera.current != null) {
                if (this.is_collision_detection) {
                    bool is_forbid_forward = false;
                    for (int i = 0; i < this.grid_width; i++)
                    {
                        for (int j = 0; j < this.grid_height; j++)
                        {
                            int map_z = (int)Mathf.Floor((cam_pos.z + dz * translate_factor) / this.grid_size);
                            int map_x = x;
                            if (this.map[map_x, map_z] == 1) {
                                is_forbid_forward = true;
                                break;
                            }        
                        }
                    }
                    if (is_forbid_forward) {
                        // translate forward or backwards
                        Camera.current.transform.Translate (0, 0, - dz * translate_factor);

                        // rotate left or right
                        Camera.current.transform.Rotate (0, dx * rotate_factor, 0);
                    } else {
                        // translate forward or backwards
                        Camera.current.transform.Translate (0, 0, dz * translate_factor);

                        // rotate left or right
                        Camera.current.transform.Rotate (0, dx * rotate_factor, 0);
                    }
                } else {
                    // translate forward or backwards
                    Camera.current.transform.Translate (0, 0, dz * translate_factor);

                    // rotate left or right
                    Camera.current.transform.Rotate (0, dx * rotate_factor, 0);
                
                }
            }

            // Debug.LogFormat ("camera x z: {0} {1}", x, z);


            // near chest
            for (int i = 0; i < this.chests.Count; i++)
            {
                if ((Mathf.Abs(this.chests[i].Item1 - x) <= 1) && (Mathf.Abs(this.chests[i].Item2 - z) <= 1))
                {
                    this.Openchest(this.chests[i].Item3);
                } else {
                    this.Closechest(this.chests[i].Item3);
                }
            }

            // near trans
            for (int i = 0; i < this.transes.Count; i++)
            {
                if ((Mathf.Abs(this.transes[i].Item1 - x) <= 1) && (Mathf.Abs(this.transes[i].Item2 - z) <= 1))
                {
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        GameObject.Find("Main Camera").transform.position = new Vector3(this.start_pos[0], this.human_height, this.start_pos[1]);
                    }
                } 
            }

            this.prev_dz = dz;



        } else {
            if (is_changed) {
                GameObject.Find("Main Camera").transform.position = new Vector3(this.grid_size / 2 + (this.grid_width / 2) * this.grid_size, this.grid_size * this.height_3, this.grid_size / 2 + (this.grid_height / 2) * this.grid_size);
                GameObject.Find("Main Camera").transform.eulerAngles = new Vector3(90f, 0f, 0f);
            }
            // get the horizontal and verticle controls (arrows, or WASD keys)
            float dx = Input.GetAxis ("Horizontal");
            float dz = Input.GetAxis ("Vertical");

            // sensitivity factors for translate and rotate
            float translate_factor = 0.6f;

            // move the camera based on the keyboard input
            if (Camera.current != null) {
                // translate 
                Camera.current.transform.Translate (0, dz * translate_factor, 0);
                Camera.current.transform.Translate (dx * translate_factor, 0, 0);
            }
        }

    }

    // Create basic map
    void Create_map() {
        // Get map from CA
        ca.Cave cave = new ca.Cave(this.seed, this.grid_width, this.grid_height, this.CA_iteration_num, this.init_fill_prob);
        this.map = cave.get_map();

        // Create walls
        int cnt = 0;
        bool is_first_empty = true;
        for (int i = 0; i < this.grid_width; i++)
        {
            for (int j = 0; j < this.grid_height; j++)
            {
                if (this.map[i, j] == 1) {
                    cnt ++;
                    for (int k = 0; k < this.wall_height; k++)
                    {
                        // create a new cube
                        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        s.name = cnt.ToString("Cube 0");  // give this cube a name
                        // modify the size and position of the cube
                        s.transform.localScale = new Vector3 (this.grid_size, this.grid_size, this.grid_size);
                        s.transform.position = new Vector3 (this.grid_size / 2 + i * this.grid_size, this.grid_size / 2 + k * this.grid_size, this.grid_size / 2 + j * this.grid_size);
                        // create a texture
                        Texture2D texture = this.make_wall_texture();
                        // change the color of the object
                        Renderer rend = s.GetComponent<Renderer>();
                        // rend.material.mainTexture = texture;
                        Material material = new Material(Shader.Find("Diffuse"));
                        material.mainTexture = texture;
                        rend.material = material;                        
                    }

                } else {
                    float rand_num = (float) this.rnd.NextDouble();
                    if (is_first_empty) {
                        this.start_pos[0] = this.grid_size / 2 + i * this.grid_size;
                        this.start_pos[1] = this.grid_size / 2 + j * this.grid_size;
                        is_first_empty = false;
                        GameObject trans = this.CreateTeleportation(i, j, true);
                        Tuple<int, int, GameObject> trans_tuple = new Tuple<int, int, GameObject> (i, j, trans);
                        this.transes.Add(trans_tuple);
                    }
                    if ((this.Around_condition(i, j))) {
                        if (rand_num < this.chest_ratio) {
                            GameObject cap = this.CreateChests(i, j);
                            Tuple<int, int, GameObject> chest = new Tuple<int, int, GameObject> (i, j, cap);
                            this.chests.Add(chest);
                            // Debug.LogFormat ("chest x z: {0} {1}", i, j);
                        }
                        if ((rand_num > this.chest_ratio) && (rand_num < this.rock_ratio + this.chest_ratio)) {
                            this.Create_rocks(i, j);
                        }
                        if (rand_num > (this.chest_ratio + this.teleportation_ratio) && rand_num < (this.rock_ratio + this.chest_ratio + this.teleportation_ratio)) {
                            GameObject trans = this.CreateTeleportation(i, j, false);
                            Tuple<int, int, GameObject> trans_tuple = new Tuple<int, int, GameObject> (i, j, trans);
                            this.transes.Add(trans_tuple);
                        }
                    }

                }
            }
        }
    }

    void Create_floor() {
        int cnt = 0;
        for (int i = 0; i < this.grid_width; i++)
        {
            for (int j = 0; j < this.grid_height; j++)
            {
                // call the routine that makes a mesh (a cube) from scratch
                Mesh floor_mesh = this.CreateFloorMesh(i, j);

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s = new GameObject("Floor-Mesh");
                s.name = cnt.ToString("Floor-Mesh 0");
                s.AddComponent<MeshFilter>();
                s.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s.GetComponent<MeshFilter>().mesh = floor_mesh;

                // create a texture
                Texture2D texture = this.make_floor_texture();

                // attach the texture to the mesh
                Renderer renderer = s.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;                
                cnt ++;
            }
        } 
    }

    GameObject CreateChests(int x, int y) { // chest position on the map
        float cent_x = this.grid_size / 2.0f + x * this.grid_size;
        float cent_z = this.grid_size / 2.0f + y * this.grid_size;
        float thickness = this.grid_size / 12.0f;
        float check_height = this.grid_size * (1 / 3.0f);

        // create a bottom
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "bottom";  // give name
        // modify the size and position
        s.transform.localScale = new Vector3 (this.grid_size * 1.0f, thickness, this.grid_size / 2.0f);
        s.transform.position = new Vector3 (cent_x, thickness / 2.0f, cent_z);
        // change the color of the object
        Renderer rend = s.GetComponent<Renderer>();
        rend.material.color = new Color (0.79f, 0.74f, 0.70f, 1f);

        // texture of borders
        Texture2D texture = this.make_chest_texture(); 
        Material material = new Material(Shader.Find("Diffuse"));

        // create a border
        s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "border 01";  // give name
        // modify the size and position
        s.transform.localScale = new Vector3 (this.grid_size * 1.0f, check_height, thickness);
        s.transform.position = new Vector3 (cent_x, thickness + check_height / 2.0f, cent_z - ((this.grid_size / 4.0f) - (thickness / 2.0f)));
        // change the color of the object
        rend = s.GetComponent<Renderer>();
        material.mainTexture = texture;
        rend.material = material;           
        // rend.material.color = new Color (0.86f, 0.74f, 0.52f, 1f);

        // create a border
        s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "border 02";  // give name
        // modify the size and position
        s.transform.localScale = new Vector3 (this.grid_size * 1.0f, check_height, thickness);
        s.transform.position = new Vector3 (cent_x, thickness + check_height / 2.0f, cent_z + ((this.grid_size / 4.0f) - (thickness / 2.0f)));
        // change the color of the object
        rend = s.GetComponent<Renderer>();
        rend = s.GetComponent<Renderer>();
        material.mainTexture = texture;
        rend.material = material;   
        // rend.material.color = new Color (0.86f, 0.74f, 0.52f, 1f);

        // create a border
        s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "border 03";  // give name
        // modify the size and position
        s.transform.localScale = new Vector3 (thickness, check_height, (this.grid_size / 2.0f) - (2 * thickness));
        s.transform.position = new Vector3 (cent_x - ((this.grid_size / 2.0f) - (thickness / 2.0f)), thickness + check_height / 2.0f, cent_z);
        // change the color of the object
        rend = s.GetComponent<Renderer>();
        rend = s.GetComponent<Renderer>();
        material.mainTexture = texture;
        rend.material = material;           
        // rend.material.color = new Color (0.86f, 0.74f, 0.52f, 1f);

        // create a border
        s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "border 04";  // give name
        // modify the size and position
        s.transform.localScale = new Vector3 (thickness, check_height, (this.grid_size / 2.0f) - (2 * thickness));
        s.transform.position = new Vector3 (cent_x + ((this.grid_size / 2.0f) - (thickness / 2.0f)), thickness + check_height / 2.0f, cent_z);
        // change the color of the object
        rend = s.GetComponent<Renderer>();
        rend = s.GetComponent<Renderer>();
        material.mainTexture = texture;
        rend.material = material;           
        // rend.material.color = new Color (0.86f, 0.74f, 0.52f, 1f);

        // create a cap
        s = GameObject.CreatePrimitive(PrimitiveType.Cube);
        s.name = "cap";  // give name
        // modify the size and position
        s.transform.localScale = new Vector3 (this.grid_size * 1.0f, thickness, this.grid_size / 2.0f);
        s.transform.position = new Vector3 (cent_x, thickness * (1.5f) + check_height, cent_z);
        // s.transform.eulerAngles = new Vector3(45f, 0f, 0f);
        // change the color of the object
        rend = s.GetComponent<Renderer>();
        rend.material.color = new Color (0.79f, 0.74f, 0.70f, 1f);

        return s;
    }

    GameObject CreateTeleportation(int x, int y, bool is_start) {
        GameObject s = new GameObject();
        for (int k = 0; k < 4; k++)
        {
            s = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // create a new cube            
            s.name = k.ToString("Cube 0");  // give this cube a name
            // modify the size and position of the cube
            s.transform.localScale = new Vector3 (this.grid_size / 2, this.grid_size / 2, this.grid_size / 2);
            s.transform.position = new Vector3 (this.grid_size / 2 + x * this.grid_size, this.grid_size / 4 + k * (this.grid_size / 2), this.grid_size / 2 + y * this.grid_size);
            // create a texture
            Texture2D texture = this.make_base_texture();
            if (k == 3)
            { 
                texture = this.make_light_texture(is_start);            
            }
            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            // rend.material.mainTexture = texture;
            Material material = new Material(Shader.Find("Diffuse"));
            material.mainTexture = texture;
            rend.material = material;              
        }
        return s;
    }

    void Openchest(GameObject cap) {
        float update_angle = 30;
        Vector3 original_position = cap.transform.position;
        Vector3 original_angles = cap.transform.eulerAngles;
        Vector3 original_scale = cap.transform.localScale;
        float r = original_scale.z;
        if (Mathf.Abs(original_angles.x - 90) < 0.00001) {return;}
        cap.transform.position = new Vector3(original_position.x, original_position.y + ((r / 2.0f) * (Mathf.Sin(original_angles.x + update_angle) - Mathf.Sin(original_angles.x))), original_position.z + ((r / 2.0f) * (Mathf.Cos(original_angles.x) - Mathf.Cos(original_angles.x + update_angle))));
        cap.transform.eulerAngles = new Vector3(original_angles.x + update_angle, original_angles.y, original_angles.z);
        // cap.transform.position = new Vector3(original_position.x, original_position.y + ((r / 2.0f) * (Mathf.Sin(original_angles.x + update_angle) - Mathf.Sin(original_angles.x))), original_position.z + ((r / 2.0f) * (Mathf.Cos(original_angles.x + update_angle) - Mathf.Cos(original_angles.x))));
        // cap.transform.eulerAngles = new Vector3(-(original_angles.x + update_angle), original_angles.y, original_angles.z);
    }

    void Closechest(GameObject cap) {
        float update_angle = 30;
        Vector3 original_position = cap.transform.position;
        Vector3 original_angles = cap.transform.eulerAngles;
        Vector3 original_scale = cap.transform.localScale;
        float r = original_scale.z;
        if (Mathf.Abs(original_angles.x - 0) < 0.00001) {return;}
        cap.transform.position = new Vector3(original_position.x, original_position.y + ((r / 2.0f) * (Mathf.Sin(original_angles.x - update_angle) - Mathf.Sin(original_angles.x))), original_position.z + ((r / 2.0f) * (Mathf.Cos(original_angles.x) - Mathf.Cos(original_angles.x - update_angle))));
        cap.transform.eulerAngles = new Vector3(original_angles.x - update_angle, original_angles.y, original_angles.z);
        // cap.transform.position = new Vector3(original_position.x, original_position.y + ((r / 2.0f) * (Mathf.Sin(original_angles.x - update_angle) - Mathf.Sin(original_angles.x))), original_position.z + ((r / 2.0f) * (Mathf.Cos(original_angles.x - update_angle) - Mathf.Cos(original_angles.x))));
        // cap.transform.eulerAngles = new Vector3(-(original_angles.x - update_angle), original_angles.y, original_angles.z);
    }

    void Create_rocks(int x, int y) {
        float cent_x = this.grid_size / 2.0f + x * this.grid_size;
        float cent_z = this.grid_size / 2.0f + y * this.grid_size;
        if (this.rnd.NextDouble() < 0.3f) { // 4 spheres

            for (int i = 0; i < 4; i++)
            {
                float radius1 = (float)this.rnd.NextDouble() * (this.grid_size / 8) + this.grid_size / 16;
                float radius2 = (float)this.rnd.NextDouble() * (this.grid_size / 16);
                float radius3 = (float)this.rnd.NextDouble() * (this.grid_size / 8) + this.grid_size / 16;
                float translate_bias1 = ((float)this.rnd.NextDouble() / 8.0f) * this.grid_size;
                float translate_bias2 = ((float)this.rnd.NextDouble() / 8.0f) * this.grid_size;
                // create a sphere
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);                
                s.name = "rock-sphere 0" + i;  // give name
                // modify the size and position
                s.transform.localScale = new Vector3 (radius1, radius2, radius3);
                s.transform.position = new Vector3 ((this.grid_size / 2) * Mathf.Ceil(i / 2) + translate_bias1 + cent_x - (this.grid_size / 2.0f), radius2 / 2.0f, (this.grid_size / 2) * Mathf.Ceil(i % 2) + translate_bias2 + cent_z - (this.grid_size / 2.0f));
                // change the color of the object          
                Renderer rend = s.GetComponent<Renderer>();      
                rend.material.color = new Color (0.63f, 0.64f, 0.65f, 1f);
            }
        } else if ((this.rnd.NextDouble() > 0.3f) && (this.rnd.NextDouble() < 0.6f)) { // N Cylinder
            int N = (int)Mathf.Ceil((float)this.rnd.NextDouble() * 3 + 1);
            float height_sum = 0;
            for (int i = 0; i < N; i++)
            {
                float radius1 = (float)this.rnd.NextDouble() * (this.grid_size / 3) + this.grid_size / 9;
                float radius2 = (float)this.rnd.NextDouble() * (this.grid_size / 3) + this.grid_size / 9;
                float height = (float)this.rnd.NextDouble() * (this.grid_size / 16);
                float translate_bias1 = ((float)this.rnd.NextDouble() / 18.0f) * this.grid_size;
                float translate_bias2 = ((float)this.rnd.NextDouble() / 18.0f) * this.grid_size;
                // create a cylinder
                GameObject ss = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ss.name = "rock-cylinder 0" + i;  // give name
                // modify the size and position
                ss.transform.localScale = new Vector3 (radius1, height, radius2);
                ss.transform.position = new Vector3 (cent_x + translate_bias1, (height) + height_sum, cent_z + translate_bias2);
                // change the color of the object
                Renderer rendd = ss.GetComponent<Renderer>();
                rendd.material.color = new Color (0.63f, 0.64f, 0.65f, 1f);
                height_sum += (height * 2);
            }
        } else { // N Cylinders and 1 spheres
            int N = (int)Mathf.Ceil((float)this.rnd.NextDouble() * 3 + 1);
            float height_sum = 0;
            for (int i = 0; i < N; i++)
            {
                float radius1 = (float)this.rnd.NextDouble() * (this.grid_size / 3) + this.grid_size / 9;
                float radius2 = (float)this.rnd.NextDouble() * (this.grid_size / 3) + this.grid_size / 9;
                float height = (float)this.rnd.NextDouble() * (this.grid_size / 16);
                float translate_bias1 = ((float)this.rnd.NextDouble() / 18.0f) * this.grid_size;
                float translate_bias2 = ((float)this.rnd.NextDouble() / 18.0f) * this.grid_size;
                // create a cylinder
                GameObject ss = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                ss.name = "rock-cylinder 0" + i;  // give name
                // modify the size and position
                ss.transform.localScale = new Vector3 (radius1, height, radius2);
                ss.transform.position = new Vector3 (cent_x + translate_bias1, (height) + height_sum, cent_z + translate_bias2);
                // change the color of the object
                Renderer rendd = ss.GetComponent<Renderer>();
                rendd.material.color = new Color (0.63f, 0.64f, 0.65f, 1f);
                height_sum += (height * 2);
            }
            float radius1s = (float)this.rnd.NextDouble() * (this.grid_size / 5) + this.grid_size / 9;
            float radius2s = (float)this.rnd.NextDouble() * (this.grid_size / 5) + this.grid_size / 9;
            float translate_bias1s = ((float)this.rnd.NextDouble() / 18.0f) * this.grid_size;
            float translate_bias2s = ((float)this.rnd.NextDouble() / 18.0f) * this.grid_size;
            float radius3 = (float)this.rnd.NextDouble() * (this.grid_size / 20);
            // create a sphere
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = "rock-sphere 00";  // give name
            // modify the size and position
            s.transform.localScale = new Vector3 (radius1s, radius3, radius2s);
            s.transform.position = new Vector3 (cent_x + translate_bias1s, height_sum + radius3 / 2.0f, cent_z + translate_bias2s);
            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (0.63f, 0.64f, 0.65f, 1f);               
        }

    }

	// create a mesh that consists of two triangles that make up a quad
	Mesh CreateFloorMesh(int i, int j) {
		
		// create a mesh object
		Mesh mesh = new Mesh();

		// vertices of a cube
		Vector3[] verts = new Vector3[4];

		// vertices for a quad

		verts[0] = new Vector3 ( this.grid_size * (i + 1), 0, this.grid_size * j);
		verts[1] = new Vector3 ( this.grid_size * (i + 1), 0,  this.grid_size * (j + 1));
		verts[2] = new Vector3 (this.grid_size * i, 0,  this.grid_size * (j + 1));
		verts[3] = new Vector3 (this.grid_size * i, 0, this.grid_size * j);

		// create uv coordinates

		Vector2[] uv = new Vector2[4];

		uv[0] = new Vector2(1, 0);
		uv[1] = new Vector2(1, 1);
		uv[2] = new Vector2(0, 1);
		uv[3] = new Vector2(0, 0);

		// two triangles for the face

		int[] tris = new int[6];  // need 3 vertices per triangle

		tris[0] = 0;
		tris[1] = 2;
		tris[2] = 1;
		tris[3] = 0;
		tris[4] = 3;
		tris[5] = 2;

		// save the vertices and triangles in the mesh object
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uv;  // save the uv texture coordinates

		mesh.RecalculateNormals();  // automatically calculate the vertex normals
        mesh.RecalculateBounds();

		return (mesh);
	}    


    Texture2D make_wall_texture() {
        Texture2D texture = Resources.Load("wall3") as Texture2D;
        return texture;
    }

    Texture2D make_floor_texture() {
        Texture2D texture = Resources.Load("floor") as Texture2D;
        return texture;
    }

    Texture2D make_light_texture(bool is_start) {
        if (is_start) {
            Texture2D texture = Resources.Load("light_starter") as Texture2D;
            return texture;                        
        } else {
            Texture2D texture = Resources.Load("light") as Texture2D;
            return texture;
        }
        
    }

    Texture2D make_base_texture() {
        Texture2D texture = Resources.Load("trans_base") as Texture2D;
        return texture;
    }

    // create a texture using Perlin noise
	Texture2D make_chest_texture() {
        int texture_width = 64;
        int texture_height = 64;
        float scale = 10;

		// create the texture and an array of colors that will be copied into the texture
		Texture2D texture = new Texture2D (texture_width, texture_height);
		Color[] colors = new Color[texture_width * texture_height];

		// create the Perlin noise pattern in "colors"
		for (int i = 0; i < texture_width; i++)
			for (int j = 0; j < texture_height; j++) {
				float x = scale * i / (float) texture_width;
				float y = scale * j / (float) texture_height;
				float t = Mathf.PerlinNoise (x, y) * 0.5f;                          // Perlin noise!
                float t2 = Mathf.PerlinNoise (x, y);
                float t3 = Mathf.PerlinNoise (x, y);
                float r = 0.95f - t;
                float g = 0.83f - t;
                float b = 0.63f - t; 
                float s = r + g + b; // sum
				colors [j * texture_width + i] = new Color (r, g, b, 1.0f);  // gray scale values (r = g = b) rgb(91%, 73%, 53%)
			}

		// copy the colors into the texture
		texture.SetPixels(colors);

		// do texture-y stuff, probably including making the mipmap levels
		texture.Apply();

		// return the texture
		return (texture);
	}

    bool Around_condition(int x, int y) { // return 0 (up), 1 (right), 2 (down), 3 (left)
        bool flag = true;
        List<int[]> around = new List<int[]>();
        List<int[]> neighbors = new List<int[]> {
            new int[] {x, y - 1}, // up
            new int[] {x + 1, y}, // right
            new int[] {x, y + 1}, // down
            new int[] {x - 1, y} // left
        };
        foreach (var item in neighbors)
        {
            if (this.map[item[0], item[1]] == 1) {
                around.Add(item);
            }
        }
        if (around.Count != 2) {
            flag = false;
        }
        if (this.map[neighbors[2][0], neighbors[2][1]] != 1) {
            flag = false;
        }
        return flag;
    }
}
