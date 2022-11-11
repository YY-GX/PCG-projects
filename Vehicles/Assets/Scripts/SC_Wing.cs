using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// TODO: 1. wing1(normal) 2. wing2(normal with curve) 3. wing3(ring)/wing4(normal reverse) 4. tail2 5. tail3 6. make wing & tail size changeable randomly 7. rotate tail & wing 8. change body if time permits
namespace space_craft {
    class  SC_Wing {
        private System.Random rnd;
        private Vector3 pos_wing;
        private int chosen_type;
        private List<GameObject> wing1;
        private List<GameObject> wing2;
        private List<GameObject> wing3;
        private bool wing_1_clockwise = true;
        private bool wing_2_clockwise = false;
        private float wing_1_delta = 0f;
        private float wing_2_delta = 0f;
        public SC_Wing(Vector3 pos_wing, System.Random rnd) {
            this.pos_wing = pos_wing;
            this.rnd = rnd;
            float rnd_number = (float)rnd.NextDouble();
            if (rnd_number < 0.33f)
            {
                this.chosen_type = 0;
                this.wing1 = this.create_wing_1_1();
                this.create_wing_1_2();
            }
            else if (rnd_number < 0.67f)
            {
                this.chosen_type = 1;
                this.wing2 = this.create_wing_2();
            }
            else
            {
                this.chosen_type = 2;
                this.wing3 = this.create_wing_3_1();    
                this.create_wing_3_2();
            }
        }
        public List<GameObject> create_wing_1_1() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_wing.x;
            float y = this.pos_wing.y;
            float z = this.pos_wing.z;
            Mesh mesh_wing_1_1 = new Mesh();
            var verts = new Vector3[12];
            float rand_num = (float) this.rnd.NextDouble() * 6f - 3f;
            // initial vertices
            verts[0] = new Vector3(x, y + .4f, z - 5f);
            verts[1] = new Vector3(x, y + .4f, z + 7f);
            verts[2] = new Vector3(x + 20f + rand_num, y + 0f, z - 8f);
            verts[3] = new Vector3(x + 21f + rand_num, y + 0f, z - 11f);
            verts[4] = new Vector3(x, y - 0.6f, z - 5f);
            verts[5] = new Vector3(x, y - 0f, z + 9f);

            // initial tris
            var tris = new int[] {
                0, 2, 3, 0, 1, 2, 4, 3, 2, 4, 2, 5, 0, 3, 4, 1, 5, 2
            };
            // create mesh
            mesh_wing_1_1.vertices = verts;
            mesh_wing_1_1.triangles = tris;
            // sd the mesh
            // mesh_wing_1_1 = sd.iterate(mesh_wing_1_1);
            // create game_obj
            GameObject s = new GameObject("Wing_1");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_wing_1_1;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.75f, .75f, .75f, 1.0f);
            Texture2D texture = make_steel_texture();
            rend.material.mainTexture = texture;
            
            // Create other 3 wings by rotating
            GameObject s_copy_1 = GameObject.Instantiate(s);
            GameObject pivot_point_1 = new GameObject("pivoprit point2");
            pivot_point_1.transform.position = new Vector3(x, y, z);
            s_copy_1.transform.parent = pivot_point_1.gameObject.transform;
            pivot_point_1.transform.eulerAngles = new Vector3(0,0,-15);
            
            GameObject s_copy_2 = GameObject.Instantiate(s);
            GameObject pivot_point_2 = new GameObject("pivot point3");
            pivot_point_2.transform.position = new Vector3(x, y, z);
            s_copy_2.transform.parent = pivot_point_2.gameObject.transform;
            pivot_point_2.transform.eulerAngles = new Vector3(0,0,180 - 15);
            
            GameObject s_copy_3 = GameObject.Instantiate(s);
            GameObject pivot_point_3 = new GameObject("pivot point4");
            pivot_point_3.transform.position = new Vector3(x, y, z);
            s_copy_3.transform.parent = pivot_point_3.gameObject.transform;
            pivot_point_3.transform.eulerAngles = new Vector3(0,0,180 + 15);
            
