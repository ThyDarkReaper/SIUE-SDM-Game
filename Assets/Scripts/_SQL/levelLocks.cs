using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class levelLocks : MonoBehaviour
{
    public int levelToCheck = 1;
    public bool isLocked = true;

    EnterDoors enterDoors;

    void Start()
    {
        enterDoors = FindObjectOfType<EnterDoors>();
        StartCoroutine(CheckLevelLock());
    }

    IEnumerator CheckLevelLock()
    {
        string url = "https://103-89-14-188.cloud-xip.com/checkLevelLock.php?level=" + levelToCheck;

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Network Error: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        Debug.Log("Received JSON: " + json);

        LevelLockResponse response = JsonUtility.FromJson<LevelLockResponse>(json);

        if (!response.success)
        {
            Debug.LogError("API Error: " + response.message);
            yield break;
        }

        isLocked = response.isLocked;
        Debug.Log("Level " + levelToCheck + " is " + (isLocked ? "locked" : "unlocked"));
        if (enterDoors != null)
        {
            enterDoors.UpdateDoorLockStatus(levelToCheck, isLocked);
        }
    }
}

[System.Serializable]
public class LevelLockResponse
{
    public bool success;
    public string message;
    public bool isLocked;
}