//This script is attached to the Elevator scene that handles what button the user presses on to go to a certain floor
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorButtonManager : MonoBehaviour
{

    string sceneName; 
    public float duration = 2f; 
    private bool isMoving = false; 


    [SerializeField] GameObject doorPart1;
    [SerializeField] GameObject doorPart2;
    [SerializeField] GameObject fadePrefab;
    //Go to Main Floor on 1
    public void OnFloor1Button(){
        sceneName = "LevelSelectorMainFloor";
        Invoke("ChangeFloor", 1.5f);
        Invoke("FadeOut", 1.25f);
    }

    //Go to LevelSelector Floor 2 on 2
    public void OnFloor2Button(){
        sceneName = "LevelSelector2ndFloor";
        Invoke("ChangeFloor", 1.5f);
        Invoke("FadeOut", 1.25f);
    }

    
    public void OpenElevatorDoor(){
        if (!isMoving)
        {
            StartCoroutine(MoveRoutine());
        }
    }

    private void ChangeFloor(){
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;
        Vector3 startPosition1 = doorPart1.transform.position;
        Vector3 startPosition2 = doorPart2.transform.position;
        Vector3 endPosition1 = startPosition1 + new Vector3(15, 0, 0);
        Vector3 endPosition2 = startPosition2 + new Vector3(10, 0, 0);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            doorPart1.transform.position = Vector3.Lerp(startPosition1, endPosition1, (elapsedTime / duration));
            doorPart2.transform.position = Vector3.Lerp(startPosition2, endPosition2, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        doorPart1.transform.position = endPosition1;
        doorPart2.transform.position = endPosition2;
        isMoving = false;
    }

    private void FadeOut()
    {
        Instantiate(fadePrefab);
    }
}
