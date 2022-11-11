using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WarmUp02 : MonoBehaviour
{
    public int seed = 7;

    private Vector3[] verts;
    private int[] tris;
    private int divNum = 100;
    private float height = .4f;
    private float bottomRadius = .4f;
    private float upperRadius = .2f;
    private int spereRestrictionRadius = 5;
    private int objNum = 500;


    // Start is called before the first frame update
    void Start()
    {
        Mesh corn_mesh = CreateFrustumOfCornMesh();

        System.Random rnd = new System.Random(seed);

        for (int i = 0; i < objNum; i++)
        {
            // Create GameObject
            GameObject s = new GameObject(i.ToString("Object 0"));
			s.AddComponent<MeshFilter>();
			s.AddComponent<MeshRenderer>();

            // Get Random Position
            float x, y, z;
            while (true)
            {
                x = (float) rnd.NextDouble() * spereRestrictionRadius * 2 - spereRestrictionRadius;
                y = (float) rnd.NextDouble() * spereRestrictionRadius * 2;
                z = (float) rnd.NextDouble() * spereRestrictionRadius * 2 - spereRestrictionRadius;
                float length = Mathf.Sqrt(x * x + (y - spereRestrictionRadius) * (y - spereRestrictionRadius) + z * z); // the center of sphere is (0, spereRestrictionRadius, 0)
                if (length < spereRestrictionRadius)
                {
                    break;
                }
            }

            // Transform
            s.transform.position = new Vector3 (x, y, z);
            s.transform.localScale = new Vector3 (1f, 1f, 1f);

            // Association
			s.GetComponent<MeshFilter>().mesh = corn_mesh;

            // Change color
            Renderer rend = s.GetComponent<Renderer>();
			rend.material.color = new Color ((float) rnd.NextDouble(), (float) rnd.NextDouble(), (float) rnd.NextDouble(), 1.0f);  // light green color

        }



    }

    Mesh CreateFrustumOfCornMesh()
    {
        // Create Mesh
        Mesh FrustumOfCornMesh = new Mesh();

        // Store vertices
        verts = new Vector3[divNum * 2 + 2];
        int verts_num = verts.Length;

        verts[verts_num - 2] = Vector3.zero; // center of bottom circle
        verts[verts_num - 1] = new Vector3(0, height, 0); // center of upper circle

        for (int i = 0; i < divNum; i++)
        {
            float degree = (float) (Mathf.PI * 2) * (i / (float)divNum);

            // bottom circle vertices
            verts[i] = new Vector3(Mathf.Cos(degree) * bottomRadius, 0, Mathf.Sin(degree) * bottomRadius);

            // upper circle vertices
            verts[i + divNum] = new Vector3(Mathf.Cos(degree) * upperRadius, height, Mathf.Sin(degree) * upperRadius);
        }

        // Store triangles
        tris = new int[divNum * 4 * 3];

        
        for (int i = 0; i < divNum; i++)
        {
            // Make upper and bottom facets

            // Make bottom
            int idx_b = i * 3;
            tris[idx_b] = verts_num - 2;
            tris[idx_b + 1] = i;
            tris[idx_b + 2] = (i + 1) % divNum;

            // Make upper
            int idx_u = i * 3 + divNum * 3;
            tris[idx_u] = verts_num - 1;
            tris[idx_u + 2] = i + divNum;
            tris[idx_u + 1] = (i + 1) % divNum + divNum;

            // Make side facets
            int idx_s = i * 6 + divNum * 2 * 3;
            tris[idx_s] = i + divNum;
            tris[idx_s + 2] = i;
            tris[idx_s + 1] = (i + 1) % divNum;

            tris[idx_s + 3] = (i + 1) % divNum;
            tris[idx_s + 4] = i + divNum;
            tris[idx_s + 5] = (i + 1) % divNum + divNum;

        }



        // Assign verts and tris
        FrustumOfCornMesh.vertices = verts;
        FrustumOfCornMesh.triangles = tris;

        // Calc normals
        FrustumOfCornMesh.RecalculateNormals();

        FrustumOfCornMesh.Optimize();

        return FrustumOfCornMesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}
