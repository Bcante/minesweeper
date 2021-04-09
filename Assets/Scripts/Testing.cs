using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public GameObject grid;
    public GameObject clone;
    public bool debug;

    private float waitTime = 2.0f;
    private float timer = 0.0f;
    private float scrollBar = 1.0f;

    public GameObject indicateur;
    public void Awake()
    {
        grid.GetComponent<Grid>().debug = debug;
        clone = Instantiate(grid, transform.position, Quaternion.identity);
        

        Time.timeScale = scrollBar;
    }

    public void Update()
    {
        timer += Time.deltaTime;
        Debug.Log(timer);
        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if (timer > waitTime)
        {
            // Remove the recorded 2 seconds.
            timer = timer - waitTime;
            Time.timeScale = scrollBar;
            Reset();
        }

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
