using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.

    private bool isPlayerInTrigger = false;

    [SerializeField] private GameObject exitCanvas;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Player")
        {
            exitCanvas.SetActive(true);
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Reset the flag when the player leaves the trigger zone.
        if (other.tag == "Player")
        {
            exitCanvas.SetActive(false);
            isPlayerInTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is in the trigger zone and the F key is pressed.
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            // Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);

            // Disable the player object since the level is over.
            enabled = false;
        }
    }

    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
