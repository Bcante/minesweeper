using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicateur : MonoBehaviour
{
    public Sprite[] sprites;
    public int nbMines;
    // Start is called before the first frame update
    public void setIndicateur()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[nbMines];
    }
    
}