            GameObject pivot_point = new GameObject("pivot point1");
            pivot_point.transform.position = new Vector3(x, y, z);
            s.transform.parent = pivot_point.gameObject.transform;
            pivot_point.transform.eulerAngles = new Vector3(0,0,15);
            
            GameObject pivot_point_par_1 = new GameObject("pivot point parent 1");
            pivot_point_par_1.transform.position = new Vector3(x, y, z);
            s_copy_1.transform.parent = pivot_point_par_1.gameObject.transform;
            s_copy_2.transform.parent = pivot_point_par_1.gameObject.transform;
            
            GameObject pivot_point_par_2 = new GameObject("pivot point parent 2");
            pivot_point_par_2.transform.position = new Vector3(x, y, z);
            s_copy_3.transform.parent = pivot_point_par_2.gameObject.transform;
            s.transform.parent = pivot_point_par_2.gameObject.transform;

            return new List<GameObject>() {pivot_point_par_1, pivot_point_par_2};
        }
        public void create_wing_1_2() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_wing.x;
            float y = this.pos_wing.y;
            float z = this.pos_wing.z;
            Mesh mesh_wing_1_2 = new Mesh();
            var verts = new Vector3[12];
            // initial vertices
            verts[0] = new Vector3(x - 0.5f, y, z - 7f);
            verts[1] = new Vector3(x - 0.5f, y, z - 15f);
            verts[2] = new Vector3(x, y + 10f, z - 17f);
            verts[3] = new Vector3(x, y + 10f, z - 18.75f);
            verts[4] = new Vector3(x + 0.5f, y, z - 7f);
            verts[5] = new Vector3(x + 0.5f, y, z - 15f);

            // initial tris
            var tris = new int[] {
                // 0, 3, 2, 0, 1, 3, 4, 2, 3, 4, 3, 5, 0, 2, 4, 3, 1, 5
                0, 2, 3, 0, 3, 1, 4, 3, 2, 4, 5, 3, 0, 4, 2, 3, 5, 1
            };
            // create mesh
            mesh_wing_1_2.vertices = verts;
            mesh_wing_1_2.triangles = tris;
            // sd the mesh
            // mesh_wing_1_2 = sd.iterate(mesh_wing_1_2);
            // create game_obj
            GameObject s = new GameObject("Wing_1");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_wing_1_2;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.75f, .75f, .75f, 1.0f);
            Texture2D texture = make_steel_texture();
            rend.material.mainTexture = texture;
            
            // Create other 3 wings by rotating
            GameObject s_copy_1 = GameObject.Instantiate(s);
            GameObject pivot_point_1 = new GameObject("pivot point5");
            pivot_point_1.transform.position = new Vector3(x, y, z);
            s_copy_1.transform.parent = pivot_point_1.gameObject.transform;
            pivot_point_1.transform.eulerAngles = new Vector3(0,0,-30);

            GameObject pivot_point = new GameObject("pivot point6");
            pivot_point.transform.position = new Vector3(x, y, z);
            s.transform.parent = pivot_point.gameObject.transform;
            pivot_point.transform.eulerAngles = new Vector3(0,0,30);
        }
        
        public List<GameObject> create_wing_2() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(4);
            float x = this.pos_wing.x;
            float y = this.pos_wing.y;
            float z = this.pos_wing.z;
            // initial param
            float rnd_number = (float)this.rnd.NextDouble();
            float wing_length = 20f + rnd_number * 3f;
            float thickness = 1f;
            float pos_z = -3f;
            // initial mesh
            Mesh mesh_wing_2_1 = new Mesh();
            var verts = new Vector3[16];
            // initial vertices
            verts[0] = new Vector3(x, y + wing_length, z + pos_z - 2f);
            verts[1] = new Vector3(x + wing_length, y, z + pos_z);
            verts[2] = new Vector3(x, y - wing_length, z + pos_z - 2f);
            verts[3] = new Vector3(x - wing_length, y, z + pos_z);
            
            verts[4] = new Vector3(x, y + wing_length, z + pos_z + 2f);
            verts[5] = new Vector3(x + wing_length, y, z + pos_z);
            verts[6] = new Vector3(x, y - wing_length, z + pos_z + 2f);
            verts[7] = new Vector3(x - wing_length, y, z + pos_z);
            
            verts[8] = new Vector3(x, y + wing_length - thickness, z + pos_z - 2f);
            verts[9] = new Vector3(x + wing_length - thickness, y, z + pos_z);
            verts[10] = new Vector3(x, y - wing_length + thickness, z + pos_z - 2f);
            verts[11] = new Vector3(x - wing_length + thickness, y, z + pos_z);
            
            verts[12] = new Vector3(x, y + wing_length - thickness, z + pos_z + 2f);
            verts[13] = new Vector3(x + wing_length - thickness, y, z + pos_z);
            verts[14] = new Vector3(x, y - wing_length + thickness, z + pos_z + 2f);
            verts[15] = new Vector3(x - wing_length + thickness, y, z + pos_z);
            // initial tris
            var tris = new int[] {
                0, 4, 5, 0, 5, 1, 1, 5, 6, 1, 6, 2, 2, 6, 7, 2, 7, 3, 3, 7, 4, 3, 4, 0,
                8, 13, 12, 8, 9, 13, 9, 14, 13, 9, 10, 14, 10, 15, 14, 10, 11, 15, 11, 12, 15, 11, 8, 12,
                // 4, 12, 13, 4, 13, 5, 5, 13, 14, 5, 14, 6, 6, 14, 15, 6, 15, 7, 7, 15, 12, 7, 12, 4,
                // 4, 13, 12, 4, 5, 13, 5, 14, 13, 5, 6, 14, 6, 15, 14, 6, 7, 15, 7, 12, 15, 7, 4, 12,
                0, 9, 8, 0, 1, 9, 1, 10, 9, 1, 2, 10, 2, 11, 10, 2, 3, 11, 3, 8, 11, 3, 0, 8,
                4, 12, 13, 4, 13, 5, 5, 13, 14, 5, 14, 6, 6, 14, 15, 6, 15, 7, 7, 15, 12, 7, 12, 4,
            };
            // create mesh
            mesh_wing_2_1.vertices = verts;
            mesh_wing_2_1.triangles = tris;
            // sd the mesh
            mesh_wing_2_1 = sd.iterate(mesh_wing_2_1);
            // create game_obj
            GameObject s1 = new GameObject("Wing_2");
            s1.AddComponent<MeshFilter>();
            s1.AddComponent<MeshRenderer>();
            s1.GetComponent<MeshFilter>().mesh = mesh_wing_2_1;
            // color & texture
            Renderer rend = s1.GetComponent<Renderer>();
            rend.material.color = new Color (.75f, .75f, .75f, 1.0f);
            // Texture2D texture = make_steel_texture();
            // rend.material.mainTexture = texture;
            
            
            pos_z = -6f;
            // initial mesh
            Mesh mesh_wing_2_2 = new Mesh();
            verts = new Vector3[16];
            // initial vertices
            verts[0] = new Vector3(x, y + wing_length, z + pos_z);
            verts[1] = new Vector3(x + wing_length, y, z + pos_z - 2f);
            verts[2] = new Vector3(x, y - wing_length, z + pos_z);
            verts[3] = new Vector3(x - wing_length, y, z + pos_z - 2f);
            
            verts[4] = new Vector3(x, y + wing_length, z + pos_z);
            verts[5] = new Vector3(x + wing_length, y, z + pos_z + 2f);
            verts[6] = new Vector3(x, y - wing_length, z + pos_z);
            verts[7] = new Vector3(x - wing_length, y, z + pos_z + 2f);
            
            verts[8] = new Vector3(x, y + wing_length - thickness, z + pos_z);
            verts[9] = new Vector3(x + wing_length - thickness, y, z + pos_z - 2f);
            verts[10] = new Vector3(x, y - wing_length + thickness, z + pos_z);
            verts[11] = new Vector3(x - wing_length + thickness, y, z + pos_z - 2f);
            
            verts[12] = new Vector3(x, y + wing_length - thickness, z + pos_z);
            verts[13] = new Vector3(x + wing_length - thickness, y, z + pos_z + 2f);
            verts[14] = new Vector3(x, y - wing_length + thickness, z + pos_z);
            verts[15] = new Vector3(x - wing_length + thickness, y, z + pos_z + 2f);
            // initial tris
            tris = new int[] {
                0, 4, 5, 0, 5, 1, 1, 5, 6, 1, 6, 2, 2, 6, 7, 2, 7, 3, 3, 7, 4, 3, 4, 0,
                8, 13, 12, 8, 9, 13, 9, 14, 13, 9, 10, 14, 10, 15, 14, 10, 11, 15, 11, 12, 15, 11, 8, 12,
                4, 12, 13, 4, 13, 5, 5, 13, 14, 5, 14, 6, 6, 14, 15, 6, 15, 7, 7, 15, 12, 7, 12, 4,
                0, 9, 8, 0, 1, 9, 1, 10, 9, 1, 2, 10, 2, 11, 10, 2, 3, 11, 3, 8, 11, 3, 0, 8,
            };
            // create mesh
            mesh_wing_2_2.vertices = verts;
            mesh_wing_2_2.triangles = tris;
            // sd the mesh
            mesh_wing_2_2 = sd.iterate(mesh_wing_2_2);
            // create game_obj
            GameObject s2 = new GameObject("Wing_2");
            s2.AddComponent<MeshFilter>();
            s2.AddComponent<MeshRenderer>();
            s2.GetComponent<MeshFilter>().mesh = mesh_wing_2_2;
            // color & texture
            rend = s2.GetComponent<Renderer>();
            rend.material.color = new Color (.75f, .75f, .75f, 1.0f);
            // Texture2D texture = make_steel_texture();
            // rend.material.mainTexture = texture;

            return new List<GameObject>() {s1, s2};
        }
        public List<GameObject> create_wing_3_1() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(4);
            float x = this.pos_wing.x;
            float y = this.pos_wing.y;
            float z = this.pos_wing.z;
            Mesh mesh_wing_3_1 = new Mesh();
            var verts = new Vector3[12];
            float rand_num = (float) this.rnd.NextDouble() * 8f - 4f;
            // initial vertices
            verts[0] = new Vector3(x, y + .4f, z - 3f);
            verts[1] = new Vector3(x, y + .4f, z + 7f);
            verts[2] = new Vector3(x + 35f + rand_num, y + 0f, z - 8f);
            verts[3] = new Vector3(x + 40f + rand_num, y + 0f, z - 11f);
            verts[4] = new Vector3(x, y - 0.6f, z - 3f);
            verts[5] = new Vector3(x, y - 0f, z + 9f);
            // verts[6] = new Vector3(x + 35f, y - 0f, z - 8f);
            // verts[7] = new Vector3(x + 35f, y - 0f, z - 9f);

            // initial tris
            var tris = new int[] {
                0, 2, 3, 0, 1, 2, 4, 3, 2, 4, 2, 5, 0, 3, 4, 1, 5, 2,
                // 6, 2, 1, 6, 5, 2, 7, 6, 1, 7, 5, 6, 7, 1, 5, 
                // 6, 1, 0, 7, 6, 0, 6, 5, 4, 7, 6, 4
            };
            // create mesh
            mesh_wing_3_1.vertices = verts;
            mesh_wing_3_1.triangles = tris;
            // sd the mesh
            mesh_wing_3_1 = sd.iterate(mesh_wing_3_1);
            // create game_obj
            GameObject s = new GameObject("Wing_1");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_wing_3_1;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.75f, .75f, .75f, 1.0f);
            Texture2D texture = make_steel_texture();
            rend.material.mainTexture = texture;
            
            // Create other 3 wings by rotating
            GameObject s_copy_1 = GameObject.Instantiate(s);
            GameObject pivot_point_1 = new GameObject("pivoprit point2");
            pivot_point_1.transform.position = new Vector3(x, y, z);
            s_copy_1.transform.parent = pivot_point_1.gameObject.transform;
            pivot_point_1.transform.eulerAngles = new Vector3(0,0,-15);
            
            GameObject s_copy_2 = GameObject.Instantiate(s);
            GameObject pivot_point_2 = new GameObject("pivot point3");
            pivot_point_2.transform.position = new Vector3(x, y, z);
            s_copy_2.transform.parent = pivot_point_2.gameObject.transform;
            pivot_point_2.transform.eulerAngles = new Vector3(0,0,180 - 15);
            
            GameObject s_copy_3 = GameObject.Instantiate(s);
            GameObject pivot_point_3 = new GameObject("pivot point4");
            pivot_point_3.transform.position = new Vector3(x, y, z);
            s_copy_3.transform.parent = pivot_point_3.gameObject.transform;
            pivot_point_3.transform.eulerAngles = new Vector3(0,0,180 + 15);
            
            GameObject pivot_point = new GameObject("pivot point1");
            pivot_point.transform.position = new Vector3(x, y, z);
            s.transform.parent = pivot_point.gameObject.transform;
            pivot_point.transform.eulerAngles = new Vector3(0,0,15);
            
            GameObject pivot_point_par_1 = new GameObject("pivot point parent 1");
            pivot_point_par_1.transform.position = new Vector3(x, y, z);
            s_copy_1.transform.parent = pivot_point_par_1.gameObject.transform;
            s_copy_2.transform.parent = pivot_point_par_1.gameObject.transform;
            
            GameObject pivot_point_par_2 = new GameObject("pivot point parent 2");
            pivot_point_par_2.transform.position = new Vector3(x, y, z);
            s_copy_3.transform.parent = pivot_point_par_2.gameObject.transform;
            s.transform.parent = pivot_point_par_2.gameObject.transform;

            return new List<GameObject>() {pivot_point_par_1, pivot_point_par_2};
        }
        public void create_wing_3_2() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_wing.x;
            float y = this.pos_wing.y;
            float z = this.pos_wing.z;
            Mesh mesh_wing_3_2 = new Mesh();
            var verts = new Vector3[14];
            // initial vertices
            verts[0] = new Vector3(x - 0.5f, y, z - 7f);
            verts[1] = new Vector3(x - 0.5f, y, z - 15f);
            verts[2] = new Vector3(x, y + 10f, z - 17f);
            verts[3] = new Vector3(x, y + 10f, z - 18.75f);
            verts[4] = new Vector3(x + 0.5f, y, z - 7f);
            verts[5] = new Vector3(x + 0.5f, y, z - 15f);
            verts[6] = new Vector3(x + 0.5f, y + 10f, z - 20f);
            verts[7] = new Vector3(x + 0.5f, y + 11f, z - 20f);

            // initial tris
            var tris = new int[] {
                // 0, 3, 2, 0, 1, 3, 4, 2, 3, 4, 3, 5, 0, 2, 4, 3, 1, 5
                0, 2, 3, 0, 3, 1, 4, 3, 2, 4, 5, 3, 0, 4, 2, 3, 5, 1,
                6, 0, 2, 6, 2, 0, 7, 6, 2, 7, 2, 6
            };
            // create mesh
            mesh_wing_3_2.vertices = verts;
            mesh_wing_3_2.triangles = tris;
            // sd the mesh
            mesh_wing_3_2 = sd.iterate(mesh_wing_3_2);
            // create game_obj
            GameObject s = new GameObject("Wing_1");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_wing_3_2;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.75f, .75f, .75f, 1.0f);
            Texture2D texture = make_steel_texture();
            rend.material.mainTexture = texture;
            
            // Create other 3 wings by rotating
            GameObject s_copy_1 = GameObject.Instantiate(s);
            GameObject pivot_point_1 = new GameObject("pivot point5");
            pivot_point_1.transform.position = new Vector3(x, y, z);
            s_copy_1.transform.parent = pivot_point_1.gameObject.transform;
            pivot_point_1.transform.eulerAngles = new Vector3(0,0,-30);

            GameObject pivot_point = new GameObject("pivot point6");
            pivot_point.transform.position = new Vector3(x, y, z);
            s.transform.parent = pivot_point.gameObject.transform;
            pivot_point.transform.eulerAngles = new Vector3(0,0,30);
        }
        
        public void update(float alpha)
        {
            if (this.chosen_type == 0)
            {
                this.update1(alpha);
            }
            else if (this.chosen_type == 1)
            {
                this.update2(alpha);
            }
            else
            {
                this.update3(alpha);
            }
        }
        public void update1(float alpha)
        {
            var pivot_point_par_1 = this.wing1[0];
            var pivot_point_par_2 = this.wing1[1];
            if (this.wing_1_delta > 13f)
            {
                this.wing_1_clockwise = false;
            }
            else if (this.wing_1_delta < 0f)
            {
                this.wing_1_clockwise = true;
            }
            
            if (this.wing_2_delta < - 13f)
            {
                this.wing_2_clockwise = true;
            }
            else if (this.wing_2_delta > 0f)
            {
                this.wing_2_clockwise = false;
            }

            if (this.wing_1_clockwise)
            {
                pivot_point_par_1.transform.eulerAngles = new Vector3(0,0,pivot_point_par_1.transform.eulerAngles.z + alpha); 
                this.wing_1_delta += alpha;
            }
            else
            {
                pivot_point_par_1.transform.eulerAngles = new Vector3(0,0,pivot_point_par_1.transform.eulerAngles.z - alpha);
                this.wing_1_delta -= alpha;
            }
            
            if (this.wing_2_clockwise)
            {
                pivot_point_par_2.transform.eulerAngles = new Vector3(0,0,pivot_point_par_2.transform.eulerAngles.z + alpha);
                this.wing_2_delta += alpha;
            }
            else
            {
                pivot_point_par_2.transform.eulerAngles = new Vector3(0,0,pivot_point_par_2.transform.eulerAngles.z - alpha);
                this.wing_2_delta -= alpha;
            }
            
            this.wing1 = new List<GameObject>() {pivot_point_par_1, pivot_point_par_2};
        }

        public void update2(float alpha)
        {
            this.wing2[0].transform.eulerAngles = new Vector3(0,0,this.wing2[0].transform.eulerAngles.z + alpha);
            this.wing2[1].transform.eulerAngles = new Vector3(0,0,this.wing2[1].transform.eulerAngles.z + alpha);
        }
        public void update3(float alpha)
        {
            var pivot_point_par_1 = this.wing3[0];
            var pivot_point_par_2 = this.wing3[1];
            if (this.wing_1_delta > 13f)
            {
                this.wing_1_clockwise = false;
            }
            else if (this.wing_1_delta < 0f)
            {
                this.wing_1_clockwise = true;
            }
            
            if (this.wing_2_delta < - 13f)
            {
                this.wing_2_clockwise = true;
            }
            else if (this.wing_2_delta > 0f)
            {
                this.wing_2_clockwise = false;
            }

            if (this.wing_1_clockwise)
            {
                pivot_point_par_1.transform.eulerAngles = new Vector3(0,0,pivot_point_par_1.transform.eulerAngles.z + alpha); 
                this.wing_1_delta += alpha;
            }
            else
            {
                pivot_point_par_1.transform.eulerAngles = new Vector3(0,0,pivot_point_par_1.transform.eulerAngles.z - alpha);
                this.wing_1_delta -= alpha;
            }
            
            if (this.wing_2_clockwise)
            {
                pivot_point_par_2.transform.eulerAngles = new Vector3(0,0,pivot_point_par_2.transform.eulerAngles.z + alpha);
                this.wing_2_delta += alpha;
            }
            else
            {
                pivot_point_par_2.transform.eulerAngles = new Vector3(0,0,pivot_point_par_2.transform.eulerAngles.z - alpha);
                this.wing_2_delta -= alpha;
            }
            
            this.wing3 = new List<GameObject>() {pivot_point_par_1, pivot_point_par_2};
        }

        public Texture2D make_steel_texture() {
            Texture2D texture = Resources.Load("steel1") as Texture2D;
            return texture;
        }
    }
}