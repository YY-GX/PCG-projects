using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sd_surface;
using System;
namespace space_craft {
    class SC_Head {
        private Vector3 pos_head;
        public SC_Head(Vector3 pos_head, System.Random rnd) {
            this.pos_head = pos_head;
            this.create_main();
            this.create_needle();
        }

        public void create_main() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_head.x;
            float y = this.pos_head.y;
            float z = this.pos_head.z;
            Mesh mesh_main = new Mesh();
            var verts = new Vector3[5];
            // initial vertices
            verts[0] = new Vector3(x, y, z + 50f);
            verts[1] = new Vector3(x, y + 7f, z);
            verts[2] = new Vector3(x + 12f, y, z);
            verts[3] = new Vector3(x, y - 7f, z);
            verts[4] = new Vector3(x - 12f, y, z);
            // initial tris
            var tris = new int[] {0, 2, 1, 0, 3, 2, 0, 4, 3, 0, 1, 4, 1, 2, 3, 3, 4, 1};
            // var tris = new int[] {0, 2, 1, 0, 3, 2, 0, 4, 3, 0, 1, 4};
            // create mesh
            mesh_main.vertices = verts;
            mesh_main.triangles = tris;
            // sd the mesh
            mesh_main = sd.iterate(mesh_main);
            // create game_obj
            GameObject s = new GameObject("Head_Main");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_main;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;
        }
        public void create_needle() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_head.x;
            float y = this.pos_head.y;
            float z = this.pos_head.z;
            Mesh mesh_needle = new Mesh();
            var verts = new Vector3[5];
            // initial vertices
            float base_pos = 24f;
            verts[0] = new Vector3(x, y, z + base_pos + 10f);
            verts[1] = new Vector3(x, y + 0.3f, z + base_pos);
            verts[2] = new Vector3(x + 0.3f, y, z + base_pos);
            verts[3] = new Vector3(x, y - 0.3f, z + base_pos);
            verts[4] = new Vector3(x - 0.3f, y, z + base_pos);
            // initial tris
            var tris = new int[] {0, 2, 1, 0, 3, 2, 0, 4, 3, 0, 1, 4, 1, 2, 3, 3, 4, 1};
            // create mesh
            mesh_needle.vertices = verts;
            mesh_needle.triangles = tris;
            // sd the mesh
            mesh_needle = sd.iterate(mesh_needle);
            // create game_obj
            GameObject s = new GameObject("Head_Needle");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_needle;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.15f, .15f, .15f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;
        }
    }
}

