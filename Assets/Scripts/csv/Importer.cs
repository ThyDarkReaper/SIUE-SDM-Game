

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;

public class Importer : MonoBehaviour
{
    public Button updateButton;

    // Base URL for your level API
    public string baseUrl = "http://103-89-14-188.cloud-xip.com/getLevel.php?level=";

    // You control BOTH of these in Inspector or code
    public int levelToLoad = 1;
    public string fileNameToWrite = "overwrite.txt";

    void Start()
    {
        updateButton.onClick.AddListener(UpdateInfo);
    }

    public void UpdateInfo()
    {
        // 🔥 You manually decide BOTH level and file
        StartCoroutine(GetLevelData(levelToLoad, fileNameToWrite));
    }
  
    // =========================
    // MAIN FETCH FUNCTION
    // =========================
    IEnumerator GetLevelData(int level, string fileName)
    {
        string url = baseUrl + level;

        Debug.Log("Fetching level from: " + url);

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Network Error: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        Debug.Log("Received JSON: " + json);

        Debug.Log("RAW RESPONSE:\n" + json);

        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        string formattedText = FormatLevel(levelData);

        // 🔥 Dynamic path using YOUR filename
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);

        WriteToFile(formattedText, fullPath);

        Debug.Log($"Level {level} written to: {fullPath}");
    }

    // =========================
    // FORMAT TO TEXT STRUCTURE
    // =========================
    string FormatLevel(LevelData level)
    {
        StringBuilder sb = new StringBuilder();

        foreach (Block block in level.blocks)
        {
            sb.AppendLine(block.questionText);

            sb.AppendLine("1101588");

            foreach (Answer ans in block.answers)
            {
                int prefix = ans.correct ? 1 : 0;
                sb.AppendLine(prefix + ans.text);
            }

            foreach (Answer ans in block.answers)
            {
                if (ans.correct)
                    sb.AppendLine("This is exactly what I was thinking! Good answer.");
                else
                    sb.AppendLine("Hmm, I don't think that is correct...");
            }

            sb.AppendLine("8675309");

            sb.AppendLine(block.explanation);
        }

        return sb.ToString();
    }

    // =========================
    // WRITE FILE
    // =========================
    void WriteToFile(string content, string filePath)
    {
        File.WriteAllText(filePath, content);
    }
}

/////////////////////////////////////////////////////
// DATA STRUCTURES
/////////////////////////////////////////////////////

[System.Serializable]
public class LevelData
{
    public int level;
    public List<Block> blocks;
}

[System.Serializable]
public class Block
{
    public string type;
    public string questionText;
    public List<Answer> answers;
    public string explanation;
}

[System.Serializable]
public class Answer
{
    public string text;
    public bool correct;
}


/*
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
}
*/