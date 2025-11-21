using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class csvreadin : MonoBehaviour
{
    public TextAsset csvFile; // Assign in Inspector
    //public TextMeshProUGUI dialogueText; // Or TextMeshProUGUI for TMP, assign in Inspector
    public Button nextButton;             // Button for user to click (set in Inspector)
    //public string[] values;
    public List<string[]> lines = new List<string[]>();
    public string[] values;
    private int currentLine = 1;  // Start after header
    //public int line = 1;
    public TextMeshProUGUI Question; // For user input, assign in Inspector
    public TextMeshProUGUI Option_1; // For user input, assign in Inspector
    public TextMeshProUGUI Option_2; // For user input, assign in Inspector
    public TextMeshProUGUI Option_3; // For user input, assign in Inspector
    public TextMeshProUGUI Option_4; // For user input, assign in Inspector
    // Start is called before the first frame update
    void Start()
    {
        LoadCSV();

        // Add the button click event
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    // Load and parse the CSV file
    void LoadCSV()
    {
        lines = csvFile.text.Split('\n');
        foreach (string line in lines)
        {
            //values = line.Split(',');
            values = line.Split(',');
            if(row.Length > 1) // Skip empty/trailing lines
                allLines.Add(values);
        }
    }

    public void OnNextButtonClicked()
    {
        if(currentLine < lines.Count)
        {
            var row = allLines[currentLine];
            if(row.Length == 5)
            {
                Question.text = row[0];
                Option_1.text = row[1];
                Option_2.text = row[2];
                Option_3.text = row[3];
                Option_4.text = row[4];
            }
            currentLine++;
        }
    }
}