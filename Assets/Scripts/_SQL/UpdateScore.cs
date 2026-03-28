using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateScore : MonoBehaviour
{
    public void CallUpdateLevelScore(string username, string levelID, float score)
    {
        StartCoroutine(UpdateLevelScore(username, levelID, score));
    }

    IEnumerator UpdateLevelScore(string username, string levelID, float score)
    {
        string postData = $"username={UnityWebRequest.EscapeURL(username)}&level={UnityWebRequest.EscapeURL(levelID)}&score={score}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "https://103-89-14-188.cloud-xip.com/updateScore.php";
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating level score: " + www.error);
        }
        else
        {
            Debug.Log("Level score updated successfully: " + www.downloadHandler.text);
        }
    }
}