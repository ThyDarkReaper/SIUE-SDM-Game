//This script handles the Player walking up and interacting with the Elevator on the LevelSelector
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorOpen : MonoBehaviour
{
    private bool playerInRange = false;

    //When player enters the collison area of elevator and reads tag player
    void OnTriggerEnter(Collider cube)
    {
        if (cube.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered the trigger zone");
        }
    }

    //When player exits
    void OnTriggerExit(Collider cube)
    {
        if (cube.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player exited the trigger zone");
        }
    }

    //On run time if the player is in the collider and presses E the user will be taken to the elevator scene
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed. Loading scene...");

            SceneManager.LoadScene("ElevatorScene");
        }
    }
}
