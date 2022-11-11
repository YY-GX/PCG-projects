/*
TODO:
1. 令createWallMesh()返回俩mesh，一个wall_mesh，一个window_mesh/door_mesh(加一个input param用于判断是door还是window)。
2. 定义不同类型的roof
*/

namespace building
{
using System;   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Building
{
    private int seed;
    private float pos_x;
    private float pos_z;
    private float footprint_length;
    private float cell_length;
    private List<int[,]> footprints = new List<int[,]>()
    {
        new int[,] {
            {1, 0, 0},
            {2, 0, 0},
            {3, 2, 1},
        },
        new int[,] {
            {2, 1, 2},
            {0, 3, 0},
            {0, 3, 0},
        },
        new int[,] {
            {0, 2, 1},
            {0, 2, 0},
            {1, 2, 0},            
        },
        new int[,] {
            {3, 0, 3},
            {3, 0, 3},
            {1, 2, 1},            
        },
        new int[,] {
            {0, 1, 0},
            {1, 3, 1},
            {0, 1, 0},            
        },
        new int[,] {
            {2, 0, 2},
            {3, 3, 3},
            {2, 0, 2},            
        },
        new int[,] {
            {0, 2, 0},
            {3, 3, 2},
            {0, 3, 0},            
        },
        new int[,] {
            {0, 3, 0},
            {3, 3, 0},
            {0, 3, 0},            
        },
    };
    private string[] roof_styles = new string[2] {"hip", "gable"};
    private int[] wall_styles = new int[4] {1, 2, 3, 4};
    private int[,] chosen_footprint;
    private string chosen_roof_style;
    private int chosen_wall_style;
    public Building(float pos_x, float pos_z, int seed, float footprint_length, float cell_length)
    {
        this.pos_x = pos_x;
        this.pos_z = pos_z;
        this.seed = seed;
        this.footprint_length = footprint_length;
        this.cell_length = cell_length;

        // randomize to get chosen footprint & chosen roof style
        System.Random rand = new System.Random();

        int num_footprints = this.footprints.Count;
        int footprint_idx = Convert.ToInt32(Mathf.Floor((float)rand.NextDouble() * num_footprints));
        this.chosen_footprint = this.footprints[footprint_idx];

        int num_roof_styles = this.roof_styles.Length;
        int roof_style_idx = (int)(Mathf.Floor((float)rand.NextDouble() * num_roof_styles));
        this.chosen_roof_style = this.roof_styles[roof_style_idx];

        int num_wall_styles = this.wall_styles.Length;
        int wall_style_idx = (int)(Mathf.Floor((float)rand.NextDouble() * num_wall_styles));
        this.chosen_wall_style = this.wall_styles[wall_style_idx];

        // Debug.Log(this.chosen_roof_style);

        // Start building things
        this.build_walls();
        this.build_roofs();

    }

    public void build_walls() {
        for (int i = 0; i < this.chosen_footprint.GetLength(0); i++)
        {
            for (int j = 0; j < this.chosen_footprint.GetLength(1); j++)
            {
                int elevation = this.chosen_footprint[i, j];
                float cell_x = this.pos_x + this.footprint_length * i;
                float cell_y = 0;
                float cell_z = this.pos_z + this.footprint_length * j;
                if (elevation == 0) {
                    continue;
                }
                for (int l = 0; l < 6; l++) // iterate different directions
                {
                    int num_per_cell = Convert.ToInt32(this.footprint_length / this.cell_length);
                    for (int m = 0; m < num_per_cell; m++)
                    {
                        if ((l == 4) || (l == 5)) { // Up or Down
                            for (int n = 0; n < num_per_cell; n++)
                            {
                                if (l == 4) {
                                    this.createWall(l, cell_x + this.cell_length * m, cell_y + this.cell_length * elevation * num_per_cell, cell_z + this.cell_length * n);
                                } else {
                                    this.createWall(l, cell_x + this.cell_length * m, cell_y, cell_z + this.cell_length * n);
                                }
                            }
                        } else { // W/N/E/S
                            for (int n = 0; n < num_per_cell * elevation; n++) // iterate elevation
                            {
                                int window_or_door = 0;
                                if (n == 0)
                                {
                                    window_or_door = 1;
                                }
                                if (l == 0)
                                {
                                    this.createWall(l, cell_x, cell_y + this.cell_length * n, cell_z + footprint_length - this.cell_length * m, window_or_door);
                                } else if (l == 1)
                                {
                                    this.createWall(l, cell_x + footprint_length - this.cell_length * m, cell_y + this.cell_length * n, cell_z + footprint_length, window_or_door);
                                } else if (l == 2)
                                {
                                    this.createWall(l, cell_x + footprint_length, cell_y + this.cell_length * n, cell_z + this.cell_length * m, window_or_door);
                                } else if (l == 3)
                                {
                                    this.createWall(l, cell_x + this.cell_length * m, cell_y + this.cell_length * n, cell_z, window_or_door);
                                }
                            }
                        }
                    }
                }
                
            }
        }
    }

    // roof type: 0(complete)/1(edge)/2(cross)
    public void build_roofs()
    {
        int[,] footprint = new int[5, 5];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if ((i == 0) || (i == 4) || (j == 0) || (j == 4))
                {
                    footprint[i, j] = 0;
                }
                else
                {
                    footprint[i, j] = this.chosen_footprint[i - 1, j - 1];
                }
            }
        }

