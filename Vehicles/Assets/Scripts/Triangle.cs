using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sd_surface {
    public class Triangle {
        public Vertex vertex1, vertex2, vertex3;
        public Edge edge1, edge2, edge3;
        public Triangle(Vertex vertex1, Vertex vertex2, Vertex vertex3, Edge edge1, Edge edge2, Edge edge3) {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
            this.edge1 = edge1;
            this.edge2 = edge2;
            this.edge3 = edge3;
        }
        public Vertex getOtherVertex(Edge e) {
            if (!e.has(this.vertex1))
            {
                return this.vertex1;
            } else if (!e.has(this.vertex2)) 
            {
                return this.vertex2;
            } else 
            {
                return this.vertex3;
            }
        }
    }
}