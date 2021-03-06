using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject minePlaceHolder;
    public GameObject indicateurPlaceHolder;

    public static int GRID_SIZE = 16;
    public int TOTAL_MINES = 40;

    public int correctFlags, wrongFlags; // Comment faire pour détecter quand on a gagné? Quand tout est revélé ? mh...

    public static GameObject[,] cells;
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
        cells = new GameObject[GRID_SIZE, GRID_SIZE];
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
                        if(Grid.gameLost) // La cellule rélévé nous à fait perdre
                        {
                            GameManager.instance.notifyLoose(GameEndReason.MINED); 
                        }
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
                        Trulean isNowFlagged = c.FlagCell();
                        updateFlagCount(isNowFlagged, c);
                    }
                }
                if ((Input.GetKey(KeyCode.LeftControl)) && Input.GetButtonDown("Fire1"))
                {
                    chordClick(c);
                }
            }
        }
    }

    /*
     * c.isMine + isNowFlagged ? correct flag++
     * c.isMine + !isNowFlagged ? correct flag--
     * !c.isMine + isNowFlagged ? wrong flag++
     * !c.isMine + !isNowFlagged ? correct flag--
     */

    private void updateFlagCount(Trulean isNowFlagged, Cell c)
    {
        if (isNowFlagged == Trulean.NA)
        {
            Debug.Log("Rien à faire ici");
        }
        if (c.isMine && isNowFlagged == Trulean.True)
        {
            correctFlags++;
        }
        if (c.isMine && isNowFlagged == Trulean.False)
        {
            correctFlags--;
        }
        if (!c.isMine && isNowFlagged == Trulean.True)
        {
            wrongFlags++;
        }
        if (!c.isMine && isNowFlagged == Trulean.False)
        {
            wrongFlags--;
        }
        
    }

    private void chordClick(Cell c)
    {
        int neighborFlags = 0;
        foreach (var cell in c.adjacentCells)
        {
            if (cell.isFlag)
            {
                neighborFlags++;
            }

        }
        if (neighborFlags == c.minesInNeighborhood)
        {
            foreach (var cell in c.adjacentCells)
            {
                cell.Reveal();
            }
        }
        else if (neighborFlags < c.minesInNeighborhood)
        {
            Debug.Log("Not enough flags to chord");
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

        plantMines(TOTAL_MINES);
        updateAll();
        
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

    private void plantMines(int minesToPlant)
    {
        for (int i = 0; i < minesToPlant; i++)
        {
            int rand = ran.Next(minableCells.Count);
            minableCells[rand].GetComponent<Cell>().plantMine();
            minableCells.RemoveAt(rand);
        }
    }

    /* 
     * x et y auront une valeur négative si on ne veut pas les mettre à jour
     * Si x est renseigné: je met à jour l'indicateur de la ligne x.
     * Si y est renseigné: je met à jour l'indicateur de la col y. 
     * */
    public void updateIndicateurs(int x, int y)
    {
        int tmpCount = 0;
        Cell c = default;
        if (x > -1)
        {
            for (int j = 0; j < cells.GetLength(1); j++) // On loop sur la deuxième dimension, la première reste fixe
            {
                c = cells[x, j].GetComponent<Cell>();
                tmpCount = c.isMine ? tmpCount + 1 : tmpCount;
            }
            indicateurs["x" + x].GetComponent<Indicateur>().nbMines = tmpCount;
            tmpCount = 0;
        }
        if (y > -1)
        {
            for (int i = 0; i < cells.GetLength(0); i++) // On loop sur la deuxième dimension, la première reste fixe
            {
                c = cells[i, y].GetComponent<Cell>();
                tmpCount = c.isMine ? tmpCount + 1 : tmpCount;
            }
            indicateurs["y" + y].GetComponent<Indicateur>().nbMines = tmpCount;
        }
    }

    public void destroyGrid()
    {
        
        foreach (var cell in cells)
        {
            Destroy(cell);
        }
        foreach (var indicateur in indicateurs.Values)
        {
            Destroy(indicateur.gameObject);
        }

    }
    
    public static bool isInBound(int x, int y)
    {
        if (x >= 0 && x < Grid.GRID_SIZE && y >= 0 && y < Grid.GRID_SIZE) return true; else return false;
    }

    /*
     * Il faut pas juste enlever les mines, il faut qu'il y en ait 0 autour.
     */
    private bool removeMine(Cell c)
    {
        minableCells.Remove(c);
        return c.removeMine();
    }

    private void firstClick(Cell c)
    {
        /*
         * Parcours de cellules dans une direction au pif pour déminer. On récupère 3 cellules "collées" (comme un tetrominos)
         * Pour toutes les cellules récupérées, on va ensuite supprimer leurs bombes voisines (pour faire comme dans un démineur normal ou le premier clic révèle 
         * jamais une case isolée)
         */
        int MAX_CLEAR = 3;
        int removedMines = 0;
        List<Cell> deletedNodes = new List<Cell>();
        bool wasMined;

        deletedNodes.Add(c);

        wasMined = removeMine(c);
        if (wasMined)
        {
            removedMines++;
        }

        for (int i = 0; i < MAX_CLEAR; i++)
        {
            c = getNextCell(c);
            wasMined = removeMine(c);
            c.isVisited = true;
            deletedNodes.Add(c);
            if (wasMined)
            {
                removedMines++;
            }
        }


        //Suppression de toutes les mines dans les cases VOISINES marquées dans notre passage
        foreach (var mainCell in deletedNodes)
        {
            foreach(Cell sideCell in mainCell.adjacentCells.ToArray())
            {
                wasMined = removeMine(sideCell);
                if (wasMined)
                {
                    removedMines++;
                }
            }
        }

        // On a supprimé des mines maintenant il faut les remettre ailleurs quand même !!
        plantMines(removedMines);
        updateAll();

        // Update de tous les voisins 
        foreach (var mainCell in deletedNodes)
        {
            foreach (Cell sideCell in mainCell.adjacentCells.ToArray())
            {
                sideCell.updatesMinesInNeighborhood();
            }
        }

        // Update de tous les indicateurs
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            updateIndicateurs(i, i);
            /*
             * Maj des gameobjects
             * */
            indicateurs["x" + i].updateIndicateur();
            indicateurs["y" + i].updateIndicateur();
        }
        
    }

    private void updateAll()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                cells[i, j].GetComponent<Cell>().updatesMinesInNeighborhood();
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
