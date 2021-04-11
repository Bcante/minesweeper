using UnityEngine;
using UnityEditor;

/*
 * Pour chaque grille on garde le time to clear et des choses comme ça
 */

public enum GameEndReason
{ 
    CLEARED,
    MINED,
    TIME_OUT
} 

public class GridHistory
{
    public float timeToClear;
    public int correctFlags;
    public int totalFlags;
    public int totalMines;
    public int xLength;
    public int yLength;
    public GameEndReason issue;

    public GridHistory(float timeToClear, int correctFlags, int totalFlags, int totalMines, int xLength, int yLength, GameEndReason issue)
    {
        this.timeToClear = timeToClear;
        this.correctFlags = correctFlags;
        this.totalFlags = totalFlags;
        this.totalMines = totalMines;
        this.xLength = xLength;
        this.yLength = yLength;
        this.issue = issue;
    }

    public GridHistory()
    {
        timeToClear = 1;
    }
}
