using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ManagePlayers : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMPro.TextMeshProUGUI feedbackText;


    public void CallDeletePlayer(TMP_InputField usernameInput)
    {
        StartCoroutine(DeletePlayer(usernameInput.text));
    }

    IEnumerator DeletePlayer(string username)
    {
        string postData = $"username={UnityWebRequest.EscapeURL(username)}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "https://103-89-14-188.cloud-xip.com/deletePlayer.php";
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error deleting player: " + www.error);
            feedbackText.text = "Error deleting player: " + www.error;
            yield break;
        }
        else
        {
            Debug.Log("Player deleted successfully: " + www.downloadHandler.text);
            feedbackText.text = "Player deleted successfully: " + www.downloadHandler.text;
            yield break;
        }
    }

    public void CallAddPlayer(TMP_InputField usernameInput)
    {
        StartCoroutine(AddPlayer(usernameInput.text));
    }

    IEnumerator AddPlayer(string username)
    {
        string postData = $"username={UnityWebRequest.EscapeURL(username)}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "https://103-89-14-188.cloud-xip.com/addPlayer.php";
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error adding player: " + www.error);
            feedbackText.text = "Error adding player: " + www.error;
            yield break;
        }
        else
        {
            Debug.Log("Player added successfully: " + www.downloadHandler.text);
            feedbackText.text = "Player added successfully: " + www.downloadHandler.text;
            yield break;
        }
    }
}