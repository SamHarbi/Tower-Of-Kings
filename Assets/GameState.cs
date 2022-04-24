using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This Script was made by following a tutorial and thus very closely replicates a lot of
    code shown there 

    raywenderlich.com. 2017. How to Save and Load a Game in Unity. [online] Available at: <https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity> [Accessed 24 April 2022].

*/

[System.Serializable]
public class GameState
{
  public int playerPos_X;
  public int playerPos_Y;
  public int playerPos_Z;

  public int[] invItems;
  public int playerHealth;

  public int[] livingEnemiesID;
}
