using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sd_surface 
{
    public class Edge {
        public Vertex vertex1, vertex2;
        public List<Triangle> adjacent_tris;
        public Vertex middle_vertex;

        public Edge(Vertex vertex1, Vertex vertex2) {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.adjacent_tris = new List<Triangle>();
        }

        public bool has(Vertex vertex) {
            return vertex == this.vertex1 || vertex == this.vertex2;
        }

        public Vertex getOtherVertex(Vertex vertex) {
            if (vertex == this.vertex1)
            {
                return this.vertex2;
            } else {
                return this.vertex1;
            }
        }
    }
}