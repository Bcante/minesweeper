using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicateur : MonoBehaviour
{
    public Sprite[] sprites;
    // Start is called before the first frame update
    public void setIndicateur()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[2];
    }
    
}
