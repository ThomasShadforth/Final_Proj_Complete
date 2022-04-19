using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GamePause
{
    
    public static bool paused;

    public static float deltaTime { get { return paused ? 0 : Time.deltaTime; } }


}
