//This script is for entering the BAYS of the clinicals (orginally it was doors of an office changed to bays of a clinic)
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterDoors : MonoBehaviour
{
    [Header("Inscribed")]
    public string sceneToLoad;
    public GameObject[] indicatorCubes; //Array of indicator cubes
    public GameObject levelSelector;
    public GameObject pickedLevel;
    public GameObject characterForPOS;
    public string targetSceneName; //Added this here to store a string of the level selected scene!

    public GameObject FadePrefab;
    private bool isDoorLocked;

    private void Start()
    {
        characterForPOS = GameObject.Find("MCharacter");
        //Ensure all indicator cubes are inactive at the start (Level Chat Boxes above levels)
        foreach (GameObject cube in indicatorCubes)
        {
            cube.SetActive(false);
        }
    }

    //Player trigger door
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            Debug.Log("Player entered the trigger.");

            if (isDoorLocked)
            {
                Debug.Log("This level is currently locked.");
                return;
            }

            //Activate all indicator cubes the player is in when the player enters
            foreach (GameObject cube in indicatorCubes)
            {
                cube.SetActive(true);
            }
        }
    }

    //Press E to enter level
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            if (isDoorLocked)
            {
                return;
            }

            KeepPlayerPOS.Instance.SetPlayerPosition(characterForPOS);//Added to grab the users POS before entering scene
            Instantiate(FadePrefab); //instatiates an object that fades the screen to black
            Invoke("Change", 0.2f);//Change scene after the fade

        }
    }

    //Player exits
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Deactivate all indicator cubes when the player exits
            foreach (GameObject cube in indicatorCubes)
            {
                cube.SetActive(false);
            }
        }
    }

    public void UpdateDoorLockStatus(int level, bool isLocked)
    {
        isDoorLocked = isLocked;

        if (isLocked)
        {
            foreach (GameObject cube in indicatorCubes)
            {
                cube.SetActive(false);
            }

            Debug.Log("Level " + level + " is locked.");
        }
    }

    private void Change() {
        SceneManager.LoadScene(targetSceneName);
    }

}
