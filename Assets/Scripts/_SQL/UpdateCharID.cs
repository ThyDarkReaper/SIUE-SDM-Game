using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateCharID : MonoBehaviour
{
    public void CallUpdateCharacterID(string username, int charID)
    {
        StartCoroutine(UpdateCharacterID(username, charID));
    }

    IEnumerator UpdateCharacterID(string username, int charID)
    {
        string postData = $"username={UnityWebRequest.EscapeURL(username)}&charID={UnityWebRequest.EscapeURL(charID.ToString())}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "http://103-89-14-161.cloud-xip.com/updateCharacterID.php";
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error updating character ID: " + www.error);
        }
        else
        {
            Debug.Log("Character ID updated successfully: " + www.downloadHandler.text);
        }
    }
}
