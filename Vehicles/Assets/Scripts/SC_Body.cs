using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace space_craft {
    class SC_Body {
        private Vector3 pos_body;
        public SC_Body(Vector3 pos_body, System.Random rnd) {
            this.pos_body = pos_body;
            this.create_main();
        }
        public void create_main() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_body.x;
            float y = this.pos_body.y;
            float z = this.pos_body.z;
            Mesh mesh_main = new Mesh();
            var verts = new Vector3[12];
            // initial vertices
            verts[0] = new Vector3(x, y + 2f, z);
            verts[1] = new Vector3(x + 5f, y, z);
            verts[2] = new Vector3(x, y - 3f, z);
            verts[3] = new Vector3(x - 5f, y, z);

            verts[4] = new Vector3(x, y + 6f, z + 25f);
            verts[5] = new Vector3(x + 7.5f, y, z + 25f);
            verts[6] = new Vector3(x, y - 5f, z + 25f);
            verts[7] = new Vector3(x - 7.5f, y, z + 25f);

            verts[8] = new Vector3(x, y + 2f, z + 30f);
            verts[9] = new Vector3(x + 2f, y, z + 30f);
            verts[10] = new Vector3(x, y - 2f, z + 30f);
            verts[11] = new Vector3(x - 2f, y, z + 30f);
            // initial tris
            var tris = new int[] {
                // 0, 1, 2, 2, 3, 0,
                4, 1, 0, 4, 5, 1, 5, 2, 1, 5, 6, 2, 6, 3, 2, 6, 7, 3, 7, 0, 3, 7, 4, 0,
                8, 5, 4, 8, 9, 5, 9, 6, 5, 9, 10, 6, 10, 7, 6, 10, 11, 7, 11, 4, 7, 11, 8, 4,
                8, 11, 10, 8, 10, 9
            };
            // create mesh
            mesh_main.vertices = verts;
            mesh_main.triangles = tris;
            // sd the mesh
            mesh_main = sd.iterate(mesh_main);
            // create game_obj
            GameObject s = new GameObject("Body_Main");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_main;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.75f, .75f, .75f, 1.0f);
            // Texture2D texture = make_a_texture();
            // rend.material.mainTexture = texture;
        }
    }
}