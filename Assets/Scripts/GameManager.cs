using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Do I really need it

    public GameObject grid;
    public GameObject clone;
    public bool debug;

    private float waitTime = 2.0f;
    private float timer = 0.0f;
    private float scrollBar = 1.0f;

    public GameObject indicateur;
    public GameObject myCamera;

    public List<GridHistory> history;

    public void Awake()
    {
        instance = this;
        history = new List<GridHistory>();
        grid.GetComponent<Grid>().debug = debug;
        clone = Instantiate(grid, transform.position, Quaternion.identity);
        myCamera.GetComponent<Transform>().position = new Vector3(3, 3, -10);
        myCamera.GetComponent<Camera>().orthographicSize = 10;

    Time.timeScale = scrollBar;
    }

    public void Update()
    {
        //timer += Time.deltaTime;
        //Debug.Log(timer);
        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        //if (timer > waitTime)
        //{
            // Remove the recorded 2 seconds.
            //timer = timer - waitTime;
            //Time.timeScale = scrollBar;
            //Reset();
        //}

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    public void GameEnd()
    {
        GridHistory newRecord = new GridHistory();
        history.Add(newRecord);
        foreach (var item in history)
        {
            Debug.Log(item.timeToClear);
        }

        Reset();
    }

    public void notifyLoose()
    {
        Debug.Log("perdu !!!");
        
        GameEnd();
    }

    public void Reset()
    {
        Destroy(clone);
        clone.GetComponent<Grid>().destroyGrid();
        clone = Instantiate(grid, transform.position, Quaternion.identity);
        Grid.gameLost = false;
    }

}
