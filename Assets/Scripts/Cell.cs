using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{
    public int x, y;
    public int minesInNeighborhood;

    public bool isMinable; // On veut dès fois s'assurer qu'aucune mine puisse être mis dans une cellule
    public bool isMine;
    public bool isFlag;
    public bool isRevealed;
    public bool isVisited; // Pour la phase de révélation après le premier clic

    public Sprite[] sprites;
    public List<Cell> adjacentCells;
    public Dictionary<String, GameObject> dict;


    private void Awake()
    {
        adjacentCells = new List<Cell>();
    }

    public void updatesMinesInNeighborhood()
    {
        adjacentCells.Clear();
        minesInNeighborhood = 0;
        for (int i = (x - 1); i < x + 2; i++)
        {
            for (int j = (y - 1); j < y + 2; j++)
            {
                bool inBound = Grid.isInBound(i, j);
                if (inBound && (i != x || j != y))
                {
                    if (Grid.cells[i, j].GetComponent<Cell>().isMine)
                    {
                        this.minesInNeighborhood++;
                    }
                    adjacentCells.Add(Grid.cells[i, j].GetComponent<Cell>());
                }
            }
        }
        GetComponent<SpriteRenderer>().sprite = sprites[11]; // J'ai honte mais Martin à dit ok, et ça c'est le sprite par défaut si vous vous demandez
    }

    internal void plantMine()
    {
        isMine = true;
    }

    public void SetCoordinates(int i, int j)
    {
        x = i;
        y = j;
    }
    public void Init()
    {
        isVisited = false;
        isMine = false;
        minesInNeighborhood = 0;
    }

    public void Reveal()
    {
        isRevealed = true;
        if (!isFlag)
        {
            if (isMine)
            {
                GetComponent<SpriteRenderer>().sprite = sprites[10]; // Sprite de isMine
                Grid.gameLost = true;
            }
            else if (minesInNeighborhood != 0)
            {
                GetComponent<SpriteRenderer>().sprite = sprites[minesInNeighborhood]; // J'ai honte mais Martin à dit ok
            }
            else
            {
                RevealNearbyTiles();
                //pour tous les voisins minesInNeighborhood 0 on tente de les révéler mais bon à coup sûr je vais me taper une boucle inifinie pfff nique la prog            
            }
        }
    }

    private void RevealNearbyTiles()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[minesInNeighborhood]; // J'ai honte mais Martin à dit ok
        foreach (var item in adjacentCells)
        {
            Cell c = item.GetComponent<Cell>();
            if (!isMine && !c.isRevealed)
            {
                c.isRevealed = true;
                if (c.minesInNeighborhood == 0) // Si le voisin en question n'a pas de isMinee dans son voisinage ET si on l'a pas encore révélé. 
                {
                    c.RevealNearbyTiles();
                }
                else
                {
                    c.GetComponent<SpriteRenderer>().sprite = sprites[c.GetComponent<Cell>().minesInNeighborhood];
                }
            }
          
        }
    }

    /* Deflag si flag il y a, flag si neni flag n'y est 
     Renvoie vrai si il y a désormais un flag
     Renvoie faux si il n'y en a plus */
    public Trulean FlagCell()
    {
        Trulean res = Trulean.NA;
        // Test si on peut isFlag. On peut que si c'est pas encore revelé
        if (!isRevealed)
        {
            
            if (isFlag)
            {
                GetComponent<SpriteRenderer>().sprite = sprites[11]; // J'ai honte mais Martin à dit ok
                res = Trulean.False;
            }
                
            else
            {
                GetComponent<SpriteRenderer>().sprite = sprites[9];
                res = Trulean.True;
            }

            isFlag = !isFlag;
            
        }
        return res;
    }

    /*
     * Renvoie true si on a trouvé une mine
     */
    public bool removeMine()
    {
        bool res;
        if (isMine)
        {
            res = true;
        }
        else
        {
            res = false;
        }
        isMine = false;
        foreach (Cell item in adjacentCells)
        {
            item.GetComponent<Cell>().updatesMinesInNeighborhood();
        }
        return res;
    }

    public void infoDebug()
    {
        Debug.Log("Cell (" + x + ";" + y + ")" + "\n"
           + "Mines autour: " + minesInNeighborhood);
    }

}