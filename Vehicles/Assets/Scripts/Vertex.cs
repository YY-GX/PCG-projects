using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sd_surface {
    public class Vertex {
        public int id;
        public Vector3 vertex;
        public Vertex new_vertex;
        public List<Edge> adjacent_edges;
        public List<Triangle> adjacent_tris;
        public Vertex(Vector3 vertex, int id) {
            this.vertex = vertex;
            this.id = id;
            this.adjacent_edges = new List<Edge>();
            this.adjacent_tris = new List<Triangle>();
        }
        public Vertex(Vector3 vertex): this(vertex, -1) {}
        public void addEdge(Edge edge) {
            this.adjacent_edges.Add(edge);
        }
        public void addTris(Triangle tris) {
            this.adjacent_tris.Add(tris);
        }
    }
}