using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public GameObject grid;
    public GameObject clone;

    public void Awake ()
    {
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

    //public GameObject placeHolder;

    //public static int GRID_SIZE = 10;

    //public static GameObject[,] cells = new GameObject[GRID_SIZE,GRID_SIZE];
    //public static bool gameLost;

    //public static float offsetX = .5f;
    //public static float offsetY = .5f;

    //void Awake()
    //{
    //    System.Random ran = new System.Random();
    //    for (int i = 0; i < GRID_SIZE; i++)
    //    {
    //        for (int j = 0; j < GRID_SIZE; j++)
    //        {
    //            GameObject g = Instantiate(placeHolder, new Vector2(i + Testing.offsetX, j + Testing.offsetY), Quaternion.identity) ;

    //            Cell c = g.GetComponent<Cell>();

    //            c.Init();
    //            c.SetCoordinates(i, j);
    //            cells[i, j] = g;

    //        }
    //    }

    //    for (int i2 = 0; i2 < GRID_SIZE; i2++)
    //    {
    //        for (int j2 = 0; j2 < GRID_SIZE; j2++)
    //        {
    //            int r = ran.Next(0, 100);
    //            bool bomb = (r <= 15);
    //            if (bomb && ((i2!= 4) && (j2!=4)))
    //            {
    //                cells[i2, j2].GetComponent<Cell>().plantMine();
    //            }
    //        }
    //    }
    //    //cells[4, 4].GetComponent<Cell>().plantMine();
    //    for (int i = 0; i < GRID_SIZE; i++)
    //    {
    //        for (int j = 0; j < GRID_SIZE; j++)
    //        {
    //            cells[i, j].GetComponent<Cell>().updatesMinesInNeighborhood();
    //            Debug.Log(cells[i, j].GetComponent<Cell>().adjacentCells.Count);
    //        }
    //    }
    //}


    //void Update()
    //{
    //    Cell c = default; // ça c'est marrant
    //    if (Input.GetMouseButtonDown(0) ||  Input.GetMouseButtonDown(1)) {
    //        if (!gameLost)
    //        {
    //            var v3 = Input.mousePosition;
    //            v3 = Camera.main.ScreenToWorldPoint(v3);
    //            int x, y;
    //            x = (int)Math.Truncate(v3.x);
    //            y = (int)Math.Truncate(v3.y);

    //            if (isInBound(x, y))
    //            {
    //                c = cells[x, y].GetComponent<Cell>();
    //            }

    //            if (Input.GetButtonDown("Fire1"))
    //            {
    //                c.Reveal();
    //            }
    //            if (Input.GetButtonDown("Fire2"))
    //            {
    //                c.FlagCell();
    //            }
    //        }
    //    }
    //}
    //    public static bool isInBound(int x, int y)
    //{
    //    if (x >= 0 && x < Testing.GRID_SIZE && y >= 0 && y < Testing.GRID_SIZE) return true; else return false;
    //}

    //private void firstClick()
    //{
    //    /*
    //     * Le premier clic doit enlever la bomb si il y en a une
    //     * il va ensuite chercher une direction
    //     * */
    //}
}
