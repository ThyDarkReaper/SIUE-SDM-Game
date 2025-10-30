using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class csvreadin : MonoBehaviour
{
    public TextAsset csvFile; // Assign in Inspector
    public TextMeshProUGUI dialogueText; // Or TextMeshProUGUI for TMP, assign in Inspector

    private List<string[]> csvData = new List<string[]>();
    private int currentLine = 1;  // Start after header
    // Start is called before the first frame update
    void Start()
    {
        LoadCSV();
        ShowLine(currentLine);
    }

    // Load and parse the CSV file
    void LoadCSV()
    {
        string[] lines = csvFile.text.Split('\n');
        foreach (string line in lines)
        {
            csvData.Add(line.Trim().Split(','));
        }
    }
    // Display a specific line in the UI textbox
    void ShowLine(int lineIndex)
    {
        if (lineIndex < csvData.Count)
        {
            string[] lineData = csvData[lineIndex];
            // Example: assuming CSV columns are ID,Character,Line
            dialogueText.text = $"{lineData[1]}: {lineData[2]}";
        }
        else
        {
            dialogueText.text = "End of Dialog";
        }
    }

    // Example method to hook to a Button to step through segments
    public void NextLine()
    {
        currentLine++;
        ShowLine(currentLine);
    }
}