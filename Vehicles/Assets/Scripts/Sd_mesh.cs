using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sd_surface {
    public class Sd_mesh {
        public List<Triangle> triangles;
        public List<Edge> edges;
        public List<Vertex> vertices;
        public Sd_mesh() {
            this.triangles = new List<Triangle>();
            this.edges = new List<Edge>();
            this.vertices = new List<Vertex>();
        }
        public Sd_mesh(Mesh mesh) : this() {
            // Initialization
            var vertices = mesh.vertices;
            var tris = mesh.triangles;

            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices.Add(new Vertex(vertices[i], i));
            }

            for (int i = 0; i < tris.Length; i += 3)
            {
                int idx1 = tris[i];
                int idx2 = tris[i + 1];
                int idx3 = tris[i + 2];
                Vertex vertex1 = this.vertices[idx1];
                Vertex vertex2 = this.vertices[idx2];
                Vertex vertex3 = this.vertices[idx3];
                Edge edge1 = this.getEdge(vertex1, vertex2);
                Edge edge2 = this.getEdge(vertex2, vertex3);
                Edge edge3 = this.getEdge(vertex3, vertex1);
                Triangle new_tris = new Triangle(vertex1, vertex2, vertex3, edge1, edge2, edge3);
                vertex1.adjacent_tris.Add(new_tris);
                vertex2.adjacent_tris.Add(new_tris);
                vertex3.adjacent_tris.Add(new_tris);
                edge1.adjacent_tris.Add(new_tris);
                edge2.adjacent_tris.Add(new_tris);
                edge3.adjacent_tris.Add(new_tris);
                this.triangles.Add(new_tris);
            }
        }
        public Mesh createMesh() {
            Mesh mesh = new Mesh();
            var vertices = new Vector3[this.triangles.Count * 3];
            var tris = new int[this.triangles.Count * 3];
            for (int i = 0; i < this.triangles.Count; i++)
            {
                int idx1 = i * 3;
                int idx2 = i * 3 + 1;
                int idx3 = i * 3 + 2;
                vertices[idx1] = this.triangles[i].vertex1.vertex;
                vertices[idx2] = this.triangles[i].vertex2.vertex;
                vertices[idx3] = this.triangles[i].vertex3.vertex;
                tris[idx1] = idx1;
                tris[idx2] = idx2;
                tris[idx3] = idx3;
            }
            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
        public void addTris(Vertex vertex1, Vertex vertex2, Vertex vertex3) {
            // Add vertexes
            if (!this.vertices.Contains(vertex1))
            {
                this.vertices.Add(vertex1);
            }
            if (!this.vertices.Contains(vertex2))
            {
                this.vertices.Add(vertex2);
            }
            if (!this.vertices.Contains(vertex3))
            {
                this.vertices.Add(vertex3);
            }
            // Add edges
            Edge edge1 = this.getEdge(vertex1, vertex2);
            Edge edge2 = this.getEdge(vertex2, vertex3);
            Edge edge3 = this.getEdge(vertex3, vertex1);
            // Add Triangle
            var tri = new Triangle(vertex1, vertex2, vertex3, edge1, edge2, edge3);
            this.triangles.Add(tri);

            // Add triangles to vertex
            vertex1.adjacent_tris.Add(tri);
            vertex2.adjacent_tris.Add(tri);
            vertex3.adjacent_tris.Add(tri);
            // Add triangles to edges
            edge1.adjacent_tris.Add(tri);
            edge2.adjacent_tris.Add(tri);
            edge3.adjacent_tris.Add(tri);
        }
        public Edge getEdge(Vertex vertex1, Vertex vertex2) {
            var edge = vertex1.adjacent_edges.Find(e => {
                return e.has(vertex2);
            });
            if (edge != null)
            {
                return edge;
            } else
            {
                Edge new_edge = new Edge(vertex1, vertex2);
                this.edges.Add(new_edge);
                vertex1.addEdge(new_edge);
                vertex2.addEdge(new_edge);
                return new_edge;
            }
        }
    }
}