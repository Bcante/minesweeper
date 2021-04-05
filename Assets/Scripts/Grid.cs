using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public GameObject placeHolder;

    public static int GRID_SIZE = 10;

    public static GameObject[,] cells = new GameObject[GRID_SIZE, GRID_SIZE];
    public List<Cell> minableCells; // Toutes les cellules où on peut théoriquement rajouter une mine

    public static bool gameLost;

    public static float offsetX = .5f;
    public static float offsetY = .5f;

    public bool isFirstClick;

    System.Random ran;

    public void Awake()
    {
        minableCells = new List<Cell>();
        isFirstClick = true;
        buildGrid();
    }

    void buildGrid()
    {
        /*
         * Initialisation des cellules
         */
        ran = new System.Random();
        Cell c;
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                GameObject g = Instantiate(placeHolder, new Vector2(i + Grid.offsetX, j + Grid.offsetY), Quaternion.identity);

                c = g.GetComponent<Cell>();
                minableCells.Add(c);
                c.Init();
                c.SetCoordinates(i, j);
                cells[i, j] = g;
            }
        }

        /*
         * Remplissage des cellules de mines
         */
        for (int i2 = 0; i2 < GRID_SIZE; i2++)
        {
            for (int j2 = 0; j2 < GRID_SIZE; j2++)
            {
                int r = ran.Next(0, 100);
                bool bomb = (r <= 100);
                if (bomb)
                {
                    c = cells[i2, j2].GetComponent<Cell>();
                    c.plantMine();
                    minableCells.Remove(c);
                }
            }
        }
        c = cells[2, 2].GetComponent<Cell>();
        c.isMinable = true;
        c.isMine = false;
        minableCells.Add(c);
        
        /*
         * Pour chaque cellule dans le voisinage, on compte le nombre de mines
         */
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                cells[i, j].GetComponent<Cell>().updatesMinesInNeighborhood();
                Debug.Log(i + "," + j + " - bomb - " + cells[i, j].GetComponent<Cell>().isMine);
            }
        }
    }

    public void destroyGrid()
    {
        
        foreach (var cell in cells)
        {
            Destroy(cell);
        }
        
    }
    void Update()
    {
        Cell c = default; // ça c'est marrant
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!gameLost)
            {
                var v3 = Input.mousePosition;
                v3 = Camera.main.ScreenToWorldPoint(v3);
                int x, y;
                x = (int)Math.Truncate(v3.x);
                y = (int)Math.Truncate(v3.y);

                if (isInBound(x, y))
                {
                    c = cells[x, y].GetComponent<Cell>();
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    if (isFirstClick)
                    {
                        firstClick(c);
                        isFirstClick = false;
                    }
                    c.Reveal();
                }
                if (Input.GetButtonDown("Fire2"))
                {   
                    c.FlagCell();
                }
            }
        }
    }
    public static bool isInBound(int x, int y)
    {
        if (x >= 0 && x < Grid.GRID_SIZE && y >= 0 && y < Grid.GRID_SIZE) return true; else return false;
    }

    private void firstClick(Cell c)
    {
        int removedMines = 0;

        /*
         * Parcours de cellules dans une direction au pif pour déminer des bailz
         */
        int MAX_CLEAR = 3;
        c.removeMine();
        for (int i = 0; i < MAX_CLEAR; i++)
        {
            c = c.adjacentCells[ran.Next(c.adjacentCells.Count)].GetComponent<Cell>();//On récupère un voisin au pif...
            c.removeMine();
            removedMines++;

        }
        /* Rajout des mines enlevées pour les foutre ailleurs */
        int randomNewMineIndex = ran.Next(minableCells.Count);
        Cell randomNewMineCell = minableCells[randomNewMineIndex];
        randomNewMineCell.isMine = true; // Faudrait qu'on tej la cellule de la quelle on vient :c
        Debug.Log(randomNewMineCell.x + "," + randomNewMineCell.y + " est déminé ");




        // Pour toutes les cellules où on à enlevé des mines, il faut les mettre ailleurs


        //int r = rnd.Next(list.Count);
    }
}
