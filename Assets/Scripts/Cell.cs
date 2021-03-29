using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{
    public int x, y;
    public int minesInNeighborhood;
    public bool isMine;
    public bool isFlag;
    public Sprite[] sprites;
    public bool revealed;
    public List<GameObject> adjacentCells;
    public Dictionary<String, GameObject> dict;


    private void Awake()
    {
        adjacentCells = new List<GameObject>();
    }

    public void updatesMinesInNeighborhood()
    {
        for (int i = (x - 1); i < x + 2; i++)
        {
            for (int j = (y - 1); j < y + 2; j++)
            {
                bool inBound = Testing.isInBound(i, j);
                if (inBound && (i != x || j != y))
                {
                    if (Testing.cells[i, j].GetComponent<Cell>().isMine)
                    {
                        this.minesInNeighborhood++;
                    }
                    adjacentCells.Add(Testing.cells[i, j]);
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
        isMine = false;
        minesInNeighborhood = 0;
    }

    public void Reveal()
    {
        revealed = true;
        if (!isFlag)
        {
            if (isMine)
            {
                GetComponent<SpriteRenderer>().sprite = sprites[10]; // Sprite de isMine
                Testing.gameLost = true;
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
            if (!isMine && !c.revealed)
            {
                c.revealed = true;
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

    public void FlagCell()
    {
        // Test si on peut isFlag. On peut que si c'est pas encore revelé
        if (!revealed)
        {
            if (isFlag)
                GetComponent<SpriteRenderer>().sprite = sprites[11]; // J'ai honte mais Martin à dit ok
            else
                GetComponent<SpriteRenderer>().sprite = sprites[9];
            isFlag = !isFlag;
        }
    }
}