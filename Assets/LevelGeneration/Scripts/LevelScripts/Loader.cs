using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject levelGameManager;          //GameManager prefab to instantiate.
    public GameObject soundManager;         //SoundManager prefab to instantiate.


    void Awake()
    {
        //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
        if (LevelGameManager.instance == null)

            //Instantiate gameManager prefab
            Instantiate(levelGameManager);

        //Check if a SoundManager has already been assigned to static variable GameManager.instance or if it's still null
        if (SoundManager.instance == null)

            //Instantiate SoundManager prefab
            Instantiate(soundManager);
    }
}
