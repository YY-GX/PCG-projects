namespace ca
{

using System;
using System.Collections.Generic;
public class Cave {
    // params
    public int seed = 7;
    private System.Random rnd;
    public int grid_width = 100;
    public int grid_height = 100;
    public int CA_iteration_num = 10;
    private float init_fill_prob = 0.4f;
    private int threshold_turn_1 = 4;
    private int threshold_turn_0 = 3;

    // map definition
    private int[,,] map; 

    
    public Cave(int seed=7, int grid_width=100, int grid_height=100, int CA_iteration_num=10, float init_fill_prob=0.4f) {
        // param initialization
        this.grid_width = grid_width;
        this.grid_height = grid_height;
        this.map = new int[grid_width, grid_height, 3];
        this.seed = seed;
        this.CA_iteration_num = CA_iteration_num;
        this.init_fill_prob = init_fill_prob;
        this.rnd = new System.Random(this.seed);


        // initialize map by filling it with 1 for init_fill_prob grids, add pointers at the same time
        for (int i = 0; i < this.grid_width; i++)
        {            
            for (int j = 0; j < this.grid_height; j++)
            {
                if ((i == 0) || (j == 0) || (i == this.grid_height - 1) || (j == this.grid_width - 1)) { // if it's border, fill it
                    // assign grid_info to i, j
                    this.map[i, j, 0] = 1;
                    this.map[i, j, 1] = -1;
                    this.map[i, j, 2] = -1;
                    continue;
                }
                if (this.rnd.NextDouble() > init_fill_prob) { // empty
                    int[] grid_info = new int[] {0, -1, -1}; // idx: 0 -> fill/empty, 1 -> pos_x, 2 -> pos_y
                    // assign grid_info to i, j
                    for (int k = 0; k < 3; k++)
                    {
                        this.map[i, j, k] = grid_info[k];
                    }
                } else { // fill
                    int[] grid_info = new int[] {1, -1, -1}; // idx: 0 -> fill/empty, 1 -> pos_x, 2 -> pos_y
                    // assign grid_info to i, j
                    for (int k = 0; k < 3; k++)
                    {
                        this.map[i, j, k] = grid_info[k];
                    }
                }
            }
        }

        Console.WriteLine(this.connectivity_check().Count);
        // run CA for CA_iteration_num tiems
        for (int i = 0; i < this.CA_iteration_num; i++)
        {
            this.Cellular_Automation_iter();
        }

        // Set roots
        this.set_roots();

        Console.WriteLine(this.connectivity_check().Count);

        // blast walls
        this.blast_walls();

        Console.WriteLine(this.connectivity_check().Count);
    }

    private void Cellular_Automation_iter() {
        for (int i = 1; i < this.grid_width - 1; i++)
        {
            for (int j = 1; j < this.grid_height - 1; j++)
            {
                if (this.map[i, j, 0] == 0) {
                    if (this.neighbour_count(i, j) > this.threshold_turn_1) {
                        this.map[i, j, 0] = 1;
                    }
                } else {
                    if (this.neighbour_count(i, j) < this.threshold_turn_0) {
                        this.map[i, j, 0] = 0;                   
                    }
                }
            }
        }
    }

    private int neighbour_count(int x, int y) {
        int cnt = 0;
        for (int i = x - 1; i < x + 2; i++)
        {
            for (int j = y - 1; j < y + 2; j++)
            {
                if ((i == x) && (j == y)) {
                    continue;
                }
                if (this.map[i, j, 0] == 1) {
                    cnt ++;
                }
            }
        }
        return cnt;
    }

    private void set_roots() {
        for (int i = 1; i < this.grid_width - 1; i++)
        {
            for (int j = 1; j < this.grid_height - 1; j++)
            {
                if (this.map[i, j, 0] == 0) {
                    // add pointer
                    bool is_found = false;

                    List<int[]> neighbors = new List<int[]>() {
                        new int[] {i, j - 1}, // left
                        new int[] {i - 1, j}, // up
                    };
                    foreach (var item in neighbors)
                    {
                        if (this.map[item[0], item[1], 0] == 0) {
                            // add pointer to up or left grid position
                            if (is_found) {
                                // root up
                                int[] root_up = this.search_root(item[0], item[1]);
                                // root left
                                int[] root_left = this.search_root(neighbors[0][0], neighbors[0][1]);
                                // up to left
                                if ((root_up[0] != root_left[0]) || (root_up[1] != root_left[1])) {
                                    this.map[root_up[0], root_up[1], 1] = root_left[0];
                                    this.map[root_up[0], root_up[1], 2] = root_left[1];
                                }
                                continue;
                            }
                            int[] root = this.search_root(item[0], item[1]);
                            this.map[i, j, 1] = root[0];
                            this.map[i, j, 2] = root[1];
                            is_found = true;
                        }
                    }                      
                }
            }
        }
    }

    private void blast_walls() {
        List<int[]> roots = this.connectivity_check();
        while (roots.Count > 1) { // end only if there's one root
            int[] blast_starter = roots[0]; // from this root to blast the walls
            int[] blast_ender = new int[2]; // goal root -> the nearest one
            float dist = float.MaxValue;
            for (int i = 1; i < roots.Count; i++)
            {
                float crr_dist = (float) (Math.Pow(blast_starter[0] - roots[i][0], 2) + Math.Pow(blast_starter[1] - roots[i][1], 2));
                if (crr_dist < dist) {
                    dist = crr_dist;
                    blast_ender = new int[] {roots[i][0], roots[i][1]};
                }
            }
            bool success = false;
            // blast
            int start_idx = Math.Min(blast_starter[0], blast_ender[0]);
            int end_idx = Math.Max(blast_starter[0], blast_ender[0]);
            int sum_idx = start_idx + end_idx;
            for (int x = start_idx + 1; x < end_idx + 1; x++) // go along y axis
            {
                int i = x;
                if (start_idx == blast_ender[0]) {
                    i = sum_idx - x;
                }
                int j = blast_starter[1];
                int is_empty = this.map[i, j, 0];
                if (is_empty == 0) { // if empty, judge root
                    int[] root_pos = this.search_root(i, j);
                    if ((root_pos[0] == blast_ender[0]) && (root_pos[1] == blast_ender[1])) {
                        success = true;
                        // let ender point to starter
                        this.map[blast_ender[0], blast_ender[1], 1] = blast_starter[0];
                        this.map[blast_ender[0], blast_ender[1], 2] = blast_starter[1];
                        break;
                    }
                    if ((root_pos[0] != blast_starter[0]) || (root_pos[1] != blast_starter[1])) {
                        // let pass by root point to starter
                        this.map[root_pos[0], root_pos[1], 1] = blast_starter[0];
                        this.map[root_pos[0], root_pos[1], 2] = blast_starter[1];
                    }
                } else { // if filled, blast it and point to crr root
                    this.map[i, j, 0] = 0;
                    this.map[i, j, 1] = blast_starter[0];
                    this.map[i, j, 2] = blast_starter[1];
                }
            }
            if (success) {
                continue;
            }
            start_idx = Math.Min(blast_starter[1], blast_ender[1]);
            end_idx = Math.Max(blast_starter[1], blast_ender[1]);
            sum_idx = start_idx + end_idx;
            for (int y = start_idx + 1; y < end_idx + 1; y++) // go along x axis
            {
                int j = y;
                if (start_idx == blast_ender[1]) {
                    j = sum_idx - y;
                }
                int i = blast_ender[0];
                int is_empty = this.map[i, j, 0];
                if (is_empty == 0) { // if empty, judge root
                    int[] root_pos = this.search_root(i, j);
                    if ((root_pos[0] == blast_ender[0]) && (root_pos[1] == blast_ender[1])) {
                        success = true;
                        // let ender point to starter
                        this.map[blast_ender[0], blast_ender[1], 1] = blast_starter[0];
                        this.map[blast_ender[0], blast_ender[1], 2] = blast_starter[1];                        
                        break;
                    }
                    if ((root_pos[0] != blast_starter[0]) || (root_pos[1] != blast_starter[1])) {
                        // let pass by root point to starter
                        this.map[root_pos[0], root_pos[1], 1] = blast_starter[0];
                        this.map[root_pos[0], root_pos[1], 2] = blast_starter[1];
                    }
                } else { // if filled, blast it and point to crr root
                    this.map[i, j, 0] = 0;
                    this.map[i, j, 1] = blast_starter[0];
                    this.map[i, j, 2] = blast_starter[1];
                }
            }

            // recheck
            roots = this.connectivity_check();
        }
    }

    private int[] search_root(int x, int y) {
        int[] root = new int[] {this.map[x, y, 1], this.map[x, y, 2]};
        if ((root[0] == -1) && (root[1] == -1)) {
            return new int[] {x, y};
        }
        return this.search_root(root[0], root[1]);
    }

    private List<int[]> connectivity_check() {
        List<int[]> roots = new List<int[]>();
        for (int i = 0; i < this.grid_width; i++)
        {
            for (int j = 0; j < this.grid_height; j++)
            {
                if ((this.map[i, j, 0] == 0) && (this.map[i, j, 1] == -1) && (this.map[i, j, 2] == -1)) { 
                    roots.Add(new int[] {i, j});
                }
            }
        }
        return roots;
    }

    public int[,] get_map() {
        int[,] return_map = new int[this.grid_width, this.grid_height];
        for (int i = 0; i < this.grid_width; i++)
        {
            for (int j = 0; j < this.grid_height; j++)
            {
                return_map[i, j] = this.map[i, j, 0];
            }
        }
        return return_map;
    }
}
    
}
