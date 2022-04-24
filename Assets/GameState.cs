using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
  public Vector2 playerPos;
  public int[] invItems;
  public int playerHealth;

  public int[] livingEnemiesID;
}
