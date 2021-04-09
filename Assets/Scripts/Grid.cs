using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public GameObject minePlaceHolder;
    public GameObject indicateurPlaceHolder;

    public static int GRID_SIZE = 10;

    public static GameObject[,] cells = new GameObject[GRID_SIZE, GRID_SIZE];
    public List<Cell> minableCells; // Toutes les cellules où on peut théoriquement rajouter une mine
    public Dictionary<String,Indicateur> indicateurs;

    public static bool gameLost;

    /*
     * Offset entre chaque cellules 
     */
    public static float offsetX = .5f;
    public static float offsetY = .5f;

    /*
     * Offset avant chaque indicateur et les cellules
     */
    public float indicOffset = .1f;

    public bool isFirstClick;
    public bool debug;

    System.Random ran;
    // Time
    public float timeToClear;

    public void Awake()
    {
        minableCells = new List<Cell>();
        indicateurs = new Dictionary<string, Indicateur>();
        isFirstClick = true;
        buildGrid();
    }
    void Update()
    {
        Cell c = default;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            var v3 = Input.mousePosition;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            int x, y;
            x = (int)Math.Truncate(v3.x);
            y = (int)Math.Truncate(v3.y);


            if (isInBound(x, y))
            {
                c = cells[x, y].GetComponent<Cell>();

                if (Input.GetButtonDown("Fire1"))
                {
                    if (!gameLost)
                    {
                        if (isFirstClick)
                        {
                            firstClick(c);
                            isFirstClick = false;
                        }
                        c.Reveal();
                    }
                    if (debug)
                    {
                        c.infoDebug();
                    }

                }
                if (Input.GetButtonDown("Fire2"))
                {
                    if (!gameLost)
                    {
                        c.FlagCell();
                    }
                }
            }
        }
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
                GameObject g = Instantiate(minePlaceHolder, new Vector2(i + Grid.offsetX, j + Grid.offsetY), Quaternion.identity);

                c = g.GetComponent<Cell>();
                minableCells.Add(c);
                c.Init();
                c.SetCoordinates(i, j);
                cells[i, j] = g;
            }
        }

        /*
         * Remplissage des cellules de mines au hasard
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
        
        /*
         * Pour chaque cellule dans le voisinage, on compte le nombre de mines
         */
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                cells[i, j].GetComponent<Cell>().updatesMinesInNeighborhood();
            }
        }

        /*
         * Initialisation des indicateurs de lignes / colonnes 
         */
        for (int i = 0; i < GRID_SIZE; i++)
        {
            GameObject IndicColGO = Instantiate(indicateurPlaceHolder, new Vector2(i + Grid.offsetX, -Grid.offsetY - indicOffset ), Quaternion.identity);
            indicateurs["x" + i] = IndicColGO.GetComponent<Indicateur>();

            GameObject IndicLigneGO = Instantiate(indicateurPlaceHolder, new Vector2(-Grid.offsetX - indicOffset, i + Grid.offsetY ), Quaternion.identity);
            indicateurs["y" + i] = IndicLigneGO.GetComponent<Indicateur>();
            // Si ma cellule est en 4,5 je maj indicateurs[x4] et indicateurs [y5]


        }
    }

    public void destroyGrid()
    {
        
        foreach (var cell in cells)
        {
            Destroy(cell);
        }
        
    }
    
    public static bool isInBound(int x, int y)
    {
        if (x >= 0 && x < Grid.GRID_SIZE && y >= 0 && y < Grid.GRID_SIZE) return true; else return false;
    }

    /*
     * Il faut pas juste enlever les mines, il faut qu'il y en ait 0 autour.
     */

    private void firstClick(Cell c)
    {
        /*
         * Parcours de cellules dans une direction au pif pour déminer. On récupère 3 cellules "collées" (comme un tetrominos)
         * Pour toutes les cellules récupérées, on va ensuite supprimer leurs bombes voisines (pour faire comme dans un démineur normal ou le premier clic révèle 
         * jamais une case isolée)
         */
        int MAX_CLEAR = 3;
        List<Cell> deletedNodes = new List<Cell>();
        //List<Cell> deletchangededNodes = new List<Cell>();

        deletedNodes.Add(c);

        c.removeMine();


        for (int i = 0; i < MAX_CLEAR; i++)
        {
            c = getNextCell(c);
            c.removeMine();
            c.isVisited = true;
            deletedNodes.Add(c);
        }

        //Suppression de toutes les mines dans les cases VOISINES marquées dans notre passage
        foreach (var mainCell in deletedNodes)
        {
            foreach(Cell sideCell in mainCell.adjacentCells.ToArray())
            {
                sideCell.removeMine();   
            }
        }

        // Update de tous les voisins 
        foreach (var mainCell in deletedNodes)
        {
            foreach (Cell sideCell in mainCell.adjacentCells.ToArray())
            {
                sideCell.updatesMinesInNeighborhood();
            }
        }
    }

    public Cell getNextCell(Cell c) {
        Cell candidate;
        List<Cell> candidates = c.adjacentCells;

        bool candidateOk = false;
        int rand = ran.Next(candidates.Count);
        
        candidate = c.adjacentCells[rand].GetComponent<Cell>();//On récupère un voisin au pif...
        while (!candidateOk && candidates.Count > 0)
        {
            if(!candidate.isVisited && (c.x == candidate.x || c.y == candidate.y ) )
            {
                candidateOk = true;
            }
            else
            {
                candidates.RemoveAt(rand);
                rand = ran.Next(candidates.Count);
                candidate = c.adjacentCells[rand].GetComponent<Cell>();//On récupère un voisin au pif...
            }

        }
        if (debug)
        {
            Debug.Log("Je libère " + candidate.x + "-" + candidate.y);
        }
        return candidate;
        
    }
}
