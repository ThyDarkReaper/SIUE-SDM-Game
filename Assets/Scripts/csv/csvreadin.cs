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
     public Button nextButton;             // Button for user to click (set in Inspector)
    public string[] values;
    public string[] lines;
    private int currentLine = 1;  // Start after header
    public int line = 1;
    // Start is called before the first frame update
    void Start()
    {
        LoadCSV();
        DisplayText(line); //WIP function to display text

        // Add the button click event
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    // Load and parse the CSV file
    void LoadCSV()
    {
        lines = csvFile.text.Split('\n');
        foreach (string line in lines)
        {
            values = line.Split(',');
        }
    }
    /*
    void DisplayText(int line);
    {
        if(lines.length > 1)
        {
            //place holder line
        }  
    }
    */

    void OnNextButtonClicked()
    {
        
    }
}