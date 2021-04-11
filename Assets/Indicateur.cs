using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicateur : MonoBehaviour
{
    public Sprite[] sprites;
    public int nbMines;
    // Start is called before the first frame update
    public void Start()
    {
        nbMines = 0;
    }
    public void updateIndicateur()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[nbMines];
    }
    
}
