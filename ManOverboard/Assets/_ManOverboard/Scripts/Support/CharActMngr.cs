using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is intended to take characters through a series of movements/animations/functions.
// It's removed from the CharBase class, so that it could be applied to a character who we want to do things, but whom the player does not necessarily interact with, such as in a movie clip.


public class CharActMngr : MonoBehaviour
{
    protected delegate void ActionCBs();

    // List of actions
    public List<int> actionList;
}
