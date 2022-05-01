using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   Singleton Pattern - A class that holds a single static field that is used to comunicate if game needs to be loaded or saved based on what menu option was 
   selected before the main scene was loaded
*/

public static class MenuSettings
{
   public static bool loadGame;
}