        for (int i = 1; i < 4; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                int crr_ele = footprint[i, j];
                int left_ele = footprint[i - 1, j];
                int up_ele = footprint[i, j - 1];
                int right_ele = footprint[i + 1, j];
                int down_ele = footprint[i, j + 1];

                float x = this.pos_x + this.footprint_length * (i - 1);
                float y = this.footprint_length * crr_ele;
                float z = this.pos_z + this.footprint_length * (j - 1);

                if (crr_ele == 0) // if it's ground, continue
                {
                    continue;
                }
                
                // complete
                if ((crr_ele == left_ele) && (crr_ele != up_ele) && (crr_ele == right_ele) && (crr_ele != down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 0, new List<bool>() {true, false, true, false});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 0, new List<bool>() {true, false, true, false});   
                    }
                }
                else if ((crr_ele != left_ele) && (crr_ele == up_ele) && (crr_ele != right_ele) && (crr_ele == down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 0, new List<bool>() {false, true, false, true});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 0, new List<bool>() {false, true, false, true});   
                    }
                }
                // edge
                // edge 4
                else if ((crr_ele != left_ele) && (crr_ele != up_ele) && (crr_ele != right_ele) && (crr_ele != down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 1, new List<bool>() {false, false, false, false});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 1, new List<bool>() {false, false, false, false});   
                    }
                }
                // edge 1
                else if ((crr_ele == left_ele) && (crr_ele != up_ele) && (crr_ele != right_ele) && (crr_ele != down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 1, new List<bool>() {true, false, false, false});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 1, new List<bool>() {true, false, false, false});   
                    }
                }
                else if ((crr_ele != left_ele) && (crr_ele == up_ele) && (crr_ele != right_ele) && (crr_ele != down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 1, new List<bool>() {false, true, false, false});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 1, new List<bool>() {false, true, false, false});   
                    }
                }
                else if ((crr_ele != left_ele) && (crr_ele != up_ele) && (crr_ele == right_ele) && (crr_ele != down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 1, new List<bool>() {false, false, true, false});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 1, new List<bool>() {false, false, true, false});   
                    }
                }
                else if ((crr_ele != left_ele) && (crr_ele != up_ele) && (crr_ele != right_ele) && (crr_ele == down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 1, new List<bool>() {false, false, false, true});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 1, new List<bool>() {false, false, false, true});   
                    }
                }
                // cross
                // cross 4
                else if ((crr_ele == left_ele) && (crr_ele == up_ele) && (crr_ele == right_ele) && (crr_ele == down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {true, true, true, true});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {true, true, true, true});   
                    }
                }
                // cross 3
                else if ((crr_ele != left_ele) && (crr_ele == up_ele) && (crr_ele == right_ele) && (crr_ele == down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {false, true, true, true});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {false, true, true, true});   
                    }
                }
                else if ((crr_ele == left_ele) && (crr_ele != up_ele) && (crr_ele == right_ele) && (crr_ele == down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {true, false, true, true});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {true, false, true, true});   
                    }
                }
                else if ((crr_ele == left_ele) && (crr_ele == up_ele) && (crr_ele != right_ele) && (crr_ele == down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {true, true, false, true});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {true, true, false, true});   
                    }
                }
                else if ((crr_ele == left_ele) && (crr_ele == up_ele) && (crr_ele == right_ele) && (crr_ele != down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {true, true, true, false});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {true, true, true, false});   
                    }
                }
                // cross 2
                else if ((crr_ele != left_ele) && (crr_ele != up_ele) && (crr_ele == right_ele) && (crr_ele == down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {false, false, true, true});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {false, false, true, true});   
                    }
                }
                else if ((crr_ele == left_ele) && (crr_ele != up_ele) && (crr_ele != right_ele) && (crr_ele == down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {true, false, false, true});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {true, false, false, true});   
                    }
                }
                else if ((crr_ele == left_ele) && (crr_ele == up_ele) && (crr_ele != right_ele) && (crr_ele != down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {true, true, false, false});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {true, true, false, false});   
                    }
                }
                else if ((crr_ele != left_ele) && (crr_ele == up_ele) && (crr_ele == right_ele) && (crr_ele != down_ele))
                {
                    if (this.chosen_roof_style == "hip")
                    {
                        this.createHipRoof(x, y, z, 2, new List<bool>() {false, true, true, false});
                    }
                    else
                    {
                        this.createGableRoof(x, y, z, 2, new List<bool>() {false, true, true, false});   
                    }
                }
            }
        }
    }
    public void createHipRoof(float x, float y, float z, int roof_type, List<bool> surround)
    {
        int roof_wall_height = 2;

        for (int l = 0; l < 4; l++) // iterate different directions
        {
            int num_per_cell = Convert.ToInt32(this.footprint_length / this.cell_length);
            for (int m = 0; m < num_per_cell; m++) // iterate along walls
            {
                // W/N/E/S
                for (int n = 0; n < roof_wall_height; n++) // iterate elevation
                {
                    int window_or_door = 0;
                    if (n == roof_wall_height - 1)
                    {
                        window_or_door = 2;
                    }
                    if (l == 0)
                    {
                        this.createWall(l, x, y + this.cell_length * n, z + footprint_length - this.cell_length * m, window_or_door);
                    } else if (l == 1)
                    {
                        this.createWall(l, x + footprint_length - this.cell_length * m, y + this.cell_length * n, z + footprint_length, window_or_door);
                    } else if (l == 2)
                    {
                        this.createWall(l, x + footprint_length, y + this.cell_length * n, z + this.cell_length * m, window_or_door);
                    } else if (l == 3)
                    {
                        this.createWall(l, x + this.cell_length * m, y + this.cell_length * n, z, window_or_door);
                    }
                }
            }
        }
        
        
        // new base point coord
        y = y + roof_wall_height * this.cell_length;
        
        // complete
        if (roof_type == 0) {
        
            // create a mesh object
            Mesh roof_mesh = new Mesh();

            // vertices of a cube
            Vector3[] roof_verts = new Vector3[4];
            roof_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length);
            roof_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z);
            roof_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
            roof_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + this.footprint_length);

            // create uv coordinates
            Vector2[] roof_uv = new Vector2[4];

            roof_uv[0] = new Vector2(0, 0);
            roof_uv[1] = new Vector2(1, 0);
            roof_uv[2] = new Vector2(1, 1);
            roof_uv[3] = new Vector2(0, 1);

            // two triangles for the face

            int[] roof_tris = new int[6];  // need 3 vertices per triangle

            roof_tris[0] = 1;
            roof_tris[1] = 3;
            roof_tris[2] = 2;
            roof_tris[3] = 1;
            roof_tris[4] = 0;
            roof_tris[5] = 3;

            // save the vertices and triangles in the mesh object
            roof_mesh.vertices = roof_verts;
            roof_mesh.triangles = roof_tris;
            roof_mesh.uv = roof_uv;  // save the uv texture coordinates

            roof_mesh.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s = new GameObject("Wall Mesh");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();

            // associate my mesh with this object
            s.GetComponent<MeshFilter>().mesh = roof_mesh;

            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

            // create a texture
            Texture2D texture = this.make_roof_texture();
            
            // attach the texture to the mesh
            Renderer renderer = s.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;
            
            // create the same obj
            GameObject s_copy = GameObject.Instantiate(s);
            GameObject cp_pivot_point = new GameObject("pivot point");
            cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            s_copy.transform.parent = cp_pivot_point.gameObject.transform;
            cp_pivot_point.transform.eulerAngles = new Vector3(0,180,0);
            
            // create pivot point
            GameObject pivot_point = new GameObject("pivot point");
            pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            s.transform.parent = pivot_point.gameObject.transform;
            s_copy.transform.parent = pivot_point.gameObject.transform;
            // rotate if left-right direction
            if (surround[0])
            {
                pivot_point.transform.eulerAngles = new Vector3(0,90,0);    
            }
        }
        // edge
        else if (roof_type == 1)
        {
            if (!surround[0] && !surround[1] && !surround[2] && !surround[3])
            { 
                    
                // create triangle wall
                // create a mesh object
                Mesh tris_wall_s__mesh = new Mesh();

                // vertices of a cube
                Vector3[] tris_wall_s__verts = new Vector3[3];
                // tris_wall_s__verts[0] = new Vector3(x - 0.5f * this.cell_length, y - 0.25f * this.cell_length, z - 0.5f * this.cell_length);
                // tris_wall_s__verts[1] = new Vector3(x + this.footprint_length + 0.5f * this.cell_length, y - 0.25f * this.cell_length, z - 0.5f * this.cell_length);
                // tris_wall_s__verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                tris_wall_s__verts[0] = new Vector3(x, y - 0.25f * this.cell_length, z);
                tris_wall_s__verts[1] = new Vector3(x + this.footprint_length, y - 0.25f * this.cell_length, z);
                tris_wall_s__verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] tris_wall_s__uv = new Vector2[3];

                tris_wall_s__uv[0] = new Vector2(0, 0);
                tris_wall_s__uv[1] = new Vector2(1, 0);
                tris_wall_s__uv[2] = new Vector2(0.5f, 0.25f);

                // two triangles for the face

                int[] tris_wall_s__tris = new int[3];  // need 3 vertices per triangle

                tris_wall_s__tris[0] = 0;
                tris_wall_s__tris[1] = 2;
                tris_wall_s__tris[2] = 1;

                // save the vertices and triangles in the mesh object
                tris_wall_s__mesh.vertices = tris_wall_s__verts;
                tris_wall_s__mesh.triangles = tris_wall_s__tris;
                tris_wall_s__mesh.uv = tris_wall_s__uv;  // save the uv texture coordinates

                tris_wall_s__mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s_tris_wall_s_ = new GameObject("Wall Mesh");
                s_tris_wall_s_.AddComponent<MeshFilter>();
                s_tris_wall_s_.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s_tris_wall_s_.GetComponent<MeshFilter>().mesh = tris_wall_s__mesh;

                // change the color of the object
                Renderer rend_tris_wall_s_ = s_tris_wall_s_.GetComponent<Renderer>();
                rend_tris_wall_s_.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture_tris_wall_s_ = this.make_roof2_texture();

                // attach the texture to the mesh
                rend_tris_wall_s_.material.mainTexture = texture_tris_wall_s_;
                
                
                
                // create the same obj
                GameObject s_tris_wall_s__copy1 = GameObject.Instantiate(s_tris_wall_s_);
                GameObject cp_pivot_point_s = new GameObject("pivot point");
                cp_pivot_point_s.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_tris_wall_s__copy1.transform.parent = cp_pivot_point_s.gameObject.transform;
                cp_pivot_point_s.transform.eulerAngles = new Vector3(0,90,0);
                
                GameObject s_tris_wall_s__copy2 = GameObject.Instantiate(s_tris_wall_s_);
                cp_pivot_point_s = new GameObject("pivot point");
                cp_pivot_point_s.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_tris_wall_s__copy2.transform.parent = cp_pivot_point_s.gameObject.transform;
                cp_pivot_point_s.transform.eulerAngles = new Vector3(0,180,0);
                
                GameObject s_tris_wall_s__copy3 = GameObject.Instantiate(s_tris_wall_s_);
                cp_pivot_point_s = new GameObject("pivot point");
                cp_pivot_point_s.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_tris_wall_s__copy3.transform.parent = cp_pivot_point_s.gameObject.transform;
                cp_pivot_point_s.transform.eulerAngles = new Vector3(0,270,0);
                    
                return;
            }
            
            
            
            
            
            // create a mesh object
            Mesh roof_side1_mesh = new Mesh();

            // vertices of a cube
            Vector3[] roof_side1_verts = new Vector3[4];
            roof_side1_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length + 0.5f * this.cell_length);
            roof_side1_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z);
            roof_side1_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
            roof_side1_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + (5f / 6f) * this.footprint_length);

            // create uv coordinates
            Vector2[] roof_side1_uv = new Vector2[4];

            roof_side1_uv[0] = new Vector2(0, 0);
            roof_side1_uv[1] = new Vector2(1, 0);
            roof_side1_uv[2] = new Vector2(1, 1);
            roof_side1_uv[3] = new Vector2(0, 1);

            // two triangles for the face

            int[] roof_side1_tris = new int[6];  // need 3 vertices per triangle

            roof_side1_tris[0] = 1;
            roof_side1_tris[1] = 3;
            roof_side1_tris[2] = 2;
            roof_side1_tris[3] = 1;
            roof_side1_tris[4] = 0;
            roof_side1_tris[5] = 3;

            // save the vertices and triangles in the mesh object
            roof_side1_mesh.vertices = roof_side1_verts;
            roof_side1_mesh.triangles = roof_side1_tris;
            roof_side1_mesh.uv = roof_side1_uv;  // save the uv texture coordinates

            roof_side1_mesh.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s_side1 = new GameObject("Wall Mesh");
            s_side1.AddComponent<MeshFilter>();
            s_side1.AddComponent<MeshRenderer>();

            // associate my mesh with this object
            s_side1.GetComponent<MeshFilter>().mesh = roof_side1_mesh;

            // change the color of the object
            Renderer rend = s_side1.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

            // create a texture
            Texture2D texture = this.make_roof_texture();
            
            // attach the texture to the mesh
            Renderer renderer = s_side1.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;
            
            
            
            
            
               
            
            // create a mesh object
            Mesh roof_side2_mesh = new Mesh();

            // vertices of a cube
            Vector3[] roof_side2_verts = new Vector3[4];
            roof_side2_verts[0] = new Vector3(x + this.footprint_length + 1 * this.cell_length, y - 0.5f * this.cell_length, z);
            roof_side2_verts[1] = new Vector3(x + this.footprint_length + 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length + 0.5f * this.cell_length);
            roof_side2_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + (5f / 6f) * this.footprint_length);
            roof_side2_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);

            // create uv coordinates
            Vector2[] roof_side2_uv = new Vector2[4];

            roof_side2_uv[0] = new Vector2(0, 0);
            roof_side2_uv[1] = new Vector2(1, 0);
            roof_side2_uv[2] = new Vector2(1, 1);
            roof_side2_uv[3] = new Vector2(0, 1);

            // two triangles for the face

            int[] roof_side2_tris = new int[6];  // need 3 vertices per triangle

            roof_side2_tris[0] = 1;
            roof_side2_tris[1] = 3;
            roof_side2_tris[2] = 2;
            roof_side2_tris[3] = 1;
            roof_side2_tris[4] = 0;
            roof_side2_tris[5] = 3;

            // save the vertices and triangles in the mesh object
            roof_side2_mesh.vertices = roof_side2_verts;
            roof_side2_mesh.triangles = roof_side2_tris;
            roof_side2_mesh.uv = roof_side2_uv;  // save the uv texture coordinates

            roof_side2_mesh.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s_side2 = new GameObject("Wall Mesh");
            s_side2.AddComponent<MeshFilter>();
            s_side2.AddComponent<MeshRenderer>();

            // associate my mesh with this object
            s_side2.GetComponent<MeshFilter>().mesh = roof_side2_mesh;

            // change the color of the object
            rend = s_side2.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

            // create a texture
            texture = this.make_roof_texture();
            
            // attach the texture to the mesh
            renderer = s_side2.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;
            
            
            
            
            
            
            
            
            
            
            
            // create triangle wall
            // create a mesh object
            Mesh tris_wall_mesh = new Mesh();

            // vertices of a cube
            Vector3[] tris_wall_verts = new Vector3[3];
            tris_wall_verts[0] = new Vector3(x + this.footprint_length + this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length + 0.5f * this.cell_length);
            tris_wall_verts[1] = new Vector3(x - this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length + 0.5f * this.cell_length);
            tris_wall_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + (5f / 6f) * this.footprint_length);

            // create uv coordinates
            Vector2[] tris_wall_uv = new Vector2[3];

            tris_wall_uv[0] = new Vector2(0, 0);
            tris_wall_uv[1] = new Vector2(1, 0);
            tris_wall_uv[2] = new Vector2(0.5f, 0.25f);

            // two triangles for the face

            int[] tris_wall_tris = new int[3];  // need 3 vertices per triangle

            tris_wall_tris[0] = 0;
            tris_wall_tris[1] = 2;
            tris_wall_tris[2] = 1;

            // save the vertices and triangles in the mesh object
            tris_wall_mesh.vertices = tris_wall_verts;
            tris_wall_mesh.triangles = tris_wall_tris;
            tris_wall_mesh.uv = tris_wall_uv;  // save the uv texture coordinates

            tris_wall_mesh.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s_tris_wall = new GameObject("Wall Mesh");
            s_tris_wall.AddComponent<MeshFilter>();
            s_tris_wall.AddComponent<MeshRenderer>();

            // associate my mesh with this object
            s_tris_wall.GetComponent<MeshFilter>().mesh = tris_wall_mesh;

            // change the color of the object
            Renderer rend_tris_wall = s_tris_wall.GetComponent<Renderer>();
            rend_tris_wall.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

            // create a texture
            Texture2D texture_tris_wall = this.make_roof_texture();
            
            // attach the texture to the mesh
            rend_tris_wall.material.mainTexture = texture_tris_wall;
            
            
            
            
            
            
            
            // // create the same obj
            // GameObject s_tris_wall_copy = GameObject.Instantiate(s_tris_wall);
            // GameObject cp_pivot_point = new GameObject("pivot point");
            // cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            // s_tris_wall_copy.transform.parent = cp_pivot_point.gameObject.transform;
            // cp_pivot_point.transform.eulerAngles = new Vector3(0,180,0);
            

            // create pivot point
            GameObject pivot_point = new GameObject("pivot point");
            pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            
            s_side1.transform.parent = pivot_point.gameObject.transform;
            s_side2.transform.parent = pivot_point.gameObject.transform;
            s_tris_wall.transform.parent = pivot_point.gameObject.transform;

            
            // rotate if left-right direction
            if (surround[0])
            {
                pivot_point.transform.eulerAngles = new Vector3(0,90,0);    
            } else if (surround[3])
            {
                pivot_point.transform.eulerAngles = new Vector3(0,180,0);
            } else if (surround[2])
            {
                pivot_point.transform.eulerAngles = new Vector3(0,270,0);
            }
        }
        // cross
        else if (roof_type == 2)
        {
            // Debug.Log(surround.Count(cc => cc));
            if (surround.Count(cc => cc) == 4)
            {
                    
                // create a mesh object
                Mesh roof_1_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_1_verts = new Vector3[3];
                roof_1_verts[0] = new Vector3(x, y, z);
                roof_1_verts[1] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                // roof_1_verts[2] = new Vector3(x + this.footprint_length, y, z);
                roof_1_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_1_uv = new Vector2[3];

                roof_1_uv[0] = new Vector2(1, 0);
                roof_1_uv[1] = new Vector2(1, 1);
                roof_1_uv[2] = new Vector2(0, 1);
                // roof_1_uv[3] = new Vector2(0, 1);

                // two triangles for the face

                int[] roof_1_tris = new int[6];  // need 3 vertices per triangle

                roof_1_tris[0] = 0;
                roof_1_tris[1] = 2;
                roof_1_tris[2] = 1;
                // roof_1_tris[3] = 1;
                // roof_1_tris[4] = 0;
                // roof_1_tris[5] = 3;

                // save the vertices and triangles in the mesh object
                roof_1_mesh.vertices = roof_1_verts;
                roof_1_mesh.triangles = roof_1_tris;
                roof_1_mesh.uv = roof_1_uv;  // save the uv texture coordinates

                roof_1_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s1 = new GameObject("Wall Mesh");
                s1.AddComponent<MeshFilter>();
                s1.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s1.GetComponent<MeshFilter>().mesh = roof_1_mesh;

                // change the color of the object
                Renderer rend = s1.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                Renderer renderer = s1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                // create a mesh object
                Mesh roof_2_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_2_verts = new Vector3[3];
                // roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[1] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_2_verts[0] = new Vector3(x + this.footprint_length, y, z);
                roof_2_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_2_uv = new Vector2[3];

                roof_2_uv[0] = new Vector2(0, 0);
                roof_2_uv[1] = new Vector2(1, 1);
                roof_2_uv[2] = new Vector2(0, 1);
                // roof_2_uv[3] = new Vector2(0, 1);

                // two triangles for the face

                int[] roof_2_tris = new int[6];  // need 3 vertices per triangle

                roof_2_tris[0] = 0;
                roof_2_tris[1] = 1;
                roof_2_tris[2] = 2;
                // roof_2_tris[3] = 1;
                // roof_2_tris[4] = 0;
                // roof_2_tris[5] = 3;

                // save the vertices and triangles in the mesh object
                roof_2_mesh.vertices = roof_2_verts;
                roof_2_mesh.triangles = roof_2_tris;
                roof_2_mesh.uv = roof_2_uv;  // save the uv texture coordinates

                roof_2_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s2 = new GameObject("Wall Mesh");
                s2.AddComponent<MeshFilter>();
                s2.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s2.GetComponent<MeshFilter>().mesh = roof_2_mesh;

                // change the color of the object
                rend = s2.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s2.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                
                
                
                GameObject s = new GameObject("s");
                s1.transform.parent = s.gameObject.transform;
                s2.transform.parent = s.gameObject.transform;
                
                
                // create the same obj
                GameObject s_copy1 = GameObject.Instantiate(s);
                GameObject cp_pivot_point = new GameObject("pivot point");
                cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_copy1.transform.parent = cp_pivot_point.gameObject.transform;
                cp_pivot_point.transform.eulerAngles = new Vector3(0,90,0);
                
                GameObject s_copy2 = GameObject.Instantiate(s);
                cp_pivot_point = new GameObject("pivot point");
                cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_copy2.transform.parent = cp_pivot_point.gameObject.transform;
                cp_pivot_point.transform.eulerAngles = new Vector3(0,180,0);
                
                GameObject s_copy3 = GameObject.Instantiate(s);
                cp_pivot_point = new GameObject("pivot point");
                cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_copy3.transform.parent = cp_pivot_point.gameObject.transform;
                cp_pivot_point.transform.eulerAngles = new Vector3(0,270,0);

            }
            else if (surround.Count(cc => cc) == 3)
            {
                 // create a mesh object
                Mesh roof_1_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_1_verts = new Vector3[3];
                roof_1_verts[0] = new Vector3(x, y, z);
                roof_1_verts[1] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_1_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_1_uv = new Vector2[3];

                roof_1_uv[0] = new Vector2(1, 0);
                roof_1_uv[1] = new Vector2(1, 1);
                roof_1_uv[2] = new Vector2(0, 1);

                // two triangles for the face

                int[] roof_1_tris = new int[6];  // need 3 vertices per triangle

                roof_1_tris[0] = 0;
                roof_1_tris[1] = 2;
                roof_1_tris[2] = 1;

                // save the vertices and triangles in the mesh object
                roof_1_mesh.vertices = roof_1_verts;
                roof_1_mesh.triangles = roof_1_tris;
                roof_1_mesh.uv = roof_1_uv;  // save the uv texture coordinates

                roof_1_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s1 = new GameObject("Wall Mesh");
                s1.AddComponent<MeshFilter>();
                s1.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s1.GetComponent<MeshFilter>().mesh = roof_1_mesh;

                // change the color of the object
                Renderer rend = s1.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                Renderer renderer = s1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                
                
                
                
                // create a mesh object
                Mesh roof_2_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_2_verts = new Vector3[3];
                // roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[1] = new Vector3(x, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                roof_2_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_2_uv = new Vector2[3];

                roof_2_uv[0] = new Vector2(0, 0);
                roof_2_uv[1] = new Vector2(1, 1);
                roof_2_uv[2] = new Vector2(0, 1);
                
                // two triangles for the face

                int[] roof_2_tris = new int[6];  // need 3 vertices per triangle

                roof_2_tris[0] = 0;
                roof_2_tris[1] = 1;
                roof_2_tris[2] = 2;

                // save the vertices and triangles in the mesh object
                roof_2_mesh.vertices = roof_2_verts;
                roof_2_mesh.triangles = roof_2_tris;
                roof_2_mesh.uv = roof_2_uv;  // save the uv texture coordinates

                roof_2_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s2 = new GameObject("Wall Mesh");
                s2.AddComponent<MeshFilter>();
                s2.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s2.GetComponent<MeshFilter>().mesh = roof_2_mesh;

                // change the color of the object
                rend = s2.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s2.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                
                
                
                GameObject s = new GameObject("s");
                s1.transform.parent = s.gameObject.transform;
                s2.transform.parent = s.gameObject.transform;
                
                
                // attach the texture to the mesh
                renderer = s1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;


                // create the same obj
                GameObject s_copy1 = GameObject.Instantiate(s);
                GameObject cp_pivot_point = new GameObject("pivot point");
                cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_copy1.transform.parent = cp_pivot_point.gameObject.transform;
                cp_pivot_point.transform.eulerAngles = new Vector3(0,90,0);

                
                
                
                
                
                
                // create roof in the other side
                // create a mesh object
                Mesh roof_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_verts = new Vector3[4];
                roof_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length);
                roof_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z);
                roof_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + this.footprint_length);
                
                // create uv coordinates
                Vector2[] roof_uv = new Vector2[4];
                
                roof_uv[0] = new Vector2(0, 0);
                roof_uv[1] = new Vector2(1, 0);
                roof_uv[2] = new Vector2(1, 1);
                roof_uv[3] = new Vector2(0, 1);
                
                // two triangles for the face
                
                int[] roof_tris = new int[6];  // need 3 vertices per triangle
                
                roof_tris[0] = 1;
                roof_tris[1] = 3;
                roof_tris[2] = 2;
                roof_tris[3] = 1;
                roof_tris[4] = 0;
                roof_tris[5] = 3;
                
                // save the vertices and triangles in the mesh object
                roof_mesh.vertices = roof_verts;
                roof_mesh.triangles = roof_tris;
                roof_mesh.uv = roof_uv;  // save the uv texture coordinates
                
                roof_mesh.RecalculateNormals();  // automatically calculate the vertex normals
                
                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s_side = new GameObject("Wall Mesh");
                s_side.AddComponent<MeshFilter>();
                s_side.AddComponent<MeshRenderer>();
                
                // associate my mesh with this object
                s_side.GetComponent<MeshFilter>().mesh = roof_mesh;
                
                // change the color of the object
                rend = s_side.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
                
                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s_side.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
            
                GameObject pivot_point_cp = new GameObject("pivot point cp");
                pivot_point_cp.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_side.transform.parent = pivot_point_cp.gameObject.transform;
                pivot_point_cp.transform.eulerAngles = new Vector3(0,180,0);
                
                
                
                
                
                // rotate the whole roof
                // create pivot point
                GameObject pivot_point = new GameObject("pivot point");
                pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                
                s.transform.parent = pivot_point.gameObject.transform;
                s_copy1.transform.parent = pivot_point.gameObject.transform;
                s_side.transform.parent = pivot_point.gameObject.transform;
                // s_side.transform.parent = pivot_point.gameObject.transform;
                
                Debug.Log(surround[0] + "" +  surround[1] + surround[2] + surround[3]);
                if (!surround[0])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,180,0);    
                } else if (!surround[3])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,270,0);
                } else if (!surround[1])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,90,0);
                }
            }
            else if (surround.Count(cc => cc) == 2)
            { 
                // create a mesh object
                Mesh roof_1_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_1_verts = new Vector3[3];
                roof_1_verts[0] = new Vector3(x, y, z);
                roof_1_verts[1] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_1_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_1_uv = new Vector2[3];

                roof_1_uv[0] = new Vector2(1, 0);
                roof_1_uv[1] = new Vector2(1, 1);
                roof_1_uv[2] = new Vector2(0, 1);

                // two triangles for the face

                int[] roof_1_tris = new int[6];  // need 3 vertices per triangle

                roof_1_tris[0] = 0;
                roof_1_tris[1] = 2;
                roof_1_tris[2] = 1;

                // save the vertices and triangles in the mesh object
                roof_1_mesh.vertices = roof_1_verts;
                roof_1_mesh.triangles = roof_1_tris;
                roof_1_mesh.uv = roof_1_uv;  // save the uv texture coordinates

                roof_1_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s1 = new GameObject("Wall Mesh");
                s1.AddComponent<MeshFilter>();
                s1.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s1.GetComponent<MeshFilter>().mesh = roof_1_mesh;

                // change the color of the object
                Renderer rend = s1.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                Renderer renderer = s1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                
                
                
                
                // create a mesh object
                Mesh roof_2_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_2_verts = new Vector3[3];
                // roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[1] = new Vector3(x, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                roof_2_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_2_uv = new Vector2[3];

                roof_2_uv[0] = new Vector2(0, 0);
                roof_2_uv[1] = new Vector2(1, 1);
                roof_2_uv[2] = new Vector2(0, 1);
                
                // two triangles for the face

                int[] roof_2_tris = new int[6];  // need 3 vertices per triangle

                roof_2_tris[0] = 0;
                roof_2_tris[1] = 1;
                roof_2_tris[2] = 2;

                // save the vertices and triangles in the mesh object
                roof_2_mesh.vertices = roof_2_verts;
                roof_2_mesh.triangles = roof_2_tris;
                roof_2_mesh.uv = roof_2_uv;  // save the uv texture coordinates

                roof_2_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s2 = new GameObject("Wall Mesh");
                s2.AddComponent<MeshFilter>();
                s2.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s2.GetComponent<MeshFilter>().mesh = roof_2_mesh;

                // change the color of the object
                rend = s2.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s2.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                

                GameObject s = new GameObject("s");
                s1.transform.parent = s.gameObject.transform;
                s2.transform.parent = s.gameObject.transform;






                
                // create roof in the 1 side
                // create a mesh object
                Mesh roof_11_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_11_verts = new Vector3[4];
                roof_11_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length + this.cell_length);
                roof_11_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z);
                roof_11_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_11_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                
                // create uv coordinates
                Vector2[] roof_11_uv = new Vector2[4];
                
                roof_11_uv[0] = new Vector2(0, 0);
                roof_11_uv[1] = new Vector2(1, 0);
                roof_11_uv[2] = new Vector2(1, 1);
                roof_11_uv[3] = new Vector2(0, 1);
                
                // two triangles for the face
                
                int[] roof_11_tris = new int[6];  // need 3 vertices per triangle
                
                roof_11_tris[0] = 1;
                roof_11_tris[1] = 3;
                roof_11_tris[2] = 2;
                roof_11_tris[3] = 1;
                roof_11_tris[4] = 0;
                roof_11_tris[5] = 3;
                
                // save the vertices and triangles in the mesh object
                roof_11_mesh.vertices = roof_11_verts;
                roof_11_mesh.triangles = roof_11_tris;
                roof_11_mesh.uv = roof_11_uv;  // save the uv texture coordinates
                
                roof_11_mesh.RecalculateNormals();  // automatically calculate the vertex normals
                
                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s_side_1 = new GameObject("Wall Mesh");
                s_side_1.AddComponent<MeshFilter>();
                s_side_1.AddComponent<MeshRenderer>();
                
                // associate my mesh with this object
                s_side_1.GetComponent<MeshFilter>().mesh = roof_11_mesh;
                
                // change the color of the object
                rend = s_side_1.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
                
                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s_side_1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
            
                GameObject pivot_point_cp = new GameObject("pivot point cp");
                pivot_point_cp.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_side_1.transform.parent = pivot_point_cp.gameObject.transform;
                pivot_point_cp.transform.eulerAngles = new Vector3(0,90,0);
                
                
                
                
                
                // create roof in the 2 side
                // create a mesh object
                Mesh roof_22_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_22_verts = new Vector3[4];
                roof_22_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length);
                roof_22_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z - this.cell_length);
                roof_22_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                roof_22_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + this.footprint_length);
                
                // create uv coordinates
                Vector2[] roof_22_uv = new Vector2[4];
                
                roof_22_uv[0] = new Vector2(0, 0);
                roof_22_uv[1] = new Vector2(1, 0);
                roof_22_uv[2] = new Vector2(1, 1);
                roof_22_uv[3] = new Vector2(0, 1);
                
                // two triangles for the face
                
                int[] roof_22_tris = new int[6];  // need 3 vertices per triangle
                
                roof_22_tris[0] = 1;
                roof_22_tris[1] = 3;
                roof_22_tris[2] = 2;
                roof_22_tris[3] = 1;
                roof_22_tris[4] = 0;
                roof_22_tris[5] = 3;
                
                // save the vertices and triangles in the mesh object
                roof_22_mesh.vertices = roof_22_verts;
                roof_22_mesh.triangles = roof_22_tris;
                roof_22_mesh.uv = roof_22_uv;  // save the uv texture coordinates
                
                roof_22_mesh.RecalculateNormals();  // automatically calculate the vertex normals
                
                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s_side_2 = new GameObject("Wall Mesh");
                s_side_2.AddComponent<MeshFilter>();
                s_side_2.AddComponent<MeshRenderer>();
                
                // associate my mesh with this object
                s_side_2.GetComponent<MeshFilter>().mesh = roof_22_mesh;
                
                // change the color of the object
                rend = s_side_2.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
                
                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s_side_2.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
            
                pivot_point_cp = new GameObject("pivot point cp");
                pivot_point_cp.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_side_2.transform.parent = pivot_point_cp.gameObject.transform;
                pivot_point_cp.transform.eulerAngles = new Vector3(0,180,0);
                
                
                
                
                
                
                
                
                // rotate the whole roof
                // create pivot point
                GameObject pivot_point = new GameObject("pivot point");
                pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                
                s.transform.parent = pivot_point.gameObject.transform;
                s_side_1.transform.parent = pivot_point.gameObject.transform;
                s_side_2.transform.parent = pivot_point.gameObject.transform;
                
                // s_side.transform.parent = pivot_point.gameObject.transform;
                
                Debug.Log(surround[0] + "" +  surround[1] + surround[2] + surround[3]);
                if (surround[1] && surround[2])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,270,0);    
                } else if (surround[2] && surround[3])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,180,0);
                } else if (surround[3] && surround[0])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,90,0);
                }
            }
        }
        
    }
    public void createGableRoof(float x, float y, float z, int roof_type, List<bool> surround)
    {
        int roof_wall_height = 2;

        for (int l = 0; l < 4; l++) // iterate different directions
        {
            int num_per_cell = Convert.ToInt32(this.footprint_length / this.cell_length);
            for (int m = 0; m < num_per_cell; m++) // iterate along walls
            {
                // W/N/E/S
                for (int n = 0; n < roof_wall_height; n++) // iterate elevation
                {
                    int window_or_door = 0;
                    if (n == roof_wall_height - 1)
                    {
                        window_or_door = 2;
                    }
                    if (l == 0)
                    {
                        this.createWall(l, x, y + this.cell_length * n, z + footprint_length - this.cell_length * m, window_or_door);
                    } else if (l == 1)
                    {
                        this.createWall(l, x + footprint_length - this.cell_length * m, y + this.cell_length * n, z + footprint_length, window_or_door);
                    } else if (l == 2)
                    {
                        this.createWall(l, x + footprint_length, y + this.cell_length * n, z + this.cell_length * m, window_or_door);
                    } else if (l == 3)
                    {
                        this.createWall(l, x + this.cell_length * m, y + this.cell_length * n, z, window_or_door);
                    }
                }
            }
        }
        
        // new base point coord
        y = y + roof_wall_height * this.cell_length;
        
        // complete
        if (roof_type == 0) {
        
            // create a mesh object
            Mesh roof_mesh = new Mesh();

            // vertices of a cube
            Vector3[] roof_verts = new Vector3[4];
            roof_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length);
            roof_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z);
            roof_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
            roof_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + this.footprint_length);

            // create uv coordinates
            Vector2[] roof_uv = new Vector2[4];

            roof_uv[0] = new Vector2(0, 0);
            roof_uv[1] = new Vector2(1, 0);
            roof_uv[2] = new Vector2(1, 1);
            roof_uv[3] = new Vector2(0, 1);

            // two triangles for the face

            int[] roof_tris = new int[6];  // need 3 vertices per triangle

            roof_tris[0] = 1;
            roof_tris[1] = 3;
            roof_tris[2] = 2;
            roof_tris[3] = 1;
            roof_tris[4] = 0;
            roof_tris[5] = 3;

            // save the vertices and triangles in the mesh object
            roof_mesh.vertices = roof_verts;
            roof_mesh.triangles = roof_tris;
            roof_mesh.uv = roof_uv;  // save the uv texture coordinates

            roof_mesh.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s = new GameObject("Wall Mesh");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();

            // associate my mesh with this object
            s.GetComponent<MeshFilter>().mesh = roof_mesh;

            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

            // create a texture
            Texture2D texture = this.make_roof_texture();
            
            // attach the texture to the mesh
            Renderer renderer = s.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;
            
            // create the same obj
            GameObject s_copy = GameObject.Instantiate(s);
            GameObject cp_pivot_point = new GameObject("pivot point");
            cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            s_copy.transform.parent = cp_pivot_point.gameObject.transform;
            cp_pivot_point.transform.eulerAngles = new Vector3(0,180,0);
            
            // create pivot point
            GameObject pivot_point = new GameObject("pivot point");
            pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            s.transform.parent = pivot_point.gameObject.transform;
            s_copy.transform.parent = pivot_point.gameObject.transform;
            // rotate if left-right direction
            if (surround[0])
            {
                pivot_point.transform.eulerAngles = new Vector3(0,90,0);    
            }
        }
        // edge
        else if (roof_type == 1)
        {
            if (!surround[0] && !surround[1] && !surround[2] && !surround[3])
            { 
                    
                // create triangle wall
                // create a mesh object
                Mesh tris_wall_s__mesh = new Mesh();

                // vertices of a cube
                Vector3[] tris_wall_s__verts = new Vector3[3];
                // tris_wall_s__verts[0] = new Vector3(x - 0.5f * this.cell_length, y - 0.25f * this.cell_length, z - 0.5f * this.cell_length);
                // tris_wall_s__verts[1] = new Vector3(x + this.footprint_length + 0.5f * this.cell_length, y - 0.25f * this.cell_length, z - 0.5f * this.cell_length);
                // tris_wall_s__verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                tris_wall_s__verts[0] = new Vector3(x, y - 0.25f * this.cell_length, z);
                tris_wall_s__verts[1] = new Vector3(x + this.footprint_length, y - 0.25f * this.cell_length, z);
                tris_wall_s__verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] tris_wall_s__uv = new Vector2[3];

                tris_wall_s__uv[0] = new Vector2(0, 0);
                tris_wall_s__uv[1] = new Vector2(1, 0);
                tris_wall_s__uv[2] = new Vector2(0.5f, 0.25f);

                // two triangles for the face

                int[] tris_wall_s__tris = new int[3];  // need 3 vertices per triangle

                tris_wall_s__tris[0] = 0;
                tris_wall_s__tris[1] = 2;
                tris_wall_s__tris[2] = 1;

                // save the vertices and triangles in the mesh object
                tris_wall_s__mesh.vertices = tris_wall_s__verts;
                tris_wall_s__mesh.triangles = tris_wall_s__tris;
                tris_wall_s__mesh.uv = tris_wall_s__uv;  // save the uv texture coordinates

                tris_wall_s__mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s_tris_wall_s_ = new GameObject("Wall Mesh");
                s_tris_wall_s_.AddComponent<MeshFilter>();
                s_tris_wall_s_.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s_tris_wall_s_.GetComponent<MeshFilter>().mesh = tris_wall_s__mesh;

                // change the color of the object
                Renderer rend_tris_wall_s_ = s_tris_wall_s_.GetComponent<Renderer>();
                rend_tris_wall_s_.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture_tris_wall_s_ = this.make_roof2_texture();

                // attach the texture to the mesh
                rend_tris_wall_s_.material.mainTexture = texture_tris_wall_s_;
                
                
                
                // create the same obj
                GameObject s_tris_wall_s__copy1 = GameObject.Instantiate(s_tris_wall_s_);
                GameObject cp_pivot_point_s = new GameObject("pivot point");
                cp_pivot_point_s.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_tris_wall_s__copy1.transform.parent = cp_pivot_point_s.gameObject.transform;
                cp_pivot_point_s.transform.eulerAngles = new Vector3(0,90,0);
                
                GameObject s_tris_wall_s__copy2 = GameObject.Instantiate(s_tris_wall_s_);
                cp_pivot_point_s = new GameObject("pivot point");
                cp_pivot_point_s.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_tris_wall_s__copy2.transform.parent = cp_pivot_point_s.gameObject.transform;
                cp_pivot_point_s.transform.eulerAngles = new Vector3(0,180,0);
                
                GameObject s_tris_wall_s__copy3 = GameObject.Instantiate(s_tris_wall_s_);
                cp_pivot_point_s = new GameObject("pivot point");
                cp_pivot_point_s.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_tris_wall_s__copy3.transform.parent = cp_pivot_point_s.gameObject.transform;
                cp_pivot_point_s.transform.eulerAngles = new Vector3(0,270,0);
                    
                return;
            }
            // create a mesh object
            Mesh roof_mesh = new Mesh();

            // vertices of a cube
            Vector3[] roof_verts = new Vector3[4];
            roof_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length + 0.5f * this.cell_length);
            roof_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z);
            roof_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
            roof_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + this.footprint_length + 0.5f * this.cell_length);

            // create uv coordinates
            Vector2[] roof_uv = new Vector2[4];

            roof_uv[0] = new Vector2(0, 0);
            roof_uv[1] = new Vector2(1, 0);
            roof_uv[2] = new Vector2(1, 1);
            roof_uv[3] = new Vector2(0, 1);

            // two triangles for the face

            int[] roof_tris = new int[6];  // need 3 vertices per triangle

            roof_tris[0] = 1;
            roof_tris[1] = 3;
            roof_tris[2] = 2;
            roof_tris[3] = 1;
            roof_tris[4] = 0;
            roof_tris[5] = 3;

            // save the vertices and triangles in the mesh object
            roof_mesh.vertices = roof_verts;
            roof_mesh.triangles = roof_tris;
            roof_mesh.uv = roof_uv;  // save the uv texture coordinates

            roof_mesh.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s = new GameObject("Wall Mesh");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();

            // associate my mesh with this object
            s.GetComponent<MeshFilter>().mesh = roof_mesh;

            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

            // create a texture
            Texture2D texture = this.make_roof_texture();
            
            // attach the texture to the mesh
            Renderer renderer = s.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;
            
            
            
            
            
            
            
            // create triangle wall
            // create a mesh object
            Mesh tris_wall_mesh = new Mesh();

            // vertices of a cube
            Vector3[] tris_wall_verts = new Vector3[3];
            tris_wall_verts[0] = new Vector3(x, y, z);
            tris_wall_verts[1] = new Vector3(x + this.footprint_length, y, z);
            tris_wall_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);

            // create uv coordinates
            Vector2[] tris_wall_uv = new Vector2[3];

            tris_wall_uv[0] = new Vector2(0, 0);
            tris_wall_uv[1] = new Vector2(1, 0);
            tris_wall_uv[2] = new Vector2(0.5f, 0.25f);

            // two triangles for the face

            int[] tris_wall_tris = new int[3];  // need 3 vertices per triangle

            tris_wall_tris[0] = 0;
            tris_wall_tris[1] = 2;
            tris_wall_tris[2] = 1;

            // save the vertices and triangles in the mesh object
            tris_wall_mesh.vertices = tris_wall_verts;
            tris_wall_mesh.triangles = tris_wall_tris;
            tris_wall_mesh.uv = tris_wall_uv;  // save the uv texture coordinates

            tris_wall_mesh.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s_tris_wall = new GameObject("Wall Mesh");
            s_tris_wall.AddComponent<MeshFilter>();
            s_tris_wall.AddComponent<MeshRenderer>();

            // associate my mesh with this object
            s_tris_wall.GetComponent<MeshFilter>().mesh = tris_wall_mesh;

            // change the color of the object
            Renderer rend_tris_wall = s_tris_wall.GetComponent<Renderer>();
            rend_tris_wall.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

            // create a texture
            Texture2D texture_tris_wall = this.make_roof_texture();
            
            // attach the texture to the mesh
            rend_tris_wall.material.mainTexture = texture_tris_wall;
            
            
            
            
            
            
            
            // create the same obj
            GameObject s_tris_wall_copy = GameObject.Instantiate(s_tris_wall);
            GameObject cp_pivot_point = new GameObject("pivot point");
            cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            s_tris_wall_copy.transform.parent = cp_pivot_point.gameObject.transform;
            cp_pivot_point.transform.eulerAngles = new Vector3(0,180,0);
            
            GameObject s_copy = GameObject.Instantiate(s);
            cp_pivot_point = new GameObject("pivot point");
            cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            s_copy.transform.parent = cp_pivot_point.gameObject.transform;
            cp_pivot_point.transform.eulerAngles = new Vector3(0,180,0);
            cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length + 0.5f * this.cell_length);
            
            // create pivot point
            GameObject pivot_point = new GameObject("pivot point");
            pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
            s.transform.parent = pivot_point.gameObject.transform;
            s_copy.transform.parent = pivot_point.gameObject.transform;
            s_tris_wall.transform.parent = pivot_point.gameObject.transform;
            s_tris_wall_copy.transform.parent = pivot_point.gameObject.transform;
            
            // rotate if left-right direction
            if (surround[0])
            {
                pivot_point.transform.eulerAngles = new Vector3(0,90,0);    
            } else if (surround[3])
            {
                pivot_point.transform.eulerAngles = new Vector3(0,180,0);
            } else if (surround[2])
            {
                pivot_point.transform.eulerAngles = new Vector3(0,270,0);
            }
        }
        // cross
        else if (roof_type == 2)
        {
            // Debug.Log(surround.Count(cc => cc));
            if (surround.Count(cc => cc) == 4)
            {
                    
                // create a mesh object
                Mesh roof_1_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_1_verts = new Vector3[3];
                roof_1_verts[0] = new Vector3(x, y, z);
                roof_1_verts[1] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                // roof_1_verts[2] = new Vector3(x + this.footprint_length, y, z);
                roof_1_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_1_uv = new Vector2[3];

                roof_1_uv[0] = new Vector2(1, 0);
                roof_1_uv[1] = new Vector2(1, 1);
                roof_1_uv[2] = new Vector2(0, 1);
                // roof_1_uv[3] = new Vector2(0, 1);

                // two triangles for the face

                int[] roof_1_tris = new int[6];  // need 3 vertices per triangle

                roof_1_tris[0] = 0;
                roof_1_tris[1] = 2;
                roof_1_tris[2] = 1;
                // roof_1_tris[3] = 1;
                // roof_1_tris[4] = 0;
                // roof_1_tris[5] = 3;

                // save the vertices and triangles in the mesh object
                roof_1_mesh.vertices = roof_1_verts;
                roof_1_mesh.triangles = roof_1_tris;
                roof_1_mesh.uv = roof_1_uv;  // save the uv texture coordinates

                roof_1_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s1 = new GameObject("Wall Mesh");
                s1.AddComponent<MeshFilter>();
                s1.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s1.GetComponent<MeshFilter>().mesh = roof_1_mesh;

                // change the color of the object
                Renderer rend = s1.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                Renderer renderer = s1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                // create a mesh object
                Mesh roof_2_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_2_verts = new Vector3[3];
                // roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[1] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_2_verts[0] = new Vector3(x + this.footprint_length, y, z);
                roof_2_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_2_uv = new Vector2[3];

                roof_2_uv[0] = new Vector2(0, 0);
                roof_2_uv[1] = new Vector2(1, 1);
                roof_2_uv[2] = new Vector2(0, 1);
                // roof_2_uv[3] = new Vector2(0, 1);

                // two triangles for the face

                int[] roof_2_tris = new int[6];  // need 3 vertices per triangle

                roof_2_tris[0] = 0;
                roof_2_tris[1] = 1;
                roof_2_tris[2] = 2;
                // roof_2_tris[3] = 1;
                // roof_2_tris[4] = 0;
                // roof_2_tris[5] = 3;

                // save the vertices and triangles in the mesh object
                roof_2_mesh.vertices = roof_2_verts;
                roof_2_mesh.triangles = roof_2_tris;
                roof_2_mesh.uv = roof_2_uv;  // save the uv texture coordinates

                roof_2_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s2 = new GameObject("Wall Mesh");
                s2.AddComponent<MeshFilter>();
                s2.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s2.GetComponent<MeshFilter>().mesh = roof_2_mesh;

                // change the color of the object
                rend = s2.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s2.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                
                
                
                GameObject s = new GameObject("s");
                s1.transform.parent = s.gameObject.transform;
                s2.transform.parent = s.gameObject.transform;
                
                
                // create the same obj
                GameObject s_copy1 = GameObject.Instantiate(s);
                GameObject cp_pivot_point = new GameObject("pivot point");
                cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_copy1.transform.parent = cp_pivot_point.gameObject.transform;
                cp_pivot_point.transform.eulerAngles = new Vector3(0,90,0);
                
                GameObject s_copy2 = GameObject.Instantiate(s);
                cp_pivot_point = new GameObject("pivot point");
                cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_copy2.transform.parent = cp_pivot_point.gameObject.transform;
                cp_pivot_point.transform.eulerAngles = new Vector3(0,180,0);
                
                GameObject s_copy3 = GameObject.Instantiate(s);
                cp_pivot_point = new GameObject("pivot point");
                cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_copy3.transform.parent = cp_pivot_point.gameObject.transform;
                cp_pivot_point.transform.eulerAngles = new Vector3(0,270,0);

            }
            else if (surround.Count(cc => cc) == 3)
            {
                 // create a mesh object
                Mesh roof_1_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_1_verts = new Vector3[3];
                roof_1_verts[0] = new Vector3(x, y, z);
                roof_1_verts[1] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_1_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_1_uv = new Vector2[3];

                roof_1_uv[0] = new Vector2(1, 0);
                roof_1_uv[1] = new Vector2(1, 1);
                roof_1_uv[2] = new Vector2(0, 1);

                // two triangles for the face

                int[] roof_1_tris = new int[6];  // need 3 vertices per triangle

                roof_1_tris[0] = 0;
                roof_1_tris[1] = 2;
                roof_1_tris[2] = 1;

                // save the vertices and triangles in the mesh object
                roof_1_mesh.vertices = roof_1_verts;
                roof_1_mesh.triangles = roof_1_tris;
                roof_1_mesh.uv = roof_1_uv;  // save the uv texture coordinates

                roof_1_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s1 = new GameObject("Wall Mesh");
                s1.AddComponent<MeshFilter>();
                s1.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s1.GetComponent<MeshFilter>().mesh = roof_1_mesh;

                // change the color of the object
                Renderer rend = s1.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                Renderer renderer = s1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                
                
                
                
                // create a mesh object
                Mesh roof_2_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_2_verts = new Vector3[3];
                // roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[1] = new Vector3(x, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                roof_2_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_2_uv = new Vector2[3];

                roof_2_uv[0] = new Vector2(0, 0);
                roof_2_uv[1] = new Vector2(1, 1);
                roof_2_uv[2] = new Vector2(0, 1);
                
                // two triangles for the face

                int[] roof_2_tris = new int[6];  // need 3 vertices per triangle

                roof_2_tris[0] = 0;
                roof_2_tris[1] = 1;
                roof_2_tris[2] = 2;

                // save the vertices and triangles in the mesh object
                roof_2_mesh.vertices = roof_2_verts;
                roof_2_mesh.triangles = roof_2_tris;
                roof_2_mesh.uv = roof_2_uv;  // save the uv texture coordinates

                roof_2_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s2 = new GameObject("Wall Mesh");
                s2.AddComponent<MeshFilter>();
                s2.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s2.GetComponent<MeshFilter>().mesh = roof_2_mesh;

                // change the color of the object
                rend = s2.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s2.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                
                
                
                GameObject s = new GameObject("s");
                s1.transform.parent = s.gameObject.transform;
                s2.transform.parent = s.gameObject.transform;
                
                
                // attach the texture to the mesh
                renderer = s1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;


                // create the same obj
                GameObject s_copy1 = GameObject.Instantiate(s);
                GameObject cp_pivot_point = new GameObject("pivot point");
                cp_pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_copy1.transform.parent = cp_pivot_point.gameObject.transform;
                cp_pivot_point.transform.eulerAngles = new Vector3(0,90,0);

                
                
                
                
                
                
                // create roof in the other side
                // create a mesh object
                Mesh roof_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_verts = new Vector3[4];
                roof_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length);
                roof_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z);
                roof_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + this.footprint_length);
                
                // create uv coordinates
                Vector2[] roof_uv = new Vector2[4];
                
                roof_uv[0] = new Vector2(0, 0);
                roof_uv[1] = new Vector2(1, 0);
                roof_uv[2] = new Vector2(1, 1);
                roof_uv[3] = new Vector2(0, 1);
                
                // two triangles for the face
                
                int[] roof_tris = new int[6];  // need 3 vertices per triangle
                
                roof_tris[0] = 1;
                roof_tris[1] = 3;
                roof_tris[2] = 2;
                roof_tris[3] = 1;
                roof_tris[4] = 0;
                roof_tris[5] = 3;
                
                // save the vertices and triangles in the mesh object
                roof_mesh.vertices = roof_verts;
                roof_mesh.triangles = roof_tris;
                roof_mesh.uv = roof_uv;  // save the uv texture coordinates
                
                roof_mesh.RecalculateNormals();  // automatically calculate the vertex normals
                
                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s_side = new GameObject("Wall Mesh");
                s_side.AddComponent<MeshFilter>();
                s_side.AddComponent<MeshRenderer>();
                
                // associate my mesh with this object
                s_side.GetComponent<MeshFilter>().mesh = roof_mesh;
                
                // change the color of the object
                rend = s_side.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
                
                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s_side.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
            
                GameObject pivot_point_cp = new GameObject("pivot point cp");
                pivot_point_cp.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_side.transform.parent = pivot_point_cp.gameObject.transform;
                pivot_point_cp.transform.eulerAngles = new Vector3(0,180,0);
                
                
                
                
                
                // rotate the whole roof
                // create pivot point
                GameObject pivot_point = new GameObject("pivot point");
                pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                
                s.transform.parent = pivot_point.gameObject.transform;
                s_copy1.transform.parent = pivot_point.gameObject.transform;
                s_side.transform.parent = pivot_point.gameObject.transform;
                // s_side.transform.parent = pivot_point.gameObject.transform;
                
                Debug.Log(surround[0] + "" +  surround[1] + surround[2] + surround[3]);
                if (!surround[0])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,180,0);    
                } else if (!surround[3])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,270,0);
                } else if (!surround[1])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,90,0);
                }
            }
            else if (surround.Count(cc => cc) == 2)
            { 
                // create a mesh object
                Mesh roof_1_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_1_verts = new Vector3[3];
                roof_1_verts[0] = new Vector3(x, y, z);
                roof_1_verts[1] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_1_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_1_uv = new Vector2[3];

                roof_1_uv[0] = new Vector2(1, 0);
                roof_1_uv[1] = new Vector2(1, 1);
                roof_1_uv[2] = new Vector2(0, 1);

                // two triangles for the face

                int[] roof_1_tris = new int[6];  // need 3 vertices per triangle

                roof_1_tris[0] = 0;
                roof_1_tris[1] = 2;
                roof_1_tris[2] = 1;

                // save the vertices and triangles in the mesh object
                roof_1_mesh.vertices = roof_1_verts;
                roof_1_mesh.triangles = roof_1_tris;
                roof_1_mesh.uv = roof_1_uv;  // save the uv texture coordinates

                roof_1_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s1 = new GameObject("Wall Mesh");
                s1.AddComponent<MeshFilter>();
                s1.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s1.GetComponent<MeshFilter>().mesh = roof_1_mesh;

                // change the color of the object
                Renderer rend = s1.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                Renderer renderer = s1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
                
                
                
                
                // create a mesh object
                Mesh roof_2_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_2_verts = new Vector3[3];
                // roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[0] = new Vector3(x, y, z);
                roof_2_verts[1] = new Vector3(x, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                roof_2_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);

                // create uv coordinates
                Vector2[] roof_2_uv = new Vector2[3];

                roof_2_uv[0] = new Vector2(0, 0);
                roof_2_uv[1] = new Vector2(1, 1);
                roof_2_uv[2] = new Vector2(0, 1);
                
                // two triangles for the face

                int[] roof_2_tris = new int[6];  // need 3 vertices per triangle

                roof_2_tris[0] = 0;
                roof_2_tris[1] = 1;
                roof_2_tris[2] = 2;

                // save the vertices and triangles in the mesh object
                roof_2_mesh.vertices = roof_2_verts;
                roof_2_mesh.triangles = roof_2_tris;
                roof_2_mesh.uv = roof_2_uv;  // save the uv texture coordinates

                roof_2_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s2 = new GameObject("Wall Mesh");
                s2.AddComponent<MeshFilter>();
                s2.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s2.GetComponent<MeshFilter>().mesh = roof_2_mesh;

                // change the color of the object
                rend = s2.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s2.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                

                GameObject s = new GameObject("s");
                s1.transform.parent = s.gameObject.transform;
                s2.transform.parent = s.gameObject.transform;






                
                // create roof in the 1 side
                // create a mesh object
                Mesh roof_11_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_11_verts = new Vector3[4];
                roof_11_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length + this.cell_length);
                roof_11_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z);
                roof_11_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z);
                roof_11_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                
                // create uv coordinates
                Vector2[] roof_11_uv = new Vector2[4];
                
                roof_11_uv[0] = new Vector2(0, 0);
                roof_11_uv[1] = new Vector2(1, 0);
                roof_11_uv[2] = new Vector2(1, 1);
                roof_11_uv[3] = new Vector2(0, 1);
                
                // two triangles for the face
                
                int[] roof_11_tris = new int[6];  // need 3 vertices per triangle
                
                roof_11_tris[0] = 1;
                roof_11_tris[1] = 3;
                roof_11_tris[2] = 2;
                roof_11_tris[3] = 1;
                roof_11_tris[4] = 0;
                roof_11_tris[5] = 3;
                
                // save the vertices and triangles in the mesh object
                roof_11_mesh.vertices = roof_11_verts;
                roof_11_mesh.triangles = roof_11_tris;
                roof_11_mesh.uv = roof_11_uv;  // save the uv texture coordinates
                
                roof_11_mesh.RecalculateNormals();  // automatically calculate the vertex normals
                
                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s_side_1 = new GameObject("Wall Mesh");
                s_side_1.AddComponent<MeshFilter>();
                s_side_1.AddComponent<MeshRenderer>();
                
                // associate my mesh with this object
                s_side_1.GetComponent<MeshFilter>().mesh = roof_11_mesh;
                
                // change the color of the object
                rend = s_side_1.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
                
                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s_side_1.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
            
                GameObject pivot_point_cp = new GameObject("pivot point cp");
                pivot_point_cp.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_side_1.transform.parent = pivot_point_cp.gameObject.transform;
                pivot_point_cp.transform.eulerAngles = new Vector3(0,90,0);
                
                
                
                
                
                // create roof in the 2 side
                // create a mesh object
                Mesh roof_22_mesh = new Mesh();

                // vertices of a cube
                Vector3[] roof_22_verts = new Vector3[4];
                roof_22_verts[0] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z + this.footprint_length);
                roof_22_verts[1] = new Vector3(x - 1 * this.cell_length, y - 0.5f * this.cell_length, z - this.cell_length);
                roof_22_verts[2] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + 0.5f * this.footprint_length);
                roof_22_verts[3] = new Vector3(x + 0.5f * this.footprint_length, y + 0.25f * this.footprint_length, z + this.footprint_length);
                
                // create uv coordinates
                Vector2[] roof_22_uv = new Vector2[4];
                
                roof_22_uv[0] = new Vector2(0, 0);
                roof_22_uv[1] = new Vector2(1, 0);
                roof_22_uv[2] = new Vector2(1, 1);
                roof_22_uv[3] = new Vector2(0, 1);
                
                // two triangles for the face
                
                int[] roof_22_tris = new int[6];  // need 3 vertices per triangle
                
                roof_22_tris[0] = 1;
                roof_22_tris[1] = 3;
                roof_22_tris[2] = 2;
                roof_22_tris[3] = 1;
                roof_22_tris[4] = 0;
                roof_22_tris[5] = 3;
                
                // save the vertices and triangles in the mesh object
                roof_22_mesh.vertices = roof_22_verts;
                roof_22_mesh.triangles = roof_22_tris;
                roof_22_mesh.uv = roof_22_uv;  // save the uv texture coordinates
                
                roof_22_mesh.RecalculateNormals();  // automatically calculate the vertex normals
                
                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s_side_2 = new GameObject("Wall Mesh");
                s_side_2.AddComponent<MeshFilter>();
                s_side_2.AddComponent<MeshRenderer>();
                
                // associate my mesh with this object
                s_side_2.GetComponent<MeshFilter>().mesh = roof_22_mesh;
                
                // change the color of the object
                rend = s_side_2.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
                
                // create a texture
                texture = this.make_roof_texture();
                
                // attach the texture to the mesh
                renderer = s_side_2.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
            
                pivot_point_cp = new GameObject("pivot point cp");
                pivot_point_cp.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                s_side_2.transform.parent = pivot_point_cp.gameObject.transform;
                pivot_point_cp.transform.eulerAngles = new Vector3(0,180,0);
                
                
                
                
                
                
                
                
                // rotate the whole roof
                // create pivot point
                GameObject pivot_point = new GameObject("pivot point");
                pivot_point.transform.position = new Vector3(x + 0.5f * this.footprint_length, y, z + 0.5f * this.footprint_length);
                
                s.transform.parent = pivot_point.gameObject.transform;
                s_side_1.transform.parent = pivot_point.gameObject.transform;
                s_side_2.transform.parent = pivot_point.gameObject.transform;
                
                // s_side.transform.parent = pivot_point.gameObject.transform;
                
                Debug.Log(surround[0] + "" +  surround[1] + surround[2] + surround[3]);
                if (surround[1] && surround[2])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,270,0);    
                } else if (surround[2] && surround[3])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,180,0);
                } else if (surround[3] && surround[0])
                {
                    pivot_point.transform.eulerAngles = new Vector3(0,90,0);
                }
            }
        }
        
    }



    // TODO: 1. roofed porch with cylinder pillars 2. add chimneys



    public Vector3[] createVectexes(Vector3 p, float h_step, float v_step, int direction, float mesh_length)
    {
        Vector3[] vextexes = new Vector3[4];
        // vertices for a quad
        if (direction == 0) { // W
            vextexes[0] = new Vector3 (p.x, p.y, p.z);
            vextexes[1] = new Vector3 (p.x, p.y, p.z - mesh_length * h_step);
            vextexes[2] = new Vector3 (p.x, p.y + mesh_length * v_step, p.z - mesh_length * h_step);
            vextexes[3] = new Vector3 (p.x, p.y + mesh_length * v_step, p.z);
        } else if (direction == 1) { // N
            vextexes[0] = new Vector3 (p.x, p.y, p.z);
            vextexes[1] = new Vector3 (p.x - mesh_length * h_step, p.y, p.z);
            vextexes[2] = new Vector3 (p.x - mesh_length * h_step, p.y + mesh_length * v_step, p.z);
            vextexes[3] = new Vector3 (p.x, p.y + mesh_length * v_step, p.z);
        } else if (direction == 2) { // E
            vextexes[0] = new Vector3 (p.x, p.y, p.z);
            vextexes[1] = new Vector3 (p.x, p.y, p.z + mesh_length * h_step);
            vextexes[2] = new Vector3 (p.x, p.y + mesh_length * v_step, p.z + mesh_length * h_step);
            vextexes[3] = new Vector3 (p.x, p.y + mesh_length * v_step, p.z);            
        } else if (direction == 3) { // S
            vextexes[0] = new Vector3 (p.x, p.y, p.z);
            vextexes[1] = new Vector3 (p.x + mesh_length * h_step, p.y, p.z);
            vextexes[2] = new Vector3 (p.x + mesh_length * h_step, p.y + mesh_length * v_step, p.z);
            vextexes[3] = new Vector3 (p.x, p.y + mesh_length * v_step, p.z);            
        } else if (direction == 4) { // Up
            vextexes[0] = new Vector3 (p.x, p.y, p.z);
            vextexes[1] = new Vector3 (p.x + mesh_length * h_step, p.y, p.z);
            vextexes[2] = new Vector3 (p.x + mesh_length * h_step, p.y, p.z + mesh_length * v_step);
            vextexes[3] = new Vector3 (p.x, p.y, p.z + mesh_length * v_step);            
        } else {
            vextexes[0] = new Vector3 (p.x, p.y, p.z);
            vextexes[3] = new Vector3 (p.x + mesh_length * h_step, p.y, p.z);
            vextexes[2] = new Vector3 (p.x + mesh_length * h_step, p.y, p.z + mesh_length * v_step);
            vextexes[1] = new Vector3 (p.x, p.y, p.z + mesh_length * v_step);            
        }
        return vextexes;
    }
    
    // direction: 0(W) 1(N) 2(E) 3(S) 4(Up) 5 (Down)
    // window_or_door: 0(window) 1(door) 2(all walls)
    public void createWall(int direction, float x, float y, float z, int window_or_door=0) {
        // initialize params
        int split_num = 6;
        float mesh_length = (this.cell_length / split_num);
        System.Random rand = new System.Random();
        double put_door_prob = 0.2;
        double actual_prob = rand.NextDouble();

        // generate meshes
        for (int i = 0; i < split_num; i++)
        {
            for (int j = 0; j < split_num; j++)
            {
                Vector3 left_bottom_pt = new Vector3();
                // vertices for a quad
                if (direction == 0) { // W
                    left_bottom_pt = new Vector3 (x, y + mesh_length * j, z - mesh_length * i);
                } else if (direction == 1) { // N
                    left_bottom_pt = new Vector3 (x - mesh_length * i, y + mesh_length * j, z);
                } else if (direction == 2) { // E
                    left_bottom_pt = new Vector3 (x, y + mesh_length * j, z + mesh_length * i);
                } else if (direction == 3) { // S
                    left_bottom_pt = new Vector3 (x + mesh_length * i, y + mesh_length * j, z);
                } else if (direction == 4) { // Up
                    left_bottom_pt = new Vector3 (x + mesh_length * i, y, z + mesh_length * j);
                } else {
                    left_bottom_pt = new Vector3 (x + mesh_length * i, y, z + mesh_length * j);
                }
                
                // when reach left bottom corner of window or door
                if ((i == 2) && (j == 1) && (direction != 4) && (direction != 5))
                {
                    // TODO: insert door or window here!
                    if (window_or_door == 0) // window
                    {
                        Vector3[] vertices = this.createVectexes(left_bottom_pt, 2, 4, direction, mesh_length);
                        this.createWindow(vertices[0], vertices[1], vertices[2], vertices[3], direction, mesh_length);
                    }
                    else if (window_or_door == 1)  // door
                    {
                        if (actual_prob < put_door_prob)
                        {
                            Vector3[] vertices = this.createVectexes(left_bottom_pt, 2, 4, direction, mesh_length);
                            this.createDoor(vertices[0], vertices[1], vertices[2], vertices[3], direction, mesh_length);   
                        }
                    }
                }
                if (((i > 1) && (i < 4)) && ((j > 0) && (j < 5)) && (direction != 4) && (direction != 5)) {
                    if (window_or_door == 1)
                    {
                        if (actual_prob < put_door_prob)
                        {
                            continue;        
                        }
                    }
                    else if (window_or_door == 0)
                    {
                        continue;
                    }
                }
                
                // create a mesh object
                Mesh wall_mesh = new Mesh();

                // vertices of a cube
                Vector3[] wall_verts = new Vector3[4];
                Vector3[] vextexes = this.createVectexes(left_bottom_pt, 1, 1, direction, mesh_length);
                wall_verts[0] = left_bottom_pt;
                wall_verts[1] = vextexes[1];
                wall_verts[2] = vextexes[2];
                wall_verts[3] = vextexes[3];

                // create uv coordinates
                Vector2[] wall_uv = new Vector2[4];

                wall_uv[0] = new Vector2(0, 0);
                wall_uv[1] = new Vector2(1, 0);
                wall_uv[2] = new Vector2(1, 1);
                wall_uv[3] = new Vector2(0, 1);

                // two triangles for the face

                int[] wall_tris = new int[6];  // need 3 vertices per triangle

                wall_tris[0] = 1;
                wall_tris[1] = 3;
                wall_tris[2] = 2;
                wall_tris[3] = 1;
                wall_tris[4] = 0;
                wall_tris[5] = 3;

                // save the vertices and triangles in the mesh object
                wall_mesh.vertices = wall_verts;
                wall_mesh.triangles = wall_tris;
                wall_mesh.uv = wall_uv;  // save the uv texture coordinates

                wall_mesh.RecalculateNormals();  // automatically calculate the vertex normals

                // create a new GameObject and give it a MeshFilter and a MeshRenderer
                GameObject s = new GameObject("Wall Mesh");
                s.AddComponent<MeshFilter>();
                s.AddComponent<MeshRenderer>();

                // associate my mesh with this object
                s.GetComponent<MeshFilter>().mesh = wall_mesh;

                // change the color of the object
                Renderer rend = s.GetComponent<Renderer>();
                rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

                // create a texture
                Texture2D texture = this.make_wall_texture();

                // attach the texture to the mesh
                Renderer renderer = s.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
                
            }
        }
    }
    

    // position of four corner points (1->left-bottom, 2->right-bottom, 3->right-top, 4->left-top)
    public void createWindow(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int direction, float mesh_length) {
        // Create Panes
        System.Random rand = new System.Random();

        if (rand.NextDouble() < 0.3)
        {
            // create a mesh object
            Mesh mesh_win1 = new Mesh();

            // vertices of a cube
            Vector3[] verts_win1 = new Vector3[4];

            // vertices for a quad

            verts_win1[0] = this.createVectexes(p1, 2, 1, direction, mesh_length)[3];
            verts_win1[1] = this.createVectexes(p1, 2, 1, direction, mesh_length)[2];
            verts_win1[2] = p3;
            verts_win1[3] = p4;

            // create uv coordinates

            Vector2[] uv_win1 = new Vector2[4];

            uv_win1[0] = new Vector2(1, 0);
            uv_win1[1] = new Vector2(1, 1);
            uv_win1[2] = new Vector2(0, 1);
            uv_win1[3] = new Vector2(0, 0);

            // two triangles for the face

            int[] tris_win1 = new int[6];  // need 3 vertices per triangle

            tris_win1[0] = 0;
            tris_win1[1] = 2;
            tris_win1[2] = 1;
            tris_win1[3] = 0;
            tris_win1[4] = 3;
            tris_win1[5] = 2;

            // save the vertices and triangles in the mesh object
            mesh_win1.vertices = verts_win1;
            mesh_win1.triangles = tris_win1;
            mesh_win1.uv = uv_win1;  // save the uv texture coordinates

            mesh_win1.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s = new GameObject("Window_1 Mesh");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
        
            // associate my mesh with this object
            s.GetComponent<MeshFilter>().mesh = mesh_win1;
        
            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        
            // create a texture
            Texture2D texture = this.make_window_texture1();

            // attach the texture to the mesh
            Renderer renderer = s.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;   

        }
        else
        {
            // create a mesh object
            Mesh mesh_room = new Mesh();

            // vertices of a cube
            Vector3[] verts_room = new Vector3[4];

            // vertices for a quad

            verts_room[0] = this.createVectexes(p1, 2, 1, direction, mesh_length)[3];
            verts_room[1] = this.createVectexes(p1, 2, 1, direction, mesh_length)[2];
            verts_room[2] = p3;
            verts_room[3] = p4;

            // create uv coordinates

            Vector2[] uv_room = new Vector2[4];

            uv_room[0] = new Vector2(1, 0);
            uv_room[1] = new Vector2(1, 1);
            uv_room[2] = new Vector2(0, 1);
            uv_room[3] = new Vector2(0, 0);

            // two triangles for the face

            int[] tris_room = new int[6];  // need 3 vertices per triangle

            tris_room[0] = 0;
            tris_room[1] = 2;
            tris_room[2] = 1;
            tris_room[3] = 0;
            tris_room[4] = 3;
            tris_room[5] = 2;

            // save the vertices and triangles in the mesh object
            mesh_room.vertices = verts_room;
            mesh_room.triangles = tris_room;
            mesh_room.uv = uv_room;  // save the uv texture coordinates

            mesh_room.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s = new GameObject("Room Mesh");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
        
            // associate my mesh with this object
            s.GetComponent<MeshFilter>().mesh = mesh_room;
        
            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        
            // create a texture
            Texture2D texture = this.make_room_texture();

            // attach the texture to the mesh
            Renderer renderer = s.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;   

            
            // create window
            // basic params
            float theta1 = (float) rand.NextDouble() * 90f;
            float theta2 = (float) rand.NextDouble() * 90f;
            float r = mesh_length / 2f;
            float thickness = mesh_length / 20f;
            
            // texture of borders
            Texture2D texture_win2 = this.make_window_texture2(); 
            Material material = new Material(Shader.Find("Diffuse"));

            // create a pane 1
            GameObject s1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s1.name = "window 01";  // give name
            // modify the size and position
            Vector3 pos = this.createVectexes(p1, 0.5f, 2.5f, direction, mesh_length)[2];
            s1.transform.position = new Vector3(pos.x, pos.y, pos.z);
            
            if (direction == 0) // W
            {
                s1.transform.localScale = new Vector3 (thickness, 3 * mesh_length, mesh_length);
            } else if (direction == 1) // N
            {
                s1.transform.localScale = new Vector3 (mesh_length, 3 * mesh_length, thickness);
            } else if (direction == 2) // E
            {
                s1.transform.localScale = new Vector3 (thickness, 3 * mesh_length, mesh_length);
            } else  // S
            {
                s1.transform.localScale = new Vector3 (mesh_length, 3 * mesh_length, thickness);
            }
            
            // create pivot point
            GameObject pivot_point = new GameObject("pivot point");
            pivot_point.transform.position = this.createVectexes(p1, 0.5f, 2.5f, direction, mesh_length)[3];
            s1.transform.parent = pivot_point.gameObject.transform;
            pivot_point.transform.eulerAngles = new Vector3(0,theta1,0);

            rend = s1.GetComponent<Renderer>();
            material.mainTexture = texture_win2;
            rend.material = material;           
            
            
            
            
            // create a pane 2
            GameObject s2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s2.name = "window 02";  // give name
            // modify the size and position
            pos = this.createVectexes(p1, 1.5f, 2.5f, direction, mesh_length)[2];
            s2.transform.position = new Vector3(pos.x, pos.y, pos.z);
            if (direction == 0) // W
            {
                s2.transform.localScale = new Vector3 (thickness, 3 * mesh_length, mesh_length);
            } else if (direction == 1) // N
            {
                s2.transform.localScale = new Vector3 (mesh_length, 3 * mesh_length, thickness);
            } else if (direction == 2) // E
            {
                s2.transform.localScale = new Vector3 (thickness, 3 * mesh_length, mesh_length);
            } else  // S
            {
                s2.transform.localScale = new Vector3 (mesh_length, 3 * mesh_length, thickness);
            }
            // create pivot point
            pivot_point = new GameObject("pivot point");
            pivot_point.transform.position = this.createVectexes(p1, 2f, 2.5f, direction, mesh_length)[2];
            s2.transform.parent = pivot_point.gameObject.transform;
            pivot_point.transform.eulerAngles = new Vector3(0,-theta2,0);

            
            
            rend = s2.GetComponent<Renderer>();
            material.mainTexture = texture_win2;
            rend.material = material;           
        }

        
        // Create Sill
        
        // basic data
        float sill_length1 = mesh_length * 0.5f;
        float sill_length2 = mesh_length * 0.25f;

        // all the vertices of the sill
        Vector3[] sill_verts = new Vector3[8];
        sill_verts[0] = p1;
        sill_verts[1] = p2;
        sill_verts[2] = this.createVectexes(p1, 2, 1, direction, mesh_length)[2];
        sill_verts[3] = this.createVectexes(p1, 1, 1, direction, mesh_length)[3];
        
        if (direction == 0) // W
        {
            sill_verts[4] = new Vector3(sill_verts[0].x - sill_length1, sill_verts[0].y + (mesh_length - sill_length2), sill_verts[0].z);
            sill_verts[5] = new Vector3(sill_verts[1].x - sill_length1, sill_verts[1].y + (mesh_length - sill_length2), sill_verts[1].z);
            sill_verts[6] = new Vector3(sill_verts[2].x - sill_length1, sill_verts[2].y, sill_verts[2].z);
            sill_verts[7] = new Vector3(sill_verts[3].x - sill_length1, sill_verts[3].y, sill_verts[3].z);
        } 
        else if (direction == 1) // N
        {
            sill_verts[4] = new Vector3(sill_verts[0].x, sill_verts[0].y + (mesh_length - sill_length2), sill_verts[0].z + sill_length1);
            sill_verts[5] = new Vector3(sill_verts[1].x, sill_verts[1].y + (mesh_length - sill_length2), sill_verts[1].z + sill_length1);
            sill_verts[6] = new Vector3(sill_verts[2].x, sill_verts[2].y, sill_verts[2].z + sill_length1);
            sill_verts[7] = new Vector3(sill_verts[3].x, sill_verts[3].y, sill_verts[3].z + sill_length1);
        } 
        else if (direction == 2) // E
        {
            sill_verts[4] = new Vector3(sill_verts[0].x + sill_length1, sill_verts[0].y + (mesh_length - sill_length2), sill_verts[0].z);
            sill_verts[5] = new Vector3(sill_verts[1].x + sill_length1, sill_verts[1].y + (mesh_length - sill_length2), sill_verts[1].z);
            sill_verts[6] = new Vector3(sill_verts[2].x + sill_length1, sill_verts[2].y, sill_verts[2].z);
            sill_verts[7] = new Vector3(sill_verts[3].x + sill_length1, sill_verts[3].y, sill_verts[3].z);
        }
        else // S
        {
            sill_verts[4] = new Vector3(sill_verts[0].x, sill_verts[0].y + (mesh_length - sill_length2), sill_verts[0].z - sill_length1);
            sill_verts[5] = new Vector3(sill_verts[1].x, sill_verts[1].y + (mesh_length - sill_length2), sill_verts[1].z - sill_length1);
            sill_verts[6] = new Vector3(sill_verts[2].x, sill_verts[2].y, sill_verts[2].z - sill_length1);
            sill_verts[7] = new Vector3(sill_verts[3].x, sill_verts[3].y, sill_verts[3].z - sill_length1);
        }

        // create uv coordinates
        Vector2[] sill_uv = new Vector2[4];

        sill_uv[0] = new Vector2(0, 0);
        sill_uv[1] = new Vector2(1, 0);
        sill_uv[2] = new Vector2(1, 1);
        sill_uv[3] = new Vector2(0, 1);

        
        // create a sill mesh
        Mesh sill_mesh1 = new Mesh();
        Mesh sill_mesh2 = new Mesh();
        Mesh sill_mesh3 = new Mesh();
        Mesh sill_mesh4 = new Mesh();
        Mesh sill_mesh5 = new Mesh();
        Mesh sill_mesh6 = new Mesh();
        
        // create sill verts
        Vector3[] sill_verts1 = new Vector3[4];
        sill_verts1[0] = sill_verts[1];
        sill_verts1[1] = sill_verts[0];
        sill_verts1[2] = sill_verts[3];
        sill_verts1[3] = sill_verts[2];
        
        Vector3[] sill_verts2 = new Vector3[4];
        sill_verts2[0] = sill_verts[4];
        sill_verts2[1] = sill_verts[5];
        sill_verts2[2] = sill_verts[6];
        sill_verts2[3] = sill_verts[7];
        
        Vector3[] sill_verts3 = new Vector3[4];
        sill_verts3[0] = sill_verts[7];
        sill_verts3[1] = sill_verts[6];
        sill_verts3[2] = sill_verts[2];
        sill_verts3[3] = sill_verts[3];
        
        Vector3[] sill_verts4 = new Vector3[4];
        sill_verts4[0] = sill_verts[0];
        sill_verts4[1] = sill_verts[1];
        sill_verts4[2] = sill_verts[5];
        sill_verts4[3] = sill_verts[4];
        
        Vector3[] sill_verts5 = new Vector3[4];
        sill_verts5[0] = sill_verts[0];
        sill_verts5[1] = sill_verts[4];
        sill_verts5[2] = sill_verts[7];
        sill_verts5[3] = sill_verts[3];
        
        Vector3[] sill_verts6 = new Vector3[4];
        sill_verts6[0] = sill_verts[5];
        sill_verts6[1] = sill_verts[1];
        sill_verts6[2] = sill_verts[2];
        sill_verts6[3] = sill_verts[6];
        // two triangles for the face

        int[] sill_tris1 = new int[6];
        sill_tris1[0] = 1;
        sill_tris1[1] = 3;
        sill_tris1[2] = 2;
        sill_tris1[3] = 1;
        sill_tris1[4] = 0;
        sill_tris1[5] = 3;
        
        int[] sill_tris2 = new int[6]; 
        sill_tris2[0] = 1;
        sill_tris2[1] = 3;
        sill_tris2[2] = 2;
        sill_tris2[3] = 1;
        sill_tris2[4] = 0;
        sill_tris2[5] = 3;
        
        int[] sill_tris3 = new int[6]; 
        sill_tris3[0] = 1;
        sill_tris3[1] = 3;
        sill_tris3[2] = 2;
        sill_tris3[3] = 1;
        sill_tris3[4] = 0;
        sill_tris3[5] = 3;
        
        int[] sill_tris4 = new int[6]; 
        sill_tris4[0] = 1;
        sill_tris4[1] = 3;
        sill_tris4[2] = 2;
        sill_tris4[3] = 1;
        sill_tris4[4] = 0;
        sill_tris4[5] = 3;
        
        int[] sill_tris5 = new int[6]; 
        sill_tris5[0] = 1;
        sill_tris5[1] = 3;
        sill_tris5[2] = 2;
        sill_tris5[3] = 1;
        sill_tris5[4] = 0;
        sill_tris5[5] = 3;
        
        int[] sill_tris6 = new int[6]; 
        sill_tris6[0] = 1;
        sill_tris6[1] = 3;
        sill_tris6[2] = 2;
        sill_tris6[3] = 1;
        sill_tris6[4] = 0;
        sill_tris6[5] = 3;
        
        // save the vertices and triangles in the mesh object
        sill_mesh1.vertices = sill_verts1;
        sill_mesh1.triangles = sill_tris1;
        sill_mesh1.uv = sill_uv;  // save the uv texture coordinates
        sill_mesh1.RecalculateNormals();  // automatically calculate the vertex normals

        sill_mesh2.vertices = sill_verts2;
        sill_mesh2.triangles = sill_tris2;
        sill_mesh2.uv = sill_uv;  // save the uv texture coordinates
        sill_mesh2.RecalculateNormals();  // automatically calculate the vertex normals
        
        sill_mesh3.vertices = sill_verts3;
        sill_mesh3.triangles = sill_tris3;
        sill_mesh3.uv = sill_uv;  // save the uv texture coordinates
        sill_mesh3.RecalculateNormals();  // automatically calculate the vertex normals
        
        sill_mesh4.vertices = sill_verts4;
        sill_mesh4.triangles = sill_tris4;
        sill_mesh4.uv = sill_uv;  // save the uv texture coordinates
        sill_mesh4.RecalculateNormals();  // automatically calculate the vertex normals
        
        sill_mesh5.vertices = sill_verts5;
        sill_mesh5.triangles = sill_tris5;
        sill_mesh5.uv = sill_uv;  // save the uv texture coordinates
        sill_mesh5.RecalculateNormals();  // automatically calculate the vertex normals
        
        sill_mesh6.vertices = sill_verts6;
        sill_mesh6.triangles = sill_tris6;
        sill_mesh6.uv = sill_uv;  // save the uv texture coordinates
        sill_mesh6.RecalculateNormals();  // automatically calculate the vertex normals

        List<Mesh> sill_meshes = new List<Mesh>()
            {sill_mesh1, sill_mesh2, sill_mesh3, sill_mesh4, sill_mesh5, sill_mesh6};
        
        // create gameobj
        foreach (var sill_mesh in sill_meshes)
        {
            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s = new GameObject("Window Mesh");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();

            // associate my mesh with this object
            s.GetComponent<MeshFilter>().mesh = sill_mesh;

            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

            // create a texture
            Texture2D texture = this.make_sill_texture();

            // attach the texture to the mesh
            Renderer renderer = s.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;   

        }

    }

    public void createDoor(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int direction, float mesh_length)
    { 
        // Create Panes
        System.Random rand = new System.Random();

        if (rand.NextDouble() < 0.5)
        {
            // create a mesh object
            Mesh mesh_door1 = new Mesh();

            // vertices of a cube
            Vector3[] verts_door1 = new Vector3[4];

            // vertices for a quad

            verts_door1[0] = p1;
            verts_door1[1] = p2;
            verts_door1[2] = p3;
            verts_door1[3] = p4;

            // create uv coordinates

            Vector2[] uv_door1 = new Vector2[4];

            uv_door1[0] = new Vector2(1, 0);
            uv_door1[1] = new Vector2(1, 1);
            uv_door1[2] = new Vector2(0, 1);
            uv_door1[3] = new Vector2(0, 0);

            // two triangles for the face

            int[] tris_door1 = new int[6];  // need 3 vertices per triangle

            tris_door1[0] = 0;
            tris_door1[1] = 2;
            tris_door1[2] = 1;
            tris_door1[3] = 0;
            tris_door1[4] = 3;
            tris_door1[5] = 2;

            // save the vertices and triangles in the mesh object
            mesh_door1.vertices = verts_door1;
            mesh_door1.triangles = tris_door1;
            mesh_door1.uv = uv_door1;  // save the uv texture coordinates

            mesh_door1.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s = new GameObject("Window_1 Mesh");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
        
            // associate my mesh with this object
            s.GetComponent<MeshFilter>().mesh = mesh_door1;
        
            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        
            // create a texture
            Texture2D texture = this.make_door_texture1();

            // attach the texture to the mesh
            Renderer renderer = s.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;   

        }
        else
        {
            // create a mesh object
            Mesh mesh_room = new Mesh();

            // vertices of a cube
            Vector3[] verts_room = new Vector3[4];

            // vertices for a quad

            verts_room[0] = p1;
            verts_room[1] = p2;
            verts_room[2] = p3;
            verts_room[3] = p4;

            // create uv coordinates

            Vector2[] uv_room = new Vector2[4];

            uv_room[0] = new Vector2(1, 0);
            uv_room[1] = new Vector2(1, 1);
            uv_room[2] = new Vector2(0, 1);
            uv_room[3] = new Vector2(0, 0);

            // two triangles for the face

            int[] tris_room = new int[6];  // need 3 vertices per triangle

            tris_room[0] = 0;
            tris_room[1] = 2;
            tris_room[2] = 1;
            tris_room[3] = 0;
            tris_room[4] = 3;
            tris_room[5] = 2;

            // save the vertices and triangles in the mesh object
            mesh_room.vertices = verts_room;
            mesh_room.triangles = tris_room;
            mesh_room.uv = uv_room;  // save the uv texture coordinates

            mesh_room.RecalculateNormals();  // automatically calculate the vertex normals

            // create a new GameObject and give it a MeshFilter and a MeshRenderer
            GameObject s = new GameObject("Room Mesh");
            s.AddComponent<MeshFilter>();
            s.AddComponent<MeshRenderer>();
        
            // associate my mesh with this object
            s.GetComponent<MeshFilter>().mesh = mesh_room;
        
            // change the color of the object
            Renderer rend = s.GetComponent<Renderer>();
            rend.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        
            // create a texture
            Texture2D texture = this.make_settingroom_texture();

            // attach the texture to the mesh
            Renderer renderer = s.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;   

            
            // create window
            // basic params
            float theta1 = (float) rand.NextDouble() * 30f;
            float theta2 = (float) rand.NextDouble() * 30f;
            float r = mesh_length / 2f;
            float thickness = mesh_length / 20f;
            
            // texture of borders
            Texture2D texture_door2 = this.make_door_texture2(); 
            Material material = new Material(Shader.Find("Diffuse"));

            // create a pane 1
            GameObject s1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s1.name = "window 01";  // give name
            // modify the size and position
            Vector3 pos = this.createVectexes(p1, 0.5f, 2f, direction, mesh_length)[2];
            s1.transform.position = new Vector3(pos.x, pos.y, pos.z);
            if (direction == 0) // W
            {
                s1.transform.localScale = new Vector3 (thickness, 4 * mesh_length, mesh_length);
            } else if (direction == 1) // N
            {
                s1.transform.localScale = new Vector3 (mesh_length, 4 * mesh_length, thickness);
            } else if (direction == 2) // E
            {
                s1.transform.localScale = new Vector3 (thickness, 4 * mesh_length, mesh_length);
            } else  // S
            {
                s1.transform.localScale = new Vector3 (mesh_length, 4 * mesh_length, thickness);
            }
            
            // create pivot point
            GameObject pivot_point = new GameObject("pivot point");
            pivot_point.transform.position = this.createVectexes(p1, 0.5f, 2f, direction, mesh_length)[3];
            s1.transform.parent = pivot_point.gameObject.transform;
            pivot_point.transform.eulerAngles = new Vector3(0,theta1,0);


            
            


            rend = s1.GetComponent<Renderer>();
            material.mainTexture = texture_door2;
            rend.material = material;           
            
            // create a pane 2
            GameObject s2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s2.name = "window 02";  // give name
            // modify the size and position
            pos = this.createVectexes(p1, 1.5f, 2f, direction, mesh_length)[2];
            s2.transform.position = new Vector3(pos.x, pos.y, pos.z);
            if (direction == 0) // W
            {
                s2.transform.localScale = new Vector3 (thickness, 4 * mesh_length, mesh_length);
            } else if (direction == 1) // N
            {
                s2.transform.localScale = new Vector3 (mesh_length, 4 * mesh_length, thickness);
            } else if (direction == 2) // E
            {
                s2.transform.localScale = new Vector3 (thickness, 4 * mesh_length, mesh_length);
            } else  // S
            {
                s2.transform.localScale = new Vector3 (mesh_length, 4 * mesh_length, thickness);
            }
            
            
            // create pivot point
            pivot_point = new GameObject("pivot point");
            pivot_point.transform.position = this.createVectexes(p1, 2f, 2f, direction, mesh_length)[2];
            s2.transform.parent = pivot_point.gameObject.transform;
            pivot_point.transform.eulerAngles = new Vector3(0,-theta2,0);
            
            
            rend = s2.GetComponent<Renderer>();
            material.mainTexture = texture_door2;
            rend.material = material;           
        }
        
        
        // Create Steps
        float porch_length = 10f;
        float step_length = mesh_length / 2f;
        GameObject step1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        step1.name = "step 01";  // give name
        GameObject step2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        step2.name = "step 02";  // give name
        // modify the size and position
        Vector3 pos_step = this.createVectexes(p1, 1f, -0.5f, direction, mesh_length)[2];
        if (direction == 0) // W
        {
            step1.transform.position = new Vector3(pos_step.x - ((step_length * porch_length) / 2), pos_step.y, pos_step.z);
            step1.transform.localScale = new Vector3 (step_length * porch_length, mesh_length, 2 * mesh_length);
            step2.transform.position = new Vector3(pos_step.x - (step_length * porch_length) - (step_length / 2), pos_step.y - (mesh_length / 4), pos_step.z);
            step2.transform.localScale = new Vector3 (step_length, mesh_length / 2, 2 * mesh_length);
        } else if (direction == 1) // N
        {
            step1.transform.position = new Vector3(pos_step.x, pos_step.y, pos_step.z + ((step_length * porch_length) / 2));
            step1.transform.localScale = new Vector3 (2 * mesh_length, mesh_length, (step_length * porch_length));
            step2.transform.position = new Vector3(pos_step.x, pos_step.y - (mesh_length / 4), pos_step.z + (step_length * porch_length) + (step_length / 2));
            step2.transform.localScale = new Vector3 (2 * mesh_length, mesh_length / 2, step_length);
        } else if (direction == 2) // E
        {
            step1.transform.position = new Vector3(pos_step.x + ((step_length * porch_length) / 2), pos_step.y, pos_step.z);
            step1.transform.localScale = new Vector3 ((step_length * porch_length), mesh_length, 2 * mesh_length);
            step2.transform.position = new Vector3(pos_step.x + (step_length * porch_length) + (step_length / 2), pos_step.y - (mesh_length / 4), pos_step.z);
            step2.transform.localScale = new Vector3 (step_length, mesh_length / 2, 2 * mesh_length);
        } else  // S
        {
            step1.transform.position = new Vector3(pos_step.x, pos_step.y, pos_step.z - ((step_length * porch_length) / 2));
            step1.transform.localScale = new Vector3 (2 * mesh_length, mesh_length, (step_length * porch_length));
            step2.transform.position = new Vector3(pos_step.x, pos_step.y - (mesh_length / 4), pos_step.z - (step_length * porch_length) - (step_length / 2));
            step2.transform.localScale = new Vector3 (2 * mesh_length, mesh_length / 2, step_length);
        }
            
            
        Material material_step = new Material(Shader.Find("Diffuse"));
        
        Renderer rend1 = step1.GetComponent<Renderer>();
        material_step.color = new Color(0.79f, 0.79f, 0.79f, 1f);
        rend1.material = material_step;
        
        Renderer rend2 = step2.GetComponent<Renderer>();
        material_step.color = new Color(0.79f, 0.79f, 0.79f, 1f);
        rend2.material = material_step;

        // create pillar
        float pillar_radius = mesh_length / 2f;
        float pillar_height = mesh_length * 4.4f;
        float pillar_step_height = mesh_length / 2f;
        GameObject pillar1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar1.name = "pillar 01";  // give name
        GameObject pillar2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar2.name = "pillar 02";  // give name
        
        pillar1.transform.localScale = new Vector3(pillar_radius * 2, pillar_height / 2f, pillar_radius * 2);
        pillar2.transform.localScale = new Vector3(pillar_radius * 2, pillar_height / 2f, pillar_radius * 2);
        
        GameObject pillar_step1_down = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pillar_step1_down.name = "pillar_step1_down";  // give name
        GameObject pillar_step1_up = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pillar_step1_up.name = "pillar_step1_up";  // give name
        GameObject pillar_step2_down = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pillar_step2_down.name = "pillar_step2_down";  // give name
        GameObject pillar_step2_up = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pillar_step2_up.name = "pillar_step2_up";  // give name

        pillar_step1_down.transform.localScale = new Vector3(pillar_radius * 2, pillar_step_height, pillar_radius * 2);
        pillar_step1_up.transform.localScale = new Vector3(pillar_radius * 2, pillar_step_height, pillar_radius * 2);
        pillar_step2_down.transform.localScale = new Vector3(pillar_radius * 2, pillar_step_height, pillar_radius * 2);
        pillar_step2_up.transform.localScale = new Vector3(pillar_radius * 2, pillar_step_height, pillar_radius * 2);

        pillar_step1_down.transform.eulerAngles = new Vector3(0,45,0);
        pillar_step1_up.transform.eulerAngles = new Vector3(0,45,0);
        pillar_step2_down.transform.eulerAngles = new Vector3(0,45,0);
        pillar_step2_up.transform.eulerAngles = new Vector3(0,45,0);

        Vector2 pillar_pos1 = new Vector2();
        Vector2 pillar_pos2 = new Vector2();
        
        // create porch ceiling
        float porch_ceiling_height = mesh_length / 2f;
        GameObject porch_ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        porch_ceiling.name = "porch_ceiling";  // give name


        float ceiling_length = (step_length * porch_length) + mesh_length + 1.5f * pillar_radius;
        float ceiling_width = 2f * mesh_length + 6f * pillar_radius;
        float ceiling_ele = pillar_step_height * 2 + pillar_height + porch_ceiling_height / 2f;

        float roof_height = mesh_length * (2f / 3f);
        float outer_length = mesh_length / 4;
        
        Vector3 porch_ceiling_up_out = new Vector3();
        Vector3 porch_ceiling_up_in = new Vector3();
        Vector3 porch_ceiling_mid_out = new Vector3();
        Vector3 porch_ceiling_mid_in = new Vector3();
        Vector3 porch_ceiling_down_out = new Vector3();
        Vector3 porch_ceiling_down_in = new Vector3();
        if (direction == 0) // W
        {
            pillar_pos1 = new Vector2(pos_step.x - (step_length * porch_length) - mesh_length, pos_step.z + mesh_length + 1.5f * pillar_radius);
            pillar_pos2 = new Vector2(pos_step.x - (step_length * porch_length) - mesh_length, pos_step.z - mesh_length - 1.5f * pillar_radius);
            porch_ceiling.transform.localScale = new Vector3(ceiling_length, porch_ceiling_height, ceiling_width);
            porch_ceiling.transform.position = new Vector3(pos_step.x - (ceiling_length / 2f), ceiling_ele, pos_step.z);
            
            porch_ceiling_up_out = new Vector3(pos_step.x - (ceiling_length + outer_length), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z - (0.25f * mesh_length + ceiling_width / 2f));
            porch_ceiling_up_in = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z - (0.25f * mesh_length + ceiling_width / 2f));
            porch_ceiling_mid_out = new Vector3(pos_step.x - (ceiling_length + outer_length), ceiling_ele + porch_ceiling_height / 2f + roof_height, pos_step.z);
            porch_ceiling_mid_in = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f + roof_height, pos_step.z);
            porch_ceiling_down_out = new Vector3(pos_step.x - (ceiling_length + outer_length), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z + (0.25f * mesh_length + ceiling_width / 2f));
            porch_ceiling_down_in = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z + (0.25f * mesh_length + ceiling_width / 2f));
        } else if (direction == 1) // N
        {
            pillar_pos1 = new Vector2(pos_step.x + mesh_length + 1.5f * pillar_radius, pos_step.z + (step_length * porch_length) + mesh_length);
            pillar_pos2 = new Vector2(pos_step.x - mesh_length - 1.5f * pillar_radius, pos_step.z + (step_length * porch_length) + mesh_length);
            porch_ceiling.transform.localScale = new Vector3(ceiling_width, porch_ceiling_height, ceiling_length);
            porch_ceiling.transform.position = new Vector3(pos_step.x, ceiling_ele, pos_step.z + (ceiling_length / 2f));
            
            porch_ceiling_up_out = new Vector3(pos_step.x - (0.25f * mesh_length + ceiling_width / 2f), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z + (ceiling_length + outer_length));
            porch_ceiling_up_in = new Vector3(pos_step.x - (0.25f * mesh_length + ceiling_width / 2f), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z);
            porch_ceiling_mid_out = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f + roof_height, pos_step.z + (ceiling_length + outer_length));
            porch_ceiling_mid_in = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f + roof_height, pos_step.z);
            porch_ceiling_down_out = new Vector3(pos_step.x + (0.25f * mesh_length + ceiling_width / 2f), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z + (ceiling_length + outer_length));
            porch_ceiling_down_in = new Vector3(pos_step.x + (0.25f * mesh_length + ceiling_width / 2f), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z);
        } else if (direction == 2) // E
        {
            pillar_pos1 = new Vector2(pos_step.x + (step_length * porch_length) + mesh_length, pos_step.z + mesh_length + 1.5f * pillar_radius);
            pillar_pos2 = new Vector2(pos_step.x + (step_length * porch_length) + mesh_length, pos_step.z - mesh_length - 1.5f * pillar_radius);
            porch_ceiling.transform.localScale = new Vector3(ceiling_length, porch_ceiling_height, ceiling_width);
            porch_ceiling.transform.position = new Vector3(pos_step.x + (ceiling_length / 2f), ceiling_ele, pos_step.z);
            
            porch_ceiling_up_out = new Vector3(pos_step.x + (ceiling_length + outer_length), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z + (0.25f * mesh_length + ceiling_width / 2f));
            porch_ceiling_up_in = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z + (0.25f * mesh_length + ceiling_width / 2f));
            porch_ceiling_mid_out = new Vector3(pos_step.x + (ceiling_length + outer_length), ceiling_ele + porch_ceiling_height / 2f + roof_height, pos_step.z);
            porch_ceiling_mid_in = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f + roof_height, pos_step.z);
            porch_ceiling_down_out = new Vector3(pos_step.x + (ceiling_length + outer_length), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z - (0.25f * mesh_length + ceiling_width / 2f));
            porch_ceiling_down_in = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z - (0.25f * mesh_length + ceiling_width / 2f));
        } else  // S
        {
            pillar_pos1 = new Vector2(pos_step.x + mesh_length + 1.5f * pillar_radius, pos_step.z - (step_length * porch_length) - mesh_length);
            pillar_pos2 = new Vector2(pos_step.x - mesh_length - 1.5f * pillar_radius, pos_step.z - (step_length * porch_length) - mesh_length);
            porch_ceiling.transform.localScale = new Vector3(ceiling_width, porch_ceiling_height, ceiling_length);
            porch_ceiling.transform.position = new Vector3(pos_step.x, ceiling_ele, pos_step.z - (ceiling_length / 2f));
            
            porch_ceiling_up_out = new Vector3(pos_step.x + (0.25f * mesh_length + ceiling_width / 2f), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z - (ceiling_length + outer_length));
            porch_ceiling_up_in = new Vector3(pos_step.x + (0.25f * mesh_length + ceiling_width / 2f), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z);
            porch_ceiling_mid_out = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f + roof_height, pos_step.z - (ceiling_length + outer_length));
            porch_ceiling_mid_in = new Vector3(pos_step.x, ceiling_ele + porch_ceiling_height / 2f + roof_height, pos_step.z);
            porch_ceiling_down_out = new Vector3(pos_step.x - (0.25f * mesh_length + ceiling_width / 2f), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z - (ceiling_length + outer_length));
            porch_ceiling_down_in = new Vector3(pos_step.x - (0.25f * mesh_length + ceiling_width / 2f), ceiling_ele + porch_ceiling_height / 2f - (1f / 16f) * mesh_length, pos_step.z);
        }
        pillar_step1_down.transform.position = new Vector3(pillar_pos1.x, pillar_step_height / 2f, pillar_pos1.y);
        pillar_step2_down.transform.position = new Vector3(pillar_pos2.x, pillar_step_height / 2f, pillar_pos2.y);
            
        pillar1.transform.position = new Vector3(pillar_pos1.x, pillar_step_height + (pillar_height / 2f), pillar_pos1.y);
        pillar2.transform.position = new Vector3(pillar_pos2.x, pillar_step_height + (pillar_height / 2f), pillar_pos2.y);
            
        pillar_step1_up.transform.position = new Vector3(pillar_pos1.x, pillar_step_height + pillar_height + (pillar_step_height / 2f), pillar_pos1.y);
        pillar_step2_up.transform.position = new Vector3(pillar_pos2.x, pillar_step_height + pillar_height + (pillar_step_height / 2f), pillar_pos2.y);
        
        // create roof
        
        // create a mesh object
        Mesh roof_mesh = new Mesh();

        // vertices of a cube
        Vector3[] roof_verts = new Vector3[4];
        roof_verts[0] = porch_ceiling_up_in;
        roof_verts[1] = porch_ceiling_up_out;
        roof_verts[2] = porch_ceiling_mid_out;
        roof_verts[3] = porch_ceiling_mid_in;

        // create uv coordinates
        Vector2[] roof_uv = new Vector2[4];

        roof_uv[0] = new Vector2(0, 0);
        roof_uv[1] = new Vector2(1, 0);
        roof_uv[2] = new Vector2(1, 1);
        roof_uv[3] = new Vector2(0, 1);

        // two triangles for the face

        int[] roof_tris = new int[6];  // need 3 vertices per triangle

        roof_tris[0] = 1;
        roof_tris[1] = 3;
        roof_tris[2] = 2;
        roof_tris[3] = 1;
        roof_tris[4] = 0;
        roof_tris[5] = 3;

        // save the vertices and triangles in the mesh object
        roof_mesh.vertices = roof_verts;
        roof_mesh.triangles = roof_tris;
        roof_mesh.uv = roof_uv;  // save the uv texture coordinates

        roof_mesh.RecalculateNormals();  // automatically calculate the vertex normals

        // create a new GameObject and give it a MeshFilter and a MeshRenderer
        GameObject s_roof1 = new GameObject("Wall Mesh");
        s_roof1.AddComponent<MeshFilter>();
        s_roof1.AddComponent<MeshRenderer>();

        // associate my mesh with this object
        s_roof1.GetComponent<MeshFilter>().mesh = roof_mesh;

        // change the color of the object
        Renderer rend1_roof = s_roof1.GetComponent<Renderer>();
        rend1_roof.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

        // create a texture
        Texture2D texture1_roof = this.make_roof1_texture();
        
        // attach the texture to the mesh
        Renderer renderer1_roof = s_roof1.GetComponent<Renderer>();
        renderer1_roof.material.mainTexture = texture1_roof;
        
        
        
        // duplicate
        roof_mesh.triangles = roof_mesh.triangles.Reverse().ToArray();
        roof_mesh.RecalculateNormals();
        // create a new GameObject and give it a MeshFilter and a MeshRenderer
        GameObject s_roof1_du = new GameObject("Wall Mesh");
        s_roof1_du.AddComponent<MeshFilter>();
        s_roof1_du.AddComponent<MeshRenderer>();

        // associate my mesh with this object
        s_roof1_du.GetComponent<MeshFilter>().mesh = roof_mesh;

        // change the color of the object
        Renderer rend1_roof_du = s_roof1_du.GetComponent<Renderer>();
        rend1_roof_du.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

        // create a texture
        Texture2D texture1_roof_du = this.make_roof1_texture();
        
        // attach the texture to the mesh
        Renderer renderer1_roof_du = s_roof1_du.GetComponent<Renderer>();
        renderer1_roof_du.material.mainTexture = texture1_roof_du;
        
        
        
        
        // create a mesh object
        Mesh roof_2_mesh2 = new Mesh();

        // vertices of a cube
        Vector3[] roof_2_verts = new Vector3[4];
        roof_2_verts[0] = porch_ceiling_down_out;
        roof_2_verts[1] = porch_ceiling_down_in;
        roof_2_verts[2] = porch_ceiling_mid_in;
        roof_2_verts[3] = porch_ceiling_mid_out;

        // create uv coordinates
        Vector2[] roof_2_uv = new Vector2[4];

        roof_2_uv[0] = new Vector2(0, 0);
        roof_2_uv[1] = new Vector2(1, 0);
        roof_2_uv[2] = new Vector2(1, 1);
        roof_2_uv[3] = new Vector2(0, 1);

        // two triangles for the face

        int[] roof_2_tris = new int[6];  // need 3 vertices per triangle

        roof_2_tris[0] = 1;
        roof_2_tris[1] = 3;
        roof_2_tris[2] = 2;
        roof_2_tris[3] = 1;
        roof_2_tris[4] = 0;
        roof_2_tris[5] = 3;

        // save the vertices and triangles in the mesh object
        roof_2_mesh2.vertices = roof_2_verts;
        roof_2_mesh2.triangles = roof_2_tris;
        roof_2_mesh2.uv = roof_2_uv;  // save the uv texture coordinates

        roof_2_mesh2.RecalculateNormals();  // automatically calculate the vertex normals

        // create a new GameObject and give it a MeshFilter and a MeshRenderer
        GameObject s_roof2 = new GameObject("Wall Mesh");
        s_roof2.AddComponent<MeshFilter>();
        s_roof2.AddComponent<MeshRenderer>();

        // associate my mesh with this object
        s_roof2.GetComponent<MeshFilter>().mesh = roof_2_mesh2;

        // change the color of the object
        Renderer rend2_roof = s_roof2.GetComponent<Renderer>();
        rend2_roof.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

        // create a texture
        Texture2D texture2_roof = this.make_roof1_texture();
        
        // attach the texture to the mesh
        Renderer renderer2_roof = s_roof2.GetComponent<Renderer>();
        renderer2_roof.material.mainTexture = texture2_roof;
        
        
        // duplicate
        roof_2_mesh2.triangles = roof_2_mesh2.triangles.Reverse().ToArray();
        roof_2_mesh2.RecalculateNormals();
        // create a new GameObject and give it a MeshFilter and a MeshRenderer
        GameObject s_roof2_du = new GameObject("Wall Mesh");
        s_roof2_du.AddComponent<MeshFilter>();
        s_roof2_du.AddComponent<MeshRenderer>();

        // associate my mesh with this object
        s_roof2_du.GetComponent<MeshFilter>().mesh = roof_mesh;

        // change the color of the object
        Renderer rend2_roof_du = s_roof2_du.GetComponent<Renderer>();
        rend2_roof_du.material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

        // create a texture
        Texture2D texture2_roof_du = this.make_roof1_texture();
        
        // attach the texture to the mesh
        Renderer renderer2_roof_du = s_roof2_du.GetComponent<Renderer>();
        renderer2_roof_du.material.mainTexture = texture2_roof_du;
        
        
        // Add texture to pillar
        
        // create a texture
        Texture2D texture1_pillar = this.make_pillar_texture();
        
        // attach the texture to the mesh
        Renderer renderer1_pillar = pillar1.GetComponent<Renderer>();
        renderer1_pillar.material.mainTexture = texture1_pillar;
        
        // create a texture
        Texture2D texture2_pillar = this.make_pillar_texture();
        
        // attach the texture to the mesh
        Renderer renderer2_pillar = pillar2.GetComponent<Renderer>();
        renderer2_pillar.material.mainTexture = texture2_pillar;
    }
    
    
    public Texture2D make_wall_texture() {
        Texture2D texture = Resources.Load("wall" + this.chosen_wall_style) as Texture2D;
        return texture;
    }

    public Texture2D make_window_texture1() {
        Texture2D texture = Resources.Load("window1") as Texture2D;
        return texture;
    }
    
    public Texture2D make_window_texture2() {
        Texture2D texture = Resources.Load("window2") as Texture2D;
        return texture;
    }
    
    public Texture2D make_door_texture1() {
        Texture2D texture = Resources.Load("door1") as Texture2D;
        return texture;
    }
    
    public Texture2D make_door_texture2() {
        Texture2D texture = Resources.Load("door2") as Texture2D;
        return texture;
    }

    public Texture2D make_room_texture() {
        Texture2D texture = Resources.Load("room") as Texture2D;
        return texture;
    }
    
    public Texture2D make_settingroom_texture() {
        Texture2D texture = Resources.Load("settingroom") as Texture2D;
        return texture;
    }
    
    public Texture2D make_sill_texture() {
        Texture2D texture = Resources.Load("sill") as Texture2D;
        return texture;
    }

    public Texture2D make_roof_texture() {
        Texture2D texture = Resources.Load("roof1") as Texture2D;
        return texture;
    }
    
    public Texture2D make_roof1_texture() {
        Texture2D texture = Resources.Load("roof") as Texture2D;
        return texture;
    }
    
    public Texture2D make_roof2_texture() {
        Texture2D texture = Resources.Load("roof2") as Texture2D;
        return texture;
    }
    
    public Texture2D make_pillar_texture() {
        Texture2D texture = Resources.Load("pillar") as Texture2D;
        return texture;
    }
}




}
