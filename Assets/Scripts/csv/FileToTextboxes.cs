using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class FileToTextboxes : MonoBehaviour
{
    public TextMeshProUGUI[] textboxes; // Assign your textboxes in the Inspector
    public string filePath = "Assets/Resources/data.txt"; // Example file path
    public TextAsset inputFile;          // Assign your text file in the Inspector
    private string[] lines;            // Stores all lines read from file
    private int currentIndex = 0;      // Tracks current page start index
    private int pageSize;              // Number of lines per "page"

    void Start()
    {

        if (File.Exists(filePath))
        {
            string[] lines = inputFile.text.Split('\n'); 
            LoadingInformation(lines, currentIndex);
        }
        else
        {
            Debug.LogError("File not found at: " + filePath);
        }
        
        
    }

    public void LoadingInformation(string[] lines, int currentIndex)
    {
        
        for (int i = currentIndex; i < 5 && i < lines.Length; i++)
        {
            textboxes[i%5].text = lines[i];
            currentIndex = i;
        }
    }
        
}
