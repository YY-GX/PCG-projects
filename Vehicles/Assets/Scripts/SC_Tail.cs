using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace space_craft {
    class SC_Tail
    {
        private System.Random rnd;
        private Vector3 pos_tail;
        private int chosen_type;
        private GameObject X;
        private GameObject tail_2;
        private GameObject tail_3;
        private GameObject tail_4;
        public SC_Tail(Vector3 pos_tail, System.Random rnd) {
            this.pos_tail = pos_tail;
            this.create_main();
            this.rnd = rnd;
            float rnd_number = (float)rnd.NextDouble();
            if (rnd_number < 0.25f)
            {
                this.chosen_type = 0;
                this.X = this.create_X();
            } else if (rnd_number < 0.5f)
            {
                this.chosen_type = 1;
                this.tail_2 = this.create_tail_2();
            } else if (rnd_number < 0.75f)
            {
                this.chosen_type = 2;
                this.tail_3 = this.create_tail_3();
            } else
            {
                this.chosen_type = 3;
                this.tail_4 = this.create_tail_4();
            }
        }
        public void create_main() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_tail.x;
            float y = this.pos_tail.y;
            float z = this.pos_tail.z;
            Mesh mesh_main = new Mesh();
            var verts = new Vector3[8];
            // initial vertices
            verts[0] = new Vector3(x, y + 2f, z);
            verts[1] = new Vector3(x + 5f, y, z);
            verts[2] = new Vector3(x, y - 3f, z);
            verts[3] = new Vector3(x - 5f, y, z);

            verts[4] = new Vector3(x, y + 3f, z - 2f);
            verts[5] = new Vector3(x + 3f, y, z - 2f);
            verts[6] = new Vector3(x, y - 3f, z - 2f);
            verts[7] = new Vector3(x - 3f, y, z - 2f);

            // initial tris
            var tris = new int[] {
                4, 0, 1, 4, 1, 5, 5, 1, 2, 5, 2, 6, 6, 2, 3, 6, 3, 7, 7, 3, 0, 7, 0, 4
            };
            // create mesh
            mesh_main.vertices = verts;
            mesh_main.triangles = tris;
            // sd the mesh
            mesh_main = sd.iterate(mesh_main);
            // create game_obj
            GameObject s = new GameObject("Tail_Main");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_main;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;            
        }
        public GameObject create_X() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(4);
            float x = this.pos_tail.x;
            float y = this.pos_tail.y;
            float z = this.pos_tail.z;
            Mesh mesh_main = new Mesh();
            float rand_num = (float) this.rnd.NextDouble() * 6f - 3f;
            var verts = new Vector3[40];
            float width1 = 0.5f / Mathf.Sqrt(2);
            float length1 = (15f + rand_num) / Mathf.Sqrt(2);
            float width2 = 0.75f / Mathf.Sqrt(2);
            float length2 = (17.5f + rand_num) / Mathf.Sqrt(2);
            rand_num = (float) this.rnd.NextDouble() * 4f - 2f;
            float reverse_lth = 10f + rand_num;
            // initial vertices
            verts[0] = new Vector3(x, y + 3f, z - 2f);
            verts[1] = new Vector3(x + 3f, y, z - 2f);
            verts[2] = new Vector3(x, y - 3f, z - 2f);
            verts[3] = new Vector3(x - 3f, y, z - 2f);

            verts[4] = new Vector3(x, y + 0.5f, z - 2.5f);
            verts[5] = new Vector3(x + 0.5f, y, z - 2.5f);
            verts[6] = new Vector3(x, y - 0.5f, z - 2.5f);
            verts[7] = new Vector3(x - 0.5f, y, z - 2.5f);

            verts[8] = new Vector3(x + length1 - width1, y + length1 + width1, z - 2.5f);
            verts[9] = new Vector3(x + length1 + width1, y + length1 - width1, z - 2.5f);

            verts[10] = new Vector3(x + length1 + width1, y - length1 + width1, z - 2.5f);
            verts[11] = new Vector3(x + length1 - width1, y - length1 - width1, z - 2.5f);

            verts[12] = new Vector3(x - length1 + width1, y - length1 - width1, z - 2.5f);
            verts[13] = new Vector3(x - length1 - width1, y - length1 + width1, z - 2.5f);

            verts[14] = new Vector3(x - length1 - width1, y + length1 - width1, z - 2.5f);
            verts[15] = new Vector3(x - length1 + width1, y + length1 + width1, z - 2.5f);

            verts[16] = new Vector3(x, y + 0.5f, z - 3.5f);
            verts[17] = new Vector3(x + 0.5f, y, z - 3.5f);
            verts[18] = new Vector3(x, y - 0.5f, z - 3.5f);
            verts[19] = new Vector3(x - 0.5f, y, z - 3.5f);

            verts[20] = new Vector3(x + length2 - width2, y + length2 + width2, z - 3.5f);
            verts[21] = new Vector3(x + length2 + width2, y + length2 - width2, z - 3.5f);

            verts[22] = new Vector3(x + length2 + width2, y - length2 + width2, z - 3.5f);
            verts[23] = new Vector3(x + length2 - width2, y - length2 - width2, z - 3.5f);

            verts[24] = new Vector3(x - length2 + width2, y - length2 - width2, z - 3.5f);
            verts[25] = new Vector3(x - length2 - width2, y - length2 + width2, z - 3.5f);

            verts[26] = new Vector3(x - length2 - width2, y + length2 - width2, z - 3.5f);
            verts[27] = new Vector3(x - length2 + width2, y + length2 + width2, z - 3.5f);

            verts[28] = new Vector3(x + length2, y + length2, z + reverse_lth);
            verts[29] = new Vector3(x + length2, y - length2, z + reverse_lth);
            verts[30] = new Vector3(x - length2, y - length2, z + reverse_lth);
            verts[31] = new Vector3(x - length2, y + length2, z + reverse_lth);

            // verts[32] = new Vector3(x, y + 3f, z - 4f);
            // verts[33] = new Vector3(x + 3f, y, z - 4f);
            // verts[34] = new Vector3(x, y - 3f, z - 4f);
            // verts[35] = new Vector3(x - 3f, y, z - 4f);

            // verts[36] = new Vector3(x, y + 3f, z - 4.5f);
            // verts[37] = new Vector3(x + 3f, y, z - 4.5f);
            // verts[38] = new Vector3(x, y - 3f, z - 4.5f);
            // verts[39] = new Vector3(x - 3f, y, z - 4.5f);
            // initial tris
            var tris = new int[] {
                4, 0, 1, 4, 1, 5, 5, 1, 2, 5, 2, 6, 6, 2, 3, 6, 3, 7, 7, 3, 0, 7, 0, 4, // link up
                5, 9, 4, 4, 9, 8, 6, 11, 10, 6, 10, 5, 7, 13, 6, 6, 13, 12, 4, 14, 7, 4, 15, 14, // tail_1
                17, 16, 21, 16, 20, 21, 18, 22, 23, 18, 17, 22, 18, 24, 25, 18, 25, 19, 19, 26, 27, 19, 27, 16, 16, 17, 18, 16, 18, 19, // tail_2
                // 21, 8, 9, 21, 20, 8, 23, 22, 10, 23, 10, 11, 25, 24, 12, 25, 12, 13, 27, 26, 14, 27, 14, 15, // (tail_1 & tail_2 link)_1
                21, 9, 17, 17, 9, 5, 17, 5, 10, 17, 10, 22, 18, 23, 11, 18, 11, 6, 18, 6, 24, 24, 6, 12, 19, 25, 13, 19, 13, 7, 26, 19, 7, 26, 7, 14, 27, 4, 16, 27, 15, 4, 20, 16, 4, 20, 4, 8, // (tail_1 & tail_2 link)_2
                28, 8, 9, 28, 21, 20, 28, 20, 8, 28, 9, 21, 29, 10, 11, 29, 23, 22, 29, 22, 10, 29, 11, 23, 30, 12, 13, 30, 25, 24, 30, 24, 12, 30, 13, 25, 31, 14, 15, 31, 27, 26, 31, 26, 14, 31, 15, 27, // reverse
                // 32, 16, 17, 32, 17, 33, 33, 17, 18, 33, 18, 34, 34, 18, 19, 34, 19, 35, 35, 19, 16, 35, 16, 32,
                // 36, 32, 33, 36, 33, 37, 37, 33, 34, 37, 34, 38, 38, 34, 35, 38, 35, 39, 39, 35, 32, 39, 32, 36, 
                // 36, 37, 38, 36, 38, 39
            };
            // create mesh
            mesh_main.vertices = verts;
            mesh_main.triangles = tris;
            // sd the mesh
            mesh_main = sd.iterate(mesh_main);
            // create game_obj
            GameObject s = new GameObject("Tail_X");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_main;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;      
            return s;
        }
        public GameObject create_tail_2() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(4);
            float x = this.pos_tail.x;
            float y = this.pos_tail.y;
            float z = this.pos_tail.z;
            Mesh mesh_tail_2_1 = new Mesh();
            var verts = new Vector3[24];
            float radius1 = 2f;
            float radius = 2f;
            float length = 7f + (float)this.rnd.NextDouble() * 2f;
            // initial vertices
            verts[0] = new Vector3(x, y + 3f, z - 2f);
            verts[1] = new Vector3(x + 3f, y, z - 2f);
            verts[2] = new Vector3(x, y - 3f, z - 2f);
            verts[3] = new Vector3(x - 3f, y, z - 2f);

            verts[18] = new Vector3(x, y + 0.5f, z - 2.5f);
            verts[19] = new Vector3(x + 0.5f, y, z - 2.5f);
            verts[20] = new Vector3(x, y - 0.5f, z - 2.5f);
            verts[21] = new Vector3(x - 0.5f, y, z - 2.5f);
            
            verts[4] = new Vector3(x + radius1, y + radius1, z - 2.5f);
            verts[5] = new Vector3(x + radius1 * 2, y, z - 2.5f);
            verts[6] = new Vector3(x + radius1, y - radius1, z - 2.5f);
            verts[7] = new Vector3(x - radius1, y - radius1, z - 2.5f);
            verts[8] = new Vector3(x - radius * 2, y, z - 2.5f);
            verts[9] = new Vector3(x - radius1, y + radius1, z - 2.5f);
            verts[10] = new Vector3(x, y, z - 2.5f);

            verts[11] = new Vector3(x + radius, y + radius, z - 2.5f - length);
            verts[12] = new Vector3(x + radius * 2, y, z - 2.5f - length);
            verts[13] = new Vector3(x + radius, y - radius, z - 2.5f - length);
            verts[14] = new Vector3(x - radius, y - radius, z - 2.5f - length);
            verts[15] = new Vector3(x - radius * 2, y, z - 2.5f - length);
            verts[16] = new Vector3(x - radius, y + radius, z - 2.5f - length);
            verts[17] = new Vector3(x, y, z - 2.5f - length);
            
            verts[22] = new Vector3(x, y, z - 2.5f);
            verts[23] = new Vector3(x, y, z - 2.5f - length);

            // initial tris
            var tris = new int[] {
                18, 0, 1, 18, 1, 19, 19, 1, 2, 19, 2, 20, 20, 2, 3, 20, 3, 21, 21, 3, 0, 21, 0, 18, // link up
                // 0, 4, 9, 0, 5, 4, 0, 1, 5, 1, 2, 5, 2, 6, 5, 2, 7, 6, 2, 8, 7, 2, 3, 8, 3, 9, 8, 3, 0, 9, // face 1 to link
                18, 10, 9, 18, 4, 22, 18, 5, 4, 18, 19, 5, 19, 20, 5, 5, 20, 6, 20, 22, 6, 20, 7, 10, 20, 8, 7, 20, 21, 8, 21, 9, 8, 21, 18, 9, // face 1 to link
                9, 10, 17, 9, 17, 16, 22, 4, 11, 22, 11, 23, 4, 5, 12, 4, 12, 11, 5, 6, 13, 5, 13, 12, 6, 22, 13, 13, 22, 23, 10, 7, 17, 17, 7, 14, 7, 8, 14, 14, 8, 15, 8, 9, 16, 8, 16, 15, // face
                // 16, 17, 14, 16, 14, 15, 11, 12, 13, 11, 13, 23
            };
            // create mesh
            mesh_tail_2_1.vertices = verts;
            mesh_tail_2_1.triangles = tris;
            // sd the mesh
            mesh_tail_2_1 = sd.iterate(mesh_tail_2_1);
            // create game_obj
            GameObject s1 = new GameObject("Tail_2");
            s1.AddComponent<MeshFilter>();
            s1.AddComponent<MeshRenderer>();
            s1.GetComponent<MeshFilter>().mesh = mesh_tail_2_1;
            // color & texture
            Renderer rend = s1.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;
            
            
            Mesh mesh_tail_2_2 = new Mesh();
            verts = new Vector3[24];
            // initial vertices
            verts[0] = new Vector3(x + radius, y + radius, z - 2.5f - length);
            verts[1] = new Vector3(x + radius * 2, y, z - 2.5f - length);
            verts[2] = new Vector3(x + radius, y - radius, z - 2.5f - length);
            verts[3] = new Vector3(x - radius, y - radius, z - 2.5f - length);
            verts[4] = new Vector3(x - radius * 2, y, z - 2.5f - length);
            verts[5] = new Vector3(x - radius, y + radius, z - 2.5f - length);
            verts[6] = new Vector3(x, y, z - 2.5f - length);
            verts[7] = new Vector3(x, y, z - 2.5f - length);

            // initial tris
            tris = new int[] {
                0, 1, 2, 0, 2, 7, 5, 6, 3, 5, 3, 4
            };
            // create mesh
            mesh_tail_2_2.vertices = verts;
            mesh_tail_2_2.triangles = tris;
            // sd the mesh
            mesh_tail_2_2 = sd.iterate(mesh_tail_2_2);
            // create game_obj
            GameObject s2 = new GameObject("Tail_2");
            s2.AddComponent<MeshFilter>();
            s2.AddComponent<MeshRenderer>();
            s2.GetComponent<MeshFilter>().mesh = mesh_tail_2_2;
            // color & texture
            rend = s2.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;    
            
            GameObject go_tail_2 = new GameObject("Tail 2 parent");
            s1.transform.parent = go_tail_2.gameObject.transform;
            s2.transform.parent = go_tail_2.gameObject.transform;
            return go_tail_2;
        }
        public GameObject create_tail_3() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(4);
            float x = this.pos_tail.x;
            float y = this.pos_tail.y;
            float z = this.pos_tail.z;
            Mesh mesh_main = new Mesh();
            var verts = new Vector3[40];
            float width1 = 0.5f / Mathf.Sqrt(2);
            float length1 = 15f / Mathf.Sqrt(2);
            float width2 = 0.75f / Mathf.Sqrt(2);
            float length2 = 17.5f / Mathf.Sqrt(2);
            float reverse_lth = 10f;
            // initial vertices
            verts[0] = new Vector3(x, y + 3f, z - 2f);
            verts[1] = new Vector3(x + 3f, y, z - 2f);
            verts[2] = new Vector3(x, y - 3f, z - 2f);
            verts[3] = new Vector3(x - 3f, y, z - 2f);

            verts[4] = new Vector3(x, y + 0.5f, z - 2.5f);
            verts[5] = new Vector3(x + 0.5f, y, z - 2.5f);
            verts[6] = new Vector3(x, y - 0.5f, z - 2.5f);
            verts[7] = new Vector3(x - 0.5f, y, z - 2.5f);

            verts[8] = new Vector3(x + length1 - width1, y + length1 + width1, z - 2.5f);
            verts[9] = new Vector3(x + length1 + width1, y + length1 - width1, z - 2.5f);

            verts[10] = new Vector3(x + length1 + width1, y - length1 + width1, z - 2.5f);
            verts[11] = new Vector3(x + length1 - width1, y - length1 - width1, z - 2.5f);

            verts[12] = new Vector3(x - length1 + width1, y - length1 - width1, z - 2.5f);
            verts[13] = new Vector3(x - length1 - width1, y - length1 + width1, z - 2.5f);

            verts[14] = new Vector3(x - length1 - width1, y + length1 - width1, z - 2.5f);
            verts[15] = new Vector3(x - length1 + width1, y + length1 + width1, z - 2.5f);

            verts[16] = new Vector3(x, y + 0.5f, z - 3.5f);
            verts[17] = new Vector3(x + 0.5f, y, z - 3.5f);
            verts[18] = new Vector3(x, y - 0.5f, z - 3.5f);
            verts[19] = new Vector3(x - 0.5f, y, z - 3.5f);

            verts[20] = new Vector3(x + length2 - width2, y + length2 + width2, z - 3.5f);
            verts[21] = new Vector3(x + length2 + width2, y + length2 - width2, z - 3.5f);

            verts[22] = new Vector3(x + length2 + width2, y - length2 + width2, z - 3.5f);
            verts[23] = new Vector3(x + length2 - width2, y - length2 - width2, z - 3.5f);

            verts[24] = new Vector3(x - length2 + width2, y - length2 - width2, z - 3.5f);
            verts[25] = new Vector3(x - length2 - width2, y - length2 + width2, z - 3.5f);

            verts[26] = new Vector3(x - length2 - width2, y + length2 - width2, z - 3.5f);
            verts[27] = new Vector3(x - length2 + width2, y + length2 + width2, z - 3.5f);

            verts[28] = new Vector3(x + length2, y + length2, z + reverse_lth);
            verts[29] = new Vector3(x + length2, y - length2, z + reverse_lth);
            verts[30] = new Vector3(x - length2, y - length2, z + reverse_lth);
            verts[31] = new Vector3(x - length2, y + length2, z + reverse_lth);
            
            // initial tris
            var tris = new int[] {
                // 4, 0, 1, 4, 1, 5, 5, 1, 2, 5, 2, 6, 6, 2, 3, 6, 3, 7, 7, 3, 0, 7, 0, 4, // link up
                5, 9, 4, 4, 9, 8, 6, 11, 10, 6, 10, 5, 7, 13, 6, 6, 13, 12, 4, 14, 7, 4, 15, 14, // tail_1
                17, 16, 21, 16, 20, 21, 18, 22, 23, 18, 17, 22, 18, 24, 25, 18, 25, 19, 19, 26, 27, 19, 27, 16, 16, 17, 18, 16, 18, 19, // tail_2
                // 21, 8, 9, 21, 20, 8, 23, 22, 10, 23, 10, 11, 25, 24, 12, 25, 12, 13, 27, 26, 14, 27, 14, 15, // (tail_1 & tail_2 link)_1
                21, 9, 17, 17, 9, 5, 17, 5, 10, 17, 10, 22, 18, 23, 11, 18, 11, 6, 18, 6, 24, 24, 6, 12, 19, 25, 13, 19, 13, 7, 26, 19, 7, 26, 7, 14, 27, 4, 16, 27, 15, 4, 20, 16, 4, 20, 4, 8, // (tail_1 & tail_2 link)_2
                28, 8, 9, 28, 21, 20, 28, 20, 8, 28, 9, 21, 29, 10, 11, 29, 23, 22, 29, 22, 10, 29, 11, 23, 30, 12, 13, 30, 25, 24, 30, 24, 12, 30, 13, 25, 31, 14, 15, 31, 27, 26, 31, 26, 14, 31, 15, 27, // reverse
                // 32, 16, 17, 32, 17, 33, 33, 17, 18, 33, 18, 34, 34, 18, 19, 34, 19, 35, 35, 19, 16, 35, 16, 32,
                // 36, 32, 33, 36, 33, 37, 37, 33, 34, 37, 34, 38, 38, 34, 35, 38, 35, 39, 39, 35, 32, 39, 32, 36, 
                // 36, 37, 38, 36, 38, 39
            };
            // create mesh
            mesh_main.vertices = verts;
            mesh_main.triangles = tris;
            // sd the mesh
            // mesh_main = sd.iterate(mesh_main);
            // create game_obj
            GameObject s1 = new GameObject("Tail_X");
            s1.AddComponent<MeshFilter>();
            s1.AddComponent<MeshRenderer>();
            s1.GetComponent<MeshFilter>().mesh = mesh_main;
            // color & texture
            Renderer rend = s1.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture; 
            
            
            
            
            Mesh mesh_main_2 = new Mesh();
            verts = new Vector3[8];
            // initial vertices
            verts[0] = new Vector3(x, y + 3f, z - 2f);
            verts[1] = new Vector3(x + 3f, y, z - 2f);
            verts[2] = new Vector3(x, y - 3f, z - 2f);
            verts[3] = new Vector3(x - 3f, y, z - 2f);

            verts[4] = new Vector3(x, y + 0.5f, z - 2.5f);
            verts[5] = new Vector3(x + 0.5f, y, z - 2.5f);
            verts[6] = new Vector3(x, y - 0.5f, z - 2.5f);
            verts[7] = new Vector3(x - 0.5f, y, z - 2.5f);
            
            // initial tris
            tris = new int[] {
                4, 0, 1, 4, 1, 5, 5, 1, 2, 5, 2, 6, 6, 2, 3, 6, 3, 7, 7, 3, 0, 7, 0, 4, // link up
            };
            // create mesh
            mesh_main_2.vertices = verts;
            mesh_main_2.triangles = tris;
            // sd the mesh
            mesh_main_2 = sd.iterate(mesh_main_2);
            // create game_obj
            GameObject s2 = new GameObject("Tail_X_2_2");
            s2.AddComponent<MeshFilter>();
            s2.AddComponent<MeshRenderer>();
            s2.GetComponent<MeshFilter>().mesh = mesh_main_2;
            // color & texture
            rend = s2.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture; 
            
            GameObject go_tail_3 = new GameObject("Tail 3 parent");
            s1.transform.parent = go_tail_3.gameObject.transform;
            s2.transform.parent = go_tail_3.gameObject.transform;
            return go_tail_3;
        }
        public GameObject create_tail_4() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(4);
            float x = this.pos_tail.x;
            float y = this.pos_tail.y;
            float z = this.pos_tail.z;
            Mesh mesh_tail_3_1 = new Mesh();
            var verts = new Vector3[24];
            float radius = 1f;
            float length = 3f;
            // initial vertices
            verts[0] = new Vector3(x, y + 3f, z - 2f);
            verts[1] = new Vector3(x + 3f, y, z - 2f);
            verts[2] = new Vector3(x, y - 3f, z - 2f);
            verts[3] = new Vector3(x - 3f, y, z - 2f);

            verts[18] = new Vector3(x, y + 0.5f, z - 2.5f);
            verts[19] = new Vector3(x + 0.5f, y, z - 2.5f);
            verts[20] = new Vector3(x, y - 0.5f, z - 2.5f);
            verts[21] = new Vector3(x - 0.5f, y, z - 2.5f);
            
            verts[4] = new Vector3(x + radius, y + radius, z - 2.5f);
            verts[5] = new Vector3(x + radius * 2, y, z - 2.5f);
            verts[6] = new Vector3(x + radius, y - radius, z - 2.5f);
            verts[7] = new Vector3(x - radius, y - radius, z - 2.5f);
            verts[8] = new Vector3(x - radius * 2, y, z - 2.5f);
            verts[9] = new Vector3(x - radius, y + radius, z - 2.5f);
            verts[10] = new Vector3(x, y, z - 2.5f);

            verts[11] = new Vector3(x + radius, y + radius, z - 2.5f - length);
            verts[12] = new Vector3(x + radius * 2, y, z - 2.5f - length);
            verts[13] = new Vector3(x + radius, y - radius, z - 2.5f - length);
            verts[14] = new Vector3(x - radius, y - radius, z - 2.5f - length);
            verts[15] = new Vector3(x - radius * 2, y, z - 2.5f - length);
            verts[16] = new Vector3(x - radius, y + radius, z - 2.5f - length);
            verts[17] = new Vector3(x, y, z - 2.5f - length);
            
            verts[22] = new Vector3(x, y, z - 2.5f);
            verts[23] = new Vector3(x, y, z - 2.5f - length);

            // initial tris
            var tris = new int[] {
                18, 0, 1, 18, 1, 19, 19, 1, 2, 19, 2, 20, 20, 2, 3, 20, 3, 21, 21, 3, 0, 21, 0, 18, // link up
                // 0, 4, 9, 0, 5, 4, 0, 1, 5, 1, 2, 5, 2, 6, 5, 2, 7, 6, 2, 8, 7, 2, 3, 8, 3, 9, 8, 3, 0, 9, // face 1 to link
                18, 10, 9, 18, 4, 22, 18, 5, 4, 18, 19, 5, 19, 20, 5, 5, 20, 6, 20, 22, 6, 20, 7, 10, 20, 8, 7, 20, 21, 8, 21, 9, 8, 21, 18, 9, // face 1 to link
                9, 10, 17, 9, 17, 16, 22, 4, 11, 22, 11, 23, 4, 5, 12, 4, 12, 11, 5, 6, 13, 5, 13, 12, 6, 22, 13, 13, 22, 23, 10, 7, 17, 17, 7, 14, 7, 8, 14, 14, 8, 15, 8, 9, 16, 8, 16, 15, // face
                // 16, 17, 14, 16, 14, 15, 11, 12, 13, 11, 13, 23
            };
            // create mesh
            mesh_tail_3_1.vertices = verts;
            mesh_tail_3_1.triangles = tris;
            // sd the mesh
            mesh_tail_3_1 = sd.iterate(mesh_tail_3_1);
            // create game_obj
            GameObject s1 = new GameObject("tail_3");
            s1.AddComponent<MeshFilter>();
            s1.AddComponent<MeshRenderer>();
            s1.GetComponent<MeshFilter>().mesh = mesh_tail_3_1;
            // color & texture
            Renderer rend = s1.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;
            
            
            Mesh mesh_tail_3_2 = new Mesh();
            verts = new Vector3[24];
            // initial vertices
            verts[0] = new Vector3(x + radius, y + radius, z - 2.5f - length);
            verts[1] = new Vector3(x + radius * 2, y, z - 2.5f - length);
            verts[2] = new Vector3(x + radius, y - radius, z - 2.5f - length);
            verts[3] = new Vector3(x - radius, y - radius, z - 2.5f - length);
            verts[4] = new Vector3(x - radius * 2, y, z - 2.5f - length);
            verts[5] = new Vector3(x - radius, y + radius, z - 2.5f - length);
            verts[6] = new Vector3(x, y, z - 2.5f - length);
            verts[7] = new Vector3(x, y, z - 2.5f - length);

            // initial tris
            tris = new int[] {
                0, 1, 2, 0, 2, 7, 5, 6, 3, 5, 3, 4
            };
            // create mesh
            mesh_tail_3_2.vertices = verts;
            mesh_tail_3_2.triangles = tris;
            // sd the mesh
            mesh_tail_3_2 = sd.iterate(mesh_tail_3_2);
            // create game_obj
            GameObject s2 = new GameObject("tail_3");
            s2.AddComponent<MeshFilter>();
            s2.AddComponent<MeshRenderer>();
            s2.GetComponent<MeshFilter>().mesh = mesh_tail_3_2;
            // color & texture
            rend = s2.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;    
            
            GameObject go_tail_3 = new GameObject("Tail 3");
            s1.transform.parent = go_tail_3.gameObject.transform;
            s2.transform.parent = go_tail_3.gameObject.transform;
            
            GameObject s_copy_1 = GameObject.Instantiate(go_tail_3);
            GameObject pivot_point_1 = new GameObject("pivoprit point2");
            pivot_point_1.transform.position = new Vector3(x, y, z);
            s_copy_1.transform.parent = pivot_point_1.gameObject.transform;
            pivot_point_1.transform.eulerAngles = new Vector3(0,0,90);
            
            GameObject go_tail_3_par = new GameObject("Tail 3 parent");
            go_tail_3.transform.parent = go_tail_3_par.gameObject.transform;
            s_copy_1.transform.parent = go_tail_3_par.gameObject.transform;
            return go_tail_3_par;
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
            else if (this.chosen_type == 2)
            {
                this.update3(alpha);
            } else
            {
                this.update4(alpha);
            }
        }
        public void update1(float alpha)
        {
            this.X.transform.eulerAngles = new Vector3(0,0,this.X.transform.eulerAngles.z + alpha);
        }
        public void update2(float alpha)
        {
            this.tail_2.transform.eulerAngles = new Vector3(0,0,this.tail_2.transform.eulerAngles.z + alpha);
        }
        public void update3(float alpha)
        {
            this.tail_3.transform.eulerAngles = new Vector3(0,0,this.tail_3.transform.eulerAngles.z + alpha);
        }
        public void update4(float alpha)
        {
            this.tail_4.transform.eulerAngles = new Vector3(0,0,this.tail_4.transform.eulerAngles.z + alpha);
        }
    }
}