using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateCharID : MonoBehaviour
{
    [System.Serializable]
    private class UpdateCharIdResponse
    {
        public bool success;
        public string message;
    }

    public void CallUpdateCharacterID(string username, int charID)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogError("UpdateCharID aborted: username is empty or missing in PlayerPrefs.");
            return;
        }

        StartCoroutine(UpdateCharacterID(username, charID));
    }

    IEnumerator UpdateCharacterID(string username, int charID)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("charID", charID);

        string url = "https://103-89-14-188.cloud-xip.com/updateCharID.php";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error updating character ID. HTTP {(int)www.responseCode}: {www.error}");
                if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    Debug.LogError("Server response: " + www.downloadHandler.text);
                }
                yield break;
            }

            string responseText = www.downloadHandler != null ? www.downloadHandler.text : string.Empty;
            UpdateCharIdResponse response = null;

            if (!string.IsNullOrEmpty(responseText))
            {
                response = JsonUtility.FromJson<UpdateCharIdResponse>(responseText);
            }

            if (response != null && response.success)
            {
                Debug.Log("Character ID updated successfully.");
            }
            else
            {
                string message = response != null && !string.IsNullOrEmpty(response.message)
                    ? response.message
                    : responseText;
                Debug.LogError("Character ID update failed: " + message);
            }
        }
    }
}
