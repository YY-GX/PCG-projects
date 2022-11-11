using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sd_surface {
    public class Subdivision {
        public int iterate_num;
        public Subdivision(int iterate_num) {
            this.iterate_num = iterate_num;
        }

        // Every iteration, split one tris into 4 & add new vertices/edged/tris
        public Mesh iterate(Mesh mesh) {
            // initial sd_mesh
            Sd_mesh sd_mesh = new Sd_mesh(mesh);
            // iterate
            for (int i = 0; i < this.iterate_num; i++)
            {
                var crr_sd_mesh = new Sd_mesh();
                for (int j = 0; j < sd_mesh.triangles.Count; j++)
                {
                    Triangle tris = sd_mesh.triangles[j];
                    Vertex mid1 = this.getMiddleVertex(tris.edge1);
                    Vertex mid2 = this.getMiddleVertex(tris.edge2);
                    Vertex mid3 = this.getMiddleVertex(tris.edge3);
                    Vertex new_vertex1 = this.getNewVertex(tris.vertex1);
                    Vertex new_vertex2 = this.getNewVertex(tris.vertex2);
                    Vertex new_vertex3 = this.getNewVertex(tris.vertex3);
                    crr_sd_mesh.addTris(new_vertex1, mid1, mid3);
                    crr_sd_mesh.addTris(mid1, new_vertex2, mid2);
                    crr_sd_mesh.addTris(mid3, mid2, new_vertex3);
                    crr_sd_mesh.addTris(mid1, mid2, mid3);
                    // crr_sd_mesh.addTris(new_vertex1, mid3, mid1);
                    // crr_sd_mesh.addTris(mid1, mid2, new_vertex2);
                    // crr_sd_mesh.addTris(mid3, new_vertex3, mid2);
                    // crr_sd_mesh.addTris(mid1, mid3, mid2);                    
                }
                sd_mesh = crr_sd_mesh;
            }
            Mesh final_mesh = sd_mesh.createMesh();
            return final_mesh;
        }
        public Vertex getMiddleVertex(Edge edge) {
            if (edge.middle_vertex != null)
            {
                return edge.middle_vertex;
            }
            Vertex up = edge.vertex1;
            Vertex down = edge.vertex2;            
            if (edge.adjacent_tris.Count == 2) {
                Vertex left = edge.adjacent_tris[0].getOtherVertex(edge);
                Vertex right = edge.adjacent_tris[1].getOtherVertex(edge);
                Vertex middle_vertex = new Vertex((3f/8f) * (up.vertex + down.vertex) + (1f/8f) * (left.vertex + right.vertex), up.id);
                edge.middle_vertex = middle_vertex;
            } else { // boundary
                edge.middle_vertex = new Vertex(0.5f * (up.vertex + down.vertex), up.id);
            }
            return edge.middle_vertex;
        }
        public Vertex getNewVertex(Vertex vertex) {
            if (vertex.new_vertex != null) {
                return vertex.new_vertex;
            }
            // Get all ajacencies
            var adjacent_edges = vertex.adjacent_edges;
            var adj_num = adjacent_edges.Count;
            // Count num of adj_edges that only has one face
            int cnt = 0;
            List<Edge> adjacent_edges_1face = new List<Edge>();
            foreach (var edge in adjacent_edges)
            {
                if (edge.adjacent_tris.Count == 1)
                {
                    cnt += 1;
                    adjacent_edges_1face.Add(edge);
                }
            }
            if (cnt == 2) // boundary
            {
                var left = adjacent_edges_1face[0].getOtherVertex(vertex).vertex;
                var right = adjacent_edges_1face[1].getOtherVertex(vertex).vertex;
                var new_vertex = new Vertex(0.125f * (left + right) + 0.75f * vertex.vertex, vertex.id);
                vertex.new_vertex = new_vertex;
                return new_vertex;
            } else {
                float beta = (adj_num == 3) ? (3f / 16f) : (3f / (8f * adj_num));
                // float beta = (adj_num == 3) ? (3f / 16f) :  ((1f / adj_num) * ((5f / 8f) - Mathf.Pow((3f / 8f) + (1f / 4f) * Mathf.Cos(Mathf.PI * 2f / adj_num), 2f)));
                Vector3 new_vertex_vector3 = (1f - adj_num * beta) * vertex.vertex;
                foreach (var adj_edge in adjacent_edges)
                {
                    new_vertex_vector3 += beta * adj_edge.getOtherVertex(vertex).vertex;
                }
                // new_vertex_vector3 += (1f - adj_num * beta) * vertex.vertex;
                var new_vertex = new Vertex(new_vertex_vector3, vertex.id);
                vertex.new_vertex = new_vertex;
                return new_vertex;
            }
        }
    }
}