using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateCharacterID : MonoBehaviour
{
    public void CallUpdateCharID(string username, int charID)
    {
        StartCoroutine(UpdateCharID(username, charID));
    }

    IEnumerator UpdateCharID(string username, int charID)
    {
        string postData = $"username={UnityWebRequest.EscapeURL(username)}&charID={UnityWebRequest.EscapeURL(charID.ToString())}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "http://103-89-14-188.cloud-xip.com/updateCharID.php";
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            {
                string errorDetails = $"Status Code: {www.responseCode}, Error: {www.error}";
                if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    errorDetails += $", Response: {www.downloadHandler.text}";
                }
            }
        else
        {
            Debug.Log("Character ID updated successfully" + www.downloadHandler.text);
        }
    }
}