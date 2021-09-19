using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int highScore;

    public PlayerData (GameManager gameManager)
    {
        playerName = gameManager.playerName;
        highScore = gameManager.highScore;

    }
}
