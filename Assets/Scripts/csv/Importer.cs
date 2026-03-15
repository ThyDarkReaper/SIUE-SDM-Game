using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Text;
 

public class Importer : MonoBehaviour 
{

    public Button updateButton; // assign your button prefab in Inspector
    public string url = "http://103-89-14-161.cloud-xip.com/changePassword.php";

    public string username = "ymoto";
    public string password = "yami";
    void Start() {
        //StartCoroutine(GetInfo());
        updateButton.onClick.AddListener(UpdateInfo);
    }

    public void UpdateInfo() {
        StartCoroutine(GetInfo());
    }

     IEnumerator GetInfo() {
        
        // Try using string-based POST data instead of WWWForm
        string postData = $"username={UnityWebRequest.EscapeURL(username)}&newPassword={UnityWebRequest.EscapeURL(password)}";
        Debug.Log($"POST Data: {postData}");
        
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        string url = "http://103-89-14-188.cloud-xip.com/changePassword.php";
        Debug.Log("Attempting to connect to: " + url);
        
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        www.timeout = 30;
        yield return www.SendWebRequest();
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            string errorDetails = $"Status Code: {www.responseCode}, Error: {www.error}";
            if (www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
            {
                errorDetails += $", Response: {www.downloadHandler.text}";
            }
            Debug.LogError("Network Error: " + errorDetails);
        }
        else
        {
            string responseText = www.downloadHandler.text;
            Debug.Log("Server Response: " + responseText);
            
            try
            {
                // Change password successful
                if (responseText.Contains("\"success\":true"))
                {
                    Debug.Log("Password changed successfully!");

                }
                // Change password failed
                else if (responseText.Contains("\"success\":false"))
                {
                    // Extract error message from JSON
                    string errorMessage = "Failed to change password";
                    int messageStart = responseText.IndexOf("\"message\":\"") + 11;
                    if (messageStart > 10)
                    {
                        int messageEnd = responseText.IndexOf("\"", messageStart);
                        if (messageEnd > messageStart)
                        {
                            errorMessage = responseText.Substring(messageStart, messageEnd - messageStart);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("JSON Parse Error: " + e.Message + " Response: " + responseText);
            }
        }
 
        
    }
    // IEnumerator GetInfo() {
    //     UnityWebRequest source = UnityWebRequest.Get("http://103-89-14-161.cloud-xip.com");
    //     yield return source.SendWebRequest();
 
    //     if (source.result != UnityWebRequest.Result.Success) {
    //         Debug.Log(source.error);
    //     }
    //     else {
    //         // Show results as text
    //         //Debug.Log("being recieved");

    //         File.WriteAllText("Assests/Scripts/csv/overwrite.txt", "being recieved");
 
    //     }
    // }
}