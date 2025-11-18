
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class FileToTextboxes : MonoBehaviour
{
    public TextMeshProUGUI[] textboxes;   // Assign your textboxes in Inspector
    public TextAsset inputFile;           // Assign your .txt file in Inspector
    public Button nextButton;             // Button for user to click (set in Inspector)
    
    private string[] lines;               // Stores all lines read from file
    private int currentIndex = 0;         // Tracks current starting index
    private int pageSize = 5;             // Number of lines per "page"

    void Start()
    {
        // Load lines from file only once
        lines = inputFile.text.Split('\n');
        LoadingInformation();
        
        // Add the button click event
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    // Loads next "page" of lines into textboxes
    public void LoadingInformation()
    {
        for (int i = 0; i < pageSize; i++)
        {
            int lineIndex = currentIndex + i;
            if (lineIndex < lines.Length)
            {
                textboxes[i].text = lines[lineIndex];
            }
            else
            {
                textboxes[i].text = ""; // Blank if no data
            }
        }
    }

    // Event called when user clicks next
    public void OnNextButtonClicked()
    {
        // Advance index by pageSize
        currentIndex += pageSize;
        
        // Clamp so we don't overflow
        if (currentIndex >= lines.Length)
        {
            currentIndex = 0; // Optionally wrap around or disable button
        }
        
        LoadingInformation();
    }
}

