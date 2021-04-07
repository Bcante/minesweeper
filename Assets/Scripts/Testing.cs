using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public GameObject grid;
    public GameObject clone;
    public bool debug;

    public void Awake ()
    {
        grid.GetComponent<Grid>().debug = debug;
        clone = Instantiate(grid, transform.position, Quaternion.identity);
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    public void Reset()
    {
        Destroy(clone);
        clone.GetComponent<Grid>().destroyGrid();
        clone = Instantiate(grid, transform.position, Quaternion.identity);
        Grid.gameLost = false;
    }

}
