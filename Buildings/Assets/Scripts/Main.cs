using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using building;

public class Main : MonoBehaviour
{
    public int seed = 7;
    public int numOfBuilding = 5;
    public float buildingDistance = 100f;
    public float footprintLength = 10f;
    public float cellLength = 2f;
    private float[,] bases;
    // Start is called before the first frame update
    void Start()
    {
        // Initialization
        if (numOfBuilding < 5) {
            this.numOfBuilding = 5;
        }
        bases = new float[this.numOfBuilding, 2];
        for (int i = 0; i < this.numOfBuilding; i++)
        {
            this.bases[i, 0] = i * this.buildingDistance;
            this.bases[i, 1] = 0f;
        }

        // Build buildings
        for (int i = 0; i < this.numOfBuilding; i++)
        {
            building.Building building = new building.Building(this.bases[i, 0], this.bases[i, 1], this.seed + i, this.footprintLength, this.cellLength);    
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
