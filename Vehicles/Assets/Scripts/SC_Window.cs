using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace space_craft {
    class SC_Window {
        private Vector3 pos_window;
        public SC_Window(Vector3 pos_window, System.Random rnd) {
            this.pos_window = pos_window;
            this.create_main();
            this.create_glass();
        }
        public void create_main() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_window.x;
            float y = this.pos_window.y;
            float z = this.pos_window.z;
            Mesh mesh_main = new Mesh();
            var verts = new Vector3[8];
            // initial vertices
            verts[0] = new Vector3(x, y + 4f, z - 10f);
            verts[1] = new Vector3(x, y + 7f, z + 20f);
            verts[2] = new Vector3(x + 2.5f, y + 3f, z + 20f);
            verts[3] = new Vector3(x + 4f, y, z + 20f);
            verts[4] = new Vector3(x + 4.5f, y - 0.5f, z + 20f);
            verts[5] = new Vector3(x - 4.5f, y - 0.5f, z + 20f);
            verts[6] = new Vector3(x - 4f, y, z + 20f);
            verts[7] = new Vector3(x - 2.5f, y + 3f, z + 20f);

            // initial tris
            var tris = new int[] {
                7, 6, 5, 1, 7, 5, 1, 5, 4, 1, 4, 2, 2, 4, 3,
                0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 7, 0, 7, 1
            };
            // create mesh
            mesh_main.vertices = verts;
            mesh_main.triangles = tris;
            // sd the mesh
            mesh_main = sd.iterate(mesh_main);
            // create game_obj
            GameObject s = new GameObject("Window_Main");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_main;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.45f, .45f, .45f, 1.0f);
            // Texture2D texture = make_window_texture();
            // rend.material.mainTexture = texture;
        }
        public void create_glass() {
            sd_surface.Subdivision sd = new sd_surface.Subdivision(5);
            float x = this.pos_window.x;
            float y = this.pos_window.y;
            float z = this.pos_window.z;
            Mesh mesh_main = new Mesh();
            var verts = new Vector3[8];
            // initial vertices
            verts[0] = new Vector3(x, y + 1.3f, z + 35f);
            verts[1] = new Vector3(x, y + 6.3f, z + 17.2f);
            verts[2] = new Vector3(x + 2f, y + 3.3f, z + 17.2f);
            verts[3] = new Vector3(x + 3.5f, y + 1.3f, z + 17.2f);
            verts[4] = new Vector3(x + 4f, y + 0.8f, z + 17.2f);
            verts[5] = new Vector3(x - 4f, y + 0.8f, z + 17.2f);
            verts[6] = new Vector3(x - 3.5f, y + 1.3f, z + 17.2f);
            verts[7] = new Vector3(x - 2f, y + 3.3f, z + 17.2f);

            // initial tris
            var tris = new int[] {
                7, 5, 6, 1, 5, 7, 1, 4, 5, 1, 2, 4, 2, 3, 4,
                0, 2, 1, 0, 3, 2, 0, 4, 3, 0, 5, 4, 0, 6, 5, 0, 7, 6, 0, 1, 7                
            };
            // create mesh
            mesh_main.vertices = verts;
            mesh_main.triangles = tris;
            // sd the mesh
            mesh_main = sd.iterate(mesh_main);
            // create game_obj
            GameObject s = new GameObject("Window_glass");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
            s.GetComponent<MeshFilter>().mesh = mesh_main;
            // color & texture
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (.53f, .73f, .87f, .1f);
            // Texture2D texture = make_glass_texture();
            // rend.material.mainTexture = texture;
        }
        public Texture2D make_glass_texture() {
            Texture2D texture = Resources.Load("window_sky") as Texture2D;
            return texture;
        }
    }
}