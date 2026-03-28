using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.Compilation;

public class Importer : MonoBehaviour
{
    // Base URL for your level API
    string baseUrl = "http://103-89-14-188.cloud-xip.com/getLevel.php?level=";

    // You control BOTH of these in Inspector or code
    public int levelToLoad = 1;
    public TextAsset fileToWrite;

    void Start()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        // 🔥 You manually decide BOTH level and file
        StartCoroutine(GetLevelData(levelToLoad, fileToWrite));
    }
  
    // =========================
    // MAIN FETCH FUNCTION
    // =========================
    IEnumerator GetLevelData(int level, TextAsset fileName)
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

        APIResponse response = JsonUtility.FromJson<APIResponse>(json);

        if (!response.success)
        {
            Debug.LogError("API Error: " + response.message);
            yield break;
        }
        
        LevelData levelData = new LevelData
        {
            level = level,
            blocks = new List<Block>()
        };

        foreach (ApiQuestion q in response.questions)
        {
            Block block = new Block();
            block.question = q.question;
            block.explanation = q.explanation;
            block.answers = new List<Answer>();

            string[] rawAnswers = { q.answer1, q.answer2, q.answer3, q.answer4 };
            foreach (string raw in rawAnswers)
            {
                if (!string.IsNullOrEmpty(raw))
                    block.answers.Add(new Answer { correct = raw[0] == '1', text = raw.Substring(1) });
            }

            levelData.blocks.Add(block);
        }

        string formattedText = FormatLevel(levelData);
        
        Debug.Log($"Level: {levelData.level}, Blocks: {levelData.blocks?.Count}");

        // 🔥 Dynamic path using YOUR filename
        string fullPath = Path.Combine(Application.dataPath, "Scripts/TextDialogueScriptsForLevels/" + fileName.name + ".txt");
        WriteToFile(formattedText, fileName);

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
            string questionFormatted = System.Text.RegularExpressions.Regex.Replace(block.question, @"([.!?])\s+", "$1\n");
            sb.AppendLine(questionFormatted);

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
    void WriteToFile(string content, TextAsset fileName)
{
    try
    {
        File.WriteAllText(Path.Combine(Application.dataPath, "Scripts/TextDialogueScriptsForLevels/" + fileName.name + ".txt"), content);
        Debug.Log("File successfully written!");

        // THIS GOES RIGHT HERE
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
    catch (System.Exception e)
    {
        Debug.LogError("File Write Error: " + e.Message);
    }
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
    public string question;
    public List<Answer> answers;
    public string explanation;
}

[System.Serializable]
public class Answer
{
    public string text;
    public bool correct;
}
[System.Serializable]
public class ApiQuestion
{
    public string question;
    public string answer1;
    public string answer2;
    public string answer3;
    public string answer4;
    public string explanation;
}
[System.Serializable]
public class APIResponse
{    
    public bool success;
    public string message;
    public string questionID;
    public List<ApiQuestion> questions;
}
